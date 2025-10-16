//using System;
//using System.Reflection;
//using Snebur.Utilidade;

//namespace Snebur.Dominio.Atributos
//{
//    [AttributeUsage(AttributeTargets.Property)]
//    public class ValidacaoUnicoApresentacaoAttribute : BaseAtributoValidacaoAsync, IAtributoValidacao
//    {
//        [MensagemValidacao]
//        public static string MensagemValidacao { get; set; } = "O {0} '{1}' já existe.";

//        public string NomePropriedade { get; set; }

//        public bool IsAceitaNulo { get; set; }

//        public ValidacaoUnicoApresentacaoAttribute(string nomePropriedade, bool isAceitaNulo = false)
//        {
//            this.NomePropriedade = nomePropriedade;
//            this.IsAceitaNulo = isAceitaNulo;
//        }

//        #region IAtributoValidacao

//        public bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
//        {
//            if (!ValidacaoUtil.IsDefinido(valorPropriedade))
//            {
//                return !ValidacaoUtil.IsPropriedadeRequerida(propriedade);
//            }
//            // throw new ErroNaoImplementado("Validacao unico não implementado");
//            return true;
//        }

//        public string RetornarMensagemValidacao(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
//        {
//            var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
//            return String.Format(MensagemValidacao, rotulo);
//        }
//        #endregion
//    }
//}