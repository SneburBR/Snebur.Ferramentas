using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Snebur.Publicacao;
using Snebur.Utilidade;

namespace Snebur.VisualStudio
{

    public class ConfiguracaoProjetoTypeScript : ConfiguracaoProjeto
    {
        private readonly bool IsDebug;

        [XmlIgnore]
        public virtual BaseCompilerOptions CompilerOptions { get; set; }
        public List<string> files { get; set; } = new List<string>();
        public List<string> exclude { get; set; }
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

        protected ConfiguracaoProjetoTypeScript(ConfiguracaoProjetoTypeScript configuracaoProjeto,
                                             List<string> arquivos,
                                             string caminhoConfiguracao,
                                             //string caminhoJavasriptSaida,
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

            this.files = this.RetornarCaminhosParcial(arquivos, caminhoConfiguracao);
            this.exclude = new List<string>
            {
                "node_modules",
                "wwwroot",
                ConstantesPublicacao.NOME_PASTA_BUILD
            };
        }

        private List<string> RetornarCaminhosParcial(List<string> arquivos,
                                                     string caminhoConfiguracao)
        {

            var caminhosParcial = new List<string>();
            var isIncluirDepedencias = GerenciadorProjetos.DiretorioProjetoTypescriptInicializacao == null;
            if (isIncluirDepedencias)
            {
                caminhosParcial.AddRange(this.Depedencias.Select(x => x.Value));
            }

            foreach (var caminho in arquivos)
            {
                var caminhoRelativo = CaminhoUtil.RetornarCaminhoRelativo(caminho, caminhoConfiguracao);
                caminhosParcial.Add(caminhoRelativo);
            }
            return caminhosParcial.Distinct().ToList();
        }

        protected virtual BaseCompilerOptions RetornarCompilarOptions()
        {
            return null;
        }

        protected override List<string> RetornarNomesProjetoDepedencia()
        {
            return this.Depedencias.Select(x => x.Key).ToList();
        }

        //public string RetornarCaminhoSaida(string caminhoProjeto, string nomeProjeto)
        //{
        //    if (Directory.Exists(GerenciadorProjetos.DiretorioProjetoTypescriptInicializacao))
        //    {
        //        throw new NotImplementedException();
        //    }

        //    //if(this.CompilerOptions is CompilerOptionsFramework compilerOptionsFramework)
        //    //{
        //    //    return Path.GetFullPath(Path.Combine(caminhoProjeto, compilerOptionsFramework.outFile));
        //    //}
        //    return Path.Combine(caminhoProjeto, ConstantesPublicacao.NOME_PASTA_BUILD, $"{nomeProjeto}.js");
        //}
    }

    public class ConfiguracaoProjetoTypeScriptFramework : ConfiguracaoProjetoTypeScript
    {
        public CompilerOptionsFramework compilerOptions { get; set; }

        [XmlIgnore]
        public override BaseCompilerOptions CompilerOptions { get => this.compilerOptions; set => this.compilerOptions = (CompilerOptionsFramework)value; }

        public ConfiguracaoProjetoTypeScriptFramework()
        {

        }

        public ConfiguracaoProjetoTypeScriptFramework(ConfiguracaoProjetoTypeScript configuracao,
                                            List<string> arquivos,
                                            string caminhoConfiguracao,
                                            string caminhoJavasriptSaida,
                                            bool isProjetoDebug) : base(configuracao, arquivos, caminhoConfiguracao, isProjetoDebug)
        {
            var options = new CompilerOptionsFramework(caminhoJavasriptSaida);
            //AutoMapearUtil.Mapear(configuracao.CompilerOptions, options, true);
            options.outFile = caminhoJavasriptSaida;
            this.CompilerOptions = options;
            this.CompilerOptions.target = configuracao.CompilerOptions.target;
            this.CompilerOptions.lib = configuracao.CompilerOptions.lib;

            this.CompilerOptions.declaration = configuracao.CompilerOptions.declaration;
            this.CompilerOptions.declarationMap = configuracao.CompilerOptions.declarationMap;
            this.CompilerOptions.removeComments = configuracao.CompilerOptions.removeComments;
            this.CompilerOptions.strict = configuracao.CompilerOptions.strict;
            this.CompilerOptions.strictFunctionTypes = configuracao.CompilerOptions.strictFunctionTypes;
            this.CompilerOptions.noImplicitOverride = configuracao.CompilerOptions.noImplicitOverride;
            this.CompilerOptions.strictNullChecks = configuracao.CompilerOptions.strictNullChecks;
            this.CompilerOptions.alwaysStrict = configuracao.CompilerOptions.alwaysStrict;
            this.CompilerOptions.noImplicitThis = configuracao.CompilerOptions.noImplicitThis;
            this.CompilerOptions.strictBindCallApply = configuracao.CompilerOptions.strictBindCallApply;
            this.CompilerOptions.strictPropertyInitialization = configuracao.CompilerOptions.strictPropertyInitialization;
            this.CompilerOptions.stripInternal = configuracao.CompilerOptions.stripInternal;
            this.CompilerOptions.noStrictGenericChecks = configuracao.CompilerOptions.noStrictGenericChecks;
            this.CompilerOptions.removeComments = configuracao.CompilerOptions.removeComments;
            this.CompilerOptions.strictBindCallApply = configuracao.CompilerOptions.strictBindCallApply;
            //this.CompilerOptions.strictFunctionTypes = configuracao.CompilerOptions.strictFunctionTypes;
        }
    }

    //public class ConfiguracaoProjetoTypeScriptApresentacao : ConfiguracaoProjetoTypeScript
    //{
    //    public CompilerOptionsApresentacao compilerOptions { get; set; }

    //    [XmlIgnore]
    //    public override BaseCompilerOptions CompilerOptions { get => this.compilerOptions; set => this.compilerOptions = (CompilerOptionsApresentacao)value; }

    //    public ConfiguracaoProjetoTypeScriptApresentacao()
    //    {

    //    }

    //    public ConfiguracaoProjetoTypeScriptApresentacao(ConfiguracaoProjetoTypeScript configuracao,
    //                                      List<string> arquivos,
    //                                      string caminhoConfiguracao,
    //                                      string caminhoJavasriptSaida,
    //                                      bool isProjetoDebug) : base(configuracao, arquivos, caminhoConfiguracao, isProjetoDebug)
    //    {
    //        this.CompilerOptions = new CompilerOptionsApresentacao(caminhoJavasriptSaida);
    //        this.CompilerOptions.declaration = configuracao.CompilerOptions.declaration;
    //        this.CompilerOptions.declarationMap = configuracao.CompilerOptions.declarationMap;
    //        this.CompilerOptions.strictFunctionTypes = configuracao.CompilerOptions.strictFunctionTypes;
    //        this.CompilerOptions.removeComments = configuracao.CompilerOptions.removeComments;
    //    }
    //}

    //public class ConfiguracaoProjetoTypeScriptRuntime : ConfiguracaoProjetoTypeScript
    //{
    //    public CompilerOptionsRuntime compilerOptions { get; set; }

    //    [XmlIgnore]
    //    public override BaseCompilerOptions CompilerOptions { get => this.compilerOptions; set => this.compilerOptions = (CompilerOptionsRuntime)value; }

    //    public ConfiguracaoProjetoTypeScriptRuntime()
    //    {
    //    }

    //    public ConfiguracaoProjetoTypeScriptRuntime(ConfiguracaoProjetoTypeScript configuracao,
    //                                      List<string> arquivos,
    //                                      string caminhoConfiguracao,
    //                                      string caminhoSaida,
    //                                      bool isProjetoDebug) : base(configuracao, arquivos, caminhoConfiguracao, isProjetoDebug)
    //    {
    //        this.CompilerOptions = new CompilerOptionsRuntime(caminhoSaida);
    //        this.CompilerOptions.declaration = false;
    //        this.CompilerOptions.strictFunctionTypes = configuracao.CompilerOptions.strictFunctionTypes;
    //    }
    //}
}