using Snebur.Dominio.Atributos;
using System;

namespace Snebur.UI
{
    [IgnorarInterfaceTS]
    public interface IAlerta
    {
        EnumResultadoAlerta Mostrar(string conteudo, string titulo, EnumTipoAlerta tipoAlerta, EnumBotoesAlerta tipoBotoes);

        EnumResultadoAlerta Mostrar(string conteudo, string titulo, EnumTipoAlerta tipoAlerta, EnumBotoesAlerta tipoBotoes, string[] textoBotoes);

        IJanelaAlerta Mostrar(string conteudo, string titulo, EnumTipoAlerta tipoAlerta, EnumBotoesAlerta tipoBotoes, Action<EnumResultadoAlerta> callback);

        IJanelaAlerta Mostrar(string conteudo, string titulo, EnumTipoAlerta tipoAlerta, EnumBotoesAlerta tipoBotoes, string[] textoBotoes, Action<EnumResultadoAlerta> callback);
    }
}
