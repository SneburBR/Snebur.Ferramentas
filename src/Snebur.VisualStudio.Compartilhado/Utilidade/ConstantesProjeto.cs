//futuramente vamos normalizar os Error, passando um parametro da mapeamento do arquivo e linha
//detalhes aqui https://www.html5rocks.com/en/tutorials/developertools/sourcemaps/

using Snebur.Publicacao;
using System;
using System.Collections.Generic;
using System.IO;

namespace Snebur.VisualStudio
{
    public static class ConstantesProjeto
    {
        public const string PASTA_WWWROOT_BUILD = ConstantesPublicacao.NOME_PASTA_WWWROOT_BUILD;
        public const string PASTA_SRC = "src";

        public const string EXTENSAO_ESTILO = ".css";
        public const string EXTENSAO_CONTROLE_SHTML = ".shtml";
        public const string EXTENSAO_CONTROLE_SHTML_TYPESCRIPT = ".shtml.ts";
        public const string EXTENSAO_CONTROLE_SHTML_SCSS = ".shtml.scss";
        public const string EXTENSAO_SCRIPT = ".js";
        public const string EXTENSAO_TYPESCRIPT = ".ts";
        public const string EXTENSAO_SASS = ".scss";

        public const string CONFIGURACAO_DOMINIO = "dominio.json";
        public const string CONFIGURACAO_TYPESCRIPT = "tsconfig.json";
        public const string CONFIGURACAO_CONTEXTO_DADOS = "contextodados.json";
        public const string CONFIGURACAO_REGRIAS_NEGOCIO = "regrasnegocio.json";
        public const string CONFIGURACAO_SASS = "compilerconfig.json";
        public const string CONFIGURACAO_SERVICOS = "servicos.json";
        public const string CONFIGURACAO_WEB_CONFIG = "Web.config";
        public const string CONFIGURACAO_APP_SETTINGS = "appSettings.config";

        public static HashSet<string> ExtensoesWeb { get; } = new HashSet<string> { EXTENSAO_TYPESCRIPT, EXTENSAO_SASS, EXTENSAO_CONTROLE_SHTML };
        public static HashSet<string> ExtensoesControlesSnebur { get; } = new HashSet<string> { EXTENSAO_CONTROLE_SHTML, EXTENSAO_CONTROLE_SHTML_TYPESCRIPT, EXTENSAO_CONTROLE_SHTML_SCSS };

    }

}