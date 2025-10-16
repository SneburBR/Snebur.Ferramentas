using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Reflection
{
    public static class AssemblyExtensao
    {
        public static bool IsVersaoTeste(this Assembly assembly)
        {
            var atributoVersao = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            if (atributoVersao != null)
            {
                var ultimaParte = atributoVersao.Version.Split('.').Last();
                if (ultimaParte.Length > 1)
                {
                    return ultimaParte.StartsWith("0");
                }
            }
            return false;
        }

        public static string RetornarVersao(this Assembly assembly)
        {
            var atributoVersao = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            if (atributoVersao != null)
            {
                return atributoVersao.Version;
            }
            return "0.0.0.0";
        }

        public static bool IsAssemblySnebur(this Assembly assembly)
        {
            return assembly.FullName.StartsWith("Snebur") ||
                   assembly.FullName.StartsWith("Snebur");
            //var atributo = assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
            //if (atributoVersao != null)
            //{
            //    return atributo.Company = ""
            //}
            //return "0.0.0.0";
            //IsAssemblySnebur
        }

        public static bool IsAssemblyEntidades(this Assembly assembly)
        {
            if (assembly.FullName.StartsWith("System"))
            {
                return false;
            }

            if(assembly.GetCustomAttribute<AssemblyEntidadesAttribute>() != null)
            {
                return true;
            }

            if (DebugUtil.IsAttached)
            {
                if (assembly.FullName.Contains("Entidades"))
                {
                    throw new Exception($"Adicione o atributos {nameof(AssemblyEntidadesAttribute)} no assembly {assembly.FullName}");
                }
            }

            var atributos = assembly.GetCustomAttributes();
            return atributos.Any(x => x.GetType().Name == nameof(AssemblyEntidadesAttribute));
        }

        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(t => t != null);
            }
        }
         
        public static string GetResourceAsString(
            this Assembly assembly, string resource, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            using (var ms = new MemoryStream())
            {
                using (var manifestResourceStream = assembly.GetManifestResourceStream(resource))
                {
                    manifestResourceStream?.CopyTo(ms);
                }
                return encoding.GetString(ms.GetBuffer()).Replace('\0', ' ').Trim();
            }
        }

        public static FileInfo GetAssemblyFile(this Assembly assembly)
        {
            return new FileInfo(new Uri(assembly.Location).LocalPath);
        }

        //public static FileInfo GetAssemblyFile(this AssemblyName assemblyName)
        //{
        //    return assemblyName.
        //}
    }
}
