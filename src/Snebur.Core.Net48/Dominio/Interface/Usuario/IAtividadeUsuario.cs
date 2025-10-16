using Snebur.Dominio.Atributos;
using System;

namespace Snebur.Dominio
{
    public interface IAtividadeUsuario : IEntidade
    {
        [ValorPadraoDataHoraServidor]
        DateTime? DataHora { get; set; }

        [ValidacaoRequerido]
        IUsuario Usuario { get; set; }

        IUsuario UsuarioNotificacao { get; set; }

        [ValorPadraoIP]
        string IP { get; set; }

        long SessaoUsuario_Id { get; set; }
        ISessaoUsuario SessaoUsuario { get; set; }

        //IUsuario UsuarioNotificacao { get; set; }

    }
}