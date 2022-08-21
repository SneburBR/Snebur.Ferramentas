using System;
using Snebur.VisualStudio.Reflexao;

namespace Snebur.VisualStudio
{
    public partial class AjudanteReflexao
    {
        public static string RetornarCaminhoTipoEnum(Type tipo)
        {
            return TipoUtil.RetornarCaminhoTipoTS(tipo);
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

            var caminhoTipo = TipoUtil.RetornarCaminhoTipoTS(tipo);
            caminhoTipo = caminhoTipo.Replace(".", "_");
            return String.Format("__$tipoEnum_{0}", caminhoTipo);
        }

        public static string RetornarAssemblyQualifiedNameTipoEnum(Type tipo)
        {
            return AjudanteReflexao.RetornarAssemblyQualifiedNameTipoBaseDominio(tipo);
        }

        //Lista

        public static string RetornarCaminhoListaTipoEnum(Type tipo)
        {
            var caminhoTipo = AjudanteReflexao.RetornarCaminhoTipo_SEM_PONTO(tipo);
            return String.Format("{0}{1}", PREFIXO_LISTA_TIPO_ENUM, caminhoTipo);
        }

        public static string RetornarNomeListaTipoEnum(Type tipo)
        {
            var caminhoTipo = TipoUtil.RetornarCaminhoTipoTS(tipo);
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
