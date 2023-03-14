using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.Linq;

namespace Snebur.VisualStudio
{
    public static class TipoCSUtil
    {
        //public static CSharpCodeProvider Compiler { get; } = new CSharpCodeProvider();

        public static string RetornarNomeTipo(Type tipo,
                                             bool isRetornarTipado = false,
                                             bool isVoidRetornarBool = false)
        {
            var nomeTipo = RetornarNomeTipoInterno(tipo);
            //TipoCSUtil.Compiler.GetTypeOutput(new CodeTypeReference(tipo));
            if (isRetornarTipado)
            {
                if (tipo == typeof(void))
                {
                    if (isVoidRetornarBool)
                    {
                        return "<bool>";
                    }
                    return String.Empty;
                }
                return $"<{nomeTipo}>";
            }

            return nomeTipo;
        }

        private static string RetornarNomeTipoInterno(Type tipo)
        {

            if (tipo.IsGenericType)
            {
                var tiposGenericos = tipo.GetGenericArguments().
                                          Select(x => x.Name).
                                          Aggregate((x1, x2) => $"{x1}, {x2}");

                return $"{tipo.Name.Substring(0, tipo.Name.IndexOf("`"))}"
                     + $"<{tiposGenericos}>";
            }

            switch (tipo.Name)
            {
                case nameof(String):

                    return "string";

                case nameof(Int32):

                    return "int";

                case nameof(Int64):

                    return "long";

                case nameof(Boolean):

                    return "bool";

                case nameof(Decimal):

                    return "decimal";

                case nameof(Double):

                    return "decimal";

                case nameof(Single):

                    return "float";
                default:
                    return $"{tipo.Namespace}.{tipo.Name}";
            }
        }
    }
}
