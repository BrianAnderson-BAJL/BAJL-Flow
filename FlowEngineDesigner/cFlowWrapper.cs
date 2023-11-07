using Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static FlowEngineDesigner.cEventManager;

namespace FlowEngineDesigner
{

  public class cFlowWrapper : Core.Flow
  {
    private bool mNeedToSave = false;
    internal Size HighlightSize = new Size(30, 30);

    internal static Vector2 HighlightCenterOffset = new Vector2(15, 15);
    private List<FunctionStep> mCurrentlyExecutingStep = new List<FunctionStep>(8);
    private PictureBox? mPictureBox = null;
    public bool NeedToSave
    {
      get { return mNeedToSave; }
      set { mNeedToSave = value; }
    }

    public void Init()
    {
      StepAdd("FlowCore.Start", Vector2.Zero);
      mCreatedDateTime = Global.CurrentDateTime;
      mModifiedLastDateTime = Global.CurrentDateTime;
    }

    new public cFlowWrapper Clone()
    {
      cFlowWrapper flow = new cFlowWrapper();
      flow.mPictureBox = mPictureBox;
      flow.FileName = FileName; //Don't need to clone, it is only read, not writen to
      flow.FileVersion = FileVersion; //Don't need to clone, it is only read, not writen to
      flow.StartPlugin = StartPlugin; //Don't need to clone, it is only read, not writen to
      flow.StartCommands = StartCommands; //Don't need to clone, it is only read, not writen to

      if (functionSteps.Count > flow.functionSteps.Capacity) //If the default size isn't big enough for the steps, lets allocate a big enough list only once.
      {
        flow.functionSteps = new List<FunctionStep>(functionSteps.Count);
      }
      for (int x = 0; x < functionSteps.Count; x++)
      {
        flow.functionSteps.Add(functionSteps[0].Clone(this));
      }

      return flow;
    }

    public List<Variable> GetVariablesFromPreviousSteps(Core.FunctionStep step)
    {
      List<Variable> variables = new List<Variable>(32);
      List<Core.FunctionStep> allPreviousSteps = new List<FunctionStep>(32);
      List<Core.FunctionStep> stepsToCheck = FindPreviousSteps(step);
      List<Core.FunctionStep> stepsTemp = new List<FunctionStep>();
      while (stepsToCheck.Count > 0)
      {
        for (int x = 0; x < stepsToCheck.Count; x++)
        {
          stepsTemp.AddRange(FindPreviousSteps(stepsToCheck[x]));
        }
        for (int x = 0; x < stepsToCheck.Count; x++)
        {
          if (allPreviousSteps.Contains(stepsToCheck[x]) == false) //We don't want duplicate variables included in the list
            allPreviousSteps.Add(stepsToCheck[x]);
        }
        stepsToCheck.Clear();
        stepsToCheck.AddRange(stepsTemp);
        stepsTemp.Clear();
      }

      for (int x = 0; x < allPreviousSteps.Count; x++)
      {
        FunctionStep fs = allPreviousSteps[x];
        if (fs.Function.RespNames.Name != "")
          variables.Add(fs.RespNames);
      }
      variables.Reverse();
      return variables;
    }

    private List<Core.FunctionStep> FindPreviousSteps(Core.FunctionStep step)
    {
      List<Core.FunctionStep> previousteps = new List<Core.FunctionStep>();
      for (int x = 0; x < functionSteps.Count; x++)
      {
        Core.FunctionStep ps = functionSteps[x];
        for (int y = 0; y < ps.LinkOutputs.Count; y++)
        {
          Link l = ps.LinkOutputs[y];
          if (l.Input.Step == step)
          {
            previousteps.Add(ps);
          }
        }
      }
      return previousteps; 
    }

    public string GetUnusedVariableName(string baseVarName = "")
    {
      if (baseVarName == "")
      {
        baseVarName = cOptions.BaseNewVariableName;
      }
      else
      {
        if (IsUsedVarialbeName(baseVarName) == false)
          return baseVarName;
      }
      for (int x = 2; x < 1024; x++)
      {
        string varName = baseVarName + x.ToString();
        if (IsUsedVarialbeName(varName) == false)
          return varName;
      }
      return Guid.NewGuid().ToString(); ;
    }

    public bool IsUsedVarialbeName(string varName)
    {
      varName = varName.ToLower();
      if (this.Variables.ContainsKey(varName)) //At this point the Variables variable should be empty so will always not find a match, but check it anyway
        return true;
      if (varName == Flow.VAR_NAME_FLOW_START)
        return true;

      for (int x = 0; x < functionSteps.Count; x++)
      {
        if (varName == functionSteps[x].RespNames.Name)
          return true;
      }
      return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="pos"></param>
    public void StepAdd(string name, Vector2 pos)
    {
 
      Core.Function function = Core.PluginManager.FindFunctionByName(name);
      Core.FunctionStep step = new FunctionStep(this, getNextId(), name, pos);
      step.Function = function;
      step.ParmVars = step.Function.Parms.ToParmVars();

      step.ExtraValues[Global.EXTRA_VALUE_IMAGE] = new Bitmap(cOptions.GetFullPath(cOptions.PluginGraphicsPath) + name + ".png");

      functionSteps.Add(step);
      mNeedToSave = true;
    }


    public void LinkDelete(Core.FunctionStep step, Core.Link link)
    {
      step.LinkOutputs.Remove(link);
    }

    public void StepDelete(Core.FunctionStep? step)
    {
      if (step == null)
        return;

      List<Core.Link> linksToDelete = new List<Core.Link>(2);
      
      //Need to remove all Output links to this steps Input
      for (int x = 0; x < functionSteps.Count; x++)
      {
        for (int y = 0; y < functionSteps[x].LinkOutputs.Count; y++)
        {
          Core.Link link = functionSteps[x].LinkOutputs[y];
          if (link.Input.Step != null && link.Input.Step.Id == step.Id)
          {
            linksToDelete.Add(link);
          }
        }

        for (int y = 0; y < linksToDelete.Count; y++)
        {
          functionSteps[x].LinkOutputs.Remove(linksToDelete[y]);
        }
        linksToDelete.Clear();
      }
      functionSteps.Remove(step);
    }

    public HIT_RESULT HitTest(Vector2 v, cCamera camera)
    {
      HIT_RESULT hr;
      for (int i = functionSteps.Count - 1; i >= 0; i--)
      {
        hr = HitTestStep(functionSteps[i], v, camera);
        if (hr.Hit == true)
        {
          return hr;
        }
      }
      hr = HitTestLinks(v, camera);
      if (hr.Hit == true)
        return hr;

      hr = HitTestComments(v, camera);
      if (hr.Hit == true)
        return hr;

      return new HIT_RESULT();
    }


    private bool IsPointNearLink(Vector2 point, Link link, cCamera camera)
    {
      Vector2 inputPos = camera.CreateDrawingPosition(link.Input.Position);
      Vector2 outputPos = camera.CreateDrawingPosition(link.Output.Position);

      double numerator = Math.Abs((outputPos.Y - inputPos.Y) * point.X - (outputPos.X - inputPos.X) * point.Y + outputPos.X * inputPos.Y - outputPos.Y * inputPos.X);

      double denominator = Math.Sqrt(Math.Pow(outputPos.Y - inputPos.Y, 2) + Math.Pow(outputPos.X - inputPos.X, 2));

      if (numerator > 0 && denominator > 0) //Don't want to devide by zero
      {
        double distance = numerator / denominator;
        return distance <= cOptions.LinkLineSelectDistance;
      }
      else
      {
        return false;
      }
    }

    private HIT_RESULT HitTestComments(Vector2 v, cCamera camera)
    {
      for (int x = 0; x < Comments.Count; x++)
      {
        Comment comment = Comments[x];
        Rectangle r = camera.CreateDrawingRect(comment.Position, comment.Size);
        if (r.Contains(v) == true)
        {
          return new HIT_RESULT() { Hit = true, HitItem = comment, ParentItem = null, Type = HIT_RESULT.HIT_TYPE.Comment, Position = comment.Position };
        }
      }
      return new HIT_RESULT();
    }

    private HIT_RESULT HitTestLinks(Vector2 v, cCamera camera)
    {
      for (int i = functionSteps.Count - 1; i >= 0; i--)
      {
        for (int x = 0; x < functionSteps[i].LinkOutputs.Count; x++)
        {
          bool hit = IsPointNearLink(v, functionSteps[i].LinkOutputs[x], camera);
          if (hit == true)
            return new HIT_RESULT() { Hit = true, ParentItem = functionSteps[i], HitItem = functionSteps[i].LinkOutputs[x], Type = HIT_RESULT.HIT_TYPE.Link };
        }
      }
      return new HIT_RESULT();
    }

    private HIT_RESULT HitTestStep(FunctionStep step, Vector2 v, cCamera camera)
    {
      Bitmap? bm = step.ExtraValues[Global.EXTRA_VALUE_IMAGE] as Bitmap;
      if (bm == null)
        throw new Exception("Missing 'image' tag in step.ExtraValues[], can't draw step");

      Size size = new Size(bm.Width, bm.Height);

      Rectangle r = camera.CreateDrawingRect(step.Position, size);
      if (r.Contains(v) == true)
      {
        //Check Input
        if (step.Function.Input != null)
        {
          Vector2 pos = (step.Position + step.Function.Input.Offset);
          r = camera.CreateDrawingRect(pos, HighlightSize);
          if (r.Contains(v) == true)
          {
            return new HIT_RESULT() { Hit = true, HitItem = step.Function.Input.Clone(step), ParentItem = step, Type = HIT_RESULT.HIT_TYPE.Input, Position = pos + HighlightCenterOffset };
          }
        }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        for (int i = 0; i < step.Function.Outputs.Count; i++)
          {
            if (step.Function.Outputs != null)
            {
              Core.Output o = step.Function.Outputs[i];
              Vector2 pos = (step.Position + o.Offset);
              r = camera.CreateDrawingRect(pos, HighlightSize);
              if (r.Contains(v) == true)
              {
                return new HIT_RESULT() { Hit = true, HitItem = o.Clone(step), ParentItem = step, Type = HIT_RESULT.HIT_TYPE.Output, Position = pos + HighlightCenterOffset };
              }
            }
          }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        //If we got here, just return the step
        return new HIT_RESULT() { Hit = true, HitItem = step, Type = HIT_RESULT.HIT_TYPE.Function, Position = step.Position };
      }
      return new HIT_RESULT() { Hit = false };
    }


    public void PopulateSampleVariablesFromPlugin()
    {
      if (StartPlugin != null)
      {
        if (this.SampleDataFormat == DATA_FORMAT.Json && this.SampleData != null && this.SampleData.Length > 0)
        {
          Variable var1 = StartPlugin.SampleVariables[Flow.VAR_NAME_FLOW_START];
          Variable? data = FindVariable(var1, Flow.VAR_NAME_FLOW_START, Flow.VAR_REQUEST, Flow.VAR_DATA);
          string tempJson = this.SampleData;
          try
          {
            Variable? varJsonData = Variable.JsonParse(ref tempJson);
            if (varJsonData is not null && data is not null && varJsonData.SubVariables.Count > 0)
            {
              data.SubVariables.Add(varJsonData.SubVariables[0]);
            }
          }
          catch (Exception ex)
          {
            cEventManager.RaiseEventTracer(SENDER.Compiler, $"Flow SampleData Json Parse error: [{FileName}], [{ex.Message}]", cEventManager.TRACER_TYPE.Error);
          }

        }
      }
    }





    #region Draw all the graphics in the picturebox
    public void Draw(Graphics graphics, cCamera camera)
    {
      // First draw the Comments
      for (int x = 0; x < Comments.Count; x++)
      {
        DrawComment(graphics, camera, Comments[x]);
      }

      // Then draw the Steps
      for (int x = 0; x < functionSteps.Count; x++)
      {
        DrawStep(graphics, camera, functionSteps[x]);
      }
      // Then draw the links on top of the steps, need to draw them after all the steps otherwise the steps will draw on top of the links going to the Input connector
      for (int x = 0; x < functionSteps.Count; x++)
      {
        DrawLinks(functionSteps[x], graphics, camera);
      }
    }

    private void DrawComment(Graphics graphics, cCamera camera, Comment comment)
    {
      
      Rectangle r = camera.CreateDrawingRect(comment.Position, comment.Size);
      Color c = cOptions.CommentColorBackgroundDefault;
      Brush b = new SolidBrush(c);
      graphics.FillRoundedRectangle(b, r, 20);


      StringFormat format = new StringFormat();
      format.LineAlignment = StringAlignment.Center;
      format.Alignment = StringAlignment.Center;

      Brush brush = new SolidBrush(cOptions.CommentColorTextDefault);

      float fontSize = 20f * camera.ZoomLevel;
      using (Font font1 = new Font("Arial", fontSize, FontStyle.Regular, GraphicsUnit.Pixel))
      {
        string[] split = comment.Text.Split(Environment.NewLine);
        Vector2 pos = camera.CreateDrawingPosition(comment.Position + new Vector2(15, 15));
        for (int x = 0; x < split.Length; x++) 
        {
          if (split[x] == "")
            split[x] = " ";

          graphics.DrawString(split[x], font1, brush, pos.ToPoint()); //, format
          SizeF sizeString = graphics.MeasureString(split[x], font1);
          pos.Y += sizeString.Height;
        }
      }
      //graphics.DrawString(

    }

    private void DrawLinks(Core.FunctionStep step, Graphics graphics, cCamera camera)
    {
      for (int x = 0; x < step.LinkOutputs.Count; x++)
      {
        Core.Link l = step.LinkOutputs[x];
        DrawLink(l, graphics, camera, 3, Color.Blue);
      }
    }

    private void DrawLink(Link link, Graphics graphics, cCamera camera, int width, Color color)
    {
      
      Pen p = new Pen(color, width);
      Vector2 v1 = link.Output.Position;
      v1 = camera.CreateDrawingPosition(v1);
      Vector2 v2 = Vector2.Zero;
      if (link.Input.Step is not null)
      {
        v2 = link.Input.Position;
        v2 = camera.CreateDrawingPosition(v2);
      }
      graphics.DrawLine(p, v1.ToPoint(), v2.ToPoint());
    }

    public void DrawSelection(HIT_RESULT hr, Graphics graphics, cCamera camera)
    {
      if (hr.Type == HIT_RESULT.HIT_TYPE.Function && hr.HitItem != null)
      {
        Size s = new Size(Global.SelectorStep.Width, Global.SelectorStep.Height);
        Rectangle r = camera.CreateDrawingRect(hr.HitItem.Position - new Vector2(10, 10), s);
        graphics.DrawImage(Global.SelectorStep, r);
      }
      else if (hr.Type == HIT_RESULT.HIT_TYPE.Comment && hr.HitItem != null)
      {
        Comment? comment = hr.HitItem as Comment;
        if (comment is not null)
        {
          Size s = new Size(comment.Size.Width + 18, comment.Size.Height + 18);
          Rectangle r = camera.CreateDrawingRect(hr.HitItem.Position - new Vector2(10, 10), s);
          graphics.DrawImage(Global.SelectorStep, r);
        }
      }
      else if (hr.Type == HIT_RESULT.HIT_TYPE.Link && hr.HitItem is not null)
      {
        Link? link = hr.HitItem as Link;
        if (link is not null)
        {
          DrawLink(link, graphics, camera, 6, Color.Orange);
          DrawLink(link, graphics, camera, 3, Color.Blue);
        }
      }
    }

    public void DrawExecutingCurrent(Graphics graphics, cCamera camera)
    {
      for (int x = 0; x < mCurrentlyExecutingStep.Count; x++)
      {
        Size s = new Size(Global.ExecutingCurrentStep.Width, Global.ExecutingCurrentStep.Height);
        Rectangle r = camera.CreateDrawingRect(mCurrentlyExecutingStep[x].Position - new Vector2(10, 10), s);
        graphics.DrawImage(Global.ExecutingCurrentStep, r);
      }
    }

    private void DrawStep(Graphics graphics, cCamera camera, FunctionStep step)
    {
      Bitmap? bm = step.ExtraValues[Global.EXTRA_VALUE_IMAGE] as Bitmap;
      if (bm == null)
        throw new Exception("Missing 'image' tag in step.ExtraValues[], can't draw step");

      Size s = new Size(bm.Width, bm.Height);
      Rectangle r = camera.CreateDrawingRect(step.Position, s);
      graphics.DrawImage(bm, r);

      if (step.Function.Input != null) //Only Flow.Start will have a null Input
      {
        Vector2 pos = (step.Position + step.Function.Input.Offset);
        r = camera.CreateDrawingRect(pos, HighlightSize);
        if ((cMouse.FlowItem is Core.Input) == false && r.Contains(cMouse.pos.ToPoint()) == true)
        {
          Core.Output? co = cMouse.FlowItem as Core.Output;
          if ((co == null) || (co != null && co.Step != null && co.Step.Id != step.Id))
          {
            graphics.DrawImage(Global.HighlightYellow, r);
          }
          else
          {
            graphics.DrawImage(Global.HighlightBlack, r);
          }
        }
        else
        {
          graphics.DrawImage(Global.HighlightBlack, r);
        }

      }
      if (step.Function.Outputs != null) {
        for (int x = 0; x < step.Function.Outputs.Count; x++)
        {
          Vector2 pos = (step.Position + step.Function.Outputs[x].Offset);
          r = camera.CreateDrawingRect(pos, HighlightSize);
          //float Distance = Vector2.Distance(cMouse.Pos, (pos + camera.Position + (HighlightCenterOffset * camera.ZoomLevel)));
          //camera.FormFlow.label1.Text = Distance.ToString();
          if ((cMouse.FlowItem is Core.Output) == false && r.Contains(cMouse.pos.ToPoint()) == true)
          {
            Core.Input? ci = cMouse.FlowItem as Core.Input;
            if ((ci == null) || (ci != null && ci.Step != null && ci.Step.Id != step.Id))
            {
              graphics.DrawImage(Global.HighlightYellow, r);
            }
            else
            {
              graphics.DrawImage(Global.HighlightBlack, r);
            }
          }
          else
          {
            graphics.DrawImage(Global.HighlightBlack, r);
          }
        }
      }
    }
    #endregion
    public void Center(cCamera camera, PictureBox pb)
    {
      float MinX = float.MaxValue;
      float MaxX = float.MinValue;
      float MinY = float.MaxValue;
      float MaxY = float.MinValue;

      FunctionStep? step = FindStepByName("FlowCore", "Start");
      if (step is not null)
      {
        camera.Position = step.Position;
        return;
      }
      //Vector2 newPos = new Vector2();
      for (int x = 0; x < functionSteps.Count; x++)
      {
        Vector2 p = functionSteps[x].Position;
        //Bitmap? bm = functionSteps[x].ExtraValues[Global.EXTRA_VALUE_IMAGE] as Bitmap;
        //if (bm == null)
        //  throw new Exception("Missing 'image' tag in step.ExtraValues[], can't draw step");

        //Size s = new Size(bm.Width, bm.Height);

        if (MinX > p.X)
          MinX = p.X;
        if (MaxX < p.X) 
          MaxX = p.X;
        if (MinY > p.Y)
          MinY = p.Y;
        if (MaxY < p.Y)
          MaxY = p.Y;
      }

      float CenterX = ((MaxX - MinX) / 2) + MinX;// - (pb.Size.Width / 2);
      float CenterY = ((MaxY - MinY) / 2) + MinY;// - (pb.Size.Height / 2);
      //CenterX *= camera.ZoomLevel;
      //CenterY *= camera.ZoomLevel;
      camera.Position = new Vector2(CenterX, CenterY);
    }

    public void XmlWriteFile(string fileName = "")
    {
      if (fileName != "")
      {
        FileName = fileName;
      }
      Xml xml = new Xml();
      xml.WriteFileNew(FileName);
      XmlWrite(xml);
      xml.WriteFileClose();

    }

    public string XmlWriteMemory()
    {
      Xml xml = new Xml();
      xml.WriteMemoryNew();
      XmlWrite(xml);
      string data = xml.ReadMemory();
      xml.WriteFileClose();
      return data;
    }

    /// <summary>
    /// Write out the flow XML data. All the reading of XML data is handled in Core.Flow since the flow engine needs to load flows also, but only the designer writes flows.
    /// </summary>
    /// <param name="fileName"></param>
    private void XmlWrite(Xml xml)
    {
      xml.WriteTagStart("Base");
      xml.WriteTagStart("MetaData");
      xml.WriteTagAndContents("FileFormatVersion", 1); //If we introduce newer tags, will will increment this, this way we can convert the v1 file to a v2
      xml.WriteTagAndContents("CreatedDateTime", mCreatedDateTime);
      xml.WriteTagAndContents("ModifiedLastDateTime", Global.CurrentDateTime);
      xml.WriteTagAndContents("FileVersion", ++FileVersion);
      xml.WriteTagAndContents("SampleDataFormat", SampleDataFormat);
      xml.WriteTagAndContents("SampleData", SampleData, Xml.BASE_64_ENCODE.Encoded);
      if (cOptions.FlowUserHistoryMax > 0)
      {
        xml.WriteTagStart("Users");
        if (cServer.UserLoggedIn is not null)
        {
          xml.WriteXml(cServer.UserLoggedIn.ToXml(Global.CurrentDateTime, User.TO_XML.LoggingOnly, xml.IndentLevel));
        }
        int max = Math.Min(cOptions.FlowUserHistoryMax, previousUsers.Count);

        for (int x = 0; x < max; x++)
        {
          if (x >= (cOptions.FlowUserHistoryMax - 1)) //Subtract one for the currentUser above. We only want to save the last x users as defined in options
            break;
          xml.WriteXml(previousUsers[x].ToXml(Global.CurrentDateTime, User.TO_XML.LoggingOnly, xml.IndentLevel));
        }
        xml.WriteTagEnd("Users");
      }
      xml.WriteTagEnd("MetaData");
      xml.WriteTagStart("Flow");

      xml.WriteTagStart("StartCommands");
      if (StartPlugin != null)
      {
        xml.WriteTagAndContents("StartPlugin", StartPlugin.Name);
      }
      else
      {
        xml.WriteTagAndContents("StartPlugin", "");
      }
      XmlWriteVariables(xml, StartCommands);
      xml.WriteTagEnd("StartCommands");

      for (int x = 0; x < functionSteps.Count; x++)
      {
        XmlWriteStep(xml, functionSteps[x]);
      }
      XmlWriteComments(xml);
      xml.WriteTagEnd("Flow");
      xml.WriteTagEnd("Base");
    }

    private void XmlWriteVariables(Xml xml, PARM_VARS parms)
    {
      for (int x = 0; x < parms.Count; x++)
      {
        xml.WriteTagStart("Variable");
        PARM_VAR pv = parms[x];
        xml.WriteTagAndContents("Name", pv.Parm.Name);
        xml.WriteTagAndContents("Literal", pv.ParmLiteralOrVariable);
        xml.WriteTagAndContents("DataType", pv.Parm.DataType);

        if (pv.ParmLiteralOrVariable == PARM_VAR.PARM_L_OR_V.Variable)
        {
          xml.WriteTagAndContents("Value", pv.VariableName, Xml.BASE_64_ENCODE.Encoded); //Value will store the variable name if it a Variable
        }
        else if (pv.ParmLiteralOrVariable == PARM_VAR.PARM_L_OR_V.Literal)
        {
          if (pv.Parm.DataType == DATA_TYPE.String || pv.Parm.DataType == DATA_TYPE.Object)
          {
            pv.GetValue(out string val);
            xml.WriteTagAndContents("Value", val, Xml.BASE_64_ENCODE.Encoded);
          }
          else if (pv.Parm.DataType == DATA_TYPE.Integer)
          {
            pv.GetValue(out long val);
            xml.WriteTagAndContents("Value", val);
          }
          else if (pv.Parm.DataType == DATA_TYPE.Decimal)
          {
            pv.GetValue(out decimal val);
            xml.WriteTagAndContents("Value", val);
          }
          else if (pv.Parm.DataType == DATA_TYPE.Boolean)
          {
            pv.GetValue(out bool val);
            xml.WriteTagAndContents("Value", val);
          }
        }
        xml.WriteTagEnd("Variable");
      }

    }

    private void XmlWriteComments(Core.Xml xml)
    {
      if (Comments.Count > 0)
      {
        xml.WriteTagStart("Comments");
        for (int x = 0; x < Comments.Count; x++)
        {
          xml.WriteTagStart("Comment");
          xml.WriteTagAndContents("Id", Comments[x].Id);
          xml.WriteTagAndContents("Position", Comments[x].Position);
          xml.WriteTagAndContents("Size", Comments[x].Size);
          xml.WriteTagAndContents("Text", Comments[x].Text);
          xml.WriteTagEnd("Comment");
        }
        xml.WriteTagEnd("Comments");
      }
    }

    private void XmlWriteStep(Core.Xml xml, Core.FunctionStep step)
    {
      xml.WriteTagStart("Step");
      xml.WriteTagAndContents("Id", step.Id);

      xml.WriteTagAndContents("PluginName", step.Function.Plugin.Name);
      xml.WriteTagAndContents("FunctionName", step.Function.Name);
      xml.WriteTagAndContents("Position", step.Position);
      xml.WriteTagAndContents("SaveResponseVariable", step.SaveResponseVariable);
      xml.WriteTagAndContents("SaveResponseVariableName", step.RespNames.Name);
      xml.WriteTagStart("Variables");
      XmlWriteVariables(xml, step.ParmVars);
      xml.WriteTagEnd("Variables");
      xml.WriteTagStart("Links");
      for (int x = 0; x < step.LinkOutputs.Count; x++)
      {
        Core.Link lw = step.LinkOutputs[x];
        xml.WriteTagStart("Link");
        xml.WriteTagAndContents("Id", lw.Id);
        xml.WriteTagAndContents("OutputLabel", lw.Output.Label);
        xml.WriteTagStart("Input");
        if (lw.Input.Step != null)
          xml.WriteTagAndContents("StepId", lw.Input.Step.Id);
        else
          xml.WriteTagAndContents("StepId", "");
        xml.WriteTagAndContents("Label", lw.Input.Label);
        xml.WriteTagEnd("Input");
        xml.WriteTagEnd("Link");
      }
      xml.WriteTagEnd("Links");
      xml.WriteTagEnd("Step");
    }


    /// <summary>
    /// Append ExtraValues to FunctionStep (the images for the designer)
    /// </summary>
    /// <param name="fileName"></param>
    public override void XmlReadFile(string fileName, READ_TIL til = READ_TIL.All)
    {
      base.XmlReadFile(fileName, til);
      for (int x = 0; x < functionSteps.Count; x++)
      {
        FunctionStep step = functionSteps[x];
        step.ExtraValues[Global.EXTRA_VALUE_IMAGE] = new Bitmap(cOptions.GetFullPath(cOptions.PluginGraphicsPath) + step.Function.Plugin.Name + "." + step.Function.Name + ".png");

      }
    }

    public override void XmlRead(ref string content, READ_TIL til = READ_TIL.All)
    {
      base.XmlRead(ref content, til);
      for (int x = 0; x < functionSteps.Count; x++)
      {
        FunctionStep step = functionSteps[x];
        step.ExtraValues[Global.EXTRA_VALUE_IMAGE] = new Bitmap(cOptions.GetFullPath(cOptions.PluginGraphicsPath) + step.Function.Plugin.Name + "." + step.Function.Name + ".png");

      }
    }

    public override string ToString()
    {
      return "Flow";
    }
  }
}
