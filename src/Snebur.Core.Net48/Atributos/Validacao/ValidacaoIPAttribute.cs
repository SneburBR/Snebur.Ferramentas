using Snebur.Utilidade;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{

    [AttributeUsage(AttributeTargets.Property)]
    public class ValidacaoIPAttribute : BaseAtributoValidacao, IAtributoValidacao
    {
        const string IP_REG_EX = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";

        [MensagemValidacao]
        public static string MensagemValidacao { get; set; } = "O ip {0} é invalido.";

    
        #region " Construtor "

        public ValidacaoIPAttribute() : base()
        {
        }
        #endregion

        #region IAtributoValidacao

        public override bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            if (!ValidacaoUtil.IsDefinido(valorPropriedade))
            {
                return !ValidacaoUtil.IsPropriedadeRequerida(propriedade);
            }
            var email = Convert.ToString(valorPropriedade);
            return ValidacaoUtil.IsIp(email);
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