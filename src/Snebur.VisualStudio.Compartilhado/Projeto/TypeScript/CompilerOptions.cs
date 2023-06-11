using Newtonsoft.Json;
using Snebur.Linq;
using Snebur.Utilidade;
using System.Collections.Generic;
using System.IO;

namespace Snebur.VisualStudio
{
    public class CompilerOptions
    {
        public bool allowJs { get; set; } = false;
        public bool noImplicitAny { get; set; } = true;
        public bool noEmitOnError { get; set; } = true;
        public bool noImplicitReturns { get; set; } = true;
        public bool preserveConstEnums { get; set; } = true;
        public bool removeComments { get; set; } = true;
        public bool sourceMap { get; set; } = true;
        public bool noImplicitThis { get; set; } = false;
        public bool strictFunctionTypes { get; set; } = false;
        public bool noImplicitOverride { get; set; } = false;
        public bool strictBindCallApply { get; set; } = false;
        public bool strict { get; set; } = false;
        public bool strictPropertyInitialization { get; set; } = false;
        public bool alwaysStrict { get; set; } = false;
        public bool noStrictGenericChecks { get; set; } = false;
        public bool stripInternal { get; set; } = false;
        public bool strictNullChecks { get; set; } = false;
        public string target { get; set; } = "ES5";
        public List<string> lib { get; set; } = new List<string> { "DOM", "ES2015.Promise", "ES5" };
        public bool declaration { get; set; } = false;
        public bool declarationMap { get; set; } = false;

        private string _outFile;

        [JsonProperty("outFile")]
        public string outFile
        {
            get => this._outFile;
            private set
            {
                if (value != null && CaminhoUtil.IsFullPath(value))
                {
                    LogVSUtil.LogErro($"outFile: caminho absoluto não é suportado {value}");
                    value = this.NormalizarCaminho(value);
                }
                this._outFile = value;
            }
        }

      
        public CompilerOptions()
        {
            //var compilerOptionsInicializacao = GerenciadorProjetosUtil.ConfiguracaoProjetoTypesriptInicializacao?.CompilerOptions;
            //if (compilerOptionsInicializacao != null)
            //{
            //    this.lib.AddRangeNew(compilerOptionsInicializacao.lib);
            //    this.target = compilerOptionsInicializacao.target;
            //}
        }

        public CompilerOptions(string caminhoSaida) : this()
        {
            this.outFile = caminhoSaida;
        }
         
        private string NormalizarCaminho(string value)
        {
            var index = value.ToLower().LastIndexOf("\\build\\");
            if (index > 0)
            {
                return value.Substring(index + 1);
            }
            return Path.GetFileName(value);
        }
    }

}