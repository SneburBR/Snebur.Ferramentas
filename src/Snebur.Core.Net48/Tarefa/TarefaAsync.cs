namespace Snebur.Tarefa
{
    public abstract class TarefaAsync : BaseTarefa
    {
        public abstract void ExecutarAsync();

        internal override void ExecutarInterno()
        {
            this.ExecutarAsync();
            //Erro erro = null;
            //try
            //{
            //    this.Executar()
            //}
            //catch (Exception ex)
            //{
            //    erro = new ErroTarefa($"Erro na tarefa {this.GetType().Name}", ex);
            //}
            //finally
            //{
            //    this.FinalizarTarefa(erro);
            //}
        }
    }
}