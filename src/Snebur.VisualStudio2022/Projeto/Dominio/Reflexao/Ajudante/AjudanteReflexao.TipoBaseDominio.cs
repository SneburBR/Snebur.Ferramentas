﻿using System.Collections.Generic;
using Snebur.VisualStudio.Reflexao;

namespace Snebur.VisualStudio
{
    public partial class AjudanteReflexao
    {
        public  static string RetornarCaminhoTipoBaseDominio(Type tipo)
        {
            return TipoUtil.RetornarCaminhoTipoTS(tipo);
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
            var caminhoTipo = TipoUtil.RetornarCaminhoTipoTS(tipo);
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
                var caminhoTipo = TipoUtil.RetornarCaminhoTipoTS(tipo);
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
