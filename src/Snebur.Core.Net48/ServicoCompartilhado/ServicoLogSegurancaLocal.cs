using Snebur.Comunicacao;
using Snebur.Utilidade;
using System;
using System.Text;

namespace Snebur.Servicos
{
    public class ServicoLogSegurancaLocal : BaseServicoLocal, IServicoLogSeguranca
    {
        public Guid NotificarLogSeguranca(string mensagem,
                              string stackTrace,
                              InfoRequisicao infoRequisicao,
                              EnumTipoLogSeguranca tipoLogSeguranca,
                              BaseInformacaoAdicionalServicoCompartilhado informacaoAdicional)
        {
            var sb = new StringBuilder();
            var descricaoTipo = EnumUtil.RetornarDescricao(tipoLogSeguranca);
            sb.AppendLine("Tipo:" + descricaoTipo);
            if (infoRequisicao != null)
            {
                sb.AppendLine("IP" + infoRequisicao.IpRequisicao);
                sb.AppendLine("UserAgente" + infoRequisicao.UserAgent);
                sb.AppendLine("Usuário" + infoRequisicao.CredencialUsuario);
            }
            sb.AppendLine(mensagem);
            this.SalvarLog(mensagem);
            this.IsDebugAttachDispararErro = true;
            return Guid.NewGuid();
        }
    }
}
