using System.CommandLine;

namespace Snebur.Cli
{
    public abstract class CommandBase : Command
    {
        protected CommandBase(string name, string description = null) : base(name, description)
        {
        }
    }
}
