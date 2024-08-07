﻿using FlowEngineCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowEngineDesigner
{
  public partial class frmVariableSelection : Form
  {
    cFlowWrapper Flow;
    PARM_VAR ParmVar;
    FunctionStep? Step;

    public frmVariableSelection(PARM_VAR parmVar, FunctionStep? step, cFlowWrapper flow)
    {
      Flow = flow;
      ParmVar = parmVar;
      Step = step;
      InitializeComponent();

      this.Text = "Variable [" + ParmVar.Parm.Name + "]";
    }

    private void VariableSelection_Load(object sender, EventArgs e)
    {
      LoadVariables();
      try
      {
        //Attempt to highlight / select the current variable
        if (ParmVar.ParmLiteralOrVariable == PARM_VAR.PARM_L_OR_V.Variable)
        {

          string? val = ParmVar.VariableName;
          string[] varNames = val!.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
          if (varNames.Length > 0)
          {
            TreeNode? node = GetNode(varNames[0], tvVariables.Nodes);
            if (node is not null && varNames.Length > 1)
            {
              for (int i = 1; i < varNames.Length; i++)
              {
                node = GetNode(varNames[i], node!.Nodes);
              }
            }
            tvVariables.SelectedNode = node;
          }
        }
      }
      catch { }
    }

    private TreeNode? GetNode(string varName, TreeNodeCollection nodes)
    {
      for (int j = 0; j < nodes.Count; j++)
      {
        if (nodes[j].Text == varName)
        {
          return nodes[j];
        }
      }
      return null;
    }

    private void LoadVariables()
    {
      tvVariables.Nodes.Clear();
      if (Flow.StartPlugin is not null)
      {
        //Flow.SampleData
        for (int x = 0; x < Flow.Variables.Count; x++)
        {
          KeyValuePair<string, Variable> kvp = Flow.Variables.ElementAt(x);
          if (IsDuplicateNodeName(tvVariables.Nodes, kvp.Key))
            continue;

          TreeNode tn = tvVariables.Nodes.Add(kvp.Key);
          tn.Tag = kvp.Value;
          LoadVariableSubNodes(tn, kvp.Value);
          tn.Expand();
        }
      }
      if (Step is not null)
      {
        List<Variable> vars = Flow.GetVariablesFromPreviousSteps(Step);
        for (int x = 0; x < vars.Count; x++)
        {
          if (IsDuplicateNodeName(tvVariables.Nodes, vars[x].Name))
            continue;

          TreeNode tn = tvVariables.Nodes.Add(vars[x].Name);
          tn.Tag = vars[x];
          LoadVariableSubNodes(tn, vars[x]);
          tn.Expand();
        }
      }
      if (tvVariables.GetNodeCount(true) < 50)
      {
        tvVariables.ExpandAll();
      }
    }

    private void LoadVariableSubNodes(TreeNode tn, Variable var)
    {
      for (int y = 0; y < var.SubVariables.Count; y++)
      {
        Variable v = var.SubVariables[y];
        if (IsDuplicateNodeName(tn.Nodes, v.Name)) 
          continue;

        TreeNode SubNode = tn.Nodes.Add(v.Name);
        SubNode.Tag = v;
        LoadVariableSubNodes(SubNode, v);
      }
    }

    private bool IsDuplicateNodeName(TreeNodeCollection tnc, string newNodeName)
    {
      for (int x = 0; x < tnc.Count; x++)
      {
        if (tnc[x].Text == newNodeName)
          return true;
      }
      return false;
    }

    private string GetSelectedVariableName()
    {
      string varName = "";
      if (tvVariables.SelectedNode is not null)
      {
        TreeNode tn = tvVariables.SelectedNode;
        while (tn is not null)
        {
          if (varName != "")
            varName = tn.Text + "." + varName;
          else
            varName = tn.Text;
          tn = tn.Parent;
        }
      }
      return varName;
    }

    private void tvVariables_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
    {
      ParmVar.SetVarRef(GetSelectedVariableName());
      ParmVar.ParmLiteralOrVariable = PARM_VAR.PARM_L_OR_V.Variable;
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void tvVariables_AfterSelect(object sender, TreeViewEventArgs e)
    {

    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      ParmVar.SetVarRef(GetSelectedVariableName());
      ParmVar.ParmLiteralOrVariable = PARM_VAR.PARM_L_OR_V.Variable;
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void bntCancel_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }
  }
}
