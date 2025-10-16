using System;

namespace Snebur.Tarefa
{
    public abstract class Tarefa : BaseTarefa
    {
        public abstract void Executar();

        internal override void ExecutarInterno()
        {
            Erro erro = null;
            try
            {
                this.Executar();
            }
            catch (Exception ex)
            {
                erro = new ErroTarefa($"Erro na tarefa {this.GetType().Name}", ex);
            }
            finally
            {
                this.FinalizarTarefa(erro);
            }
        }
    }
}
