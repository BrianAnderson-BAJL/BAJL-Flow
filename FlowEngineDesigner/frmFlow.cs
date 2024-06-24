using FlowEngineCore;
using FlowEngineCore.Administration.Messages;
using Microsoft.VisualBasic.Devices;
using System.Numerics;
using static FlowEngineDesigner.cEventManager;

namespace FlowEngineDesigner
{
  public partial class frmFlow : Form
  {
    cFlowWrapper Flow = new cFlowWrapper();
    public cCamera Camera;

    HIT_RESULT SelectedItem;
    HIT_RESULT ResizeItem;

    public frmFlow(cFlowWrapper? flowWrapper = null)
    {
      InitializeComponent();
      Camera = new cCamera(this);
      if (flowWrapper is not null)
      {
        Flow = flowWrapper;
      }
      TitleText();
    }

    private void pictureBox1_Paint(object sender, PaintEventArgs e)
    {
      e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

      if (cMouse.OverallState == cMouse.OVERALL_STATE.DrawComment && cMouse.DraggingStart.HasValue)
      {
        Vector2 v = Camera.CreateRealPosition(cMouse.DraggingStart.Value);
        Size s = Camera.CreateRealSize(cMouse.pos - cMouse.DraggingStart.Value); //Need to calculate the real position before we can get the drawing location
        Rectangle r = Camera.CreateDrawingRect(v, s);
        Brush b = new SolidBrush(cOptions.CommentColorBackgroundDefault);
        e.Graphics.FillRoundedRectangle(b, r, 20);
      }

      Flow.Draw(e.Graphics, Camera);

      if (cMouse.DraggingStart.HasValue == true && (cMouse.FlowItem is FlowEngineCore.Input || cMouse.FlowItem is FlowEngineCore.Output))
      {
        Pen p = new Pen(Color.Blue, 3);

        Vector2 v1 = Camera.CreateDrawingPosition(cMouse.DraggingStart.Value);
        Vector2 v2 = cMouse.pos;
        e.Graphics.DrawLine(p, v1.ToPoint(), v2.ToPoint());
      }

      if (SelectedItem.Hit == true && SelectedItem.HitItem is not null)
      {
        Flow.DrawSelection(SelectedItem, e.Graphics, Camera);
      }
      Flow.DrawExecutingCurrent(e.Graphics, Camera);
      tslCameraPosition.Text = Camera.Position.ToString();
    }

    private void frmFlow_Load(object sender, EventArgs e)
    {

      Camera.Position = new Vector2(200, 200);
      pictureBox1.MouseWheel += PictureBox1_MouseWheel;
      Flow.Init();
      pictureBox1.Refresh();
      ShowZoom();
    }

    private void PictureBox1_MouseWheel(object? sender, MouseEventArgs e)
    {
      if (e.Delta > 0)
      {
        Camera.ZoomIn();
      }
      else if (e.Delta < 0)
      {
        Camera.ZoomOut();
      }

      Vector2 MousePos = new Vector2(e.X, e.Y);
      Vector2 Center = new Vector2(pictureBox1.Width / 2, pictureBox1.Height / 2);

      Camera.Position += ((Center - MousePos) / 10f);

      pictureBox1.Refresh();
      ShowZoom();

    }

    private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        cMouse.ButtonRight = cMouse.BUTTON_STATE.Down;
        cMouse.DraggingStart = new Vector2(e.X, e.Y);
      }
      else if (e.Button == MouseButtons.Left)
      {
        cMouse.ButtonLeft = cMouse.BUTTON_STATE.Down;
        cMouse.DraggingStart = new Vector2(e.X, e.Y);// / Camera.ZoomLevel);// - Camera.Position;
        //Vector2 v = Camera.CalculatePosition(e.X, e.Y);
        SelectedItem = Flow.HitTest(cMouse.DraggingStart.Value, Camera);
        if (SelectedItem.Hit == true)
        {
          cMouse.FlowItem = SelectedItem.HitItem;
          if (SelectedItem.Type == HIT_RESULT.HIT_TYPE.Input || SelectedItem.Type == HIT_RESULT.HIT_TYPE.Output)
          {
            cMouse.DraggingStart = SelectedItem.Position;
            cMouse.FlowItem = SelectedItem.HitItem;
            cMouse.PreviousHitItem = SelectedItem;
          }
          else if (SelectedItem.Type == HIT_RESULT.HIT_TYPE.Resize)
          {
            cMouse.PreviousHitItem = SelectedItem.Clone();
            //We want to fool the system into thinking it actually selected the parent comment box so the resize handles continue to be drawn
            SelectedItem.Type = HIT_RESULT.HIT_TYPE.Comment;
            SelectedItem.HitItem = SelectedItem.ParentItem;
            SelectedItem.ParentItem = null;

          }
          else if (SelectedItem.Type == HIT_RESULT.HIT_TYPE.Comment)
          {
            cMouse.PreviousHitItem = SelectedItem;
          }
        }
      }
    }

    private void MoveSelectedItem(FlowBase item, Vector2 amount)
    {
      item.Position += amount;
      Flow.NeedToSave = true;
    }

    private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
    {
      cMouse.pos = new Vector2(e.X, e.Y);
      if (cMouse.ButtonRight == cMouse.BUTTON_STATE.Down && cMouse.DraggingStart.HasValue == true)
      {
        Vector2 v = cMouse.pos - cMouse.DraggingStart.Value;
        cMouse.DraggingStart = cMouse.pos;
        Camera.Move(v);
      }
      else if (cMouse.FlowItem is not null && cMouse.DraggingStart.HasValue == true)
      {
        Vector2 v = cMouse.pos - cMouse.DraggingStart.Value;
        v /= Camera.ZoomLevel;
        if (cMouse.PreviousHitItem.Type == HIT_RESULT.HIT_TYPE.Resize)
        {
          cMouse.DraggingStart = cMouse.pos;
          ResizeHandle? rh = cMouse.PreviousHitItem.HitItem as ResizeHandle;
          rh?.Resize(v);
        }
        else if (cMouse.FlowItem is FlowEngineCore.FunctionStep || cMouse.FlowItem is FlowEngineCore.Comment)
        {
          cMouse.DraggingStart = cMouse.pos;
          MoveSelectedItem(cMouse.FlowItem, v);
        }
      }
      else if (cMouse.OverallState == cMouse.OVERALL_STATE.DrawComment && cMouse.ButtonLeft == cMouse.BUTTON_STATE.Down == true)
      {

      }
      else
      {
        if (SelectedItem.Type == HIT_RESULT.HIT_TYPE.Comment)
        {
          HIT_RESULT hr = Flow.HitTestResizeOnly(new Vector2(e.X, e.Y), Camera);
          pictureBox1.Cursor = GetCursor(hr.HitItem as ResizeHandle);
        }
        else
        {
          if (ResizeItem.Hit == true)
          {
            pictureBox1.Cursor = Cursors.Default;
            ResizeItem = new HIT_RESULT();
          }
        }
      }

      pictureBox1.Refresh();
    }

    private Cursor GetCursor(ResizeHandle? rh)
    {
      if (rh is null)
        return Cursors.Default;

      if (rh.Location == ResizeHandle.RESIZE_LOCATION.TopLeft || rh.Location == ResizeHandle.RESIZE_LOCATION.BottomRight)
        return Cursors.SizeNWSE;
      else if (rh.Location == ResizeHandle.RESIZE_LOCATION.TopMiddle || rh.Location == ResizeHandle.RESIZE_LOCATION.BottomMiddle)
        return Cursors.SizeNS;
      else if (rh.Location == ResizeHandle.RESIZE_LOCATION.TopRight || rh.Location == ResizeHandle.RESIZE_LOCATION.BottomLeft)
        return Cursors.SizeNESW;
      else if (rh.Location == ResizeHandle.RESIZE_LOCATION.MiddleLeft || rh.Location == ResizeHandle.RESIZE_LOCATION.MiddleRight)
        return Cursors.SizeWE;

      return Cursors.Default;
    }

    private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        cMouse.ButtonRight = cMouse.BUTTON_STATE.Up;
      }
      else if (e.Button == MouseButtons.Left)
      {
        if (cMouse.OverallState == cMouse.OVERALL_STATE.DrawComment)
        {
          CreateComment();
        }
        else
        {
          CreateLink();
        }
        cMouse.ButtonLeft = cMouse.BUTTON_STATE.Up;
      }

      if (cMouse.ButtonLeft == cMouse.BUTTON_STATE.Up)
      {
        cMouse.FlowItem = null;
        cMouse.DraggingStart = null;
      }
    }

    private void CreateLink()
    {
      if (cMouse.PreviousHitItem.Hit == false)
        return;

      FlowEngineCore.FunctionStep? previousStep = cMouse.PreviousHitItem.ParentItem as FlowEngineCore.FunctionStep;
      if (previousStep is null)
        return;


      HIT_RESULT hit = Flow.HitTest(cMouse.pos, Camera);
      if (hit.Hit == false)
        return;


      //Linking from an Output to an Input
      if (hit.Type == HIT_RESULT.HIT_TYPE.Input)
      {
        if (cMouse.PreviousHitItem.Type != HIT_RESULT.HIT_TYPE.Output) //If we just hit an Input the previous step needs to be from an Output
          return;

        FlowEngineCore.Input? input = hit.HitItem as FlowEngineCore.Input;
        if (input is null || input.Step is null)
          return;

        FlowEngineCore.Output? output = cMouse.FlowItem as FlowEngineCore.Output;
        if (output is null)
          return;

        output = output.Clone(previousStep);
        previousStep.LinkAdd(Flow, output, input);
      }

      //Linking from an Input to an Output
      if (hit.Type == HIT_RESULT.HIT_TYPE.Output)
      {
        if (cMouse.PreviousHitItem.Type != HIT_RESULT.HIT_TYPE.Input) //If we just hit an Output the previous step needs to be from an Input
          return;

        FlowEngineCore.Output? output = hit.HitItem as FlowEngineCore.Output;
        if (output is null || output.Step is null)
          return;
        FlowEngineCore.Input? input = cMouse.FlowItem as FlowEngineCore.Input;
        if (input is null)
          return;

        input = input.Clone(previousStep);
        previousStep.LinkAdd(Flow, output, input);
      }
    }

    private void CreateComment()
    {
      if (cMouse.DraggingStart.HasValue == false)
        return;

      cMouse.OverallState = cMouse.OVERALL_STATE.None;
      pictureBox1.Cursor = Cursors.Default;
      Vector2 pos = (cMouse.DraggingStart.Value - Camera.Position) / Camera.ZoomLevel;
      int width = (int)((cMouse.pos.X - cMouse.DraggingStart.Value.X) / Camera.ZoomLevel);
      int height = (int)((cMouse.pos.Y - cMouse.DraggingStart.Value.Y) / Camera.ZoomLevel);
      if (width < ResizeHandle.MIN_SIZE)
        width = ResizeHandle.MIN_SIZE;
      if (height < ResizeHandle.MIN_SIZE)
        height = ResizeHandle.MIN_SIZE;
      Size size = new Size(width, height);
      Flow.CommentAdd(pos, size);
    }


    /// <summary>
    /// Drag and drop only ocures for the form not the controls on the form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmFlow_DragDrop(object sender, DragEventArgs e)
    {
      if (e.Data is not null)
      {
        FlowEngineCore.Function? fw = e.Data.GetData(typeof(FlowEngineCore.Function)) as FlowEngineCore.Function;
        if (fw is not null)
        {
          Point p = pictureBox1.PointToClient(new Point(e.X, e.Y));
          Vector2 v = Camera.CreateRealPosition(p.X, p.Y);
          if (fw.Plugin.Name == "FlowCore" && fw.Name == "Start")
          {
            FunctionStep? start = Flow.FindStepByName(fw.Plugin.Name, fw.Name);
            if (start is not null)
            {
              cEventManager.RaiseEventTracer(SENDER.Compiler, "Flow Already has a FlowCore.Start, can only have one start step per flow.", cEventManager.TRACER_TYPE.Error);
              return;
            }
          }
          string objType = fw.Plugin.Name + "." + fw.Name;
          Flow.StepAdd(objType, v);
          pictureBox1.Refresh();
        }
      }
    }

    private void frmFlow_DragEnter(object sender, DragEventArgs e)
    {
      e.Effect = DragDropEffects.Copy;
    }

    private void frmFlow_Resize(object sender, EventArgs e)
    {
      pictureBox1.Refresh();
    }

    private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      SaveAs();
    }

    private void SaveAs()
    {
      saveFileDialog1.Filter = "Flow file (*.flow)|*.flow|All files (*.*)|*.*";
      saveFileDialog1.DefaultExt = "flow";
      saveFileDialog1.FileName = Flow.FileName;
      saveFileDialog1.OverwritePrompt = true;
      DialogResult dr = saveFileDialog1.ShowDialog();
      if (dr == DialogResult.OK)
      {
        Flow.XmlWriteFile(saveFileDialog1.FileName);
        TitleText();
      }
    }

    private void closeToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void openToolStripMenuItem_Click(object sender, EventArgs e)
    {
      openFileDialog1.Filter = "Flow file (*.flow)|*.flow|All files (*.*)|*.*";
      openFileDialog1.Multiselect = true;
      saveFileDialog1.FileName = "";
      openFileDialog1.DefaultExt = "flow";
      DialogResult dr = openFileDialog1.ShowDialog();
      if (dr == DialogResult.OK)
      {
        Flow = new cFlowWrapper();
        Flow.XmlReadFile(openFileDialog1.FileName);
        Flow.PopulateSampleVariablesFromPlugin();
        pictureBox1.Refresh();
        TitleText();
      }

    }

    public void HighlightStep(int stepId)
    {
      FunctionStep? step = Flow.FindStepById(stepId);
      if (step is null)
        return;

      SelectedItem.Hit = true;
      SelectedItem.HitItem = step;
      SelectedItem.Position = step.Position;
      SelectedItem.Type = HIT_RESULT.HIT_TYPE.Function;
      Vector2 pos = (-step.Position - new Vector2(100,100)) * Camera.ZoomLevel;
      pos.X += pictureBox1.Width / 2;
      pos.Y += pictureBox1.Height / 2;
      Camera.Position = pos;
      pictureBox1.Refresh();
    }

    public void TitleText()
    {
      if (Flow.FileName.Length > 0)
      {
        this.Text = $"Flow - [{Flow.FileName}]";
      }
      else
      {
        this.Text = "Flow - [New]";
      }
    }

    private void saveToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (Flow.FileName == "")
      {
        SaveAs();
        return;
      }
      else
      {
        Flow.XmlWriteFile();
      }
    }

    private void tsbPlay_Click(object sender, EventArgs e)
    {
      FlowDebug flowDebug = new FlowDebug(cOptions.AdministrationPrivateKey, cServer.UserLoggedIn.SessionKey, Flow.FileName, FlowRequest.START_TYPE.Now, Flow.XmlWriteMemory(), "", "");
      cServer.SendAndResponse(flowDebug.GetPacket(), Callback_FlowDebug);
    }

    private void Callback_FlowDebug(FlowEngineCore.Administration.EventArgsPacket e)
    {
    }

    private void TsbComment_Click(object sender, EventArgs e)
    {
      if (cMouse.OverallState == cMouse.OVERALL_STATE.None)
      {
        cMouse.OverallState = cMouse.OVERALL_STATE.DrawComment;
        this.pictureBox1.Cursor = Cursors.Cross;
      }
      else
      {
        cMouse.OverallState = cMouse.OVERALL_STATE.None;
        this.pictureBox1.Cursor = Cursors.Default;
      }
    }

    private void toolStripDropDownButton1_Click(object sender, EventArgs e)
    {
      Camera.ZoomIn();
      pictureBox1.Refresh();
      ShowZoom();
    }

    private void ShowZoom()
    {
      tslZoom.Text = Camera.ZoomLevel.ToString("0%");

    }

    private void tsbZoomOut_Click(object sender, EventArgs e)
    {
      Camera.ZoomOut();
      pictureBox1.Refresh();
      ShowZoom();
    }

    private void tslZoom_Click(object sender, EventArgs e)
    {
      Camera.ZoomLevel = 1f;
      pictureBox1.Refresh();
      ShowZoom();
    }


    private void pictureBox1_DoubleClick(object sender, EventArgs e)
    {
      if (SelectedItem.Hit == true)
      {
        FlowEngineCore.FunctionStep? step = SelectedItem.HitItem as FlowEngineCore.FunctionStep;
        if (step is not null)
        {
          frmStepProperties f = new frmStepProperties(step, Flow);
          f.Location = CenterOfForm(f);
          f.Show();
        }
        FlowEngineCore.Comment? comment = SelectedItem.HitItem as FlowEngineCore.Comment;
        if (comment is not null)
        {
          frmCommentProperties f = new frmCommentProperties(comment);
          f.Location = CenterOfForm(f);
          f.Show();
        }
      }
    }

    private Point CenterOfForm(Form childForm)
    {
      Point pt = new Point(this.Left + (this.Width / 2) - (childForm.Width / 2), this.Top + (this.Height / 2) - (childForm.Height / 2));
      return pt;
    }


    private void frmFlow_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Delete)
      {
        if (SelectedItem.Type == HIT_RESULT.HIT_TYPE.Function)
        {
          Flow.StepDelete(SelectedItem.HitItem as FlowEngineCore.FunctionStep);
          SelectedItem = new HIT_RESULT();
          pictureBox1.Refresh();
        }
        else if (SelectedItem.Type == HIT_RESULT.HIT_TYPE.Link)
        {
          FunctionStep? fs = SelectedItem.ParentItem as FlowEngineCore.FunctionStep;
          FlowEngineCore.Link? link = SelectedItem.HitItem as FlowEngineCore.Link;
          if (fs is null || link is null)
            return;

          Flow.LinkDelete(fs, link);
          SelectedItem = new HIT_RESULT();
          pictureBox1.Refresh();
        }
        else if (SelectedItem.Type == HIT_RESULT.HIT_TYPE.Comment)
        {
          Comment? comment = SelectedItem.HitItem as FlowEngineCore.Comment;
          if (comment is not null)
          {
            Flow.Comments.Remove(comment);
          }
          SelectedItem = new HIT_RESULT();
          pictureBox1.Refresh();
        }
      }
    }

    private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
    {
      frmFlowProperties f = new frmFlowProperties(Flow);
      f.Location = CenterOfForm(f);
      f.Show();
    }

    private void frmFlow_Activated(object sender, EventArgs e)
    {
      serverToolStripMenuItem.Enabled = cServer.IsConnectedToServer;
    }

    /// <summary>
    /// I use this to activate the form if it isn't the active window. If this isn't there it will take 2 clicks to activate a menu or toolstrip button, makes it rather annoying.
    /// </summary>
    /// <param name="m"></param>
    protected override void WndProc(ref Message m)
    {
      const int WM_PARENTNOTIFY = 0x0210;
      if (this.Focused == false && m.Msg == WM_PARENTNOTIFY)
      {
        // Make this form auto-grab the focus when menu/controls are clicked
        this.Activate();
      }
      base.WndProc(ref m);
    }

    private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
    {
      frmAdministrationFile f = new frmAdministrationFile(FILE_MODE.Save, Flow, FlowWrapperChanged_Save_Callback);
      f.Location = CenterOfForm(f);
      f.Show();
    }

    private void openToolStripMenuItem1_Click(object sender, EventArgs e)
    {
      frmAdministrationFile f = new frmAdministrationFile(FILE_MODE.Open, Flow, FlowWrapperChanged_Open_Callback);
      f.Location = CenterOfForm(f);
      f.Show();
    }
    private void FlowWrapperChanged_Open_Callback(cFlowWrapper flowWrapper)
    {
      Flow = flowWrapper;
      Flow.PopulateSampleVariablesFromPlugin();
      TitleText();
      Flow.Center(this.Camera, pictureBox1);
      pictureBox1.Refresh();
    }

    private void FlowWrapperChanged_Save_Callback(cFlowWrapper flowWrapper)
    {
      Flow = flowWrapper;
      Flow.PopulateSampleVariablesFromPlugin();
      TitleText();
      pictureBox1.Refresh();
    }

    private void tssCenterView_Click(object sender, EventArgs e)
    {
      Flow.Center(Camera, pictureBox1);
      pictureBox1.Refresh();
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {

    }

    private void tslCameraPosition_Click(object sender, EventArgs e)
    {

    }
  }
}
