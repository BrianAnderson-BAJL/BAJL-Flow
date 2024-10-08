﻿using FlowEngineCore.Administration.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowEngineDesigner
{
  partial class frmAbout : Form
  {
    public frmAbout()
    {
      InitializeComponent();
      this.Text = String.Format("About {0}", AssemblyTitle);
      this.labelProductName.Text = AssemblyProduct;
      this.labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
      this.labelCopyright.Text = AssemblyCopyright;
      this.labelCompanyName.Text = AssemblyCompany;
      this.textBoxDescription.Text = AssemblyDescription;
    }

    #region Assembly Attribute Accessors

    public string AssemblyTitle
    {
      get
      {
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
        if (attributes.Length > 0)
        {
          AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
          if (titleAttribute.Title != "")
          {
            return titleAttribute.Title;
          }
        }
        return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
      }
    }

    public string AssemblyVersion
    {
      get
      {
        return Assembly.GetExecutingAssembly().GetName().Version.ToString();
      }
    }

    public string AssemblyDescription
    {
      get
      {
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
        if (attributes.Length == 0)
        {
          return "";
        }
        return ((AssemblyDescriptionAttribute)attributes[0]).Description;
      }
    }

    public string AssemblyProduct
    {
      get
      {
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
        if (attributes.Length == 0)
        {
          return "";
        }
        return ((AssemblyProductAttribute)attributes[0]).Product;
      }
    }

    public string AssemblyCopyright
    {
      get
      {
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
        if (attributes.Length == 0)
        {
          return "";
        }
        return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
      }
    }

    public string AssemblyCompany
    {
      get
      {
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
        if (attributes.Length == 0)
        {
          return "";
        }
        return ((AssemblyCompanyAttribute)attributes[0]).Company;
      }
    }
    #endregion

    private void frmAbout_Load(object sender, EventArgs e)
    {
      ServerPluginsGet pluginsGet = new ServerPluginsGet(cServer.Info.PrivateKey, cServer.UserLoggedIn!.SessionKey);
      cServer.SendAndResponse(pluginsGet.GetPacket(), Callback_ServerPlugins);
    }

    private void Callback_ServerPlugins(FlowEngineCore.Administration.EventArgsPacket e)
    {
      if (e.Packet.PeekResponseCode() != BaseResponse.RESPONSE_CODE.Success)
        return;

      ServerPluginsGetResponse data = new ServerPluginsGetResponse(e.Packet);
      lvPlugins.Items.Clear();
      for (int x = 0; x < data.Plugins.Count; x++)
      {
        ListViewItem lvi = lvPlugins.Items.Add(data.Plugins[x].Name);
        lvi.SubItems.Add(data.Plugins[x].VersionAssembly);
        lvi.SubItems.Add(data.Plugins[x].VersionFile);
      }
    }

    private void okButton_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
