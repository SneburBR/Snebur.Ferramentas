using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using System.IO;

namespace Snebur.VisualStudio.Projeto.TypeScript
{
    public class ArquivoTSSistema : BaseArquivoTypeScript
    {
        private const string NAMESPACE = "Snebur";
        private const string ARQUIVO_REFLEXAO = "Reflexao.ts";
        private const string ARQUIVO_APLICACAO = "Aplicacao.ts";
        private const string ARQUIVO_APLICACAO_CONFIG = "Aplicacao.Config.ts";
        private const string REFERENCIAS = ".Referencias.ts";
        private const string VARIAVEIS = ".Variaveis.ts";

        public static string[] ArquivosSistema { get; set; } = { ARQUIVO_REFLEXAO, ARQUIVO_APLICACAO, ARQUIVO_APLICACAO_CONFIG };

        public ArquivoTSSistema(ProjetoTypeScript projeto, FileInfo arquivo, int prioridadeProjeto) : base(projeto, arquivo, prioridadeProjeto)
        {

        }

        protected override string RetornarClasseSuperior()
        {
            return null;
        }

        protected override string RetornarNamespace()
        {
            return NAMESPACE;
        }

        protected override EnumTipoArquivoTypeScript RetornarTipoArquivoTypeScript()
        {
            if (this.Arquivo.Name.EndsWith(REFERENCIAS))
            {
                return EnumTipoArquivoTypeScript.SistemaReferencias;
            }

            if (this.Arquivo.Name.EndsWith(VARIAVEIS))
            {
                return EnumTipoArquivoTypeScript.SistemaVariaveis;
            }

            if (this.Arquivo.Name.EndsWith(".d.ts"))
            {
                return EnumTipoArquivoTypeScript.SistemaDeclarationType;
            }


            switch (this.Arquivo.Name)
            {
                case ARQUIVO_REFLEXAO:

                    return EnumTipoArquivoTypeScript.SistemaReflexao;

                case ARQUIVO_APLICACAO:

                    return EnumTipoArquivoTypeScript.SistemaAplicacao;

                case ARQUIVO_APLICACAO_CONFIG:

                    return EnumTipoArquivoTypeScript.SistemaAplicacaoConfig;

                default:
                    throw new Exception("Arquivo sistema não suportado");

            }
        }

        public static bool IsArquivoSistema(string nomeArquivo)
        {
            return ArquivoTSSistema.ArquivosSistema.Contains(nomeArquivo) ||
                                   nomeArquivo.EndsWith(REFERENCIAS) ||
                                   nomeArquivo.EndsWith(VARIAVEIS) ||
                                   nomeArquivo.EndsWith(".d.ts");
        }
    }
}
