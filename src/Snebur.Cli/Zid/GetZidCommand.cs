using Snebur.Utilidade;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace Snebur.Cli.Zid;
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

