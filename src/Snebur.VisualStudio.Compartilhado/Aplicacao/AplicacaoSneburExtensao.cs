using System;

namespace Snebur.VisualStudio
{
    public static class AplicacaoSneburExtensao
    {
        public static BaseAplicacaoVisualStudio VisualStudio(this AplicacaoSnebur aplicacaoSnebur)
        {
            if (aplicacaoSnebur is BaseAplicacaoVisualStudio x)
            {
                return x;
            }
            throw new Exception($"A aplicação {aplicacaoSnebur} não é do tipo visual studio");
        }
    }
}
