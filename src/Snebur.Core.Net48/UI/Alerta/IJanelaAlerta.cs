using Snebur.Dominio.Atributos;
using System;

namespace Snebur.UI
{
    [IgnorarInterfaceTS]
    public interface IJanelaAlerta : IDisposable
    {
        void Fechar();
    }
}
