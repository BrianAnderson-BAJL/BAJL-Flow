using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineDesigner
{
  internal class cLayout
  {
    public string Name = "";
    public List<cLayoutForm> FormLayouts = new List<cLayoutForm>();
    

    public void ExecuteLayout()
    {
      for (int i = 0; i < FormLayouts.Count; i++)
      {
        cLayoutForm layoutForm = FormLayouts[i];
        Form? f = null;
        if (layoutForm.layoutForm == cLayoutForm.LAYOUT_FORM.Main)
        {
          f = FindForm("frmMain");
        }
        else if (layoutForm.layoutForm == cLayoutForm.LAYOUT_FORM.Toolbox) 
        {
          f = FindForm("frmToolbox");
          if (f is null && layoutForm.open == true)
          {
            f = new frmToolbox();
            f.Show();
          }
        }
        else if (layoutForm.layoutForm == cLayoutForm.LAYOUT_FORM.Flow)
        {
          f = FindForm("frmFlow");
          if (f is null && layoutForm.open == true)
          {
            f = new frmFlow();
            f.Show();
          }
        }
        else if (layoutForm.layoutForm == cLayoutForm.LAYOUT_FORM.Tracer)
        {
          f = FindForm("frmTracer");
          if (f is null && layoutForm.open == true)
          {
            f = new frmTracer();
            f.Show();
          }
        }

        if (f is not null)
        {
          f.Left = (int)layoutForm.position.X;
          f.Top = (int)layoutForm.position.Y;
          f.Width = (int)layoutForm.size.X;
          f.Height = (int)layoutForm.size.Y;
        }
      }
    }

    public void ExecuteLayout(Form f, cLayoutForm.LAYOUT_FORM form)
    {
      cLayoutForm? layoutForm = FindLayout(form);
      if (layoutForm is null)
        return;

      f.Left = (int)layoutForm.position.X;
      f.Top = (int)layoutForm.position.Y;
      f.Width = (int)layoutForm.size.X;
      f.Height = (int)layoutForm.size.Y;

    }

    private cLayoutForm? FindLayout(cLayoutForm.LAYOUT_FORM form) 
    {
      for (int x = 0; x < FormLayouts.Count; x++)
      {
        cLayoutForm layoutForm = FormLayouts[x];

        if (layoutForm.layoutForm == form)
        {
          return layoutForm;
        }
      }
      return null;
    }

    private Form? FindForm(string name)
    {
      for (int x = 0; x < Application.OpenForms.Count; x++)
      {
        if (Application.OpenForms[x].Name == name)
          return Application.OpenForms[x];
      }
      return null;
    }
  }
}
