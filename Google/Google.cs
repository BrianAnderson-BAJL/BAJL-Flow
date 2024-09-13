using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using FlowEngineCore;
using Google.Apis.Auth.OAuth2;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Drawing;

namespace Google
{
  public class Google : FlowEngineCore.Plugin
  {
    private const string SETTING_FIREBASE_JSON_PATH = "Firebase JSON file path";

    private const int ERROR_UNKNOWN = (int)STEP_ERROR_NUMBERS.GoogleErrorMin + 0;
    private const int ERROR_MISSING_NOTIFICATION_TOKEN = (int)STEP_ERROR_NUMBERS.GoogleErrorMin + 1;

    public override void Init()
    {
      base.Init();

      Function func = new Function("Send Notifications", this, SendNotifications);
      PARM parm = new PARM("Title", DATA_TYPE.String);
      parm.Description = "The title of the notification";
      func.Parms.Add(parm);
      parm = new PARM("Message", DATA_TYPE.String);
      parm.Description = "The message body of the notification";
      func.Parms.Add(parm);
      parm = new PARM("Notification tokens", DATA_TYPE.String);
      parm.Description = "A variable that needs to contain sub variables of device registration notifiication tokens";
      func.Parms.Add(parm);
      Functions.Add(func);


      //   SETTINGS
      mSettings.SettingAdd(new Setting(SETTING_FIREBASE_JSON_PATH, ""));

      mSettings.SettingAdd(new Setting("", "Designer", "BackgroundColor", Color.Transparent));
      mSettings.SettingAdd(new Setting("", "Designer", "BorderColor", Color.Bisque));
      mSettings.SettingAdd(new Setting("", "Designer", "FontColor", Color.White));
    }

    public override void StartPlugin(Dictionary<string, object> GlobalPluginValues)
    {
      base.StartPlugin(GlobalPluginValues);

      string jsonPath = mSettings.SettingGetAsString(SETTING_FIREBASE_JSON_PATH);

      AppOptions options = new AppOptions()
      {
      Credential = GoogleCredential.FromFile(jsonPath)
      };
      FirebaseApp.Create(options);
    }

    public override void StartPluginDesigner(Dictionary<string, object> GlobalPluginValues)
    {
      base.StartPluginDesigner(GlobalPluginValues);
      //We just want to open the database like normal
      StartPlugin(GlobalPluginValues);
    }


    public override void StopPlugin()
    {
      base.StopPlugin();
    }


    public RESP SendNotifications(Flow flow, Variable[] vars)
    {
      string msgTitle = vars[0].GetValueAsString();
      string msgBody = vars[1].GetValueAsString();
      

      List<string> tempDeviceTokens = new List<string>(vars[2].Count);

      //if the device notification token contains a single token, use that, else use the sub variables as a list of device tokens
      if (vars[2].Count == 0 && vars[2].DataType == DATA_TYPE.String)
      {
        tempDeviceTokens.Add(vars[2].GetValueAsString());
      }
      else
      {
        for (int x = 0; x < vars[2].Count; x++)
        {
          string token = vars[2][x].GetValueAsString();
          if (token is null || token.Length == 0)
            continue;

          tempDeviceTokens.Add(token);
          if (tempDeviceTokens.Count == 500) //Google only allows you to send 500 notifications at a single time
          {
            try
            {
              SendActualNotifications(msgTitle, msgBody, tempDeviceTokens);
            }
            catch (Exception ex)
            {
              return RESP.SetError(ERROR_UNKNOWN, ex.Message);
            }
            tempDeviceTokens.Clear();
          }
        }
      }

      if (tempDeviceTokens.Count == 0)
        return RESP.SetSuccess();

      try
      {
        SendActualNotifications(msgTitle, msgBody, tempDeviceTokens);
      }
      catch (Exception ex)
      {
        return RESP.SetError(ERROR_UNKNOWN, ex.Message);
      }
      return RESP.SetSuccess();
    }

    private BatchResponse SendActualNotifications(string msgTitle, string msgBody, List<string> deviceTokens)
    {

      IReadOnlyList<string> deviceNotificationTokens = new ReadOnlyCollection<string>(deviceTokens);

      MulticastMessage msg = new MulticastMessage()
      {
        Tokens = deviceNotificationTokens,
        Notification = new Notification()
        {
          Title = msgTitle,
          Body = msgBody
        }
      };

      //Every flow executes in its own thread, all the async functions are useless in the flow engine.
      BatchResponse br = FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(msg).Result;

      return br;
    }

  }
}
