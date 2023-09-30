﻿using Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowEngineDesigner
{
  public partial class frmFlowProperties : Form
  {
    private cFlowWrapper Flow;
    private List<ucParameter> userControls = new List<ucParameter>();

    public frmFlowProperties(cFlowWrapper flow)
    {
      Flow = flow;
      InitializeComponent();
      WinApi.SetTabWidth(txtSampleData, 4);
    }

    private void frmFlowProperties_Load(object sender, EventArgs e)
    {
      for (int x = 0; x < PluginManager.Plugins.Count; x++)
      {
        cmbPlugins.Items.Add(PluginManager.Plugins[x]);
        if (Flow.StartPlugin != null && PluginManager.Plugins[x].Name == Flow.StartPlugin.Name)
        {
          cmbPlugins.SelectedIndex = x;
        }
      }
      cmbPlugins.SelectedIndexChanged += cmbPlugins_SelectedIndexChanged;
      txtCreatedDateTime.Text = Flow.CreateDateTime.ToString();
      txtModifiedLastDateTime.Text = Flow.ModifiedLastDateTime.ToString();
      if (Flow.SampleDataFormat == DATA_FORMAT.Json)
      {
        rbSampleDataFormatJson.Checked = true;
      }
      else if (Flow.SampleDataFormat == DATA_FORMAT.Xml)
        rbSampleDataFormatXml.Checked = true;
      txtSampleData.Text = Flow.SampleData;

      AddFlowStartCommands();
    }

    private void label3_Click(object sender, EventArgs e)
    {

    }

    private void cmbPlugins_SelectedIndexChanged(object? sender, EventArgs e)
    {
      Flow.StartPlugin = cmbPlugins.SelectedItem as Core.Plugin;
      if (Flow.StartPlugin != null)
      {
        Flow.StartCommands = Flow.StartPlugin.FlowStartCommands.Clone();
      }
      AddFlowStartCommands();
    }

    private void AddFlowStartCommands()
    {
      if (Flow.StartPlugin == null)
        return;

      for (int x = 0; x < userControls.Count; x++)
      {
        gbStartOptions.Controls.Remove(userControls[x]);
      }
      userControls.Clear();


      int StartX = 10;
      int StartY = 20;
      for (int x = 0; x < Flow.StartCommands.Count; x++)
      {

        PARM p = Flow.StartCommands[x];


        PARM_Integer? ParmInt = p as PARM_Integer;
        if (ParmInt != null)
        {
          ucParameterInteger uc = new ucParameterInteger(ParmInt);
          uc.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
          uc.Location = new Point(StartX, StartY + (uc.Size.Height * x) + 5);
          gbStartOptions.Controls.Add(uc);
          userControls.Add(uc);
        }
        PARM_Various? ParmStr = p as PARM_Various;
        if (ParmStr != null)
        {
          ucParameterString uc = new ucParameterString(ParmStr, null, Flow);
          uc.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
          uc.Location = new Point(StartX, StartY + (uc.Size.Height * x) + 5);
          gbStartOptions.Controls.Add(uc);
          userControls.Add(uc);

        }
        PARM_Decimal? ParmDec = p as PARM_Decimal;
        if (ParmDec != null)
        {
          //txtValue.Text = ParmDec.Value.ToString();
        }
        PARM_DropDownList? ParmDdl = p as PARM_DropDownList;
        if (ParmDdl != null)
        {
          ucParameterDropDownList uc = new ucParameterDropDownList(ParmDdl);
          uc.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
          uc.Location = new Point(StartX, StartY + (uc.Size.Height * x) + 5);
          gbStartOptions.Controls.Add(uc);
          userControls.Add(uc);
        }
      }
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      for (int x = 0; x < userControls.Count; x++)
      {
        ucParameter parm = userControls[x] as ucParameter;
        if (parm != null)
        {
          parm.UpdateValues();
        }
      }
      this.Close();
    }

    private void rbSampleDataFormatNone_CheckedChanged(object sender, EventArgs e)
    {
      Flow.SampleDataFormat = DATA_FORMAT._None;
      txtSampleData.ReadOnly = true;
    }

    private void rbSampleDataFormatJson_CheckedChanged(object sender, EventArgs e)
    {
      Flow.SampleDataFormat = DATA_FORMAT.Json;
      txtSampleData.ReadOnly = false;
    }

    private void rbSampleDataFormatXml_CheckedChanged(object sender, EventArgs e)
    {
      Flow.SampleDataFormat = DATA_FORMAT.Xml;
      txtSampleData.ReadOnly = false;
    }

    private void txtSampleData_TextChanged(object sender, EventArgs e)
    {
      Flow.SampleData = txtSampleData.Text;
      if (Flow.SampleDataFormat == DATA_FORMAT.Json)
      {
        string temp = Flow.SampleData;
        try
        {
          Variable? v = Variable.JsonParse(ref temp);
          rbSampleDataFormatJson.ForeColor = Color.Green;
        }
        catch
        {
          rbSampleDataFormatJson.ForeColor = Color.Red;
        }
      }
    }
  }
}