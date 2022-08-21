using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.VisualStudio.Reflexao;
using Snebur.VisualStudio.Utilidade;

namespace Snebur.VisualStudio.Projeto.Dominio
{
    public partial class AjudanteReflexao
    {
        public  static string RetornarCaminhoTipoBaseDominio(Type tipo)
        {
            return TipoUtil.RetornarCaminhoTipo(tipo);
        }

        

        public static string RetornarDeclaracaoTipoBaseDominio(Type tipo)
        {
            if (tipo.BaseType == null && tipo == typeof(object))
            {
                return "null";
            }
            else
            {
                var caminhoTipo = AjudanteReflexao.RetornarCaminhoTipo_SEM_PONTO(tipo);
                return String.Format("__$tipoBaseDominio{0}", caminhoTipo);
            }
        }

        public static string RetornarAssemblyQualifiedNameTipoBaseDominio(Type tipo)
        {
            return AjudanteReflexao.RetornarAssemblyQualifiedName(tipo);
        }

        //Lista

        public static string RetornarCaminhoListaTipoBaseDominio(Type tipo)
        {
            var caminhoTipo = TipoUtil.RetornarCaminhoTipo(tipo);
            return  String.Format("{0}{1}", PREFIXO_LISTA_TIPO_BASEDOMINIO, caminhoTipo);
        }

        public static string RetornarNomeListaTipoBaseDominio(Type tipo)
        {
            return String.Format("{0}{1}", PREFIXO_LISTA_TIPO_BASEDOMINIO, tipo.Name);
        }
        public static string RetornarDeclaracaoListaTipoBaseDominio(Type tipo)
        {
            if (tipo == null)
            {
                throw new NullReferenceException("");

            }
            else
            {
                var caminhoTipo = TipoUtil.RetornarCaminhoTipo(tipo);
                caminhoTipo = caminhoTipo.Replace(".", "_");
                return String.Format("__$tipoListaBaseDominio_{0}", caminhoTipo);
            }
        }

        public static string RetornarAssemblyQualifiedNameListaTipoBaseDominio(Type tipo)
        {
            var tipoList = typeof(List<>);
            var tipoListGenerico = tipoList.MakeGenericType(tipo);

            var q = tipoListGenerico.AssemblyQualifiedName;
            var posicao = q.IndexOf(", Version");
            if (posicao > 0)
            {
                return q.Substring(0, posicao).Trim() + "]], mscorlib";
            }
            return q;
        }
    }

}
