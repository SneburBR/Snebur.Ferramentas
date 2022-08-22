using System;

namespace Snebur.VisualStudio
{
    public class GerenciadorProjetosUtil
    {
        public static string DiretorioProjetoTypescriptInicializacao => BaseAplicacaoVisualStudio.Instancia.GerenciadorProjetos.DiretorioProjetoTypescriptInicializacao;
        public static ConfiguracaoProjetoTypeScript ConfiguracaoProjetoTypesriptInicializacao => BaseAplicacaoVisualStudio.Instancia.GerenciadorProjetos.ConfiguracaoProjetoTypesriptInicializacao;

    }
}