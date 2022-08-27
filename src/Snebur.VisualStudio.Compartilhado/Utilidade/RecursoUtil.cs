using System;
using System.IO;
using System.Reflection;

namespace Snebur.VisualStudio.Utilidade
{
    public static class RecursoUtil
    {
        public static string RetornarRecursoTexto(string caminhoRecursoRelativo)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var caminhoRecurso = RecursoUtil.RetornarCaminhoRecurso(assembly, caminhoRecursoRelativo);
            using (var stream = assembly.GetManifestResourceStream(caminhoRecurso))
            {
                if (stream == null)
                {
                    LogVSUtil.LogErro($"O recurso Embedded '{caminhoRecurso}' não foi encontrado no assembly '{assembly.FullName}' ");
                    return null;
                }

                using (var reader = new StreamReader(stream, System.Text.Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private static string RetornarCaminhoRecurso(Assembly assembly, string caminhoRecursoRelativo)
        {
            return $"{new AssemblyName(assembly.FullName).Name}.{caminhoRecursoRelativo}";
        }
    }

}
