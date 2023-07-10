using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grizzlly.BlazorJS
{
    public sealed record class Emit
    {
        public string Name { get; }
        public Action<object?> Func { get; }
        public Emit(string name, Action<object?> func)
        {
            Name = name;
            Func = func;
        }
    }
}
