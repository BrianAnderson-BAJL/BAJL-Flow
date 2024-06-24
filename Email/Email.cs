using FlowEngineCore;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using System.Drawing;
using System.Net.Mail;

namespace Email
{
  public class Email : FlowEngineCore.Plugin
  {
    private bool KeepRunning = true;
    private Thread? checkEmailThread;
    private SmtpClient? mySmtpClient = null;

    private const string SETTING_NAME_MAIL_SERVER_URL = "Mail Server Url";
    private const string SETTING_NAME_MAIL_SERVER_PORT = "Mail Server Port";
    private const string SETTING_NAME_MAIL_SERVER_USER_NAME = "Mail Server User name";
    private const string SETTING_NAME_MAIL_SERVER_PASSWORD = "Mail Server Password";

    private const int ERROR_EMAIL_BAD_TEMPLATE_PATH = (int)STEP_ERROR_NUMBERS.EmailErrorMin + 0;
    private const int ERROR_EMAIL_UNABLE_TO_SEND_EMAIL = (int)STEP_ERROR_NUMBERS.EmailErrorMin + 1;

    public override void Init()
    {
      base.Init();

      Function func;

      func = new Function("Send", this, Send);
      PARM parm = new PARM("toEmailAddress", DATA_TYPE.Various);
      parm.Description = "The actual email address to send this to (e.g. brian@bajlllc.com)";
      func.Parms.Add(parm);
      parm = new PARM("toEmailAddressName", DATA_TYPE.Various);
      parm.Description = "(optional) The name of the person that this email is being sent to (e.g. Brian Anderson)";
      func.Parms.Add(parm);

      parm = new PARM("fromEmailAddress", DATA_TYPE.Various);
      parm.Description = "(optional) The actual email address this email will be sent from (e.g. brian@bajlllc.com) if it is left blank the 'Mail Server User Name' setting will be used instead.";
      func.Parms.Add(parm);

      parm = new PARM("fromEmailAddressName", DATA_TYPE.Various);
      parm.Description = "(optional) The name of the person that this email is being sent from (e.g. Brian Anderson)";
      func.Parms.Add(parm);

      func.Parms.Add(new PARM("subject", DATA_TYPE.Various));
      func.Parms.Add(new PARM("body", DATA_TYPE.Various));

      parm = new PARM("content images to attach", DATA_TYPE.Various);
      parm.Description = "(optional) If you are sending HTML in the body and have Content-ID images to link, then populate sub variables in this variable";
      func.Parms.Add(parm);

      parm = new PARM("files to attach", DATA_TYPE.Various);
      parm.Description = "(optional) If you need to attach files to the email then populate sub variables in this variable";
      func.Parms.Add(parm);

      Functions.Add(func);

      func = new Function("Load Template", this, LoadTemplate);
      parm = new PARM("FileName", DATA_TYPE.Various);
      parm.Description = "The path and file name to the template file. A text file that will be used in the email.";
      func.Parms.Add(parm);
      parm = new PARM("%(code)s", DATA_TYPE.Various);
      parm.Description = "The value that needs to be changed in the template file.";
      parm.AllowMultiple = PARM.PARM_ALLOW_MULTIPLE.Multiple;
      parm.NameChangeable = true;
      func.Parms.Add(parm);
      func.DefaultSaveResponseVariable = true;
      func.RespNames.Name = "template";
      Functions.Add(func);


      //   SETTINGS
      mSettings.SettingAdd(new Setting(SETTING_NAME_MAIL_SERVER_URL, ""));
      mSettings.SettingAdd(new Setting(SETTING_NAME_MAIL_SERVER_PORT, 587L));
      mSettings.SettingAdd(new Setting(SETTING_NAME_MAIL_SERVER_USER_NAME, ""));
      mSettings.SettingAdd(new Setting(SETTING_NAME_MAIL_SERVER_PASSWORD, ""));

      mSettings.SettingAdd(new Setting("", "Designer", "BackgroundColor", Color.Transparent));
      mSettings.SettingAdd(new Setting("", "Designer", "BorderColor", Color.Blue));
      mSettings.SettingAdd(new Setting("", "Designer", "FontColor", Color.White));

    }

    public override void StartPlugin(Dictionary<string, object> GlobalPluginValues)
    {
      base.StartPlugin(GlobalPluginValues);

      checkEmailThread = new Thread(CheckEmailThread);
      checkEmailThread.Start();

    }

    public override void StopPlugin()
    {
      base.StopPlugin();
      KeepRunning = false;
    }

    private void CheckEmailThread()
    {
      //ImapClient imapClient = new ImapClient();
      //imapClient.Connect("imap.example.com", 993, true);
      //imapClient.Authenticate("username", "password");
      //imapClient.Inbox.Open(FolderAccess.ReadOnly);

      //while (KeepRunning == true)
      //{
      //    var uids = imapClient.Inbox.Search(SearchQuery.All);
      //    foreach (var uid in uids)
      //    {
      //      var mimeMessage = imapClient.Inbox.GetMessage(uid);
      //      // mimeMessage.WriteTo($"{uid}.eml"); // for testing
      //      //yield return mimeMessage;
      //    }
      //}
      //imapClient.Disconnect(true);
      //imapClient.Dispose();
    }


    public RESP LoadTemplate(Flow flow, Variable[] vars)
    {
      string fileName = vars[0].GetValueAsString();

      fileName = Options.GetTemplateFileNameRelativePath(fileName);
      string template;
      try
      {
        template = File.ReadAllText(fileName);
      }
      catch (Exception ex)
      {
        return RESP.SetError(ERROR_EMAIL_BAD_TEMPLATE_PATH, ex.Message);
      }

      for (int x = 1; x < vars.Length; x++)
      {
        string searchFor = vars[x].Name;
        string replaceWith = vars[x].GetValueAsString();
        template = template.Replace(searchFor, replaceWith);
      }

      return RESP.SetSuccess(new Variable("template", template));
    }

    public RESP Send(Flow flow, Variable[] vars)
    {
      string toEmailAddress = vars[0].GetValueAsString();
      string? toEmailAddressName = vars[1].GetValueAsString();
      string fromEmailAddress = vars[2].GetValueAsString();
      string? fromEmailAddressName = vars[3].GetValueAsString();
      string subject = vars[4].GetValueAsString();
      string body = vars[5].GetValueAsString();
      string contentId = vars[6].GetValueAsString();
      try
      {
        string url = mSettings.SettingGetAsString(SETTING_NAME_MAIL_SERVER_URL);
        int port = mSettings.SettingGetAsInt(SETTING_NAME_MAIL_SERVER_PORT);
        string uid = mSettings.SettingGetAsString(SETTING_NAME_MAIL_SERVER_USER_NAME);
        string pwd = mSettings.SettingGetAsString(SETTING_NAME_MAIL_SERVER_PASSWORD);

        if (fromEmailAddress is null || fromEmailAddress == "") //If no from email is set, lets just use the user id credentials
          fromEmailAddress = uid;

        if (toEmailAddressName == "")
          toEmailAddressName = null;
        if (fromEmailAddressName == "")
          fromEmailAddressName = null;



        if (mySmtpClient is null)
        {
          mySmtpClient = new SmtpClient(url, port);
          mySmtpClient.Timeout = 5000;
          mySmtpClient.EnableSsl = true;
          mySmtpClient.UseDefaultCredentials = false;
          mySmtpClient.Credentials = new System.Net.NetworkCredential(uid, pwd);
        }
        // add from,to mailaddresses
        MailAddress to = new MailAddress(toEmailAddress, toEmailAddressName);
        MailAddress from = new MailAddress(fromEmailAddress, fromEmailAddressName);
        MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

        // add ReplyTo
        MailAddress replyTo = new MailAddress(fromEmailAddress);
        myMail.ReplyToList.Add(replyTo);

        // set subject and encoding
        myMail.Subject = subject;
        myMail.SubjectEncoding = System.Text.Encoding.UTF8;

        //TODO: Fix to work with multiple content id attachments
        Attachment myAtt = new Attachment(Options.GetTemplateFileNameRelativePath(contentId));
        contentId = Path.GetFileName(contentId);
        myAtt.ContentId = contentId;
        myMail.Attachments.Add(myAtt);
       
        // set body-message and encoding
        myMail.Body = body;
        myMail.BodyEncoding = System.Text.Encoding.UTF8;
        // text or html
        myMail.IsBodyHtml = true;

        

        mySmtpClient.Send(myMail);
      }

      catch (SmtpException ex)
      {
        return RESP.SetError(ERROR_EMAIL_UNABLE_TO_SEND_EMAIL, ex.Message);
      }
      catch
      {
        throw;
      }





      return RESP.SetSuccess();
    }
  }
}
