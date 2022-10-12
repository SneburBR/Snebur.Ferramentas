using System.Windows;
using System.Windows.Media;

namespace Snebur.VisualStudio.Utilidade
{

    public class CorUtil
    {
        public static SolidColorBrush RetornarBrushWindowsText( )
        {
            return RetornarBrushResources(VsBrushes.WindowTextKey, Colors.White);
        }

        public static SolidColorBrush RetornarBrushBackground()
        {
            return RetornarBrushResources(VsBrushes.BackgroundKey, Colors.Black);
        }
        private static SolidColorBrush RetornarBrushResources(object key, Color padrao)
        {

            dynamic objectCorWindowText = Application.Current.Resources[key];
            if (objectCorWindowText != null && objectCorWindowText.Color != null)
            {
                var cor = (Color)objectCorWindowText.Color;
                //var mediaColor = System.Windows.Media.Color.FromArgb(cor.A, cor.R, cor.G, cor.B);
                return new SolidColorBrush(cor);
            }
            return new SolidColorBrush(padrao);

            
        }
    }
}
