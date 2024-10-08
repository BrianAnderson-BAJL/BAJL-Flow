﻿using FlowEngineCore;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using System;
using System.Drawing;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Threading;

namespace Email
{
  public class Email : FlowEngineCore.Plugin
  {
    private enum RECONNECT
    {
      No,
      HadErrorNeedToReconnect,
    }

    private bool KeepRunning = true;
    private Thread? checkEmailThread;
    private object CriticalSection = new object();
    private Stack<SmtpClient> SmtpClients = new Stack<SmtpClient>();
    private int PoolSizeMax = 5;
    private int PoolSize = 0;

    private const string SETTING_NAME_MAIL_SERVER_URL = "Mail Server Url";
    private const string SETTING_NAME_MAIL_SERVER_PORT = "Mail Server Port";
    private const string SETTING_NAME_MAIL_SERVER_USER_NAME = "Mail Server User name";
    private const string SETTING_NAME_MAIL_SERVER_PASSWORD = "Mail Server Password";
    private const string SETTING_NAME_MAIL_SERVER_TIMEOUT = "Mail Server Timeout";
    private const string SETTING_NAME_MAIL_POOL_MIN_SIZE= "Mail Server Pool min size";
    private const string SETTING_NAME_MAIL_POOL_MAX_SIZE = "Mail Server Pool max size";

    private const int ERROR_EMAIL_BAD_TEMPLATE_PATH = (int)STEP_ERROR_NUMBERS.EmailErrorMin + 0;
    private const int ERROR_EMAIL_UNABLE_TO_SEND_EMAIL = (int)STEP_ERROR_NUMBERS.EmailErrorMin + 1;
    private const int ERROR_EMAIL_UNABLE_TO_GET_SMTP_CLIENT = (int)STEP_ERROR_NUMBERS.EmailErrorMin + 2;

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

      parm = new PARM("content images to attach", DATA_TYPE.String) { StringSubType = STRING_SUB_TYPE.TemplateFile };
      parm.Description = "(optional) If you are sending HTML in the body and have Content-ID images to link, then populate sub variables in this variable";
      func.Parms.Add(parm);

      parm = new PARM("files to attach", DATA_TYPE.String) { StringSubType = STRING_SUB_TYPE.TemplateFile};
      parm.Description = "(optional) If you need to attach files to the email then populate sub variables in this variable";
      func.Parms.Add(parm);

      Functions.Add(func);

      func = new Function("Load Template", this, LoadTemplate);
      parm = new PARM("FileName", DATA_TYPE.String) {StringSubType = STRING_SUB_TYPE.TemplateFile };
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
      mSettings.SettingAdd(new Setting(SETTING_NAME_MAIL_SERVER_TIMEOUT, 10000)); 
      mSettings.SettingAdd(new Setting(SETTING_NAME_MAIL_POOL_MIN_SIZE, 1L));
      mSettings.SettingAdd(new Setting(SETTING_NAME_MAIL_POOL_MAX_SIZE, 5L));

      mSettings.SettingAdd(new Setting("", "Designer", "BackgroundColor", Color.Transparent));
      mSettings.SettingAdd(new Setting("", "Designer", "BorderColor", Color.Blue));
      mSettings.SettingAdd(new Setting("", "Designer", "FontColor", Color.White));

    }

    public override void StartPlugin(Dictionary<string, object> GlobalPluginValues)
    {
      base.StartPlugin(GlobalPluginValues);

      int minPoolSize = mSettings.SettingGetAsInt(SETTING_NAME_MAIL_POOL_MIN_SIZE);
      PoolSizeMax = mSettings.SettingGetAsInt(SETTING_NAME_MAIL_POOL_MAX_SIZE);
      PoolCreateClient(minPoolSize);

      //checkEmailThread = new Thread(CheckEmailThread);
      //checkEmailThread.Start();

    }

    private void PoolCreateClient(int number)
    {
      string url = mSettings.SettingGetAsString(SETTING_NAME_MAIL_SERVER_URL);
      int port = mSettings.SettingGetAsInt(SETTING_NAME_MAIL_SERVER_PORT);
      string uid = mSettings.SettingGetAsString(SETTING_NAME_MAIL_SERVER_USER_NAME);
      string pwd = mSettings.SettingGetAsString(SETTING_NAME_MAIL_SERVER_PASSWORD);
      int timeout = mSettings.SettingGetAsInt(SETTING_NAME_MAIL_SERVER_TIMEOUT);

      for (int x = 0; x < number; x++)
      {
        SmtpClient mySmtpClient = new SmtpClient(url, port);
        mySmtpClient.Timeout = timeout;
        mySmtpClient.EnableSsl = true;
        mySmtpClient.UseDefaultCredentials = false;
        mySmtpClient.Credentials = new System.Net.NetworkCredential(uid, pwd);
        lock (CriticalSection)
        {
          SmtpClients.Push(mySmtpClient);
          PoolSize++;
        }
        mLog?.Write($"Created new email SmtpClient, pool size is now [{PoolSize}], url [{url}], port [{port}], timeout [{timeout}], uid [{uid}], pwd [{new string('*', pwd.Length)}]", LOG_TYPE.INF);
      }
    }

    private SmtpClient? PoolGetClient()
    {
      //If there is a SmtpClient available, just return it and go
      lock (CriticalSection)
      {
        if (SmtpClients.Count > 0)
        {
          return SmtpClients.Pop();
        }
        if (PoolSize < PoolSizeMax)
        {
          PoolCreateClient(1);
          return SmtpClients.Pop();
        }
      }

      //If we got here, need to wait for an existing connection
      int timeout = mSettings.SettingGetAsInt(SETTING_NAME_MAIL_SERVER_TIMEOUT);
      timeout = timeout * 2; //It could take longer for a connection to be returned
      TimeElapsed time = new TimeElapsed();
      while (time.HowLong().TotalMilliseconds < timeout)
      {
        Thread.Sleep(10);
        lock (CriticalSection)
        {
          if (SmtpClients.Count > 0)
          {
            return SmtpClients.Pop();
          }
        }
      }

      //Couldn't get a connection within the timeout period
      return null;
    }

    private void PoolReturnClient(SmtpClient client, RECONNECT reconnect = RECONNECT.No)
    {
      if (reconnect == RECONNECT.HadErrorNeedToReconnect)
      {
        client.Dispose();
        lock (CriticalSection)
        {
          PoolSize--;
        }
        mLog?.Write($"Email SmtpClient had an error, disposing and creating a new one, pool size is now [{PoolSize}]", LOG_TYPE.INF);
        PoolCreateClient(1);
        return;
      }

      //If we got here, no error, just add it back to the stack
      lock (CriticalSection)
      {
        SmtpClients.Push(client);
      }
    }

    public override void StopPlugin()
    {
      base.StopPlugin();
      KeepRunning = false;
    }

    private void CheckEmailThread()
    {
      //ImapClient imapClient = new ImapClient();
      //imapClient.Alert += ImapClient_Alert;
      //List<ImapEventGroup> eve = new List<ImapEventGroup>();
      //ImapMailboxFilter filter;
      
      //eve.Add(new ImapEventGroup(
      //imapClient.Notify(
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

    private void ImapClient_Alert(object? sender, AlertEventArgs e)
    {
      throw new NotImplementedException();
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

      SmtpClient? mySmtpClient = PoolGetClient();
      if (mySmtpClient is null)
      {
        return RESP.SetError(ERROR_EMAIL_UNABLE_TO_GET_SMTP_CLIENT, "Timedout apptempting to get SmtpClient");
      }
      RECONNECT reconnect = RECONNECT.No;
      try
      {

        if (fromEmailAddress is null || fromEmailAddress == "") //If no from email is set, lets just use the user id credentials
          fromEmailAddress = mSettings.SettingGetAsString(SETTING_NAME_MAIL_SERVER_USER_NAME);

        if (toEmailAddressName == "")
          toEmailAddressName = null;
        if (fromEmailAddressName == "")
          fromEmailAddressName = null;



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
        //On an error, lets kill the connection and force it to be recreated
        reconnect = RECONNECT.HadErrorNeedToReconnect;
        return RESP.SetError(ERROR_EMAIL_UNABLE_TO_SEND_EMAIL, ex.Message);
      }
      catch
      {
        //On an error, lets kill the connection and force it to be recreated
        reconnect = RECONNECT.HadErrorNeedToReconnect;
        throw;
      }
      finally
      {
        PoolReturnClient(mySmtpClient, reconnect);
      }

      return RESP.SetSuccess();
    }
  }
}
