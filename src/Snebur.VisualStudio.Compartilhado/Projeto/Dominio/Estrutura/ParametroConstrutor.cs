using System;
using System.Linq;
using System.Reflection;

namespace Snebur.VisualStudio
{
    public class EstruturaParametroConstrutor : BaseEstrutura
    {
        public ParameterInfo Parametro { get; }
        public string NomeParametro { get; set; }
        public bool IsOpcional { get; }
        public bool IsInicializarPropriedade { get; } = true;

        public EstruturaParametroConstrutor(ParameterInfo parametro) : base(parametro.ParameterType)
        {
            this.Parametro = parametro;
            this.NomeParametro = parametro.Name;

            if (parametro.GetCustomAttributes().Any(x => x.GetType().Name == AjudanteAssembly.NomeTipoParametroOpcionalTS))
            {
                this.IsOpcional = true;
            }
        }

        public EstruturaParametroConstrutor(string nomeParametro, string caminhoTipo, bool isOpcional, bool isPropriedadeInicializadora) : base(null)
        {
            this.CaminhoTipo = caminhoTipo;
            this.NomeParametro = nomeParametro;
            this.IsOpcional = isOpcional;
            this.IsInicializarPropriedade = isPropriedadeInicializadora;
        }
        public EstruturaParametroConstrutor(string nomeParametro, Type tipoParametro, bool isOpcional, bool isPropriedadeInicializadora) : base(tipoParametro)
        {
            this.NomeParametro = nomeParametro;
            this.IsOpcional = isOpcional;
            this.IsInicializarPropriedade = isPropriedadeInicializadora;
        }

        public string RetornarTypeScript()
        {
            var opcional = this.IsOpcional ? "?" : String.Empty;
            var caminhoTipo = this.Parametro != null
                                    ? this.RetornarCaminhoTipoParametro(this.Parametro)
                                    : this.CaminhoTipo;
            return String.Format(" {0}{1} : {2} ", this.NomeParametro, opcional, caminhoTipo);
        }
    }
}
