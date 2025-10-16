using System;

namespace Snebur.Dominio
{
    public interface IArquivoDeletada
    {
        long Imagem_Id { get; set; }

        DateTime DataHoraCadastro { get; set; }

        DateTime DataHoraArquivoDeletado { get; set; }
    }
}
