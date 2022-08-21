using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;

namespace Snebur.VisualStudio.Projeto.TypeScript
{
    public class ConfiguracaoProjetoTypeScript
    {
        public compilerOptions compilerOptions { get; set; }

        public List<string> files { get; set; } = new List<string>();

        public List<string> exclude { get; set; }

        public List<string> ProjetosDepedentes { get; set; } = new List<string>();

        public int PrioridadeProjeto { get; set; } = 0;

        public string UrlDesenvolvimento { get; set; } = "";

        public ConfiguracaoProjetoTypeScript()
        {

        }

        public ConfiguracaoProjetoTypeScript(ConfiguracaoProjetoTypeScript configuracao, string caminhoJavasriptSaida, List<string> arquivos)
        {
            this.compilerOptions = new compilerOptions(caminhoJavasriptSaida);
            this.files = arquivos;

            this.UrlDesenvolvimento = configuracao.UrlDesenvolvimento;
            this.PrioridadeProjeto = configuracao.PrioridadeProjeto;
            this.ProjetosDepedentes = configuracao.ProjetosDepedentes;
            this.exclude = new List<string>();
            this.exclude.Add("node_modules");
            this.exclude.Add("wwwroot");
        }

    }
}
