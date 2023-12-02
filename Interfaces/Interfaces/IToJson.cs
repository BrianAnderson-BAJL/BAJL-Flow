using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
  public enum JSON_ROOT_BLOCK_OPTIONS
  {
    None,
    AddRootBlock,
    StripNameFromRootAndAddBlock,
    StripRootBlock
  }

  internal interface IToJson
  {
    string ToJson(JSON_ROOT_BLOCK_OPTIONS options = JSON_ROOT_BLOCK_OPTIONS.None, int TabIndents = 0);
  }
}
