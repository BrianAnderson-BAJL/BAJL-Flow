using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
  public class ValidatorAtLeastOneRec : Core.FunctionValidator
  {

    public ValidatorAtLeastOneRec() 
    {
      Name = "At least one record";
      Descriptoin = "If the recordset contains at least one record then this is a success. If the recordset doesn't contain any records then it is a failure/error.";
    }

    public override void Validate(RESP resp)
    {
      if (resp.Success == false)
        return; //It already failed for another reason, no need to check more

      if (resp.Variable is null)
      {
        resp.SetError(Core.STEP_ERROR_NUMBERS.FV_MustHaveOneRecord, Name);
        return;
      }

      if (resp.Variable.SubVariables.Count == 0)
      {
        resp.SetError(STEP_ERROR_NUMBERS.FV_MustHaveOneRecord, Name);
        return;
      }
    }

    public override string ToString()
    {
      return Name;
    }
  }
}
