using Snebur.Utilidade;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace Snebur.Cli.Zid;

abstract class CommandBase : Command
{
    protected CommandBase(string name, string? description = null) : base(name, description)
    {
    }
}
/// <summary>
/// Generate a Zyoncore Id from zid from id, command sn --get-id 10
/// </summary>
class GetZidCommand : CommandBase
{
    //write a descript for this command, that get Id generated hash called Zid (Zyoncore Id) Ex: 1 1 => 15656

    public GetZidCommand()
        : base("--get-zid", "Generate a Zyoncore Id (Zid)")
    {
        AddArgument(new Argument<int>("id", "Id to generate Zid"));
        this.Handler = CommandHandler.Create<int>(HandleCommand);

    }

    private void HandleCommand(int id)
    {
        Console.WriteLine($"Zid: {id} => {ZidUtil.RetornarZid(id)}");
    }
}

class GetIdCommand : CommandBase
{
    //write a descript for this command, that get Id generated hash called Zid (Zyoncore Id) Ex: 1 1 => 15656

    public GetIdCommand()
        : base("--get-id", "Get Id from Zyoncore Id (Zid)")
    {
        AddArgument(new Argument<int>("zid", "Id to generate Zid"));

        this.Handler = CommandHandler.Create<int>(HandleCommand);

    }

    private void HandleCommand(int zid)
    {
        Console.WriteLine($"Id: {zid} => {ZidUtil.RetornarId(zid)}");
    }
}

