using System.IO;
using System.Reflection;

namespace Snebur.VisualStudio.Utilidade
{
    public class RecursoUtil
    {

        public static string RetornarRecursoTexto(string caminhoRecurso)
        {
            
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(caminhoRecurso))
            {
                using (var reader = new StreamReader(stream, System.Text.Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }

}
