using Snebur.Utilidade;
using Snebur.VisualStudio.Utilidade;

namespace Snebur.VisualStudio
{
    public class Repositorio
    {
        private static System.Windows.Media.SolidColorBrush _brushWindowText;
        private static System.Windows.Media.SolidColorBrush _brusBackground;
        public static System.Windows.Media.SolidColorBrush BrushWindowText
               => LazyUtil.RetornarValorLazy(ref _brushWindowText, CorUtil.RetornarBrushWindowsText);

        public static System.Windows.Media.SolidColorBrush BrushBackground
               => LazyUtil.RetornarValorLazy(ref _brusBackground, CorUtil.RetornarBrushBackground);



    }
}
