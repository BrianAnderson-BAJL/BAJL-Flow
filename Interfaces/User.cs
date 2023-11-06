using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
  public class User
  {
    public enum TO_XML
    {
      UserFile,
      LoggingOnly,
    }

    public string LoginId = "";
    public string passwordHash = "";
    public string NameFirst = "";
    public string NameSur = "";
    public string SecurityProfile = "";
    public string ServerKey = "";
    public string SessionKey = "";
    public int LoginAttempts = 0;
    public DateTime LockOutUntil = DateTime.UtcNow;
    public bool NeedToChangePassword = false;

    public DateTime SessionKeyExpiration = DateTime.MinValue;
    public DateTime ModifiedDateTime = DateTime.MinValue;

    public void FromXml(ref string userBlockXml)
    {
      LoginId = Xml.GetXMLChunk(ref userBlockXml, "LoginId");
      passwordHash = Xml.GetXMLChunk(ref userBlockXml, "PasswordHash");
      NameFirst = Xml.GetXMLChunk(ref userBlockXml, "FirstName");
      NameSur = Xml.GetXMLChunk(ref userBlockXml, "SurName");
      SecurityProfile = Xml.GetXMLChunk(ref userBlockXml, "SecurityProfile");
      ModifiedDateTime = Xml.GetXMLChunkAsDateTime(ref userBlockXml, "ModifiedDateTime");
      NeedToChangePassword = Xml.GetXMLChunkAsBool(ref userBlockXml, "NeedToChangePassword");
    }

    public string ToXml(DateTime CurrentDateTime, TO_XML Minimal = TO_XML.UserFile, int indentLevel = 0)
    {
      Xml xml = new Xml();
      xml.WriteMemoryNew(indentLevel);

      xml.WriteTagStart("User");
      xml.WriteTagAndContents("LoginId", LoginId);
      xml.WriteTagAndContents("FirstName", NameFirst);
      xml.WriteTagAndContents("SurName", NameSur);
      if (Minimal == TO_XML.UserFile) //We only want the security profile in the user.xml file, other things might be wrapped with this in the future.
      {
        xml.WriteTagAndContents("PasswordHash", passwordHash);
        xml.WriteTagAndContents("SecurityProfile", SecurityProfile);
      }
      if (ModifiedDateTime == DateTime.MinValue)
      {
        xml.WriteTagAndContents("ModifiedDateTime", CurrentDateTime);
      }
      else
      {
        xml.WriteTagAndContents("ModifiedDateTime", ModifiedDateTime);
      }
      xml.WriteTagAndContents("NeedToChangePassword", NeedToChangePassword);
      xml.WriteTagEnd("User");

      return xml.ReadMemory();
    }

  }
}
