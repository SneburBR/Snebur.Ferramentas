using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidacaoIntervaloAttribute : RangeAttribute, IAtributoValidacao
    {
        [MensagemValidacao]
        public static string MensagemValidacao { get; set; } = "O campo {0} deve estar entre {1} e {2}.";

        public double Minimo { get; set; }
        public double Maximo { get; set; }

        #region Construtores

        [IgnorarConstrutorTS]
        public ValidacaoIntervaloAttribute(double maximo) : this(0, maximo)
        {
        }

        public ValidacaoIntervaloAttribute(double minimo, double maximo) : base(minimo, maximo)
        {
            this.Minimo = minimo;
            this.Maximo = maximo;
        }
        #endregion

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
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
                    return new ValidationResult(this.ErrorMessage, new List<string>() { validationContext.MemberName });
                }
            }
            return resultado;
        }
        #region IAtributoValidacao

        public bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            if (!ValidacaoUtil.IsDefinido(valorPropriedade))
            {
                return !ValidacaoUtil.IsPropriedadeRequerida(propriedade);
            }
            var valorTipado = Convert.ToDouble(valorPropriedade);
            if (this.Minimo > 0 && this.Maximo > 0)
            {
                if (ReflexaoUtil.IsTipoNumerico(propriedade.PropertyType))
                {
                    return (valorTipado >= this.Minimo && valorTipado <= this.Maximo);
                }
            }
            if (this.Minimo > 0)
            {
                return (valorTipado >= this.Minimo);
            }
            if (this.Maximo > 0)
            {
                return (valorTipado <= this.Maximo);
            }
            return false;
        }

        public string RetornarMensagemValidacao(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
            return String.Format(MensagemValidacao, rotulo, this.Minimum.ToString(), this.Maximum.ToString());
        }
        #endregion
    }
}