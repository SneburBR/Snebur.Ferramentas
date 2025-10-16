using Snebur.Utilidade;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Snebur.Servicos
{
    public abstract class BaseServicoLocal
    {
        private static readonly object _lock = new object();
        private const int LIMITE_DIAS_LOGS = 30; // dias

        protected bool IsDebugAttachDispararErro { get; set; }
        private string _retornarRepositorioLogs;
        private string CaminhoRetornarRepositorioLogs
        {
            get
            {
                if (this._retornarRepositorioLogs == null)
                {
                    this._retornarRepositorioLogs = CaminhoUtil.Combine(ConfiguracaoUtil.CaminhoAppDataAplicacao, "Logs", this.RetornarPastaLog());
                    DiretorioUtil.CriarDiretorio(this._retornarRepositorioLogs);
                }
                return this._retornarRepositorioLogs;
            }
        }

        protected BaseServicoLocal()
        {
            Task.Run(this.ManutencaoAsync);
        }

        private void ManutencaoAsync()
        {
            var dirtorio = this.CaminhoRetornarRepositorioLogs;
            if (!Directory.Exists(dirtorio))
            {
                return;
            }

            foreach (var arquivoLog in Directory.EnumerateDirectories(dirtorio, "*.log", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    var dataUltimaModificacao = File.GetLastWriteTime(arquivoLog);
                    var dias = (DateTime.Now - dataUltimaModificacao).TotalDays;

                    if (dias > LIMITE_DIAS_LOGS)
                    {
                        ArquivoUtil.DeletarArquivo(arquivoLog, ignorarErro: true, isForcar: true);
                    }
                }
                catch
                {

                }
            }
        }
        

        private string RetornarPastaLog()
        {
            if (this is ServicoErroLocal)
            {
                return "Erros";
            }
            if (this is ServicoLogSegurancaLocal)
            {
                return "Seguranca";
            }
            if (this is ServicoLogAplicacaoLocal)
            {
                return "Aplicacao";
            }
            if (this is ServicoLogDesempenhoLocal)
            {
                return "Desempenho";
            }
            throw new ErroNaoSuportado("O servico local não é suportado");
        }

        protected void SalvarLog(string log)
        {
            if (!DebugUtil.IsAttached)
            {
                ThreadUtil.ExecutarAsync((Action)(() =>
                {
                    this.SalvarArquivoLogLocal(log);
                }));
            }
        }

        private void SalvarArquivoLogLocal(string log)
        {
            try
            {
                var nomeArquivo = String.Format("{0}.log", DateTime.Now.ToString("yyyy-MMMM-dd"));
                var caminhoArquivo = CaminhoUtil.Combine(this.CaminhoRetornarRepositorioLogs, nomeArquivo);

                lock (_lock)
                {
                    using (var sw = new StreamWriter(caminhoArquivo, true, Encoding.UTF8))
                    {
                        sw.WriteLine(String.Format("{0} -- Utc {1}", DateTime.Now.ToLongTimeString(), DateTime.UtcNow.ToString()));
                        sw.WriteLine(log);
                        sw.Flush();
                        sw.Close();
                    }
                }
            }
            catch (Exception)
            {
                if (DebugUtil.IsAttached)
                {
                    throw;
                }
            }
        }
    }
}