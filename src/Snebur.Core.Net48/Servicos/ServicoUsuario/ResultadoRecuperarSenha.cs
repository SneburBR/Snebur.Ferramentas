using Snebur.Dominio;

namespace Snebur.Comunicacao
{
    public class ResultadoRecuperarSenha : BaseResultadoRecuperarSenha
    {

        #region Campos Privados

        private bool _isUsuarioEncontrado;
        private EnumStatusCodigoRecuperarSenha _status;

        #endregion

        public bool IsUsuarioEncontrado { get => this.RetornarValorPropriedade(this._isUsuarioEncontrado); set => this.NotificarValorPropriedadeAlterada(this._isUsuarioEncontrado, this._isUsuarioEncontrado = value); }

        public EnumStatusCodigoRecuperarSenha Status { get => this.RetornarValorPropriedade(this._status); set => this.NotificarValorPropriedadeAlterada(this._status, this._status = value); }

    }
}