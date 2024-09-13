using FlowEngineCore;
using OpenTK.Audio.OpenAL;
using ScottPlot.Colormaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineDesigner
{
  internal class cServerInformation
  {
    public string Profile = "";
    public bool WasLastConnected = false;

    public string Url = "";
    public int Port = 7002;

    public string LoginId = "";
    public bool RememberLoginId = true;
    public string Password = "";
    public bool RememberPassword = false;
                                     
    public string PrivateKey = "";

    public bool DebugAlways = true;
    public bool AutoConnect = true;

    public string LastFilePath = "/";


    public override string ToString()
    {
      return Profile;
    }
    private static string garbage = "ds!kvj#u^hvhu;834qun.,kifkjrfo[e]erfki2**&jorij43nfnukbh(jfsd$khuoi4~u3jfu5rmng0snxcd]d.whcvf'j/a8?*@}:{";
    private const int randomGarbageSize = 3;
    public void WriteXml(Xml xml)
    {
      if (RememberLoginId == false || RememberPassword == false)
        AutoConnect = false; //Can't auto connect if we don't know the user info

      xml.WriteTagStart("Server");
      xml.WriteTagAndContents("Profile", Profile);
      xml.WriteTagAndContents("WasLastConnected", WasLastConnected);
      xml.WriteTagAndContents("Url", Url);
      xml.WriteTagAndContents("Port", Port);
      if (RememberLoginId == true)
        xml.WriteTagAndContents("LoginId", LoginId);
      else
        xml.WriteTagAndContents("LoginId", "");
      if (RememberPassword == true)
        xml.WriteTagAndContents("Password", junkIt(Password));
      else
        xml.WriteTagAndContents("Password", "");

      xml.WriteTagAndContents("RememberLoginId", RememberLoginId);
      xml.WriteTagAndContents("RememberPassword", RememberPassword);
      xml.WriteTagAndContents("PrivateKey", junkIt(PrivateKey));
      xml.WriteTagAndContents("DebugAlways", DebugAlways);
      xml.WriteTagAndContents("AutoConnect", AutoConnect);
      xml.WriteTagAndContents("LastFilePath", LastFilePath);
      xml.WriteTagEnd("Server");
    }

    public void ReadXml(string xml)
    {
      WasLastConnected = Xml.GetXMLChunkAsBool(ref xml, "WasLastConnected");
      Profile = Xml.GetXMLChunk(ref xml, "Profile");
      Url = Xml.GetXMLChunk(ref xml, "Url");
      Port = Xml.GetXMLChunkAsInt(ref xml, "Port");
      LoginId = Xml.GetXMLChunk(ref xml, "LoginId");
      Password = noice(Xml.GetXMLChunk(ref xml, "Password"));
      PrivateKey = noice(Xml.GetXMLChunk(ref xml, "PrivateKey"));
      DebugAlways = Xml.GetXMLChunkAsBool(ref xml, "DebugAlways");
      AutoConnect = Xml.GetXMLChunkAsBool(ref xml, "AutoConnect");
      RememberLoginId = Xml.GetXMLChunkAsBool(ref xml, "RememberLoginId");
      RememberPassword = Xml.GetXMLChunkAsBool(ref xml, "RememberPassword");
      LastFilePath = Xml.GetXMLChunk(ref xml, "LastFilePath");
    }


    private string junkIt(string junk)
    {
      byte[] garbageBytes = System.Text.Encoding.UTF8.GetBytes(garbage);
      byte[] junkBytes = System.Text.Encoding.UTF8.GetBytes(junk);
      byte[] rand = SecureHasher.RandomByte(junk.Length + 1);
      
      List<byte> newVal = new List<byte>(rand.Length + junkBytes.Length);
      for (int x = 0; x < junkBytes.Length; x++)
      {
        newVal.Add(rand[x]);
        newVal.Add(junkBytes[x]);
      }
      newVal.Add(rand[rand.Length - 1]);
      for (int x = 0; x < newVal.Count; x++)
      {
        newVal[x] += garbageBytes[x % garbageBytes.Length];
      }

      return System.Convert.ToBase64String(newVal.ToArray());
    }

    private string noice(string junk)
    {
      byte[] garbageBytes = System.Text.Encoding.UTF8.GetBytes(garbage);
      byte[] data = System.Convert.FromBase64String(junk);
      List<byte> junkBytes = new List<byte>(data.Length);

      for (int x = 0; x < data.Length; x++)
      {
        if (x % 2 == 1)
          junkBytes.Add((byte)(data[x] - garbageBytes[x % garbageBytes.Length]));
      }

      return System.Text.Encoding.UTF8.GetString(junkBytes.ToArray());
    }
  }
}
