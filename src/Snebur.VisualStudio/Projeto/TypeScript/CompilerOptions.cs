using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;

namespace Snebur.VisualStudio.Projeto.TypeScript
{
    public class compilerOptions
    {
        public bool allowJs { get; set; } = false;
        public bool noImplicitAny { get; set; } = true;
        public bool noEmitOnError { get; set; } = false;
        public bool preserveConstEnums { get; set; } = true;
        public bool removeComments { get; set; } = true;
        public bool sourceMap { get; set; } = true;
        public string outFile { get; set; }
        public string target { get; set; } = "es5";
        public string[] lib { get; set; } = { "dom", "es2015.promise", "es5" };
        public compilerOptions(string caminhoJavasriptSaida)
        {
            this.outFile = caminhoJavasriptSaida;
        }
    }
}



//public bool allowJs { get; set; } = true;

//public bool allowSyntheticDefaultImports { get; set; } = false;

//public bool allowUnreachableCode { get; set; } = false;

//public bool allowUnusedLabels { get; set; } = false;

//public string charset { get; set; } = "utf8";

//public bool declaration { get; set; } = false;

//public bool diagnostics { get; set; } = false;

//public bool emitBOM { get; set; } = false;

//public bool emitDecoratorMetadata { get; set; } = false;

//public bool experimentalDecorators { get; set; } = false;

//public bool forceConsistentCasingInFileNames { get; set; } = false;

//public bool inlineSourceMap { get; set; } = false;

//public bool inlineSources { get; set; } = false;

//public bool init { get; set; } = false;

//public bool isolatedModules { get; set; } = false;

//public string jsx { get; set; } = "Preserve";

//public bool listFiles { get; set; } = false;

//public string locale { get; set; } = "";


//public string mapRoot { get; set; } = null;

//public string module { get; set; }

//public string moduleResolution { get; set; } = "Classic";

//public string newLine { get; set; }

//public bool noEmit { get; set; } = false;

//public bool noEmitHelpers { get; set; } = false;

//public bool noEmitOnError { get; set; } = false;

//public bool noFallthroughCasesInSwitch { get; set; } = false;

//public bool noImplicitAny { get; set; } = false;

//public bool noImplicitReturns { get; set; } = false;

//public bool noImplicitUseStrict { get; set; } = false;

//public bool noLib { get; set; } = false;

//public bool noResolve { get; set; } = false;

//public string Out { get; set; }

//public string outDir { get; set; }

//public string outFile { get; set; }

//public bool preserveConstEnums { get; set; } = false;

//public bool pretty { get; set; } = false;


//public string project { get; set; } = null;

//public string reactNamespace { get; set; } = "React";

//public bool removeComments { get; set; } = false;

//public string rootDir { get; set; }

//public bool skipDefaultLibCheck { get; set; } = false;

//public bool sourceMap { get; set; } = false;

//public string sourceRoot { get; set; }

//public bool strictNullChecks { get; set; } = false;

//public bool stripInternal { get; set; } = false;

//public bool suppressExcessPropertyErrors { get; set; } = false;

//public bool suppressImplicitAnyIndexErrors { get; set; } = false;

//public string target { get; set; } = "ES5";

//public bool traceResolution { get; set; } = false;