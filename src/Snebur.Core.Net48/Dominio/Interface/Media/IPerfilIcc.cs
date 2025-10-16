using System;

namespace Snebur.Dominio
{
    public interface IPerfilIcc
    {
        string Nome { get; set; }

        string Checksum { get; set; }

        DateTime? DataHoraCadastro { get; set; }

        long TotalBytes { get; set; }
    }
}
