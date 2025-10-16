using System.Collections.Generic;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    [IgnorarInterfaceTS]
    public interface IAtributoValidacao
    {
        bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade);

        string RetornarMensagemValidacao(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade);
    }

    [IgnorarInterfaceTS]
    public interface IAtributoValidacaoEntidade
    {
        bool IsValido(object servico, List<Entidade> todasEntidades, Entidade entidade);

        string RetornarMensagemValidacao(Entidade entidade);
    }
}