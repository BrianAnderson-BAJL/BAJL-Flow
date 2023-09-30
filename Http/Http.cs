using Core;
using System.Collections.Specialized;
using System.Drawing;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;

namespace Http
{
  public class Http : Core.Plugin
  {
    private Thread? ListenerThread;
    private HttpListener? Listener;

    private const string PARM_URL = "Url Path";
    private const string PARM_METHOD = "Method";
    private const string PARM_HOST = "Host";
    private const string PARM_CONNECTION_HANDLE = "connection_handle";
    private const string PARM_DATA_FORMAT = "data_format";

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
        SettingAddIfMissing(s);
        SettingAddIfMissing(s);
        SettingAddIfMissing(new Setting("", "Designer", "BackgroundColor", Color.Transparent));
        SettingAddIfMissing(new Setting("", "Designer", "BorderColor", Color.Orange));
        SettingAddIfMissing(new Setting("", "Designer", "FontColor", Color.Black));
      }
      //SETTINGS


      //FUNCTIONS
      {
        Function f = new Function("Connect", this, Connect);
        f.Parms.Add(PARM_URL, "");
        f.Parms.Add("Port", 443);
        f.Parms.Add("Timeout in ms", 5000);
        f.DefaultSaveResponseVariable = true;
        f.RespNames = new Variable(PARM_CONNECTION_HANDLE, DATA_TYPE.Object);
        Functions.Add(f); //Connect
        f = new Function("Send", this, Send);
        f.Parms.Add(PARM_CONNECTION_HANDLE, DATA_TYPE.Object);
        f.Parms.Add(Flow.VAR_DATA, "");
        PARM_DropDownList pddl = new PARM_DropDownList(PARM_DATA_FORMAT, PARM.PARM_REQUIRED.Yes, "JSON");
        pddl.OptionAdd("RAW");
        pddl.OptionAdd("JSON");
        pddl.OptionAdd("XML");
        f.Parms.Add(pddl);
        Functions.Add(f); //Send
        f = new Function("Receive", this, Receive);
        f.Parms.Add(PARM_CONNECTION_HANDLE, "");
        f.Parms.Add("Timeout in ms", 10000);
        f.DefaultSaveResponseVariable = true;
        f.RespNames = new Variable(Flow.VAR_REQUEST);
        f.RespNames.Add(new Variable(VAR_HEADERS));
        f.RespNames.Add(new Variable(Flow.VAR_DATA));
        Functions.Add(f); //Receive
        f = new Function("Disconnect", this, Disconnect);
        f.Parms.Add(PARM_CONNECTION_HANDLE, DATA_TYPE.Object);
        Functions.Add(f); //Disconnect
        f = new Function("ConnectSendReceiveDisconnect", this, ConnectSendReceiveDisconnect);
        f.Parms.Add(PARM_URL, "");
        f.Parms.Add("Port", 443);
        f.Parms.Add(Flow.VAR_DATA, "");
        f.Parms.Add("Connect Timeout in ms", 5000);
        f.Parms.Add("Receive Timeout in ms", 10000);
        f.DefaultSaveResponseVariable = true;
        f.RespNames = new Variable(Flow.VAR_REQUEST);
        f.RespNames.Add(new Variable(VAR_HEADERS));
        f.RespNames.Add(new Variable(Flow.VAR_DATA));
        Functions.Add(f); //ConnectSendReceiveDisconnect
      }
      //FUNCTIONS

      //FLOW START COMMANDS
      {
        FlowStartCommands.Add(PARM_URL, "/");
        PARM_DropDownList pddl = new PARM_DropDownList(PARM_METHOD, PARM.PARM_REQUIRED.Yes, "GET");
        pddl.OptionAdd("GET");
        pddl.OptionAdd("POST");
        pddl.OptionAdd("PUT");
        pddl.OptionAdd("DELETE");
        FlowStartCommands.Add(pddl);
        FlowStartCommands.Add(PARM_HOST, "");
      }
      //FLOW START COMMANDS

      //SAMPLE VARIABLES FOR DESIGNER
      {
        Variable v = new Variable(Flow.VAR_NAME_FLOW_START);
        Variable request = new Variable(Flow.VAR_REQUEST);
        request.Add(new Variable(PARM_CONNECTION_HANDLE, DATA_TYPE.Object));
        request.Add(new Variable(VAR_HEADERS, DATA_TYPE.Array));
        request.Add(new Variable(Flow.VAR_DATA));
        v.Add(request);
        SampleVariables.Add(Flow.VAR_NAME_FLOW_START, v);
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
        Global.Write("unable to start listening, HttpListenerException [" + e.Message + "]");
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
              if (request.ContentType == "application/json" && data != null && data.Length > 0)
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
        if (k != null)
        {
          VariableString headerVar = new VariableString(k, "");
          string[]? v = request.Headers.GetValues(k);
          if (v != null)
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
      if (uri == null)
        uri = new Uri("");
      string url = uri.LocalPath.ToString();
      url = Options.FixUrl(url);
      Global.Write(String.Format("Http Looking for flow Url [{0}], method [{1}], host [{2}]", url, method, host));
      for (int x = 0; x < Flows.Count; x++)
      {
        Flow f = Flows[x];
        PARM_Various? pUrl = f.StartCommands.FindParmByName(PARM_URL) as PARM_Various;
        PARM_DropDownList? pMethod = f.StartCommands.FindParmByName(PARM_METHOD) as PARM_DropDownList;
        PARM_Various? pHost = f.StartCommands.FindParmByName(PARM_HOST) as PARM_Various;
        if (pUrl != null && pMethod != null && pHost != null)
        {
          if (Options.FixUrl(pUrl.Value) == url && pMethod.Value == method && HostsMatch(pHost.Value, host) == true)
          {
            result.Add(f);
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

    public override void StartPlugin()
    {
      ListenerThread = new Thread(ListenerThreadRuntime);
      ListenerThread.Start();
    }

    public override void StopPlugin()
    {
      if (Listener != null)
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
    /// <param name="Parms 0">"connection_handle"</param>
    /// <param name="Parms 1">"data"</param>
    /// <param name="Resps">Output Index 0 = SUCCESS</param>
    /// <param name="Resps">Output Index 1 = ERROR</param>
    public static RESP Send(PARMS Parms)
    {
      Global.Write("Http.Send");
      if (Parms.Count != 3)
        return RESP.SetError(1, "Missing parameters");

      HttpListenerContext? context = Parms.ResolveObjectValue(PARM_CONNECTION_HANDLE) as HttpListenerContext;
      string? dataformat = Parms.ResolveDropDownListValue(PARM_DATA_FORMAT);
      Variable? data = Parms.ResolveVariable("data");
      if (context is null)
        return RESP.SetError(1, String.Format("Unable to resolve [{0}] parameter", PARM_CONNECTION_HANDLE));
      if (data is null)
        return RESP.SetError(1, "Unable to resolve [data] parameter");
      if (dataformat is null)
        return RESP.SetError(1, String.Format("Unable to resolve [{0}] parameter", PARM_DATA_FORMAT));

      string rawData = "";
      if (dataformat == "JSON")
        rawData = data.JsonCreate(true);
      else if (dataformat == "XML")
        rawData = ""; //TODO: Handle XML Data


      HttpListenerResponse response = context.Response;
      response.Headers.Clear();
      response.StatusCode = (int)HttpStatusCode.OK;
      byte[] outputData = System.Text.Encoding.UTF8.GetBytes(rawData);
      response.ContentLength64 = outputData.Length;
      response.OutputStream.Write(outputData, 0, outputData.Length);
      response.Close();
      Global.Write(String.Format("Http.Send - Sent [{0}] bytes of data to client", outputData.Length));
      return RESP.SetSuccess();
    }

    public static RESP Disconnect(PARMS Parms)
    {
      Global.Write("Http.Disconnect");
      return RESP.SetSuccess();
    }


    public static RESP ConnectSendReceiveDisconnect(PARMS Parms)
    {
      Global.Write("Http.ConnectSendReceiveDisconnect");
      return RESP.SetSuccess();
    }


    public static RESP Connect(PARMS Parms)
    {
      Global.Write("Http.Connect");
      return RESP.SetSuccess();
    }


    public static RESP Receive(PARMS Parms)
    {
      Global.Write("Http.Receive");
      return RESP.SetSuccess();
    }
  }
}