namespace Snebur.VisualStudio
{
    public static class DiretorioInicializarUtil
    {
        public static string DiretorioProjetoTypescriptInicializacao { get; private set; }

        public static void SetDiretorioProjetoTypescriptInicializacao(string diretorioProjetoTypescript)
        {
            if (diretorioProjetoTypescript == null)
            {
                LogVSUtil.LogErro("Não possível definir diretorioProjetoTypescript null");
                return;
            }

            if(DiretorioProjetoTypescriptInicializacao!= diretorioProjetoTypescript)
            {
                DiretorioProjetoTypescriptInicializacao = diretorioProjetoTypescript;
            }
        }

        public static void ClearDiretorioProjetoTypescriptInicializacao()
        {
             DiretorioProjetoTypescriptInicializacao = null;
        }
    }
}
