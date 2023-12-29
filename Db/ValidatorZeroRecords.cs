using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
  public class ValidatorZeroRecords : Core.FunctionValidator
  {

    public ValidatorZeroRecords()
    {
      Name = "Zero records found is SUCCESS";
      Descriptoin = "If the SQL returns zero records in the recordset then consider this a success. Might want to ensure that an Email address does not already appear in the database.";
    }

    public override void Validate(RESP resp)
    {
      if (resp.Success == false)
        return; //It already failed for another reason, maybe bad SQL, no need to check more

      if (resp.Variable is null)
      {
        resp.SetSuccess();
        return;
      }

      if (resp.Variable.SubVariables.Count == 0)
      {
        resp.SetSuccess();
        return;
      }
    }

    public override string ToString()
    {
      return Name;
    }
  }
}
