using Snebur.Utilidade;
using System;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidacaoCredencialAttribute : BaseAtributoValidacaoAsync, IAtributoValidacao
    {
        [MensagemValidacao]
        public static string MensagemValidacaoSenhaIncorreta { get; set; } = "Senha incorreta. ";

        [MensagemValidacao]
        public static string MensagemValidacaoUsuarioNaoExiste { get; set; } = "O {0} '{1}' não existe.";

        //public string CaminhoTipoEntidadeUsuario { get; set; }
        public string NomePropriedadeIdentificador { get; set; }

        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public Type TipoDominio { get; set; }

        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public PropertyInfo Propriedade { get; set; }

        [IgnorarConstrutorTS]
        public ValidacaoCredencialAttribute(Type tipoDominio, string nomePropriedadeIdentificador)
        {
            this.TipoDominio = tipoDominio;
            this.NomePropriedadeIdentificador = nomePropriedadeIdentificador;
            //this.CaminhoTipoEntidadeUsuario = $"{this.TipoEntidadeUsuario.Namespace}.{this.TipoEntidadeUsuario.Name}";
            this.Propriedade = tipoDominio.GetProperty(nomePropriedadeIdentificador);
            if (this.Propriedade == null)
            {
                throw new Exception($"A propriedade {nomePropriedadeIdentificador}  não foi encontrada em {nomePropriedadeIdentificador}");
            }
        }

        public ValidacaoCredencialAttribute(string nomePropriedadeIdentificador)
        {
            throw new Exception("Não usar esse construtor, está aqui para auxiliar o dminio Typescript");
        }
        #region IAtributoValidacao

        public bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            return true;
            //if (!ValidacaoUtil.ValidarValidacaoRequerido(propriedade, valorPropriedade))
            //{
            //    return true;
            //}
            //// throw new ErroNaoImplementado("Validacao unico não implementado");
            //return true;
        }

        public string RetornarMensagemValidacao(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
            return String.Format(MensagemValidacaoSenhaIncorreta, rotulo);
        }
        #endregion
    }
}