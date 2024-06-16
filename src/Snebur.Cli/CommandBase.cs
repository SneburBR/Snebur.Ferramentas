using System.CommandLine;

namespace Snebur.Cli;

abstract class CommandBase : Command
{
    protected CommandBase(string name, string? description = null) : base(name, description)
    {
    }
}

