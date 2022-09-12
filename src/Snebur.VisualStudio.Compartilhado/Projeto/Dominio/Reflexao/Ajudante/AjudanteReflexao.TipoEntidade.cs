using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Dominio;
using Snebur.Utilidade;
using Snebur.VisualStudio.Reflexao;
using Snebur.VisualStudio.Utilidade;

namespace Snebur.VisualStudio
{
    public partial class AjudanteReflexao
    {

        public static string RetornarCaminhoListaTipoEntidade(Type tipo)
        {
            var caminhoTipo = TipoUtil.RetornarCaminhoTipoTS(tipo);
            return String.Format("{0}{1}", PREFIXO_LISTA_TIPO_ENTIDADE, caminhoTipo);
        }

        public static string RetornarNomeTipoEntidade(Type tipo)
        {
            return String.Format("{0}{1}", PREFIXO_LISTA_TIPO_ENTIDADE, tipo.Name);
        }

        public static string RetornarDeclaracaoListaTipoEntidade(Type tipo)
        {
            if (tipo == null)
            {
                throw new NullReferenceException("O tipo não foi definido");
            }
            else
            {
                if (tipo.Name == AjudanteAssembly.TipoInterfaceIEntidade.Name)
                {
                    tipo = AjudanteAssembly.TipoEntidade;
                }
                var caminhoTipo = AjudanteReflexao.RetornarCaminhoTipo_SEM_PONTO(tipo);
                return String.Format("__$tipoListaEntidade_{0}", caminhoTipo);
            }
        }

        public static string RetornarAssemblyQualifiedNameListaTipoEntidade(Type tipo)
        {
            var tipoListaEntidades = typeof(ListaEntidades<>);
            var assemblyName = tipoListaEntidades.RetornarAssemblyQualifiedName();
            var posicaoItemGerenciado = assemblyName.IndexOf("`1") + 2;
            if (posicaoItemGerenciado > 0)
            {
                var inicio = assemblyName.Substring(0, posicaoItemGerenciado);
                var fim = assemblyName.Substring(posicaoItemGerenciado);
                return $"{inicio}[[{tipo.RetornarAssemblyQualifiedName()}]]{fim}";
            }

            throw new NotImplementedException();

            //var partes = "";

            //var tipoListaEntidadesTipado = tipoListaEntidades.MakeGenericType(tipo);

            //var tipoListaEntidadesTipado2 = tipoListaEntidades.MakeGenericType(tipo);

            //return tipoListaEntidadesTipado.RetornarAssemblyQualifiedName();

            //return String.Format("Snebur.Dominio.ListaEntidades`1[[{0}]], Snebur", tipo.RetornarAssemblyQualifiedName());
        }

    }
}
