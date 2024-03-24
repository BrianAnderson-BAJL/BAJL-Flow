using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
  public class Tenant
  {
    /// <summary>
    /// This is the name that is used for the directory that will store flows for this tenant
    /// </summary>
    public string ShortName = "";

    /// <summary>
    /// The unique name of the tenant
    /// </summary>
    public string Name = "";

    public Tenant() { }
    

    public static Tenant None
    {
      get { return new Tenant(); }
    }
  }
}
