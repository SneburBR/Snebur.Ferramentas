using Snebur.Utilidade;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{

    [AttributeUsage(AttributeTargets.Property)]
    public class ValidacaoRequeridoDebugAttribute : RequiredAttribute, IAtributoValidacao
    {
        [MensagemValidacao]
        public static string MensagemValidacao { get; set; } = "O campo {0} deve ser preenchido.";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!DebugUtil.IsAttached)
            {
                return ValidationResult.Success;
            }

            var resultado = base.IsValid(value, validationContext);
            if (resultado != null)
            {
                var propriedade = validationContext.ObjectType.GetProperty(validationContext.MemberName);
                var paiPropriedade = validationContext.ObjectInstance;
                var valorPropriedade = value;
                if (this.IsValido(propriedade, paiPropriedade, valorPropriedade))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    this.ErrorMessage = this.RetornarMensagemValidacao(propriedade, paiPropriedade, valorPropriedade);
                    resultado.ErrorMessage = this.ErrorMessage;
                }
            }
            return resultado;
        }

        #region IAtributoValidacao

        public bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            return ValidacaoUtil.IsValidacaoRequerido(propriedade, valorPropriedade, paiPropriedade);
        }

        public string RetornarMensagemValidacao(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
            return String.Format(MensagemValidacao, rotulo);
        }
        #endregion
    }
}