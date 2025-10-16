using Snebur.Utilidade;
using System;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidacaoMoedaAttribute : BaseAtributoValidacao, IAtributoValidacao
    {
        public static string MensagemValidacaoPadrao { get; set; } = "O valor campo {0} é invalido.";

        [MensagemValidacao]
        public static string MensagemValidacaoNegativa { get; set; } = "O valor do campo {0} não pode ser negativo";

        [MensagemValidacao]
        public static string MensagemValidacaoNaoNulo { get; set; } = "O  valor do campo {0} não pode ser zero";

        [MensagemValidacao]
        public static string MensagemValidacaoValorMaximo { get; set; } = "O  valor do campo {0} não pode ser superior a {1}";

        [MensagemValidacao]
        public static string MensagemValidacaoValorMinimo { get; set; } = "O  valor do campo {0} não pode ser inferior a {1}";

        public bool AceitarNegativo { get; set; } = false;

        public bool AceitarNulo { get; set; } = false;

        public decimal ValorMaximo { get; set; } = Int32.MaxValue;

        public decimal ValorMinimo { get; set; } = Int32.MinValue;

        [IgnorarConstrutorTS]
        public ValidacaoMoedaAttribute() : this(false, false, 0, Int32.MaxValue)
        {
        }
        [IgnorarConstrutorTS]
        public ValidacaoMoedaAttribute(bool aceitarNulo, bool aceitarNegativo) :
            this(aceitarNulo, aceitarNegativo, aceitarNegativo ? 0 : Int32.MinValue, Int32.MaxValue)
        {
        }
        [IgnorarConstrutorTS]
        public ValidacaoMoedaAttribute(bool aceitarNulo, double valorMinimo, double valorMaximo) : this(aceitarNulo, valorMinimo < 0, valorMinimo, valorMaximo)
        {
        }

        public ValidacaoMoedaAttribute(bool aceitarNulo,    
                                       bool aceitarNegativo,
                                       double valorMinimo,
                                       double valorMaximo)
        {
            this.AceitarNulo = aceitarNulo;
            this.AceitarNegativo = aceitarNegativo;
            this.ValorMinimo = (decimal)valorMinimo;
            this.ValorMaximo = (decimal)valorMaximo;

            if (!this.AceitarNegativo && this.ValorMinimo < 0)
            {
                this.ValorMinimo = 0;
            }
        }

        #region IAtributoValidacao

        public override bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            if (!ValidacaoUtil.IsValidacaoRequerido(propriedade, valorPropriedade))
            {
                return true;
            }

            if(valorPropriedade == null)
            {
                return this.AceitarNulo;
            }
            var valor = Convert.ToDecimal(valorPropriedade);
            return (valor >= this.ValorMinimo && valor <= this.ValorMaximo);
        }

        public override string RetornarMensagemValidacao(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
            return String.Format(MensagemValidacaoPadrao, rotulo);
        }
        #endregion
    }
}