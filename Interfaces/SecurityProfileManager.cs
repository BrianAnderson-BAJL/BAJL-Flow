using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FlowEngineCore.SecurityProfile;

namespace FlowEngineCore
{
  internal class SecurityProfileManager
  {
    private static List<SecurityProfile> Profiles = new List<SecurityProfile>(8);
    private static object mCriticalSection = new object();


    public static IReadOnlyList<SecurityProfile> GetProfiles
    {
      get { return Profiles.AsReadOnly(); }
    }

    public static SecurityProfile FindByName(string name)
    {
      name = name.ToLower();
      lock (mCriticalSection)
      {
        for (int x = 0; x < Profiles.Count; x++)
        {
          if (Profiles[x].Name.ToLower() == name)
            return Profiles[x];
        }
      }
      return SecurityProfile.NoAccess;
    }

    private static SecurityProfile? FindByNameNullReturnable(string name)
    {
      name = name.ToLower();
      lock (mCriticalSection)
      {
        for (int x = 0; x < Profiles.Count; x++)
        {
          if (Profiles[x].Name.ToLower() == name)
            return Profiles[x];
        }
      }
      return null;
    }


    public static void Save()
    {
      Xml xml = new Xml();
      xml.WriteFileNew(Options.GetFullPath(Options.GetSettings.SettingGetAsString("SecurityProfilesPath")));
      xml.WriteTagStart("Profiles");
      lock (mCriticalSection)
      {
        for (int x = 0; x < Profiles.Count; x++)
        {
          SecurityProfile sp = Profiles[x];
          xml.WriteTagStart("Profile");
          xml.WriteTagAndContents("Name", sp.Name);
          xml.WriteTagAndContents("AdministrationUsers", sp.AdministrationUsers);
          xml.WriteTagAndContents("AdministrationSecurityProfiles", sp.AdministrationSecurityProfiles);
          xml.WriteTagAndContents("AdministrationFlows", sp.AdministrationFlows);
          xml.WriteTagEnd("Profile");
        }
      }
      xml.WriteTagEnd("Profiles");
      xml.WriteFileClose();
    }

    public static void Load()
    {
      FlowEngineCore.Xml xml = new FlowEngineCore.Xml();
      string profiles = xml.FileRead(Options.GetFullPath(Options.GetSettings.SettingGetAsString("SecurityProfilesPath")));
      profiles = Xml.GetXMLChunk(ref profiles, "Profiles");
      string profileXml = Xml.GetXMLChunk(ref profiles, "Profile");
      List<SecurityProfile> spList = new List<SecurityProfile>(128);
      while (profileXml.Length > 0)
      {
        SecurityProfile sp = new SecurityProfile();
        sp.Name = Xml.GetXMLChunk(ref profileXml, "Name");
        string temp = Xml.GetXMLChunk(ref profileXml, "AdministrationUsers");
        Enum.TryParse<SecurityProfile.SECURITY_ACCESS_LEVEL>(temp, true, out sp.AdministrationUsers);
        temp = Xml.GetXMLChunk(ref profileXml, "AdministrationSecurityProfiles");
        Enum.TryParse<SecurityProfile.SECURITY_ACCESS_LEVEL>(temp, true, out sp.AdministrationSecurityProfiles);
        temp = Xml.GetXMLChunk(ref profileXml, "AdministrationFlows");
        Enum.TryParse<SecurityProfile.SECURITY_ACCESS_LEVEL>(temp, true, out sp.AdministrationFlows);

        spList.Add(sp);
        profileXml = Xml.GetXMLChunk(ref profiles, "Profile");
      }
      lock (mCriticalSection)
      {
        Profiles.Clear();
        for (int x = 0; x < spList.Count; x++)
        {
          Profiles.Add(spList[x]);
        }
      }
    }

    public static RECORD_RESULT Add(FlowEngineCore.Administration.Messages.SecurityProfileAdd spa)
    {
      SecurityProfile? sp = FindByNameNullReturnable(spa.Name);
      if (sp is not null)
        return RECORD_RESULT.Duplicate;

      sp = new SecurityProfile();
      sp.Name = spa.Name;
      sp.AdministrationUsers = spa.AdministrationUsers;
      sp.AdministrationSecurityProfiles = spa.AdministrationSecurityProfiles;
      sp.AdministrationFlows = spa.AdministrationFlows;

      lock (mCriticalSection)
      {
        Profiles.Add(sp);
        Save();
      }
      return RECORD_RESULT.Success;
    }

    public static RECORD_RESULT Edit(FlowEngineCore.Administration.Messages.SecurityProfileEdit spe)
    {
      SecurityProfile? sp = FindByNameNullReturnable(spe.OldName);
      if (sp is null)
        return RECORD_RESULT.Error;

      sp.Name = spe.Name;
      sp.AdministrationUsers = spe.AdministrationUsers;
      sp.AdministrationSecurityProfiles = spe.AdministrationSecurityProfiles;
      sp.AdministrationFlows = spe.AdministrationFlows;

      Save();
      return RECORD_RESULT.Success;
    }
    public static RECORD_RESULT Delete(FlowEngineCore.Administration.Messages.SecurityProfileDelete spd)
    {
      lock (mCriticalSection)
      {
        for (int x = 0; x < Profiles.Count; x++)
        {
          if (Profiles[x].Name.ToLower() == spd.Name.ToLower())
          {
            Profiles.RemoveAt(x);
            Save();
            return RECORD_RESULT.Success;
          }
        }
      }
      return RECORD_RESULT.Error;
    }
  }
}
