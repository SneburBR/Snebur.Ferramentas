using System;
using System.Collections.Generic;
using System.Linq;

namespace Snebur
{
    public static class AssemblyQualifiedNameExtensao
    {
        public static string RetornarAssemblyQualifiedName(this Type tipo)
        {
            var assemblyQualifiedName = RetornarAssemblyQualifiedNameInterno(tipo);
            ValidarAssemblyQualifiedName(assemblyQualifiedName);
            return assemblyQualifiedName;
        }

        private static string RetornarAssemblyQualifiedNameInterno(Type tipo)
        {
            var assemblyQualifiedName = tipo.AssemblyQualifiedName;
            if (tipo.IsGenericType)
            {
                if (tipo != tipo.GetGenericTypeDefinition())
                {
                    if (tipo.GetGenericArguments().Length > 1)
                    {
                        throw new NotImplementedException();
                    }
                    var tipoGenerico = tipo.GetGenericArguments().Single();
                    var assemblyQualifiedNameTipoGenerico = tipoGenerico.RetornarAssemblyQualifiedName();

                    throw new NotImplementedException();

                }
            }
            var posicao = assemblyQualifiedName.IndexOf(", Version");
            if (posicao > 0)
            {
                return assemblyQualifiedName.Substring(0, posicao).Trim();
            }
            return assemblyQualifiedName;
        }

        public static string RetornarAssemblyQualifiedNameList(this Type tipo)
        {
            var assemblyQualifiedName = RetornarAssemblyQualifiedNameListInterno(tipo);
            ValidarAssemblyQualifiedName(assemblyQualifiedName);

            return assemblyQualifiedName;
        }

        public static string RetornarAssemblyQualifiedNameListInterno(Type tipo)
        {
            var tipoList = typeof(List<>);
            var tipoListGenerico = tipoList.MakeGenericType(tipo);

            var assemblyQualifiedName = tipoListGenerico.AssemblyQualifiedName;
            var posicao = assemblyQualifiedName.IndexOf(", Version");
            if (posicao > 0)
            {
                return assemblyQualifiedName.Substring(0, posicao).Trim() + "]], mscorlib";
            }
            return assemblyQualifiedName;
        }

        private static void ValidarAssemblyQualifiedName(string assemblyQualifiedName)
        {
            //if (DebugUtil.IsAttached)
            //{
            //    if (Type.GetType(assemblyQualifiedName) == null)
            //    {
            //        throw new Exception($"Não possível obter o tipo pelo caminho {assemblyQualifiedName}");
            //    }
            //}
        }

    }
}
