using Snebur.Dominio.Atributos;
using System;

namespace Snebur.Dominio
{
    public interface IDpiVisualizacao
    {
        [IgnorarPropriedade]
        Func<double?, double> FuncaoNormamlizarDpiVisualizacao { get; set; }
    }
}
