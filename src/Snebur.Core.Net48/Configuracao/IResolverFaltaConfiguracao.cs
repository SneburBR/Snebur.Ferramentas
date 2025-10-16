using Snebur.Dominio.Atributos;
using System;

namespace Snebur.Aplicacao.Configuracao
{
    [IgnorarInterfaceTS]
    public interface IResolverFaltaApplicationSettings : IDisposable
    {
        void Resolver(IContextoConfiugracao contextoConfiguracao);

        object RetornarValor(string settingName, object valorPadrao, Type tipo);
    }
}
