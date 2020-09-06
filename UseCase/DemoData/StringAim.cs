using System.Diagnostics;
using Ai.Infrastructure.Search;

namespace UseCase.DemoData
{
    [DebuggerDisplay("Name= {Name}")]
    public class StringAim : Aim
    {
        public StringAim(string name) : base(name)
        {
        }

        public static implicit operator StringAim(string str) => new StringAim(str);
    }
}