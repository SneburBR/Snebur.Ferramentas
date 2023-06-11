using System;

namespace Snebur.VisualStudio
{
    public class GerenciadorProjetosUtil
    {
        public static string DiretorioProjetoTypescriptInicializacao
        {
            get
            {
                var caminho = DiretorioInicializarUtil.DiretorioProjetoTypescriptInicializacao;
                if (caminho == null)
                {
                    LogVSUtil.LogErro("DiretorioProjetoTypescriptInicializacao não definido");
                }
                return caminho;
            }
        }
    }
}