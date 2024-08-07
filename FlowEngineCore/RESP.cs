﻿using FlowEngineCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore
{
  public class RESP
  {
    public const uint SUCCESS = 0;
    public const uint ERROR = 1;
    private int mErrorNumber = 0;
    private string mErrorDescription = "";
    private Output.TYPE mOutputType = Output.TYPE.Success;
    private Variable? OutputVar = null;
    private static ILog? mLog = null;

    public static ILog? Log
    {
      get { return mLog; }
      set { mLog = value; }
    }

    public Variable? Variable
    {
      get { return OutputVar; }
      private set { OutputVar = value; }
    }

    public static RESP SetSuccess(Variable? output = null)
    {
      RESP r = new RESP();
      r.mOutputType = Output.TYPE.Success;
      r.mErrorNumber = 0;
      r.OutputVar = output;
      return r;
    }

    public static RESP SetError(int errornumber, string errorDescription, uint outputIndex = 1)
    {
      RESP r = new RESP();
      r.mErrorNumber = errornumber;
      r.mOutputType = (Output.TYPE)outputIndex;
      r.mErrorDescription = errorDescription;
      mLog?.Write(errorDescription, LOG_TYPE.DBG);
      return r;
    }

    public void SetError(STEP_ERROR_NUMBERS errorNumber, string errorDescription)
    {
      mOutputType = Output.TYPE.Error;
      mErrorNumber = (int)errorNumber;
      mErrorDescription = errorDescription;
    }

    public void SetSuccess()
    {
      mOutputType = Output.TYPE.Success;
      mErrorNumber = 0;
      mErrorDescription = "";
    }

    public bool Success
    {
      get 
      { 
        if (mOutputType == Output.TYPE.Success)
          return true;
        else
          return false;
      }
    }

    public Output.TYPE OutputType
    {
      get {return mOutputType; }
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
