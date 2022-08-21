using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.VisualStudio.Utilidade;
using System.Reflection;
using Snebur.VisualStudio.Reflexao;
using Snebur.Reflexao;
using Snebur.Utilidade;

namespace Snebur.VisualStudio.Projeto.Dominio
{
    public partial class GeradorDominioReflexao : BaseGeradorDominio
    {
 
        public string RetornarAdicionarTipo(string caminhoTipo, string declaracao)
        {
            return String.Format("$Reflexao.Tipos.Adicionar(\"{0}\",{1});", caminhoTipo, declaracao);
        }

    }


}
