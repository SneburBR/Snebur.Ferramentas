using Snebur.Utilidade;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidacaoTextoSomentoNumerosAttribute : BaseAtributoValidacao, IAtributoValidacao
    {
         public bool IsAceitarPontosSinais { get; set; } = false;
        [MensagemValidacao]
        public static string MensagemValidacao { get; set; } = "A campo {0} é invalido (somente números).";

        #region "Construtor"

        public ValidacaoTextoSomentoNumerosAttribute( 
            [ParametroOpcionalTS] bool isAceitarPontosSinais = false) : base() 
        {
            this.IsAceitarPontosSinais = isAceitarPontosSinais;
        }
        #endregion

        #region IAtributoValidacao

        public override bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            if (!ValidacaoUtil.IsDefinido(valorPropriedade))
            {
                return !ValidacaoUtil.IsPropriedadeRequerida(propriedade);
            }
            var texto = Convert.ToString(valorPropriedade);
            if (this.IsAceitarPontosSinais)
            {
                return ValidacaoUtil.IsSomenteNumerosPontosSinais(texto);
            }
            return ValidacaoUtil.IsSomenteNumeros(texto);
        }

        [Display]
        public override string RetornarMensagemValidacao(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
            return String.Format(MensagemValidacao, rotulo);
        }
        #endregion

    }
}