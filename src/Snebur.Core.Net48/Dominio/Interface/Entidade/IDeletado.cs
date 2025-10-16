using Snebur.Dominio.Atributos;
using System;

namespace Snebur.Dominio
{
    public interface IDeletado : IEntidade
    {
        bool IsDeletado { get; set; }

        [ValorPadraoDataHoraServidor]
        DateTime? DataHoraCadastro { get; set; }
        DateTime? DataHoraDeletado { get; set; }

        long? SessaoUsuarioDeletado_Id { get; set; }

        ISessaoUsuario SessaoUsuarioDeletado { get; set; }
    }
}
