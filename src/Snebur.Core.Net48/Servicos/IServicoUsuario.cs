using Snebur.Dominio;
using Snebur.Seguranca;
using System;

namespace Snebur.Comunicacao
{
    public interface IServicoUsuario : IBaseServico
    {
        bool IsExisteInformacaoIp();
        void AtualizarInformacaoIp(DadosIPInformacao ipInformacao);

        ResultadoExisteIdentificadoUsuario ExisteIdentificadorUsuario(string identificadorUsuario);

        EnumResultadoValidacaoCredencial ValidarCredencial(CredencialUsuario credencial);

        bool SessaoUsuarioAtiva(CredencialUsuario credencial, Guid identificadorSessaoUsuario);

        IUsuario RetornarUsuario(CredencialUsuario credencial);

        ResultadoAutenticacao Autenticar(CredencialUsuario credencial);

        ISessaoUsuario RetornarSessaoUsuario(Guid identificadorSessaoUsuario);

        IUsuario CadastrarNovoUsuario(NovoUsuario novoUsuario, bool isAlterarSenhaProximoAcesso);

        ResultadoEnviarCodigoRecuperarSenha EnviarCodigoRecuperarSenha(string identificadorAmigavel);

        ResultadoValidarCodigoRecuperarSenha ValidarCodigRecuperarSenha(string identificadorAmigavel, string codigoRecuperarSenha);

        ResultadoRecuperarSenha RecuperarSenha(string identificadorAmigavel, string codigoRecuperarSenha, string novaSenha);

        ResultadoAlterarSenha AlterarSenha(CredencialUsuario credencial, string novaSenha);

        void FinalizarSessaoUsuario(Guid identificadorSessaoUsuario);

    }

}