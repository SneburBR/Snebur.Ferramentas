using Snebur.Utilidade;
using System;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidacaoSenhaAttribute : BaseAtributoValidacao, IAtributoValidacao
    {
        public int TamanhoMinimo { get; set; } = 4;
        public int TamanhoMaximo { get; set; } = 36;

        private string MensagemValidacao = "O campo {0} é invalido.";

        [MensagemValidacao]
        public static string MensagemValidacaoMaximo { get; set; } = "O campo '{0}' deve ter no máximo {1} caracteres.";

        [MensagemValidacao]
        public static string MensagemValidacaoMinimo { get; set; } = "O campo '{0}' deve ter no mínimo {1} caracteres.";

        [MensagemValidacao]
        public static string MensagemValidacaoIntervalo { get; set; } = "O campo '{0}' deve ter entre {1} e {2} caracteres.";

        //public bool HashMd5 { get; set; } = true;

        [IgnorarConstrutorTS]
        public ValidacaoSenhaAttribute()
        {
        }

        public ValidacaoSenhaAttribute(int tamanhoMinimo, int tamanhoMaximo)
        {
            this.TamanhoMinimo = tamanhoMinimo;
            this.TamanhoMaximo = tamanhoMaximo;
        }

        #region IAtributoValidacao

        public override bool IsValido(PropertyInfo propriedade, 
                                     object paiPropriedade, object valorPropriedade)
        {
            var senha = Convert.ToString(valorPropriedade);
            if (!(senha.Length >= this.TamanhoMinimo))
            {
                return false;
            }
            return true;
            //throw new NotImplementedException();
        }

        public override string RetornarMensagemValidacao(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
            return String.Format(this.MensagemValidacao, rotulo);
        }
        #endregion
    }
}