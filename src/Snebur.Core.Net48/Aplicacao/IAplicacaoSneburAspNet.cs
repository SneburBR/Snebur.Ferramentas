using Snebur.Comunicacao;
using Snebur.Dominio.Atributos;

namespace Snebur
{
    [IgnorarInterfaceTS]
    public interface IAplicacaoSneburAspNet
    {
        bool IsPossuiRequisicaoAspNetAtiva { get; }
        string RetornarValueCabecalho(string chave);
        T GetHttpContext<T>();

        //string RetornarUrlRequisicao();

        //InformacaoSessaoUsuario InformacaoSessaoUsuarioRequisicaoAtual { get; }

        //[EditorBrowsable(EditorBrowsableState.Never)]
        //Guid IdentificadorSessaoUsuarioRequisicaoAtual { get; }

        //[EditorBrowsable(EditorBrowsableState.Never)]
        //CredencialUsuario CredencialUsuarioRequisicaoAtual { get; }

        //[EditorBrowsable(EditorBrowsableState.Never)]
        //string IdentificadorProprietarioRequisicaoAtual { get; }

        //[EditorBrowsable(EditorBrowsableState.Never)]
        string UserAgent { get; }
        string IpRequisicao { get; }

        InfoRequisicao RetornarInfoRequisicao();

 

        //string RetornarIpDaRequisicao();
    }
}
