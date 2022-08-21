using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using System.Windows;
using Microsoft.VisualStudio.Shell;

namespace Snebur.VisualStudio.Utilidade
{

    public class CorUtil
    {
        public static System.Windows.Media.SolidColorBrush RetornarBrushWindowsText()
        {
            dynamic objectCorWindowText = Application.Current.Resources[VsBrushes.WindowTextKey];
            if (objectCorWindowText != null && objectCorWindowText.Color != null)
            {
                var cor = (System.Windows.Media.Color)objectCorWindowText.Color;
                //var mediaColor = System.Windows.Media.Color.FromArgb(cor.A, cor.R, cor.G, cor.B);
                return new System.Windows.Media.SolidColorBrush(cor);
            }
            else
            {
                throw new Exception("Não foi possivel obter a cor WidowText do DinamicResources ");
            }
        }
    }
}
