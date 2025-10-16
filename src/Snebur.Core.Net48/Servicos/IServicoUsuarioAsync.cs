using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Seguranca;
using System;
using System.Threading.Tasks;

namespace Snebur.Comunicacao
{
    [IgnorarInterfaceTS]
    public interface IServicoUsuarioAsync : IBaseServico
    {
        Task<bool> IsExisteInformacaoIpAsync();
        Task  AtualizarInformacaoIpAsync(DadosIPInformacao ipInformacao);
        Task<ResultadoExisteIdentificadoUsuario> ExisteIdentificadorUsuarioAsync(string identificadorUsuario);

        Task<EnumResultadoValidacaoCredencial> ValidarCredencialAsync(CredencialUsuario credencial);

        Task<bool> SessaoUsuarioAtivaAsync(CredencialUsuario credencial, Guid identificadorSessaoUsuario);

        Task<IUsuario> RetornarUsuarioAsync(CredencialUsuario credencial);

        Task<ResultadoAutenticacao> AutenticarAsync(CredencialUsuario credencial);

        Task<ISessaoUsuario> RetornarSessaoUsuarioAsync(Guid identificadorSessaoUsuario);

        Task<IUsuario> CadastrarNovoUsuarioAsync(NovoUsuario novoUsuario, bool isAlterarSenhaProximoAcesso);

        Task<ResultadoEnviarCodigoRecuperarSenha> EnviarCodigoRecuperarSenhaAsync(string identificadorAmigavel);

        Task<ResultadoValidarCodigoRecuperarSenha> ValidarCodigRecuperarSenhaAsync(string identificadorAmigavel, string codigoRecuperarSenha);

        Task<ResultadoRecuperarSenha> RecuperarSenhaAsync(string identificadorAmigavel, string codigoRecuperarSenha, string novaSenha);

        Task<ResultadoAlterarSenha> AlterarSenhaAsync(CredencialUsuario credencial, string novaSenha);

        Task FinalizarSessaoUsuarioAsync(Guid identificadorSessaoUsuario);
    }

}