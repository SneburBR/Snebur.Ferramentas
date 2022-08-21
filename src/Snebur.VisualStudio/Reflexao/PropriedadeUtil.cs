using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using System.Reflection;

namespace Snebur.VisualStudio.Reflexao
{
    public class PropriedadeUtil
    {
        internal static Boolean PossuiAtributo(PropertyInfo propriedade, string nomeAtributo)
        {
            return propriedade.GetCustomAttributes().Any(k => k.GetType().Name == nomeAtributo);
        }

       
    }
}
