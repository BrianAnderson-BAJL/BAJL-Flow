using FlowEngineCore;
using FlowEngineCore.Interfaces;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using static FlowEngineCore.PARM;

namespace Http
{

  public class Http : FlowEngineCore.Plugin
  {
    private class RestParameter
    {
      public int StartForwardSlashCount;
      public DATA_TYPE DataType = DATA_TYPE.String;
      public string Name;

      public RestParameter(int startForwardSlashCount, DATA_TYPE dataType, string name)
      {
        StartForwardSlashCount = startForwardSlashCount;
        DataType = dataType;
        Name = name;
      }
    }

    private class UrlParameter
    {
      public string Name;
      public DATA_TYPE DataType;

      public UrlParameter(string name, DATA_TYPE dataType)
      {
        Name = name;
        DataType = dataType;
      }
    }

    private Thread? ListenerThread;
    private HttpListener? Listener;

    private const string SETTING_URL_PARAMS_LOWERCASE = "Set all URL parameter keys to lowercase";
    private const string SETTING_ERROR_NO_FLOW_MESSAGE = "Error Message For No Flow";
    private const string PARM_URL = "Url Path";
    private const string PARM_METHOD = "Method";
    private const string PARM_HOST = "Host";
    private const string PARM_CONNECTION_HANDLE = "network_conn_handle";
    private const string PARM_DATA_FORMAT = "data_format";

    private const string PARM_DATA_FORMAT_RAW = "RAW";
    private const string PARM_DATA_FORMAT_JSON = "JSON";
    private const string PARM_DATA_FORMAT_XML = "XML";
    private const string VAR_HEADERS = "headers";
    private const string VAR_PARAMETERS = "parameters";
    private const string REST_PARAMETERS = "rest_parameters";
    private const string URL_PARAMETERS = "url_parameters";


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
        mSettings.SettingAdd(new Setting(SETTING_ERROR_NO_FLOW_MESSAGE, "No flow capable of handling request."));
        mSettings.SettingAdd(new Setting(SETTING_URL_PARAMS_LOWERCASE, true));
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

        
        PARM tmpParm = function.Parms.Add(Flow.VAR_DATA, DATA_TYPE.String);
        tmpParm.NameChangeable = true;
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
        root.SubVariableAdd(new Variable(PARM_CONNECTION_HANDLE));
        Variable headers = new Variable(VAR_HEADERS);
        root.SubVariableAdd(headers);
        Variable parameters = new Variable(VAR_PARAMETERS);
        root.SubVariableAdd(parameters);
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
      string noFlowErrorMessage = mSettings.SettingGetAsString(SETTING_ERROR_NO_FLOW_MESSAGE);
      bool queryStringLowercase = mSettings.SettingGetAsBoolean(SETTING_URL_PARAMS_LOWERCASE);
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
          }
          try
          {
            List<Flow> flows = FindFlows(request.Url, request.HttpMethod, request.UserHostName);
            if (flows.Count == 0) //No flow found, return error
            {
              HttpListenerResponse response = context.Response;
              response.Headers.Clear();
              response.StatusCode = (int)HttpStatusCode.NotFound; //Return 404 - NOT FOUND
              if (Options.ServerType == Options.SERVER_TYPE.Development) //Only send back error messages if it is a development server
              {
                byte[] outputData = System.Text.Encoding.UTF8.GetBytes(noFlowErrorMessage);
                response.ContentLength64 = outputData.Length;
                response.OutputStream.Write(outputData, 0, outputData.Length);
              }
              response.Close();
              mLog?.Write($"No flows found for request! Url [{request.Url}], HttpMethod [{request.HttpMethod}], UserHostName [{request.UserHostName}]", LOG_TYPE.INF);
              continue;
            }

            Variable baseVar = new Variable(Flow.VAR_NAME_FLOW_START);
            baseVar.SubVariableAdd(new Variable(PARM_CONNECTION_HANDLE, context));
            baseVar.SubVariableAdd(GetHeaders(request));
            
            if (request.ContentType == "application/json" && data is not null && data.Length > 0)
            {
              try
              {
                Variable? v = Variable.JsonParse(ref data, "data");
                baseVar.SubVariableAdd(v);
              }
              catch (Exception ex) //Bad JSON data
              {
                HttpListenerResponse response = context.Response;
                response.Headers.Clear();
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                if (Options.ServerType == Options.SERVER_TYPE.Development) //Only send back error messages if it is a development server
                {
                  byte[] outputData = System.Text.Encoding.UTF8.GetBytes(ex.Message);
                  response.ContentLength64 = outputData.Length;
                  response.OutputStream.Write(outputData, 0, outputData.Length);
                }
                response.Close();
                mLog?.Write(ex, LOG_TYPE.INF);
                continue;
              }
            }

            if (flows.Count > 0)
            {
              for (int flowIndex = 0; flowIndex < flows.Count; flowIndex++)
              {
                Variable varParam;
                try
                {
                  varParam = GetRestParameters(flows[flowIndex], request);
                }
                catch (Exception ex)
                {
                  HttpListenerResponse response = context.Response;
                  response.Headers.Clear();
                  response.StatusCode = (int)HttpStatusCode.BadRequest;
                  if (Options.ServerType == Options.SERVER_TYPE.Development) //Only send back error messages if it is a development server
                  {
                    byte[] outputData = System.Text.Encoding.UTF8.GetBytes(ex.Message);
                    response.ContentLength64 = outputData.Length;
                    response.OutputStream.Write(outputData, 0, outputData.Length);
                  }
                  response.Close();
                  mLog?.Write("Failed to get correct REST Parameters", ex, LOG_TYPE.INF);

                  continue;
                }

                for (int x = 0; x < request.QueryString.Count; x++)
                {
                  string? key = request.QueryString.GetKey(x);
                  string[]? values = request.QueryString.GetValues(x);
                  if (key is null || values is null || values.Length == 0)
                    continue;
                  if (queryStringLowercase == true)
                    key = key.ToLower();

                  Variable queryVar = new Variable(key);
                  if (values.Length > 1)
                  {
                    queryVar.SubVariablesFormat = DATA_FORMAT_SUB_VARIABLES.Array;
                    queryVar.SubVariableAdd(new Variable("", values[0]));
                  }
                  else
                  {
                    queryVar.Value = values[0];
                    queryVar.DataType = DATA_TYPE.String;
                  }
                  varParam.SubVariableAdd(queryVar);
                }
                
                string ? threadName = FlowEngine.GetNextThreadName();
                mLog?.Write($"HTTP received a request [{data}]", LOG_TYPE.INF, threadName);
                Variable var = baseVar;
                var.SubVariableAdd(varParam);
                if (flowIndex > 1) //If we have more than one flow, we need to clone the variable
                  var = baseVar.Clone();
                FlowEngine.StartFlow(new FlowRequest(var, this, flows[flowIndex], FlowRequest.START_TYPE.WaitForEvent, FlowRequest.CLONE_FLOW.CloneFlow, threadName));
              }
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

    private Variable GetRestParameters(Flow flow, HttpListenerRequest request)
    {
      Variable param = new Variable(VAR_PARAMETERS);

      if (request.Url is null)
        return param;


      List<RestParameter>? restParam = flow.ExtraValues[VAR_PARAMETERS] as List<RestParameter>;
      if (restParam is null)
        return param;

      //PARM_VAR? pUrl = flow.StartCommands.FindByParmName(PARM_URL);
      //if (pUrl is null)
      //  return param;

      string url = request.Url.ToString();


      int index = -1;
      int count = 0;
      //Variable? paramSubVariable;
      do
      {
        index = url.IndexOf('/', index + 1);
        if (index == -1)
          break;
        count++;
        for (int x = 0; x < restParam.Count; x++)
        {
          if (count == restParam[x].StartForwardSlashCount)
          {
            int endPos = url.IndexOf('/', index + 1);
            if (endPos == -1)
              endPos = url.IndexOf('?', index + 1);
            if (endPos == -1)
              endPos = url.Length;
            string val = url.Substring(index + 1, endPos - index - 1);
            string parmName = restParam[x].Name;
            DATA_TYPE dt = restParam[x].DataType;
            try
            {
              if (dt == DATA_TYPE.String)
                param.SubVariableAdd(new Variable(parmName, val));
              else if (dt == DATA_TYPE.Integer)
                param.SubVariableAdd(new Variable(parmName, long.Parse(val)));
              else if (dt == DATA_TYPE.Decimal)
                param.SubVariableAdd(new Variable(parmName, decimal.Parse(val)));
              else if (dt == DATA_TYPE.Boolean)
                param.SubVariableAdd(new Variable(parmName, bool.Parse(val)));
            }
            catch (Exception ex)
            {
              throw new Exception($"Failed to convert REST parameter [{parmName}:{val}] to [{dt}]", ex);
            }
          }
        }
      } while (index > 0);

      return param;
    }

    //private Variable? FindNextParameter(ref string paramUrl, string actualUrl)
    //{
    //  Variable? found = null;
    //  int startPos = paramUrl.IndexOf('<');
    //  int endPos = paramUrl.IndexOf('>');
    //  if (startPos > -1 && endPos > startPos)
    //  {
    //    int forwardSlashesToSkip = CountOfBefore(paramUrl, startPos);
    //    string paramName = paramUrl.Substring(startPos, endPos - startPos);

    //    found = new Variable(paramName);
    //  }
    //  return found;
    //}

    private RestParameter? FindNextParameterDefinition(ref string paramUrl)
    {
      RestParameter? found = null;
      int startPos = paramUrl.IndexOf('<');
      int endPos = paramUrl.IndexOf('>');
      bool queryStringLowercase = mSettings.SettingGetAsBoolean(SETTING_URL_PARAMS_LOWERCASE);

      if (startPos <= -1)
        return null;
      if (endPos <= startPos)
        return null;

      int forwardSlashesToSkip = CountOfBefore(paramUrl, startPos);
      forwardSlashesToSkip += 2; //Need to add 2 for the 2 forward slashes in 'HTTPS://'
      string paramDataTypeAndName = paramUrl.Substring(startPos + 1, endPos - (startPos + 1));
      string left = paramUrl.Substring(0, startPos);
      paramUrl = left + paramUrl.Substring(endPos + 1);
      string[] paramDataTypeAndNameSplit = paramDataTypeAndName.Split(':', StringSplitOptions.TrimEntries);
      if (paramDataTypeAndNameSplit.Length == 2)
      {
        string dataTypeStr = paramDataTypeAndNameSplit[0];
        string name = paramDataTypeAndNameSplit[1];
        if (queryStringLowercase == true)
          name = name.ToLower();
        DATA_TYPE dataType = Enum.Parse<DATA_TYPE>(dataTypeStr);
        found = new RestParameter(forwardSlashesToSkip, dataType, name);
      }
      else if (paramDataTypeAndNameSplit.Length == 1)
      {
        //If data type isn't defined then it defaults to String.
        string name = paramDataTypeAndNameSplit[0];
        if (queryStringLowercase == true)
          name = name.ToLower();
        found = new RestParameter(forwardSlashesToSkip, DATA_TYPE.String, name);
      }
      return found;
    }
    private int CountOfBefore(string paramUrl, int beforeCharIndex, char charToCount = '/')
    {
      int count = 0;
      for (int x = 0; x < beforeCharIndex; x++)
      {
        if (paramUrl[x] == charToCount)
          count++;
      }
      return count;
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
          Flow flow = Flows[x];
          PARM_VAR? pUrl = flow.StartCommands.FindByParmName(PARM_URL);
          PARM_VAR? pMethod = flow.StartCommands.FindByParmName(PARM_METHOD);
          PARM_VAR? pHost = flow.StartCommands.FindByParmName(PARM_HOST);
          if (pUrl is not null && pMethod is not null && pHost is not null)
          {
            pUrl.GetValue(out string flowUrl, flow);
            pMethod.GetValue(out string flowMethod, flow);
            pHost.GetValue(out string flowHost, flow);
            Options.FixUrl(ref url, ref flowUrl); //May need to chop off parameters for checking
            if (flowUrl == url && flowMethod == method && HostsMatch(flowHost, host) == true)
            {
              result.Add(flow);
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

    public override void FlowAdd(Flow flow)
    {
      PARM_VAR? pUrl = flow.StartCommands.FindByParmName(PARM_URL);
      if (pUrl is null)
        return;

      string paramUrl = pUrl.Var.GetValueAsString();
      List<RestParameter> restParameters = new List<RestParameter>();
      RestParameter? restParam;
      do
      {
        restParam = FindNextParameterDefinition(ref paramUrl);
        if (restParam is not null)
          restParameters.Add(restParam);
      } while (restParam is not null);
      flow.ExtraValues.Add(VAR_PARAMETERS, restParameters);
      base.FlowAdd(flow);
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

      HttpListenerResponse response = context.Response;
      response.Headers.Clear();

      string rawData = "";
      if (dataformat == PARM_DATA_FORMAT_JSON)
      {
        rawData = data.ToJson();
        response.Headers.Add("Content-Type", "application/json");
      }
      else if (dataformat == PARM_DATA_FORMAT_XML)
        throw new NotImplementedException(PARM_DATA_FORMAT_XML + " is not implemented yet");
      else if (dataformat == PARM_DATA_FORMAT_RAW)
        rawData = data.Value;

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