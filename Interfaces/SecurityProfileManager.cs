﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.SecurityProfile;

namespace Core
{
  internal class SecurityProfileManager
  {
    private static List<SecurityProfile> Profiles = new List<SecurityProfile>(8);
    private static object mCriticalSection = new object();


    public static IReadOnlyList<SecurityProfile> GetProfiles
    {
      get { return Profiles.AsReadOnly(); }
    }

    public static SecurityProfile? FindByName(string name)
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

    public static void FileWrite()
    {
      Xml xml = new Xml();
      xml.WriteFileNew(Options.SecurityProfilesPath);
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

    public static void FileLoad()
    {
      Core.Xml xml = new Core.Xml();
      string profiles = xml.FileRead(Options.SecurityProfilesPath);
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

    public static RECORD_RESULT Add(Core.Administration.Messages.SecurityProfileAdd spa)
    {
      SecurityProfile? sp = FindByName(spa.Name);
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
        FileWrite();
      }
      return RECORD_RESULT.Success;
    }

    public static RECORD_RESULT Edit(Core.Administration.Messages.SecurityProfileEdit spe)
    {
      SecurityProfile? sp = FindByName(spe.OldName);
      if (sp is null)
        return RECORD_RESULT.Error;

      sp.Name = spe.Name;
      sp.AdministrationUsers = spe.AdministrationUsers;
      sp.AdministrationSecurityProfiles = spe.AdministrationSecurityProfiles;
      sp.AdministrationFlows = spe.AdministrationFlows;

      FileWrite();
      return RECORD_RESULT.Success;
    }
    public static RECORD_RESULT Delete(Core.Administration.Messages.SecurityProfileDelete spd)
    {
      lock (mCriticalSection)
      {
        for (int x = 0; x < Profiles.Count; x++)
        {
          if (Profiles[x].Name.ToLower() == spd.Name.ToLower())
          {
            Profiles.RemoveAt(x);
            FileWrite();
            return RECORD_RESULT.Success;
          }
        }
      }
      return RECORD_RESULT.Error;
    }
  }
}