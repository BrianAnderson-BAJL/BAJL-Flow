using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
  public class RESP
  {
    public const uint SUCCESS = 0;
    public const uint ERROR = 1;
    private int mErrorNumber = 0;
    private string mErrorDescription = "";
    private uint mOutputIndex = 0; //0 = SUCCESS
    private Variable? Output = null;


    public Variable? Variable
    {
      get { return Output; }
      private set { Output = value; }
    }

    public static RESP SetSuccess(Variable? output = null)
    {
      RESP r = new RESP();
      r.mOutputIndex = SUCCESS;
      r.mErrorNumber = 0;
      return r;
    }
    public static RESP SetError(int errornumber, string errorDescription, uint outputIndex = 1)
    {
      RESP r = new RESP();
      r.mErrorNumber = errornumber;
      r.mOutputIndex = outputIndex;
      Global.Write(errorDescription);
      return r;
    }
    public bool Success
    {
      get 
      { 
        if (mOutputIndex == 0)
          return true;
        else
          return false;
      }
    }

    public int OutputIndex
    {
      get {return (int)mErrorNumber; }
    }

    public int ErrorNumber
    { 
      get { return mErrorNumber; }
    }

    public string ErrorDescription
    { 
      get { return mErrorDescription; } 
    }
  }
}
