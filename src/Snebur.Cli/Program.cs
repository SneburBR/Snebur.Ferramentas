using System;
using Snebur.Cli.Zid;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;

namespace Snebur.VisualStudio.Comandos
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            var projeto = @"E:\Github\Zyoncore\Sigi.Legacy\src\Zyoncore.Sigi.Control\Zyoncore.Sigi.Control.csproj";
            var caminhoSolution = @"E:\Github\Zyoncore\Sigi.Legacy\Sigi.Legacy.sln";
            var destino = @"E:\temp\source";

            //CopiarProjectSourceUtil.Copiar(projeto, caminhoSolution,  destino);

            var rootCommand = new RootCommand("Snebur CLI");
            rootCommand.Handler = CommandHandler.Create((Action)HandleDefault);

            var zidCommand = new Command("zid", "Parent command for Zid operations");
            zidCommand.AddCommand(new GetZidCommand());
            zidCommand.AddCommand(new GetIdCommand());
            rootCommand.AddCommand(zidCommand);
            return await rootCommand.InvokeAsync(args);

        }
        private static void HandleDefault()
        {
            ///write a snebur ascii art
            var greeting = "Welcome to Snebur CLI";
            var sneburAsciIArt = $@"
                ____    _   _   _____   ____    _   _   ____           ____ _     ___ 
               / ___|  | \ | | | ____| | __ )  | | | | |  _ \         / ___| |   |_ _|
               \___ \  |  \| | |  _|   |  _ \  | | | | | |_) |       | |   | |    | | 
                ___) | | |\  | | |___  | |_) | | |_| | |  _ <        | |___| |___ | | 
               |____/  |_| \_| |_____| |____/   \___/  |_| \_\        \____|_____|___|  

             ";


            //write in pen red
            Console.WriteLine(greeting);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(sneburAsciIArt);
            Console.ResetColor();
        }
    }
}
