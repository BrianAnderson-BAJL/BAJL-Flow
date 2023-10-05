using Core.Administration.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
  internal class UserManager
  {
    private static List<User> Users = new List<User>(128);
    private static object mCriticalSection = new object();
    //private static UserManager userManager = new UserManager();

    private UserManager() //private constructor, so can't be created except by the singleton
    {

    }

    public static IReadOnlyList<User> GetUsers
      {
      get { return Users.AsReadOnly(); }
      }
   
    public static int UserCount
    {
      get {return Users.Count;}
    }

    public static User? FindBySessionKey(string sessionKey)
    {
      
      lock (mCriticalSection)
      {
        for (int x = 0; x < Users.Count; x++)
        {
          if (Users[x].SessionKey == sessionKey)
          {
            return Users[x];
          }
        }
      }
      return null;
    }

    public static User? FindByLoginId(string loginId)
    {
      loginId = loginId.ToLower();
      lock (mCriticalSection)
      {
        for (int x = 0; x < Users.Count; x++)
        {
          if (Users[x].LoginId.ToLower() == loginId)
            return Users[x];
        }
      }
      return null;
    }

    public static RECORD_RESULT CheckLoginIdInUse(string loginId, out string newLoginId)
    {
      int Count = 1;
      newLoginId = loginId;
      string TempLoginId = loginId;
      while (FindByLoginId(TempLoginId) is not null)
      {
        TempLoginId = loginId + Count.ToString();
        Count++;
        if (Count > 100)
          break;
      }
      if (TempLoginId == loginId)
      {
        return RECORD_RESULT.Success;
      }
      else
      {
        newLoginId = TempLoginId;
        return RECORD_RESULT.Duplicate;
      }
    }

    public static void FileWrite()
    {
      Xml xml = new Xml();
      xml.WriteFileNew(Options.UserPath);
      xml.WriteTagStart("Users");
      lock (mCriticalSection)
      {
        for (int x = 0; x < Users.Count; x++)
        {
          string user = Users[x].ToXml(DateTime.UtcNow, User.TO_XML.UserFile, xml.IndentLevel);
          xml.WriteXml(user);
        }
      }
      xml.WriteTagEnd("Users");
      xml.WriteFileClose();
    }

    public static void FileLoad()
    {
      Core.Xml xml = new Core.Xml();
      string users = xml.FileRead(Options.UserPath);
      users = Xml.GetXMLChunk(ref users, "Users");
      string userXml = Xml.GetXMLChunk(ref users, "User");
      List<User> userList = new List<User>(128);
      while (userXml.Length > 0)
      {
        User user = new User();
        user.FromXml(ref userXml);
        userList.Add(user);
        userXml = Xml.GetXMLChunk(ref users, "User");
      }
      lock (mCriticalSection)
      {
        Users.Clear();
        for (int x = 0; x < userList.Count; x++)
        {
          Users.Add(userList[x]);
        }
      }
    }



    public static RECORD_RESULT Add(UserAdd? userMessage)
    {
      if (userMessage is null)
        return RECORD_RESULT.Error;

      User user = new User();
      user.LoginId = userMessage.LoginId;
      user.passwordHash = SecureHasherV1.Hash(userMessage.Password);
      user.NameFirst = userMessage.NameFirst;
      user.NameSur = userMessage.NameSur;
      user.ModifiedDateTime = DateTime.UtcNow;
      user.SecurityProfile = userMessage.SecurityProfile;
      user.NeedToChangePassword = true;
      lock (mCriticalSection)
      {
        if (FindByLoginId(userMessage.LoginId) is not null)
          return RECORD_RESULT.Duplicate;

        Users.Add(user);
        FileWrite();
      }
      return RECORD_RESULT.Success;
    }

    public static RECORD_RESULT Add(string loginId, string password, string firstName, string surName, string securityProfile, DateTime modifiedDateTime)
    {

      User user = new User();
      user.LoginId = loginId;
      user.passwordHash = SecureHasherV1.Hash(password);
      user.NameFirst = firstName;
      user.NameSur = surName;
      user.ModifiedDateTime = DateTime.UtcNow;
      user.SecurityProfile = securityProfile;
      user.NeedToChangePassword = true;
      lock (mCriticalSection)
      {
        if (FindByLoginId(loginId) is not null)
          return RECORD_RESULT.Duplicate;

        Users.Add(user);
        FileWrite();
      }
      return RECORD_RESULT.Success;
    }

    public static bool Delete(string loginId) 
    {
      loginId = loginId.ToLower();
      lock (mCriticalSection)
      {
        for (int x = 0; x < Users.Count; x++)
        {
          if (Users[x].LoginId.ToLower() == loginId)
          {
            Users.RemoveAt(x);
            UserManager.FileWrite();
            return true;
          }
        }
      }
      return false;
    }

  }
}
