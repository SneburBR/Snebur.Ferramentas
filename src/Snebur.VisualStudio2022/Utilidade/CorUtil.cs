using System.Windows;
using System.Windows.Media;

namespace Snebur.VisualStudio.Utilidade
{

    public class CorUtil
    {
        public static SolidColorBrush RetornarBrushWindowsText()
        {

            dynamic objectCorWindowText = Application.Current.Resources[VsBrushes.WindowTextKey];
            if (objectCorWindowText != null && objectCorWindowText.Color != null)
            {
                var cor = (Color)objectCorWindowText.Color;
                //var mediaColor = System.Windows.Media.Color.FromArgb(cor.A, cor.R, cor.G, cor.B);
                return new SolidColorBrush(cor);
            }
            return new SolidColorBrush(Colors.White);

            //if (objectCorWindowText != null)
            //{
            //    var cor = (System.Windows.Media.Color)objectCorWindowText.Color;
            //    //var mediaColor = System.Windows.Media.Color.FromArgb(cor.A, cor.R, cor.G, cor.B);
            //    return new System.Windows.Media.SolidColorBrush(cor);
            //}
            //else
            //{
            //    throw new Exception("Não foi possivel obter a cor WidowText do DinamicResources ");
            //}
        }
    }
}
