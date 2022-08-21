using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snebur.VisualStudio
{
    public class TipoCSUtil
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
