using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.VisualBasic.Devices;
using Core;

namespace FlowEngineDesigner
{
  public partial class frmFlow : Form
  {
    cFlowWrapper Flow = new cFlowWrapper();
    cCamera Camera;

    HIT_RESULT SelectedItem;

    public frmFlow()
    {
      InitializeComponent();
      Camera = new cCamera(this);
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

      if (cMouse.DraggingStart.HasValue == true && (cMouse.FlowItem is Core.Input || cMouse.FlowItem is Core.Output))
      {
        Pen p = new Pen(Color.Blue, 3);

        Vector2 v1 = Camera.CreateDrawingPosition(cMouse.DraggingStart.Value);
        Vector2 v2 = cMouse.pos; 
        e.Graphics.DrawLine(p, v1.ToPoint(), v2.ToPoint());
      }

      if (SelectedItem.Hit == true && SelectedItem.HitItem != null)
      {
        Flow.DrawSelection(SelectedItem, e.Graphics, Camera);
      }
      Flow.DrawExecutingCurrent(e.Graphics, Camera);
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
      else if (cMouse.FlowItem != null && cMouse.DraggingStart.HasValue == true)
      {
        if (cMouse.FlowItem is Core.FunctionStep || cMouse.FlowItem is Core.Comment)
        {
          Vector2 v = cMouse.pos - cMouse.DraggingStart.Value;
          v /= Camera.ZoomLevel;
          cMouse.DraggingStart = cMouse.pos;
          MoveSelectedItem(cMouse.FlowItem, v);
        }
      }
      else if (cMouse.OverallState == cMouse.OVERALL_STATE.DrawComment && cMouse.ButtonLeft == cMouse.BUTTON_STATE.Down == true)
      {
        
      }

      pictureBox1.Refresh();
    }

    private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        cMouse.ButtonRight = cMouse.BUTTON_STATE.Up;
      }
      else if (e.Button == MouseButtons.Left)
      {
        if (cMouse.OverallState == cMouse.OVERALL_STATE.DrawComment && cMouse.DraggingStart.HasValue == true)
        {
          cMouse.OverallState = cMouse.OVERALL_STATE.None;
          pictureBox1.Cursor = Cursors.Default;
          Comment c = new Comment();
          c.Position = (cMouse.DraggingStart.Value - Camera.Position) / Camera.ZoomLevel;

          c.Size = new Size((int)((cMouse.pos.X - cMouse.DraggingStart.Value.X) / Camera.ZoomLevel), (int)((cMouse.pos.Y - cMouse.DraggingStart.Value.Y) / Camera.ZoomLevel));
          c.ColorBackground = cOptions.CommentColorBackgroundDefault;
          c.ColorText = cOptions.CommentColorTextDefault;
          Flow.Comments.Add(c);
        }
        else
        {
          HIT_RESULT hit = Flow.HitTest(cMouse.pos, Camera);
          if (hit.Hit == true)
          {
            if (hit.Type == HIT_RESULT.HIT_TYPE.Input)
            {
              Core.Input? ci = hit.HitItem as Core.Input;
              if (ci != null && ci.Step != null)
              {
                Core.Output? co = cMouse.FlowItem as Core.Output;
                if (co != null)
                {
                  if (cMouse.PreviousHitItem.Type == HIT_RESULT.HIT_TYPE.Output && cMouse.PreviousHitItem.ParentItem is Core.FunctionStep)
                  {
                    Core.FunctionStep? step = cMouse.PreviousHitItem.ParentItem as Core.FunctionStep;
                    if (step != null)
                    {
                      co = co.Clone(step);
                      step.LinkAdd(Flow, co, ci);
                    }
                  }
                }
              }
            }
            if (hit.Type == HIT_RESULT.HIT_TYPE.Output)
            {
              Core.Output? co = hit.HitItem as Core.Output;
              if (co != null && co.Step != null)
              {
                Core.Input? ci = cMouse.FlowItem as Core.Input;
                if (ci != null)
                {
                  if (cMouse.PreviousHitItem.Type == HIT_RESULT.HIT_TYPE.Input && cMouse.PreviousHitItem.ParentItem is Core.FunctionStep)
                  {
                    Core.FunctionStep? step = cMouse.PreviousHitItem.ParentItem as Core.FunctionStep;
                    if (step != null)
                    {
                      ci = ci.Clone(step);
                      step.LinkAdd(Flow, co, ci);
                    }
                  }
                }
              }
            }
          }
        }
        cMouse.ButtonLeft = cMouse.BUTTON_STATE.Up;
      }

      if (cMouse.ButtonLeft == cMouse.BUTTON_STATE.Up)
      {
        cMouse.FlowItem = null;
        cMouse.DraggingStart = null;
      }
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {

    }

    private void pictureBox1_DragDrop(object sender, DragEventArgs e)
    {

    }

    /// <summary>
    /// Drag and drop only ocures for the form not the controls on the form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmFlow_DragDrop(object sender, DragEventArgs e)
    {
      if (e.Data != null)
      {
        Core.Function? fw = e.Data.GetData(typeof(Core.Function)) as Core.Function;
        if (fw != null)
        {
          Point p = pictureBox1.PointToClient(new Point(e.X, e.Y));
          Vector2 v = Camera.CreateRealPosition(p.X, p.Y);
          if (fw.Plugin.Name == "Flow" && fw.Name == "Start")
          {
            FunctionStep? start = Flow.FindStepByName(fw.Plugin.Name, fw.Name);
            if (start != null)
            {
              cEventManager.RaiseEventTracer(Flow, "Flow Already has a Flow.Start, can only have one start step per flow.", cEventManager.TRACER_TYPE.Error);
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
        Flow.XmlWrite(saveFileDialog1.FileName);
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
        Flow.XmlRead(openFileDialog1.FileName);
        Flow.PopulateSampleVariablesFromPlugin();
        pictureBox1.Refresh();
        TitleText();
      }

    }

    private void TitleText()
    {
      this.Text = String.Format("Flow - [{0}]", Flow.FileName);
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
        Flow.XmlWrite();
      }
    }

    private void tsbPlay_Click(object sender, EventArgs e)
    {
      cFlowWrapper flowRun = Flow.Clone();  //Need to clone it before running
      flowRun.Execute(pictureBox1);
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

    private void frmFlow_MouseEnter(object sender, EventArgs e)
    {
      if (cOptions.FocusOnMouseEnter == true)
      {
        this.Focus();
      }
    }

    private void toolStrip1_MouseEnter(object sender, EventArgs e)
    {
      if (cOptions.FocusOnMouseEnter == true)
      {
        this.Focus();
      }
    }

    private void pictureBox1_MouseEnter(object sender, EventArgs e)
    {
      if (cOptions.FocusOnMouseEnter == true)
      {
        this.Focus();
      }
    }

    private void pictureBox1_DoubleClick(object sender, EventArgs e)
    {
      if (SelectedItem.Hit == true)
      {
        Core.FunctionStep? step = SelectedItem.HitItem as Core.FunctionStep;
        if (step is not null)
        {
          frmStepProperties f = new frmStepProperties(step, Flow);
          f.Show();
        }
        Core.Comment? comment = SelectedItem.HitItem as Core.Comment;
        if (comment is not null)
        {
          frmCommentProperties f = new frmCommentProperties(comment);
          f.Show();
        }
      }
    }

    private void frmFlow_KeyPress(object sender, KeyPressEventArgs e)
    {

    }

    private void frmFlow_KeyUp(object sender, KeyEventArgs e)
    {

    }

    private void frmFlow_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Delete)
      {
        if (SelectedItem.Type == HIT_RESULT.HIT_TYPE.Function)
        {
          Flow.StepDelete(SelectedItem.HitItem as Core.FunctionStep);
          SelectedItem = new HIT_RESULT();
          pictureBox1.Refresh();
        }
      }
    }

    private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
    {
      frmFlowProperties f = new frmFlowProperties(Flow);
      f.Show();
    }
  }
}
