using Microsoft.CSharp;
using System;
using System.CodeDom;

namespace Snebur.VisualStudio
{
    public static class TipoCSUtil
    {
        public static CSharpCodeProvider Compiler { get; } = new CSharpCodeProvider();

        public static string RetornarNomeTipo(Type tipo, bool isRetornarTipado = false, bool isVoidRetornarBool = false)
        {
            var nomeTipo = TipoCSUtil.Compiler.GetTypeOutput(new CodeTypeReference(tipo));
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
    }
}
