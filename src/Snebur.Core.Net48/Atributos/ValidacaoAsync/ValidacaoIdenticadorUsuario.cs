using Snebur.Utilidade;
using System;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidacaoIdenticadorUsuarioAttribute : BaseAtributoValidacaoAsync, IAtributoValidacao
    {
        [MensagemValidacao]
        public static string MensagemValidacaoIdentificador { get; set; } = "O {0} '{1}' não existe.";

        [MensagemValidacao]
        public static string MensagemValidacaoNovoIdentificador { get; set; } = "O {0} '{1}' já existe.";

        public bool IsNovoIdentificador { get; set; }

        //[IgnorarConstrutorTS]

        public ValidacaoIdenticadorUsuarioAttribute(bool isNovoIdentificador)
        {
            this.IsNovoIdentificador = isNovoIdentificador;
        }
        #region IAtributoValidacao

        public bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            return true;
        }

        public string RetornarMensagemValidacao(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
            var mensagem = this.IsNovoIdentificador ? MensagemValidacaoNovoIdentificador : MensagemValidacaoIdentificador;
            return String.Format(MensagemValidacaoIdentificador, rotulo);
        }
        #endregion
    }
}