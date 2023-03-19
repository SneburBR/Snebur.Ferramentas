using System;
using System.Collections.Generic;
using System.IO;

namespace Snebur.VisualStudio
{
    public class ArquivoTSSistema : BaseArquivoTypeScript
    {
        private const string NAMESPACE = "Snebur";

        private const string ARQUIVO_REFLEXAO = "reflexao.ts";
        private const string ARQUIVO_APLICACAO = "aplicacao.ts";
        private const string ARQUIVO_APLICACAO_CONFIG = "aplicacao.config.ts";
        private const string EXTENSAO_REGISTRAR_ELEMENTO = ".registrar.ts";
        private const string EXNTESAO_INICIO = ".inicio.ts";
        private const string EXTENSAO_REFERENCIAS = ".referencias.ts";
        private const string EXTENSAO_VARIAVEIS = ".variaveis.ts";
        private const string EXTENSAO_DECLARACAO = ".d.ts";
        private const string EXTENSAO_REFLEXAO = ".reflexao.ts";
        private const string EXTENSAO_CONFIG = ".config.ts";

        private static Dictionary<string, EnumTipoArquivoTypeScript> DicionarioSubExtensaoTipoArquivo = new Dictionary<string, EnumTipoArquivoTypeScript>
        {
            {EXNTESAO_INICIO,EnumTipoArquivoTypeScript.SistemaInicio },
            {EXTENSAO_REFERENCIAS,EnumTipoArquivoTypeScript.SistemaReferencias },
            {EXTENSAO_VARIAVEIS,EnumTipoArquivoTypeScript.SistemaVariaveis },
            {EXTENSAO_DECLARACAO,EnumTipoArquivoTypeScript.SistemaDeclarationType },
            {EXTENSAO_REGISTRAR_ELEMENTO,EnumTipoArquivoTypeScript.SistemaReferencias },
            {EXTENSAO_REFLEXAO,EnumTipoArquivoTypeScript.SistemaExtensaoReflexao },
            {EXTENSAO_CONFIG,EnumTipoArquivoTypeScript.SistemaLocalConfig }
            //{"." + ARQUIVO_APLICACAO,EnumTipoArquivoTypeScript.SistemaExtensaoAplicacao },
            //{"." + ARQUIVO_APLICACAO_CONFIG,EnumTipoArquivoTypeScript.SistemaExtensaoAplicacaoConfiguracao },
        };

        private static Dictionary<string, EnumTipoArquivoTypeScript> DicionarioIniciaComTipoArquivo = new Dictionary<string, EnumTipoArquivoTypeScript>
        {
            { Path.GetFileNameWithoutExtension( ARQUIVO_APLICACAO_CONFIG), EnumTipoArquivoTypeScript.SistemaExtensaoAplicacaoConfiguracao }
        };

        private static Dictionary<string, EnumTipoArquivoTypeScript> DicionarioNomeArquivoTipoArquivo = new Dictionary<string, EnumTipoArquivoTypeScript>
        {
            {ARQUIVO_REFLEXAO,EnumTipoArquivoTypeScript.SistemaReflexao },
            {ARQUIVO_APLICACAO,EnumTipoArquivoTypeScript.SistemaAplicacao },
            {ARQUIVO_APLICACAO_CONFIG,EnumTipoArquivoTypeScript.SistemaAplicacaoConfiguracao }
        };

        //public static HashSet<string> ArquivosSistema { get; } = new HashSet<string> { ARQUIVO_REFLEXAO, ARQUIVO_APLICACAO, ARQUIVO_APLICACAO_CONFIG, ARQUIVO_REGISTRAR_ELEMENTO_CONTROLE };

        public ArquivoTSSistema(FileInfo arquivo, int prioridadeProjeto) : base(arquivo, prioridadeProjeto)
        {

        }

        protected override string RetornarCaminhoClasseBase()
        {
            return null;
        }

        protected override string RetornarNamespace()
        {
            return NAMESPACE;
        }

        protected override EnumTipoArquivoTypeScript RetornarTipoArquivoTypeScript()
        {
            var nomeArquivo = this.Arquivo.Name.ToLower();
            if (ArquivoTSSistema.DicionarioNomeArquivoTipoArquivo.ContainsKey(nomeArquivo))
            {
                return ArquivoTSSistema.DicionarioNomeArquivoTipoArquivo[nomeArquivo];
            }

            if (ArquivoTSSistema.DicionarioSubExtensaoTipoArquivo.ContainsKey(this.SubExtensao))
            {
                return ArquivoTSSistema.DicionarioSubExtensaoTipoArquivo[this.SubExtensao];
            }


            foreach (var item in ArquivoTSSistema.DicionarioIniciaComTipoArquivo)
            {
                var iniciaCom = item.Key;
                if (this.Arquivo.Name.StartsWith(iniciaCom))
                {
                    var tipoArquivo = item.Value;
                    return tipoArquivo;
                }
            }
            throw new Exception("Arquivo sistema não suportado");
        }

        public static bool IsArquivoSistema(FileInfo arquivo)
        {
            var nomeArquivo = arquivo.Name.ToLower();
            if (ArquivoTSSistema.DicionarioNomeArquivoTipoArquivo.ContainsKey(nomeArquivo))
            {
                return true;
            }

            foreach (var item in ArquivoTSSistema.DicionarioIniciaComTipoArquivo)
            {
                var iniciaCom = item.Key;
                if (arquivo.Name.StartsWith(iniciaCom))
                {
                    return true;
                }
            }

            var subExtensao = Snebur.Utilidade.ArquivoUtil.RetornarSubExtenmsao(arquivo.Name).ToLower();

            if (ArquivoTSSistema.DicionarioSubExtensaoTipoArquivo.ContainsKey(subExtensao))
            {
                return true;
            }

            return false;
        }
    }
}
