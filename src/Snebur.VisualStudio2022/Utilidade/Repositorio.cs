using Snebur.VisualStudio.Utilidade;

namespace Snebur.VisualStudio
{
    public class Repositorio
    {
        private static System.Windows.Media.SolidColorBrush _brushWindowText;
        public static System.Windows.Media.SolidColorBrush BrushWindowText
        {
            get
            {
                if (_brushWindowText == null)
                {
                    _brushWindowText = CorUtil.RetornarBrushWindowsText();
                }
                return _brushWindowText;
            }
        }


    }
}
