
using System.Collections.Generic;
using System.Linq;
using Snebur.Linq;

namespace Snebur.VisualStudio
{
    public abstract class BaseCompilerOptions
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

        protected BaseCompilerOptions()
        {
            var compilerOptionsInicializacao = GerenciadorProjetosUtil.ConfiguracaoProjetoTypesriptInicializacao?.CompilerOptions;
            if (compilerOptionsInicializacao != null)
            {
                this.lib.AddRangeNew(compilerOptionsInicializacao.lib);
                this.target = compilerOptionsInicializacao.target;
            }
        }
    }

    public class CompilerOptionsFramework : BaseCompilerOptions
    {
        public string outFile { get; set; }

        public CompilerOptionsFramework(string caminhoSaida) : base()
        {
            this.outFile = caminhoSaida;
        }
    }

    //public class CompilerOptionsApresentacao : BaseCompilerOptions
    //{
    //    public string outDir { get; set; }

    //    public CompilerOptionsApresentacao()
    //    {

    //    }

    //    public CompilerOptionsApresentacao(string caminhoSaida) : base()
    //    {
    //        this.outDir = caminhoSaida;
    //    }
    //}

    //public class CompilerOptionsRuntime : BaseCompilerOptions
    //{
    //    public string outDir { get; set; }

    //    public CompilerOptionsRuntime():base()
    //    {

    //    }

    //    public CompilerOptionsRuntime(string caminhoSaida)
    //    {
    //        this.outDir = caminhoSaida;
    //        this.declaration = false;
    //    }
    //}
}