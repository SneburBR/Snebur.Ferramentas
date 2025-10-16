using Snebur.Utilidade;
using System;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    //http://ddd.online24hs.com.br

    [AttributeUsage(AttributeTargets.Property)]
    public class ValidacaoTelefoneAttribute : BaseAtributoValidacao, IAtributoValidacao
    {
        [MensagemValidacao]
        public static string MensagemValidacao { get; set; } = "O campo {0} é invalido.";

        #region IAtributoValidacao

        public override bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            if (!ValidacaoUtil.IsDefinido(valorPropriedade))
            {
                return !ValidacaoUtil.IsPropriedadeRequerida(propriedade);
            }
            var telefone = Convert.ToString(valorPropriedade);
            return ValidacaoUtil.IsTelefone(telefone);
        }

        public override string RetornarMensagemValidacao(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
            return String.Format(MensagemValidacao, rotulo);
        }

        #endregion
    }
}