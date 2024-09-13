//ubuntu.22.04-x64
//centos-x64

using FlowEngineCore;
using System.Reflection;

//This is a way to resolve the assemblies for the plugins, all the support DLLs for plugins (MySql, FireBase, ...) need to be in the base directory with the exe
AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

FlowEngineCore.FlowEngine engine = new FlowEngineCore.FlowEngine();
engine.Init(args);
engine.Run();


static Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
{
  string baseName = args.Name.Substring(0, args.Name.IndexOf(","));
  string assemblyFileName = baseName + ".dll";
  assemblyFileName = AppDomain.CurrentDomain.BaseDirectory + assemblyFileName;
  try
  {
    return Assembly.LoadFile(assemblyFileName);
  }
  catch (FileNotFoundException fnfe)
  {
    FlowEngineCore.Global.WriteAlways(Global.FullExceptionMessage(fnfe), LOG_TYPE.ERR);
  }
  catch (Exception ex)
  {
    FlowEngineCore.Global.WriteAlways(Global.FullExceptionMessage(ex), LOG_TYPE.ERR);
  }
  return null;
}
