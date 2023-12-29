using Core;
using Core.Parsers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Runtime.ExceptionServices;
using System.Security.Policy;
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

#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
    private struct PREV_STEP
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
    {
      public Core.FunctionStep Step;
      public int OutputIndex;
      public int Depth;

      public PREV_STEP(Core.FunctionStep step, int outputIndex, int depth)
      {
        this.Step = step;
        this.OutputIndex = outputIndex;
        this.Depth = depth;
      }

      public static bool operator ==(PREV_STEP a, PREV_STEP b)
      {
        if (a.Step == b.Step && a.OutputIndex == b.OutputIndex)
          return true;

        return false;
      }
      public static bool operator !=(PREV_STEP a, PREV_STEP b)
      {
        if (a.Step != b.Step || a.OutputIndex != b.OutputIndex)
          return true;

        return false;
      }

    }
    public List<Variable> GetVariablesFromPreviousSteps(Core.FunctionStep step)
    {
      int depth = 0;
      List<Variable> variables = new List<Variable>(32);
      List<PREV_STEP> allPreviousSteps = new List<PREV_STEP>(32);
      List<PREV_STEP> stepsToCheck = FindPreviousSteps(step, depth++);
      List<PREV_STEP> stepsTemp = new List<PREV_STEP>();
      while (stepsToCheck.Count > 0)
      {
        for (int x = 0; x < stepsToCheck.Count; x++)
        {
          stepsTemp.AddRange(FindPreviousSteps(stepsToCheck[x].Step, depth++));
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
        PREV_STEP ps = allPreviousSteps[x];
        if (ps.Step.Function.RespNames.Name != "" && ps.OutputIndex == RESP.SUCCESS)
        {
          if (ps.Step.Name == "Database.Select")
          {
            Variable var = ps.Step.RespNames.Clone();
            PopulateVariablesFromSql(ps, ref var);
            variables.Add(var);
          }
          else
          {
            variables.Add(ps.Step.RespNames);
          }
        }
        else if (ps.OutputIndex != RESP.SUCCESS && ps.Depth == 0)
        {
          Variable err = new Variable(Flow.VAR_NAME_PREVIOUS_STEP);
          err.Add(new VariableInteger("ErrorNumber", 0));
          err.Add(new VariableString("ErrorDescription", ""));
          variables.Add(err);
        }
      }
      variables.Reverse();
      return variables;
    }

    private void PopulateVariablesFromSql(PREV_STEP ps, ref Variable var)
    {
      if (ps.Step.ParmVars.Count == 0)
        return;

      PARM_VAR parmVar = ps.Step.ParmVars[0];
      if (parmVar.ParmLiteralOrVariable == PARM_VAR.PARM_L_OR_V.Literal && parmVar.Var.DataType == DATA_TYPE.String)
      {
        VariableString? vs = parmVar.Var as VariableString;
        if (vs is null)
          return;

        SqlParser sqlParser = new SqlParser();
        sqlParser.ParseSql(vs.Value);
        List<string> fields = sqlParser.GetListOf(SqlParser.ParsedUnit.UNIT_TYPE.Field);
        var.SubVariablesFormat = DATA_FORMAT_SUB_VARIABLES.Array;
        for (int x = 0; x < fields.Count; x++)
        {
          var.Add(new VariableString(fields[x], ""));
        }
      }
    }

    private List<PREV_STEP> FindPreviousSteps(Core.FunctionStep step, int depth)
    {
      List<PREV_STEP> previousteps = new List<PREV_STEP>();
      for (int x = 0; x < functionSteps.Count; x++)
      {
        Core.FunctionStep ps = functionSteps[x];
        for (int y = 0; y < ps.LinkOutputs.Count; y++)
        {
          Link l = ps.LinkOutputs[y];
          if (l.Input.Step == step)
          {
            previousteps.Add(new PREV_STEP(ps, y, depth));
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
      if (varName == Flow.VAR_NAME_PREVIOUS_STEP)
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
      if (step is null)
        return;

      List<Core.Link> linksToDelete = new List<Core.Link>(2);
      
      //Need to remove all Output links to this steps Input
      for (int x = 0; x < functionSteps.Count; x++)
      {
        for (int y = 0; y < functionSteps[x].LinkOutputs.Count; y++)
        {
          Core.Link link = functionSteps[x].LinkOutputs[y];
          if (link.Input.Step is not null && link.Input.Step.Id == step.Id)
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

    public HIT_RESULT HitTestResizeOnly(Vector2 v, cCamera camera)
    {
      for (int x = 0; x < Comments.Count; x++)
      {
        for (int y = 0; y < Comments[x].ResizeRects.Length; y++)
        {
          ResizeHandle resizeHandle = Comments[x].ResizeRects[y];
          Rectangle r = camera.CreateDrawingRectNoShrink(resizeHandle.GetDrawingRect());
          if (r.Contains(v) == true)
          {
            return new HIT_RESULT() { Hit = true, HitItem = resizeHandle, ParentItem = Comments[x], Type = HIT_RESULT.HIT_TYPE.Resize, Position = Comments[x].Position };
          }
        }
      }
      return new HIT_RESULT();
    }


    private bool IsPointNearLink(Vector2 point, Link link, cCamera camera)
    {
      Vector2 inputPos = camera.CreateDrawingPosition(link.Input.Position);
      Vector2 outputPos = camera.CreateDrawingPosition(link.Output.Position);

      //Compare with a bounding box for the line segment first
      int x = (int)Math.Min(inputPos.X, outputPos.X);
      int y = (int)Math.Min(inputPos.Y, outputPos.Y);
      int width = (int)Math.Abs(inputPos.X - outputPos.X);
      if (width < cOptions.LinkLineSelectDistance)
        width = cOptions.LinkLineSelectDistance;
      int height = (int)Math.Abs(inputPos.Y - outputPos.Y);
      if (height < cOptions.LinkLineSelectDistance)
        height = cOptions.LinkLineSelectDistance;
      Rectangle r = new Rectangle(x, y, width, height);
      if (r.Contains(point) == false)
        return false;

      //We got here, must be within the bounding box, now check if it is near the actual line.
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
      Rectangle r;
      for (int x = 0; x < Comments.Count; x++)
      {
        for (int y = 0; y < Comments[x].ResizeRects.Length; y++)
        {
          ResizeHandle resizeHandle = Comments[x].ResizeRects[y];
          r = camera.CreateDrawingRectNoShrink(resizeHandle.GetDrawingRect());
          if (r.Contains(v) == true)
          {
            return new HIT_RESULT() { Hit = true, HitItem = resizeHandle, ParentItem = Comments[x], Type = HIT_RESULT.HIT_TYPE.Resize, Position = Comments[x].Position };
          }
        }
        Comment comment = Comments[x];
        r = camera.CreateDrawingRect(comment.Position, comment.Size);
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
      if (bm is null)
        throw new Exception("Missing 'image' tag in step.ExtraValues[], can't draw step");

      Size size = new Size(bm.Width, bm.Height);

      Rectangle r = camera.CreateDrawingRect(step.Position, size);
      if (r.Contains(v) == true)
      {
        //Check Input
        if (step.Function.Input is not null)
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
            if (step.Function.Outputs is not null)
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
      if (StartPlugin is not null)
      {
        if (this.SampleDataFormat == DATA_FORMAT.Json && this.SampleData is not null && this.SampleData.Length > 0)
        {
          //string temp = this.SampleData;
          
          //Variable var1 = Variable.JsonParse(ref temp);
          //Variable? data = FindVariable(var1, Flow.VAR_NAME_FLOW_START, Flow.VAR_REQUEST, Flow.VAR_DATA);
          string tempJson = this.SampleData;
          try
          {
            Variable? varJsonData = Variable.JsonParse(ref tempJson);
            if (varJsonData is not null)
            {
              Variables.Add(varJsonData.Name, varJsonData);
            }
            //if (varJsonData is not null && data is not null && varJsonData.SubVariables.Count > 0)
            //{
            //  for (int x = 0; x < varJsonData.SubVariables.Count; x++)
            //  {
            //    data.SubVariables.Add(varJsonData.SubVariables[x]);
            //  }
            //}
          }
          catch (Exception ex)
          {
            cEventManager.RaiseEventTracer(SENDER.Compiler, $"Flow SampleData Json Parse error: [{FileName}], [{ex.Message}]", cEventManager.TRACER_TYPE.Error);
          }

        }
      }
    }

    public void CommentAdd(Vector2 pos, SizeF size)
    {
      Comment comment = new Comment();
      comment.Position = pos;

      comment.Size = size;
      comment.ColorBackground = cOptions.CommentColorBackgroundDefault;
      comment.ColorText = cOptions.CommentColorTextDefault;

      Comments.Add(comment);
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
      comment.Selected = false;
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
      link.Selected = false;
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
      if (hr.HitItem is null)
        return;

      hr.HitItem.Selected = true;

      if (hr.Type == HIT_RESULT.HIT_TYPE.Function)
      {
        Size s = new Size(Global.SelectorStep.Width, Global.SelectorStep.Height);
        Rectangle r = camera.CreateDrawingRect(hr.HitItem.Position - new Vector2(10, 10), s);
        graphics.DrawImage(Global.SelectorStep, r);
      }
      else if (hr.Type == HIT_RESULT.HIT_TYPE.Comment)
      {
        Comment? comment = hr.HitItem as Comment;
        if (comment is not null)
        {
          SizeF s = new SizeF(comment.Size.Width + 18, comment.Size.Height + 18);
          Rectangle r = camera.CreateDrawingRect(hr.HitItem.Position - new Vector2(10, 10), s);
          graphics.DrawImage(Global.SelectorStep, r);

          for (int x = 0; x < comment.ResizeRects.Length; x++)
          {
            graphics.FillRoundedRectangle(new SolidBrush(Color.Gray), camera.CreateDrawingRectNoShrink(comment.ResizeRects[x].GetDrawingRect()), 1);
          }

        }
      }
      else if (hr.Type == HIT_RESULT.HIT_TYPE.Link)
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
      step.Selected = false;
      Bitmap? bm = step.ExtraValues[Global.EXTRA_VALUE_IMAGE] as Bitmap;
      if (bm is null)
        throw new Exception("Missing 'image' tag in step.ExtraValues[], can't draw step");

      Size s = new Size(bm.Width, bm.Height);
      Rectangle r = camera.CreateDrawingRect(step.Position, s);
      graphics.DrawImage(bm, r);

      if (step.Function.Input is not null) //Only Flow.Start will have a null Input
      {
        Vector2 pos = (step.Position + step.Function.Input.Offset);
        r = camera.CreateDrawingRect(pos, HighlightSize);
        if ((cMouse.FlowItem is Core.Input) == false && r.Contains(cMouse.pos.ToPoint()) == true)
        {
          Core.Output? co = cMouse.FlowItem as Core.Output;
          if ((co is null) || (co is not null && co.Step is not null && co.Step.Id != step.Id))
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
      if (step.Function.Outputs is not null) {
        for (int x = 0; x < step.Function.Outputs.Count; x++)
        {
          Vector2 pos = (step.Position + step.Function.Outputs[x].Offset);
          r = camera.CreateDrawingRect(pos, HighlightSize);
          //float Distance = Vector2.Distance(cMouse.Pos, (pos + camera.Position + (HighlightCenterOffset * camera.ZoomLevel)));
          //camera.FormFlow.label1.Text = Distance.ToString();
          if ((cMouse.FlowItem is Core.Output) == false && r.Contains(cMouse.pos.ToPoint()) == true)
          {
            Core.Input? ci = cMouse.FlowItem as Core.Input;
            if ((ci is null) || (ci is not null && ci.Step is not null && ci.Step.Id != step.Id))
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
      for (int x = 0; x < functionSteps.Count; x++)
      {
        Vector2 p = functionSteps[x].Position;

        if (MinX > p.X)
          MinX = p.X;
        if (MaxX < p.X) 
          MaxX = p.X;
        if (MinY > p.Y)
          MinY = p.Y;
        if (MaxY < p.Y)
          MaxY = p.Y;
      }

      float CenterX = ((MaxX - MinX) / 2) + MinX;
      float CenterY = ((MaxY - MinY) / 2) + MinY;
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
      if (StartPlugin is not null)
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
        if (pv.Parm.Name != pv.ParmName)
          xml.WriteTagAndContents("ParamName", pv.Parm.Name); //If the user changed the name of the parameter, we need to store the actual parameter name in the XML flow file so we can find it when loading.
        xml.WriteTagAndContents("Name", pv.ParmName);
        xml.WriteTagAndContents("Literal", pv.ParmLiteralOrVariable);

        if (pv.ParmLiteralOrVariable == PARM_VAR.PARM_L_OR_V.Variable)
        {
          xml.WriteTagAndContents("DataType", pv.Parm.DataType);
          xml.WriteTagAndContents("Value", pv.VariableName, Xml.BASE_64_ENCODE.Encoded); //Value will store the variable name if it a Variable
        }
        else if (pv.ParmLiteralOrVariable == PARM_VAR.PARM_L_OR_V.Literal)
        {
          if (pv.Parm.DataType == DATA_TYPE.String || pv.Parm.DataType == DATA_TYPE.Object)
          {
            xml.WriteTagAndContents("DataType", pv.Parm.DataType);
            pv.GetValue(out string val);
            xml.WriteTagAndContents("Value", val, Xml.BASE_64_ENCODE.Encoded);
          }
          else if (pv.Parm.DataType == DATA_TYPE.Integer)
          {
            xml.WriteTagAndContents("DataType", pv.Parm.DataType);
            pv.GetValue(out long val);
            xml.WriteTagAndContents("Value", val);
          }
          else if (pv.Parm.DataType == DATA_TYPE.Decimal)
          {
            xml.WriteTagAndContents("DataType", pv.Parm.DataType);
            pv.GetValue(out decimal val);
            xml.WriteTagAndContents("Value", val);
          }
          else if (pv.Parm.DataType == DATA_TYPE.Boolean)
          {
            xml.WriteTagAndContents("DataType", pv.Parm.DataType);
            pv.GetValue(out bool val);
            xml.WriteTagAndContents("Value", val);
          }
          else if (pv.Parm.DataType == DATA_TYPE.Various)
          {
            xml.WriteTagAndContents("DataType", pv.Var.DataType);
            if (pv.Var.DataType == DATA_TYPE.String)
            {
              pv.GetValue(out string val);
              xml.WriteTagAndContents("Value", val, Xml.BASE_64_ENCODE.Encoded);
            }
            else if (pv.Var.DataType == DATA_TYPE.Integer)
            {
              pv.GetValue(out long val);
              xml.WriteTagAndContents("Value", val);
            }
            else if (pv.Var.DataType == DATA_TYPE.Decimal)
            {
              pv.GetValue(out decimal val);
              xml.WriteTagAndContents("Value", val);
            }
            else if (pv.Var.DataType == DATA_TYPE.Boolean)
            {
              pv.GetValue(out bool val);
              xml.WriteTagAndContents("Value", val);
            }
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
      if (step.Validator is not null)
      {
        xml.WriteTagAndContents("ValidatorName", step.Validator.Name);
      }
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
        if (lw.Input.Step is not null)
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
