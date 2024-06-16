using Snebur.Utilidade;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace Snebur.Cli.Zid;

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

