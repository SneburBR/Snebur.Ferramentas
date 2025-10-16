using System;

namespace Snebur.Utilidade
{
    public class ThreadControleErro
    {
        private Action Acao { get; set; }
        private string NomeThread { get; }

        public ThreadControleErro(Action acao, string nomeThread)
        {
            this.Acao = acao;
            this.NomeThread = nomeThread;
        }

        public void ExecutarAsync()
        {
            ThreadUtil.ExecutarThread(this.Executar, this.NomeThread);
        }

        public void Executar()
        {
            ValidacaoUtil.ValidarReferenciaNula(this.Acao, nameof(this.Acao));

            try
            {
                this.Acao.Invoke();
            }
            catch (Exception ex)
            {
                if (!DebugUtil.IsAttached)
                {
                    var mensagem = $"Erro na thread {this.NomeThread}";
                    LogUtil.ErroAsync(new Exception(mensagem, ex));
                }
            }
            finally
            {
                this.Acao = null;
            }
        }

    }
}