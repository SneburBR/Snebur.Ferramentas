using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Snebur.Publicacao;
using Snebur.Utilidade;

namespace Snebur.VisualStudio
{

    public class ConfiguracaoProjetoTypeScript : ConfiguracaoProjeto
    {
        private readonly bool IsDebug;

        [JsonProperty("compilerOptions")]
        public CompilerOptions CompilerOptions { get; private set; }

        [JsonProperty("files")]
        public List<string> Files { get; set; } = new List<string>();

        [JsonProperty("exclude")]
        public List<string> Exclude { get; set; }
        public List<string> ProjetosDepedentes { get; set; } = new List<string>();
        public int PrioridadeProjeto { get; set; } = 0;
        public string UrlDesenvolvimento { get; set; } = "";

        //public bool IsProjetoDebug { get; set; } = false;

        public bool IsIgnorar { get; set; } = false;

        //public bool IsEditarApresentacao { get; set; } = false;
        public Dictionary<string, string> Depedencias { get; set; } = new Dictionary<string, string>();
        public bool IsProjetoApresentacao { get; set; }
        public bool IsProjetoPublicacao { get; set; }
        public bool IsIgnorarNormnalizacaoCompilacao { get; set; }
        public List<string> NomesPastaServidor { get; set; } = new List<string>();
        public List<string> Extras { get; set; } = new List<string>();
        public bool IsDebugScriptsDepedentes { get; set; }

        public ConfiguracaoProjetoTypeScript()
        {

        }


        public ConfiguracaoProjetoTypeScript(ConfiguracaoProjetoTypeScript configuracaoProjeto,
                                            List<string> arquivos,
                                            string caminhoConfiguracao,
                                            string caminhoJavasriptSaida,
                                            bool isProjetoDebug)
        {

            this.IsDebug = isProjetoDebug;
            //this.IsEditarApresentacao = configuracaoProjeto.IsEditarApresentacao;
            this.UrlDesenvolvimento = configuracaoProjeto.UrlDesenvolvimento;
            this.PrioridadeProjeto = configuracaoProjeto.PrioridadeProjeto;
            this.ProjetosDepedentes = configuracaoProjeto.ProjetosDepedentes;
            this.Depedencias = configuracaoProjeto.Depedencias;

            this.IsProjetoApresentacao = configuracaoProjeto.IsProjetoApresentacao;
            this.IsIgnorarNormnalizacaoCompilacao = configuracaoProjeto.IsIgnorarNormnalizacaoCompilacao;
            this.NomesPastaServidor = configuracaoProjeto.NomesPastaServidor;
            this.IsDebugScriptsDepedentes = configuracaoProjeto.IsDebugScriptsDepedentes;
            //this.NomeArquivoApresentacao = configuracaoProjeto.NomeArquivoApresentacao;

            this.Files = this.RetornarCaminhosParcial(arquivos, caminhoConfiguracao);
            this.Exclude = new List<string>
            {
                "node_modules",
                "wwwroot",
                ConstantesPublicacao.NOME_PASTA_BUILD
            };


            this.CompilerOptions = new CompilerOptions(caminhoJavasriptSaida); ;
            this.CompilerOptions.target = configuracaoProjeto.CompilerOptions.target;
            this.CompilerOptions.lib = configuracaoProjeto.CompilerOptions.lib;

            this.CompilerOptions.declaration = configuracaoProjeto.CompilerOptions.declaration;
            this.CompilerOptions.declarationMap = configuracaoProjeto.CompilerOptions.declarationMap;
            this.CompilerOptions.removeComments = configuracaoProjeto.CompilerOptions.removeComments;
            this.CompilerOptions.strict = configuracaoProjeto.CompilerOptions.strict;
            this.CompilerOptions.strictFunctionTypes = configuracaoProjeto.CompilerOptions.strictFunctionTypes;
            this.CompilerOptions.noImplicitOverride = configuracaoProjeto.CompilerOptions.noImplicitOverride;
            this.CompilerOptions.strictNullChecks = configuracaoProjeto.CompilerOptions.strictNullChecks;
            this.CompilerOptions.alwaysStrict = configuracaoProjeto.CompilerOptions.alwaysStrict;
            this.CompilerOptions.noImplicitThis = configuracaoProjeto.CompilerOptions.noImplicitThis;
            this.CompilerOptions.strictBindCallApply = configuracaoProjeto.CompilerOptions.strictBindCallApply;
            this.CompilerOptions.strictPropertyInitialization = configuracaoProjeto.CompilerOptions.strictPropertyInitialization;
            this.CompilerOptions.stripInternal = configuracaoProjeto.CompilerOptions.stripInternal;
            this.CompilerOptions.noStrictGenericChecks = configuracaoProjeto.CompilerOptions.noStrictGenericChecks;
            this.CompilerOptions.removeComments = configuracaoProjeto.CompilerOptions.removeComments;
            this.CompilerOptions.strictBindCallApply = configuracaoProjeto.CompilerOptions.strictBindCallApply;
            //this.CompilerOptions.strictFunctionTypes = configuracao.CompilerOptions.strictFunctionTypes;
        }

        private List<string> RetornarCaminhosParcial(List<string> arquivos,
                                                     string caminhoConfiguracao)
        {

            var caminhosParcial = new List<string>();
            caminhosParcial.AddRange(this.Depedencias.Select(x => x.Value));

            foreach (var caminho in arquivos)
            {
                var caminhoRelativo = CaminhoUtil.RetornarCaminhoRelativo(caminho, caminhoConfiguracao);
                caminhosParcial.Add(caminhoRelativo);
            }
            return caminhosParcial.Distinct().ToList();
        }



        protected override List<string> RetornarNomesProjetoDepedencia()
        {
            return this.Depedencias.Select(x => x.Key).ToList();
        }



    }

}