using Snebur.Dominio.Atributos;
using System;

namespace Snebur.Dominio
{
    public interface IArquivo : IEntidade
    {
        string NomeArquivo { get; set; }

        string CaminhoArquivo { get; set; }

        string Checksum { get; set; }

        long TotalBytesLocal { get; set; }

        long TotalBytes { get; set; }

        [Indexar]
        bool IsExisteArquivo { get; set; }

        [ValorPadraoDataHoraServidor]
        DateTime? DataHoraCadastro { get; set; }

        DateTime? DataHoraInicioEnvio { get; set; }

        DateTime? DataHoraFimEnvio { get; set; }

        DateTime? DataHoraArquivoDeletado { get; set; }

        EnumStatusArquivo Status { get; set; }

        double Progresso { get; set; }

        ISessaoUsuario SessaoUsuario { get; set; }

        EnumMimeType MimeType { get; set; }
    }
}
