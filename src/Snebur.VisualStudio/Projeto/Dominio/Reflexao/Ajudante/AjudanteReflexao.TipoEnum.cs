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
        public static string RetornarCaminhoTipoEnum(Type tipo)
        {
            return TipoUtil.RetornarCaminhoTipo(tipo);
        }

        public static string RetornarDeclaracaoTipoEnum(Type tipo)
        {
            if (tipo == null)
            {
                throw new NullReferenceException("");
            }

            if (!tipo.IsEnum)
            {
                throw new NotSupportedException("Tipo não suportado");
            }

            var caminhoTipo = TipoUtil.RetornarCaminhoTipo(tipo);
            caminhoTipo = caminhoTipo.Replace(".", "_");
            return String.Format("__$tipoEnum_{0}", caminhoTipo);
        }

        public static string RetornarAssemblyQualifiedNameTipoEnum(Type tipo)
        {
            return AjudanteReflexao.RetornarAssemblyQualifiedNameListaTipoBaseDominio(tipo);
        }

        //Lista

        public static string RetornarCaminhoListaTipoEnum(Type tipo)
        {
            var caminhoTipo = AjudanteReflexao.RetornarCaminhoTipo_SEM_PONTO(tipo);
            return String.Format("{0}{1}", PREFIXO_LISTA_TIPO_ENUM, caminhoTipo);
        }

        public static string RetornarNomeListaTipoEnum(Type tipo)
        {
            var caminhoTipo = TipoUtil.RetornarCaminhoTipo(tipo);
            return String.Format("{0}{1}", PREFIXO_LISTA_TIPO_ENUM, caminhoTipo);
        }

        public static string RetornarDeclaracaoListaTipoEnum(Type tipo)
        {
            if (tipo == null)
            {
                throw new NullReferenceException("");
            }

            if (!tipo.IsEnum)
            {
                throw new NotSupportedException("Tipo não suportado");
            }

            var caminhoTipo = AjudanteReflexao.RetornarCaminhoTipo_SEM_PONTO(tipo);
            return String.Format("__$tipoListaEnum_{0}", caminhoTipo);

        }

        public static string RetornarAssemblyQualifiedNameListaTipoEnum(Type tipo)
        {
            return RetornarAssemblyQualifiedName(tipo);
        }
    }
}
