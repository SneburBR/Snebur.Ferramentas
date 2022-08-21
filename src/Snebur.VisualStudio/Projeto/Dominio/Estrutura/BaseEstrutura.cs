using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.VisualStudio.Utilidade;
using Snebur.VisualStudio.Reflexao;
using Snebur.Utilidade;
using Snebur.Reflexao;

namespace Snebur.VisualStudio.Projeto.Dominio.Estrutura
{
    public class BaseEstrutura
    {
        public readonly string TAB = "    ";
        public Type Tipo { get; set; }

        public string NomeTipo { get; set; }

        public string CaminhoTipo { get; set; }

        public string Namespace { get; set; }

        public bool Nullable { get; set; }

        public BaseEstrutura(Type tipo)
        {
            this.Tipo = tipo;
            this.NomeTipo = TipoUtil.RetornarNomeTipo(this.Tipo);
            this.Namespace = TipoUtil.RetornarNameSpace(this.Tipo);
            this.CaminhoTipo = TipoUtil.RetornarCaminhoTipo(this.Tipo);
            this.Nullable = ReflexaoUtil.IsTipoNullable(this.Tipo);
        }

        protected string RetornarValorTypeScript(object valor)
        {
            if (valor == null)
            {
                return "null";
            }
            var tipo = valor.GetType();
            if (ReflexaoUtil.TipoRetornaColecao(tipo))
            {

                throw new NotImplementedException();
                // return String.Format(" new {0}();", this.CaminhoTipo);

            }

            tipo = ReflexaoUtil.RetornarTipoSemNullable(tipo);

            if (ReflexaoUtil.TipoRetornaTipoPrimario(tipo))
            {
                var tipoPrimarioEnum = ReflexaoUtil.RetornarTipoPrimarioEnum(tipo);
                return this.RetornarValorTipoPrimario(tipoPrimarioEnum, valor);
            }

            throw new NotSupportedException("RetornarValorTypeScript");
        }

        protected string RetornarValorPadraoTipoPrimario(EnumTipoPrimario tipoPrimarioEnum)
        {

            switch (tipoPrimarioEnum)
            {

                case EnumTipoPrimario.Boolean:
                    return "false";
                case EnumTipoPrimario.DateTime:
                    return " new Date()";
                case EnumTipoPrimario.Decimal:
                case EnumTipoPrimario.Double:
                case EnumTipoPrimario.Integer:
                case EnumTipoPrimario.Long:
                case EnumTipoPrimario.EnumValor:
                    return "0";
                case EnumTipoPrimario.String:
                    return "null";
                case EnumTipoPrimario.Guid:

                    return String.Format("\"{0}\"", Guid.Empty.ToString());

                case EnumTipoPrimario.TimeSpan:

                    return " new TimeSpan(0,0,0,0,0);";

                default:
                    throw new NotSupportedException(String.Format("Tipo primário não suportado {0} ", tipoPrimarioEnum.ToString()));
            }


        }

        protected string RetornarValorTipoPrimario(EnumTipoPrimario tipoPrimarioEnum, object valor)
        {

            switch (tipoPrimarioEnum)
            {

                case EnumTipoPrimario.Boolean:
                    return valor.ToString().ToLower();
                case EnumTipoPrimario.DateTime:

                    throw new NotImplementedException("RetornarValorTipoPrimario.DateTime");
                //return " new Date();";
                case EnumTipoPrimario.Integer:
                case EnumTipoPrimario.Long:

                    return Convert.ToInt64(valor).ToString();

                case EnumTipoPrimario.Decimal:
                case EnumTipoPrimario.Double:

                    return Convert.ToDecimal(valor).ToString().Replace(",", ".");

                case EnumTipoPrimario.String:
                case EnumTipoPrimario.Guid:

                    return String.Format("\"{0}\"", valor.ToString());

                case EnumTipoPrimario.TimeSpan:

                    throw new NotImplementedException("RetornarValorTipoPrimario.TimeSpan");

                default:
                    throw new NotSupportedException(String.Format("Tipo primário não suportado {0} ", tipoPrimarioEnum.ToString()));
            }

        }

    }
}
