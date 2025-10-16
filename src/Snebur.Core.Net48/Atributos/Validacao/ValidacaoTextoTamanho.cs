using Snebur.Utilidade;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidacaoTextoTamanhoAttribute : StringLengthAttribute, IAtributoValidacao
    {
        //private const int ACRECIMO_DELETADO = 36;
        private const int ACRECIMO_DELETADO = 0;

        [MensagemValidacao]
        public static string MensagemValidacaoMaximo { get; set; } = "O campo '{0}' deve ter no máximo {1} caracteres.";

        [MensagemValidacao]
        public static string MensagemValidacaoMinimo { get; set; } = "O campo '{0}' deve ter no mínimo {1} caracteres.";

        [MensagemValidacao]
        public static string MensagemValidacaoIntervalo { get; set; } = "O campo '{0}' deve ter entre {1} e {2} caracteres.";

        public int TamanhoMinimo { get { return this.MinimumLength; } set { this.MinimumLength = value; } }

        public int TamanhoMaximo { get; set; }

        #region Construtores

        //[IgnorarConstrutorTS]
        //public ValidacaoTextoTamanhoAttribute(int tamanhoMaximo) : base(tamanhoMaximo + ACRECIMO_DELETADO)
        //{
        //    this.TamanhoMaximo = tamanhoMaximo;
        //}

        [IgnorarConstrutorTS]
        public ValidacaoTextoTamanhoAttribute(int tamanhoMaximo) : base(tamanhoMaximo + ACRECIMO_DELETADO)
        {
            this.TamanhoMaximo = tamanhoMaximo;
        }

        public ValidacaoTextoTamanhoAttribute(int tamanhoMinimo, int tamanhoMaximo) : this(tamanhoMaximo)
        {
            this.TamanhoMinimo = tamanhoMinimo;
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

                this.ErrorMessage = this.RetornarMensagemValidacao(propriedade, paiPropriedade, valorPropriedade);
                resultado.ErrorMessage = this.ErrorMessage;
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
            var texto = Convert.ToString(valorPropriedade);
            if (this.TamanhoMinimo > 0 && this.TamanhoMaximo > 0)
            {
                return texto.Length >= this.TamanhoMinimo &&
                       texto.Length <= this.TamanhoMaximo;
            }
            if (this.TamanhoMinimo > 0)
            {
                return texto.Length >= this.TamanhoMinimo;
            }
            if (this.TamanhoMaximo > 0)
            {
                return texto.Length <= this.TamanhoMaximo;
            }
            return false;
        }

        public string RetornarMensagemValidacao(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
            if (this.TamanhoMinimo > 0 && this.TamanhoMaximo > 0)
            {
                return String.Format(MensagemValidacaoIntervalo, rotulo, this.TamanhoMinimo, this.TamanhoMaximo);
            }
            if (this.TamanhoMinimo > 0)
            {
                return String.Format(MensagemValidacaoMinimo, rotulo, this.TamanhoMinimo);
            }
            if (this.TamanhoMaximo > 0)
            {
                return String.Format(MensagemValidacaoMaximo, rotulo, this.TamanhoMaximo);
            }
            throw new ErroOperacaoInvalida("O tamanho minimo e o tamanho máximo não foram definido");
        }
        #endregion
    }
}