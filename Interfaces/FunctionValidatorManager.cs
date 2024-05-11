using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FlowEngineCore.FunctionValidator;

namespace FlowEngineCore
{
  public class FunctionValidatorManager
  {
    private List<FunctionValidator> mValidators = new List<FunctionValidator>();

    public ReadOnlyCollection<FunctionValidator> Validators
    {
      get { return mValidators.AsReadOnly(); }
    }

    public void Add(FunctionValidator validator)
    {
      mValidators.Add(validator);
    }

    public FunctionValidator? FindByName(string name)
    {
      for (int x = 0; x < mValidators.Count; x++)
      {
        if (mValidators[x].Name == name)
          return mValidators[x];
      }
      return null;
    }

  }
}
