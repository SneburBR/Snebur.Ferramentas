using Snebur.Dominio.Atributos;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Snebur.Dominio
{

    [IgnorarInterfaceTS]
    public interface IIdentificacao : IEntidadeSeguranca, IMembrosDe, IIdentificador
    {
        [DefaultValue("NewId()")]
        new Guid Identificador { get; set; }

        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        IEnumerable<IPermissaoEntidade> PermissoesEntidade { get; }
    }
}
