using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
  public class Link : FlowBase
  {
    public Input Input = new Input("Input", Vector2.Zero);
    public Output Output = new Output();
    //public Flow Flow;

    public Link(int id, Output output, Input input)
    {
      Id = id;
      Output = output;
      Input = input;
    }
  }
}
