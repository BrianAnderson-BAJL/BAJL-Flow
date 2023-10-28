﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
  public interface IDatabase
  {
    void Connect(string connectionString);
    VariableInteger Execute(string SQL, params Variable[] vars);
    Variable Select(string SQL, params Variable[] vars);
  }
}
