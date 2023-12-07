using Core;
using Core.Interfaces;
using System.Collections.Specialized;
using System.Drawing;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using static Core.PARM;

namespace Http
{
  public class Http : Core.Plugin
  {
    private Thread? ListenerThread;
    private HttpListener? Listener;

    private const string PARM_URL = "Url Path";
    private const string PARM_METHOD = "Method";
    private const string PARM_HOST = "Host";
    private const string PARM_CONNECTION_HANDLE = "network_conn_handle";
    private const string PARM_DATA_FORMAT = "data_format";

    private const string PARM_DATA_FORMAT_RAW = "RAW";
    private const string PARM_DATA_FORMAT_JSON = "JSON";
    private const string PARM_DATA_FORMAT_XML = "XML";
    private const string VAR_HEADERS = "headers";


    /// <summary>
    /// Initialize the plugin, create the settings, functions, start commands, and sample variables
    /// </summary>
    public override void Init()
    {
      base.Init();
      //AppDomain.CurrentDomain.GetAssemblies()
      Assembly? a1 = Assembly.GetAssembly(typeof(System.Threading.Thread));
      
      //SETTINGS
      {
        Setting s = new Setting("Uri", DATA_TYPE.String);
        s.Description = "The URIs this Http pluging should listen on, comma delimited. (http://*:80,http://*:443, ...)";
        SettingAdd(s);
        SettingAdd(new Setting("", "Designer", "BackgroundColor", Color.Transparent));
        SettingAdd(new Setting("", "Designer", "BorderColor", Color.Orange));
        SettingAdd(new Setting("", "Designer", "FontColor", Color.Black));
      }
      //SETTINGS


      //FUNCTIONS
      {
        Function function = new Function("Connect", this, Connect);
        function.Parms.Add(PARM_URL, DATA_TYPE.String);

        PARM parm = function.Parms.Add("Port", DATA_TYPE.Integer);
        parm.ValidatorAdd(PARM.PARM_VALIDATION.NumberDefaultValue, 443);
        parm.ValidatorAdd(PARM.PARM_VALIDATION.NumberMax, 64535);
        parm.ValidatorAdd(PARM.PARM_VALIDATION.NumberMin, 1);

        parm = function.Parms.Add("Timeout in ms", DATA_TYPE.Integer);
        parm.ValidatorAdd(PARM.PARM_VALIDATION.NumberDefaultValue, 5000);
        parm.ValidatorAdd(PARM.PARM_VALIDATION.NumberMin, 0);
        function.DefaultSaveResponseVariable = true;
        function.RespNames = new Variable(PARM_CONNECTION_HANDLE, DATA_TYPE.Object);
        Functions.Add(function); //Connect


        function = new Function("Send", this, Send);
        function.Parms.Add(PARM_CONNECTION_HANDLE, DATA_TYPE.Object);
        PARM pddl = function.Parms.Add("Http response code", STRING_SUB_TYPE.DropDownList);
        pddl.ValidatorAdd(PARM_VALIDATION.StringDefaultValue, "200 - OK");

        foreach (int val in Enum.GetValues(typeof(HttpStatusCode)))
        {
          String name = Enum.GetName(typeof(HttpStatusCode), val)!;
          pddl.OptionAdd($"{val} - {name}");
        }

        function.Parms.Add(Flow.VAR_DATA, DATA_TYPE.String);
        pddl = new PARM(PARM_DATA_FORMAT, STRING_SUB_TYPE.DropDownList, PARM.PARM_REQUIRED.Yes);
        pddl.ValidatorAdd(PARM_VALIDATION.StringDefaultValue, PARM_DATA_FORMAT_JSON);
        pddl.OptionAdd(PARM_DATA_FORMAT_RAW);
        pddl.OptionAdd(PARM_DATA_FORMAT_JSON);
        pddl.OptionAdd(PARM_DATA_FORMAT_XML);
        function.Parms.Add(pddl);
        Functions.Add(function); //Send


        function = new Function("Receive", this, Receive);
        function.Parms.Add(PARM_CONNECTION_HANDLE, DATA_TYPE.Object);
        parm = function.Parms.Add("Timeout in ms", DATA_TYPE.Integer);
        parm.ValidatorAdd(PARM.PARM_VALIDATION.NumberDefaultValue, 10000);
        parm.ValidatorAdd(PARM.PARM_VALIDATION.NumberMin, 0);
        function.DefaultSaveResponseVariable = true;
        function.RespNames = new Variable(Flow.VAR_REQUEST);
        function.RespNames.Add(new Variable(VAR_HEADERS));
        function.RespNames.Add(new Variable(Flow.VAR_DATA));
        Functions.Add(function); //Receive


        function = new Function("Disconnect", this, Disconnect);
        function.Parms.Add(PARM_CONNECTION_HANDLE, DATA_TYPE.Object);
        Functions.Add(function); //Disconnect


        function = new Function("Connect Send Receive Disconnect", this, ConnectSendReceiveDisconnect);
        function.Parms.Add(PARM_URL, DATA_TYPE.String);
        parm = function.Parms.Add("Port", DATA_TYPE.Integer);
        parm.ValidatorAdd(PARM.PARM_VALIDATION.NumberDefaultValue, 443);
        parm.ValidatorAdd(PARM.PARM_VALIDATION.NumberMax, 64535);
        parm.ValidatorAdd(PARM.PARM_VALIDATION.NumberMin, 1);

        function.Parms.Add(Flow.VAR_DATA, DATA_TYPE.String);

        parm = function.Parms.Add("Connect Timeout in ms", DATA_TYPE.Integer);
        parm.ValidatorAdd(PARM.PARM_VALIDATION.NumberDefaultValue, 5000);
        parm.ValidatorAdd(PARM.PARM_VALIDATION.NumberMin, 0);

        parm = function.Parms.Add("Receive Timeout in ms", DATA_TYPE.Integer);
        parm.ValidatorAdd(PARM.PARM_VALIDATION.NumberDefaultValue, 10000);
        parm.ValidatorAdd(PARM.PARM_VALIDATION.NumberMin, 0);

        function.DefaultSaveResponseVariable = true;
        function.RespNames = new Variable(Flow.VAR_REQUEST);
        function.RespNames.Add(new Variable(VAR_HEADERS));
        function.RespNames.Add(new Variable(Flow.VAR_DATA));
        Functions.Add(function); //ConnectSendReceiveDisconnect
      }
      //FUNCTIONS

      //FLOW START COMMANDS
      {
        PARM parm = FlowStartCommands.Add(PARM_URL, DATA_TYPE.String);
        parm.ValidatorAdd(PARM_VALIDATION.StringDefaultValue, "/");

        PARM pddl = new PARM(PARM_METHOD, STRING_SUB_TYPE.DropDownList);
        pddl.ValidatorAdd(PARM_VALIDATION.StringDefaultValue, "GET");
        pddl.OptionAdd("GET");
        pddl.OptionAdd("POST");
        pddl.OptionAdd("PUT");
        pddl.OptionAdd("DELETE");
        FlowStartCommands.Add(pddl);
        FlowStartCommands.Add(PARM_HOST, DATA_TYPE.String);
      }
      //FLOW START COMMANDS

      //SAMPLE VARIABLES FOR DESIGNER
      {

        Variable root = new Variable(Flow.VAR_NAME_FLOW_START, DATA_FORMAT_SUB_VARIABLES.Block);
        //Variable request = new Variable(Flow.VAR_REQUEST);
        root.Add(new Variable(PARM_CONNECTION_HANDLE, DATA_TYPE.Object));
        Variable headers = new Variable(VAR_HEADERS, DATA_FORMAT_SUB_VARIABLES.Block);
        root.Add(headers);
        Variable data = new Variable(Flow.VAR_DATA);
        data.Add(new VariableString("YOUR_SAMPLE_DATA", "GOES_HERE"));
        root.Add(data);
        SampleStartData = root;
      }
      //SAMPLE VARIABLES FOR DESIGNER
    }

    private void ListenerThreadRuntime()
    {
      Listener = new HttpListener();
      Setting? uris = SettingFind("Uri");
      if (uris is null)
      {
        Global.Write("Unable to start HTTP listening, no URIs to listen on");
        return;
      }
      int index = -1;
      string[] urisString = { };
      if (uris.Value is null)
        return;

      if (uris.Value.ToString() == "")
        return;

      urisString = uris.Value.ToString()!.Split(',');
      for (index = 0; index < urisString.Length; index++) 
      {
        if (urisString[index].Right(1) != "/") //Listener needs to have a '/' (forward slash) at the end for some reason 
          urisString[index] += "/";
        Listener.Prefixes.Add(urisString[index]);
      }

      try
      {
        Listener.Start();
      }
      catch (HttpListenerException e)
      {
        Global.Write($"unable to start listening, HttpListenerException [{e.Message}]", DEBUG_TYPE.Error);
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
        {
          Global.Write("May need to run this command to allow the Http plugin to start correctly and to run in non-admin mode: netsh http add urlacl url=[URL HERE] user=Everyone");
          for (index = 0; index < urisString.Length; index++)
          {
            Global.Write(urisString[index]);
          }
        }
      }
      catch (Exception e) 
      {
        Global.Write("unable to start listening, " + e.Message);
      }
      while (Listener.IsListening == true)
      {
        try
        {
          HttpListenerContext context = Listener.GetContext();
          HttpListenerRequest request = context.Request;

          if (request.ContentLength64 > 0)
          {
            Stream body = request.InputStream;
            StreamReader reader = new StreamReader(body, request.ContentEncoding);
            string data = reader.ReadToEnd();
            try
            {
              List<Flow> flows = FindFlows(request.Url, request.HttpMethod, request.UserHostName);
              if (flows.Count == 0)
              {
                HttpListenerResponse response = context.Response;
                response.Headers.Clear();
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                byte[] outputData = System.Text.Encoding.UTF8.GetBytes("No flow capable of handling request.");
                response.ContentLength64 = outputData.Length;
                response.OutputStream.Write(outputData, 0, outputData.Length);
                response.Close();
                continue;
              }

              Variable baseVar = new Variable(Flow.VAR_NAME_FLOW_START);
              Variable requestVar = new Variable(Flow.VAR_REQUEST);
              baseVar.Add(requestVar);
              requestVar.Add(new VariableObject(PARM_CONNECTION_HANDLE, context));
              requestVar.Add(GetHeaders(request));
              if (request.ContentType == "application/json" && data is not null && data.Length > 0)
              {
                Variable? v = Variable.JsonParse(ref data);
                requestVar.Add(v);
                //string junk = v.SubVariables[0].JsonCreate();
              }
              for (int i = 0; i < flows.Count; i++)
              {
                FlowEngine.StartFlow(new FlowRequest(baseVar, this, flows[i]));
              }
            }
            catch
            {
            }
          }
        }
        catch (HttpListenerException)
        {
          //Do nothing, it was killed by the flow engine, probably stopping.
        }
        catch (Exception ex)
        {
          Global.Write(ex.Message);
        }
      };
    }

    private Variable GetHeaders(HttpListenerRequest request)
    {
      Variable headers = new Variable(VAR_HEADERS);
      for (int x = 0; x < request.Headers.AllKeys.Length; x++)
      {
        string? k = request.Headers.AllKeys[x];
        if (k is not null)
        {
          VariableString headerVar = new VariableString(k, "");
          string[]? v = request.Headers.GetValues(k);
          if (v is not null)
          {
            for (int y = 0; y < v.Length; y++)
            {
              headerVar.Add(new VariableString(k, v[y]));
            }
          }
          headers.Add(headerVar);
        }
      }
      return headers;
    }


    private List<Flow> FindFlows(Uri? uri, string method, string host)
    {
      List<Flow> result = new List<Flow>();
      if (FlowStartCommands.Count < 3)
        return result;  //Something is wrong, exit

      //Fix any uri or url problems
      if (uri is null)
        uri = new Uri("");
      string url = uri.LocalPath.ToString();
      url = Options.FixUrl(url);
      Global.Write($"Http Looking for flow Url [{url}], method [{method}], host [{host}]");
      lock (mFlowsCriticalSection)
      {
        for (int x = 0; x < Flows.Count; x++)
        {
          Flow f = Flows[x];
          PARM_VAR? pUrl = f.StartCommands.FindByParmName(PARM_URL);
          PARM_VAR? pMethod = f.StartCommands.FindByParmName(PARM_METHOD);
          PARM_VAR? pHost = f.StartCommands.FindByParmName(PARM_HOST);
          if (pUrl is not null && pMethod is not null && pHost is not null)
          {
            pUrl.GetValue(out string flowUrl, f);
            pMethod.GetValue(out string flowMethod, f);
            pHost.GetValue(out string flowHost, f);
            if (Options.FixUrl(flowUrl) == url && flowMethod == method && HostsMatch(flowHost, host) == true)
            {
              result.Add(f);
            }
          }
        }
      }
      return result;
    }

    private bool HostsMatch(string allowedHost, string requestHost)
    {
      bool match = false;
      string[] host_vals = allowedHost.Split(':');
      if (host_vals.Length > 0)
        allowedHost = host_vals[0]; //We are ignoring the port

      host_vals = requestHost.Split(':');
      if (host_vals.Length > 0)
        requestHost = host_vals[0]; //We are ignoring the port

      allowedHost = allowedHost.ToLower().Trim();
      requestHost = requestHost.ToLower().Trim();

      if (allowedHost == "*" || allowedHost == "") //Allows all hosts
        return true;

      if (allowedHost == requestHost)
        return true;


      return match;
    }

    public override void StartPlugin(Dictionary<string, object> GlobalPluginValues)
    {
      ListenerThread = new Thread(ListenerThreadRuntime);
      ListenerThread.Start();
    }

    public override void StopPlugin()
    {
      if (Listener is not null)
      {
        try
        {
          Listener.Stop();
        }
        catch { } //Ignore all errors when shutting down.
      }
      base.StopPlugin();
    }

    public override void Dispose()
    {

    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="vars 0">"connection_handle"</param>
    /// <param name="vars 1">"HTTP response code"</param>
    /// <param name="vars 2">"data"</param>
    /// <param name="vars 3">"data format"</param>
    /// <param name="Resps">Output Index 0 = SUCCESS</param>
    /// <param name="Resps">Output Index 1 = ERROR</param>
    public static RESP Send(Core.Flow flow, Variable[] vars)
    {
      Global.Write("Http.Send");

      vars[0].GetValue(out object obj);
      vars[1].GetValue(out string tempStr);
      string tempStr2 = tempStr.StartingNumericOnly();
      int.TryParse(tempStr2, out int responseCode);
      Variable data = vars[2];
      vars[3].GetValue(out string dataformat);
      HttpListenerContext? context = obj as HttpListenerContext;

      if (context is null)
        return RESP.SetError(1, $"Unable to resolve [{PARM_CONNECTION_HANDLE}] parameter");
      if (data is null)
        return RESP.SetError(1, "Unable to resolve [data] parameter");
      if (dataformat != PARM_DATA_FORMAT_XML && dataformat != PARM_DATA_FORMAT_RAW && dataformat != PARM_DATA_FORMAT_JSON)
        return RESP.SetError(1, $"Unknown data format [{dataformat}] for parameter [{PARM_DATA_FORMAT}]");
      if (responseCode < 0)
        return RESP.SetError(1, $"Http response code invalid value [{responseCode}], original value[{tempStr}]");

      string rawData = "";
      if (dataformat == PARM_DATA_FORMAT_JSON)
        rawData = data.ToJson(JSON_ROOT_BLOCK_OPTIONS.StripNameFromRootAndAddBlock);
      else if (dataformat == PARM_DATA_FORMAT_XML)
        throw new NotImplementedException(PARM_DATA_FORMAT_XML + " is not implemented yet");
      else if (dataformat == PARM_DATA_FORMAT_RAW)
        throw new NotImplementedException(PARM_DATA_FORMAT_RAW + " is not implemented yet");

      HttpListenerResponse response = context.Response;
      response.Headers.Clear();
      response.StatusCode = responseCode;
      byte[] outputData = System.Text.Encoding.UTF8.GetBytes(rawData);
      response.ContentLength64 = outputData.Length;
      response.OutputStream.Write(outputData, 0, outputData.Length);
      response.Close();
      Global.Write($"Http.Send - Sent [{outputData.Length}] bytes of data to client");
      return RESP.SetSuccess();
    }

    public static RESP Disconnect(Core.Flow flow, Variable[] vars)
    {
      Global.Write("Http.Disconnect");
      return RESP.SetSuccess();
    }


    public static RESP ConnectSendReceiveDisconnect(Core.Flow flow, Variable[] vars)
    {
      Global.Write("Http.ConnectSendReceiveDisconnect");
      return RESP.SetSuccess();
    }


    public static RESP Connect(Core.Flow flow, Variable[] vars)
    {
      Global.Write("Http.Connect");
      return RESP.SetSuccess();
    }


    public static RESP Receive(Core.Flow flow, Variable[] vars)
    {
      Global.Write("Http.Receive");
      return RESP.SetSuccess();
    }
  }
}