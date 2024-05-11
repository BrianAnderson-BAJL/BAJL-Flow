using FlowEngineCore;
using FlowEngineCore.Interfaces;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using static FlowEngineCore.PARM;

namespace Http
{
  public class Http : FlowEngineCore.Plugin
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
      
      //SETTINGS
      {
        Setting s = new Setting("Uri", DATA_TYPE.String);
        s.Description = "The URIs this Http pluging should listen on, comma delimited. (http://*:80,http://*:443, ...)";
        mSettings.SettingAdd(s);
        mSettings.SettingAdd(new Setting("", "Designer", "BackgroundColor", Color.Transparent));
        mSettings.SettingAdd(new Setting("", "Designer", "BorderColor", Color.Orange));
        mSettings.SettingAdd(new Setting("", "Designer", "FontColor", Color.Black));
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
        function.RespNames.SubVariableAdd(new Variable(VAR_HEADERS));
        function.RespNames.SubVariableAdd(new Variable(Flow.VAR_DATA));
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
        function.RespNames.SubVariableAdd(new Variable(VAR_HEADERS));
        function.RespNames.SubVariableAdd(new Variable(Flow.VAR_DATA));
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

        Variable root = new Variable(Flow.VAR_NAME_FLOW_START);
        //Variable request = new Variable(Flow.VAR_REQUEST);
        root.SubVariableAdd(new Variable(PARM_CONNECTION_HANDLE, DATA_TYPE.Object));
        Variable headers = new Variable(VAR_HEADERS);
        root.SubVariableAdd(headers);
        Variable data = new Variable(Flow.VAR_DATA);
        data.SubVariableAdd(new Variable("YOUR_SAMPLE_DATA", "GOES_HERE"));
        root.SubVariableAdd(data);
        SampleStartData = root;
      }
      //SAMPLE VARIABLES FOR DESIGNER
    }

    private void AddAddressToHttpConfig(string address)
    {
      AddAddressToHttpConfig(address, Environment.UserDomainName, Environment.UserName);
    }

    private void AddAddressToHttpConfig(string address, string domain, string user)
    {
      string args = $"http add urlacl url={address} user={domain}\\{user}";

      ProcessStartInfo psi = new ProcessStartInfo("netsh", args);
      psi.Verb = "runas";
      psi.CreateNoWindow = true;
      psi.WindowStyle = ProcessWindowStyle.Hidden;
      psi.UseShellExecute = true;

      Process? p = Process.Start(psi);
      if (p is not null)
      {
        p.WaitForExit();
      }
    }

    private void ListenerThreadRuntime()
    {
      Thread.CurrentThread.Name = "HTTP Plugin";
      Setting? uris = mSettings.SettingFind("Uri");
      if (uris is null)
      {
        mLog?.Write("Unable to start HTTP listening, no URIs to listen on", LOG_TYPE.WAR);
        return;
      }
      int index = -1;
      string[] urisString = { };
      if (uris.Value is null)
      {
        mLog?.Write("Uri Value is null", LOG_TYPE.WAR);
        return;
      }
      if (uris.Value.ToString() == "")
      {
        mLog?.Write("Uri Value is blank", LOG_TYPE.WAR);
        return;
      }

      Listener = new HttpListener();
      urisString = uris.Value.ToString()!.Split(',');
      for (index = 0; index < urisString.Length; index++) 
      {
        if (urisString[index].Right(1) != "/") //Listener needs to have a '/' (forward slash) at the end for some reason 
          urisString[index] += "/";
        Listener.Prefixes.Add(urisString[index]);
        mLog?.Write($"HTTP plugin will be Listening on [{urisString[index]}]", LOG_TYPE.INF);
      }

      try
      {
        Listener.Start();
        mLog?.Write($"HTTP plugin Listener started", LOG_TYPE.INF);
      }
      catch (HttpListenerException e)
      {
        mLog?.Write($"unable to start listening, HttpListenerException [{e.Message}]", LOG_TYPE.ERR);
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
          mLog?.Write("May need to run this command to allow the Http plugin to start correctly and to run in non-admin mode: netsh http add urlacl url=[URL HERE] user=Everyone");
          for (index = 0; index < urisString.Length; index++)
          {
            mLog?.Write(urisString[index]);
          }
        }
      }
      catch (Exception e)
      {
        mLog?.Write("unable to start listening", e, LOG_TYPE.WAR);
      }
      

      while (Listener.IsListening == true)
      {
        try
        {
          
          HttpListenerContext context = Listener.GetContext();
          HttpListenerRequest request = context.Request;
          string data = "";
          if (request.ContentLength64 > 0) //Has a body
          {
            Stream body = request.InputStream;
            StreamReader reader = new StreamReader(body, request.ContentEncoding);
            data = reader.ReadToEnd();
            //
          }
          try
          {
            List<Flow> flows = FindFlows(request.Url, request.HttpMethod, request.UserHostName);
            if (flows.Count == 0) //No flow found, return error
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
            //Variable requestVar = new Variable(Flow.VAR_REQUEST);
            baseVar.SubVariableAdd(new Variable(PARM_CONNECTION_HANDLE, context));
            //requestVar.Add();
            baseVar.SubVariableAdd(GetHeaders(request));
            if (request.ContentType == "application/json" && data is not null && data.Length > 0)
            {
              Variable? v = Variable.JsonParse(ref data, "data");
              baseVar.SubVariableAdd(v);
              //string junk = v.SubVariables[0].JsonCreate();
            }
            for (int i = 0; i < flows.Count; i++)
            {
              string? threadName = FlowEngine.GetNextThreadName();
              mLog?.Write($"HTTP received a request [{data}]", LOG_TYPE.INF, threadName);
              FlowEngine.StartFlow(new FlowRequest(baseVar, this, flows[i], FlowRequest.START_TYPE.WaitForEvent, FlowRequest.CLONE_FLOW.CloneFlow, threadName));
            }
          }
          catch
          {
          }
          
        }
        catch (HttpListenerException exHttp)
        {
          mLog?.Write(exHttp);
          //Do nothing, it was killed by the flow engine, probably stopping.
        }
        catch (Exception ex)
        {
          mLog?.Write(ex);
        }
      };
    }

    private Variable GetHeaders(HttpListenerRequest request)
    {
      Variable headers = new Variable(VAR_HEADERS);
      for (int x = 0; x < request.Headers.AllKeys.Length; x++)
      {
        string? headerName = request.Headers.AllKeys[x];
        if (headerName is not null)
        {
          string[]? headerValues = request.Headers.GetValues(headerName);
          if (headerValues is not null && headerValues.Length > 0)
          {
            //TODO: For now only adding the first header value, should add all at some point.
            headers.SubVariableAdd(new Variable(headerName, headerValues[0]));
          }
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
      mLog?.Write($"Http Looking for flow Url [{url}], method [{method}], host [{host}]", LOG_TYPE.DBG);
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
      base.StartPlugin(GlobalPluginValues);
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
    public RESP Send(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("Http.Send", LOG_TYPE.DBG);

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
        rawData = data.ToJson();
      else if (dataformat == PARM_DATA_FORMAT_XML)
        throw new NotImplementedException(PARM_DATA_FORMAT_XML + " is not implemented yet");
      else if (dataformat == PARM_DATA_FORMAT_RAW)
        rawData = data.Value;

      HttpListenerResponse response = context.Response;
      response.Headers.Clear();
      response.StatusCode = responseCode;
      byte[] outputData = System.Text.Encoding.UTF8.GetBytes(rawData);
      response.ContentLength64 = outputData.Length;
      response.OutputStream.Write(outputData, 0, outputData.Length);
      response.Close();
      mLog?.Write($"Http.Send - Sent [{outputData.Length}] bytes of data to client", LOG_TYPE.DBG);
      return RESP.SetSuccess();
    }

    public RESP Disconnect(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("Http.Disconnect", LOG_TYPE.DBG);
      return RESP.SetSuccess();
    }


    public RESP ConnectSendReceiveDisconnect(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("Http.ConnectSendReceiveDisconnect", LOG_TYPE.DBG);
      return RESP.SetSuccess();
    }


    public RESP Connect(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("Http.Connect", LOG_TYPE.DBG);
      return RESP.SetSuccess();
    }


    public RESP Receive(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("Http.Receive", LOG_TYPE.DBG);
      return RESP.SetSuccess();
    }
  }
}