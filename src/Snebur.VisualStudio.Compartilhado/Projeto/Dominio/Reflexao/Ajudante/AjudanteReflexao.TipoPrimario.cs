using System;
using Snebur.Reflexao;
using Snebur.Utilidade;

namespace Snebur.VisualStudio
{
    public partial class AjudanteReflexao
    {
        public static string RetornarCaminhoTipoPrimario(EnumTipoPrimario tipoPrimarioEnum)
        {
            return EnumUtil.RetornarDescricao(tipoPrimarioEnum);
        }

        public static string RetornarDeclaracaoTipoPrimario(EnumTipoPrimario tipoPrimarioEnum)
        {
            return String.Format("__$tipoTipoPrimario_{0}", EnumUtil.RetornarDescricao(tipoPrimarioEnum));
        }

        public static string RetornarAssemblyQualifiedNameTipoPrimario(Type tipo)
        {
            return tipo.RetornarAssemblyQualifiedName();
            //return AjudanteReflexao.RetornarAssemblyQualifiedName_NOVO(tipo);
        }

        //Lista
        public static string RetornarDeclaracaoListaTipoPrimario(EnumTipoPrimario tipoPrimarioEnum)
        {
            return String.Format("__$tipoListaTipoPrimario_{0}", EnumUtil.RetornarDescricao(tipoPrimarioEnum));
        }

        public static string RetornarCaminhoListaTipoPrimario(EnumTipoPrimario tipoPrimarioEnum)
        {
            return String.Format("{0}{1}", AjudanteReflexao.PREFIXO_LISTA_TIPO_PRIMARIO, EnumUtil.RetornarDescricao(tipoPrimarioEnum));
        }

        public static string RetornarAssemblyQualifiedNameListaTipoPrimario(Type tipo)
        {
            return tipo.RetornarAssemblyQualifiedName();
            //return AjudanteReflexao.RetornarAssemblyQualifiedName_NOVO(tipo);
        }

    }
}
