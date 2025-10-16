using System;
using System.Collections.Generic;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class BaseValidacaoEntidadeAttribute : BaseAtributoDominio, IAtributoValidacaoEntidade
    {

        public abstract bool IsValido(object contextoDados,
                                      List<Entidade> todasEntidades,
                                      Entidade entidade);

        public abstract string RetornarMensagemValidacao(Entidade entidade);

    }
}
