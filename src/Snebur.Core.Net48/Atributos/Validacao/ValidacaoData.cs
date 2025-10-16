using Snebur.Utilidade;
using System;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidacaoDataAttribute : BaseAtributoValidacao, IAtributoValidacao
    {
        [MensagemValidacao]
        public static string MensagemValidacao { get; set; } = "O campo {0} é invalido.";
        public DateTime? DataMaxima { get; set; }
        public DateTime? DataMinima { get; set; }
        public EnumTipoData TipoData { get; set; }
        public bool IsPrimeiraHoraDoDia { get; set; }
        public bool IsUltimaHoraDoDia { get; set; }
        public bool IsHoraFimD { get; set; }

        [IgnorarConstrutorTS]
        public ValidacaoDataAttribute() : this(EnumTipoData.Normal, null, null)
        {
        }
        [IgnorarConstrutorTS]
        public ValidacaoDataAttribute(EnumTipoData dataTempoEnum) : this(dataTempoEnum, null, null)
        {
        }

        public ValidacaoDataAttribute(EnumTipoData tipoData,
                                      [ParametroOpcionalTS] DateTime? dataMinima,
                                      [ParametroOpcionalTS] DateTime? dataMaxima)
        {
            this.TipoData = tipoData;
            this.DataMinima = dataMinima ?? DataHoraUtil.RetornarDataMinima(tipoData);
            this.DataMaxima = dataMaxima ?? DataHoraUtil.RetornarDataMaxima(tipoData);
        }

        [IgnorarConstrutorTS]
        public ValidacaoDataAttribute(int anoInicio, int anoFIm)
        {
            this.TipoData = EnumTipoData.Normal;
            this.DataMinima = new DateTime(anoInicio, 1, 1);
            this.DataMaxima = new DateTime(anoFIm, 12, 31);
        }

        #region IAtributoValidacao

        public override bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            if (!ValidacaoUtil.IsDefinido(valorPropriedade))
            {
                return !ValidacaoUtil.IsPropriedadeRequerida(propriedade);
            }
            var dataComparar = Convert.ToDateTime(valorPropriedade);
            if( dataComparar >= this.DataMinima && dataComparar <= this.DataMaxima)
            {
                if (this.IsPrimeiraHoraDoDia)
                {
                    return dataComparar == dataComparar.DataPrimeiraHoraDia(dataComparar.Kind);
                }
                if (this.IsUltimaHoraDoDia)
                {
                    return dataComparar == dataComparar.DataUltimaHora(dataComparar.Kind);
                }
                return true;
            }
            return false;
        }

        public override string RetornarMensagemValidacao(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
            return String.Format(MensagemValidacao, rotulo);
        }
        #endregion
    }
}