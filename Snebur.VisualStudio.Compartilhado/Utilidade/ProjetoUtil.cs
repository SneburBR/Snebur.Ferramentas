using System;
using System.Collections.Generic;

namespace Snebur.VisualStudio
{
    internal class ProjetoUtil
    {
        public const string CONFIGURACAO_DOMINIO = "dominio.json";
        public const string CONFIGURACAO_TYPESCRIPT = "tsconfig.json";
        public const string CONFIGURACAO_CONTEXTO_DADOS = "contextodados.json";
        public const string CONFIGURACAO_REGRIAS_NEGOCIO = "regrasnegocio.json";
        public const string CONFIGURACAO_SASS = "compilerconfig.json";
        public const string CONFIGURACAO_SERVICOS = "servicos.json";
        public const string CONFIGURACAO_WEB_CONFIG = "Web.config";
        public const string CONFIGURACAO_APP_SETTINGS = "appSettings.config";
        internal static IEnumerable<string> RetornarTodosArquivos(string caminhoProjeto, bool v)
        {
            throw new NotImplementedException();
        }
    }
}
