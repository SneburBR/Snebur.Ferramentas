using System;

namespace Snebur.VisualStudio
{
    public class ConfiguracaoGeralUtil
    {
        public static IConfiguracaoGeral Instance => AplicacaoSnebur.AtualTipada<BaseAplicacaoVisualStudio>().
                                                     ConfiguracaoGeral;

   }

    public interface IConfiguracaoGeral
    {
        string CaminhoProjetos { get; }
        string CaminhoInstalacaoVisualStudio { get; }
    }
}