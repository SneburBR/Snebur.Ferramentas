using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Snebur.AcessoDados;
using Snebur.Reflexao;
using Snebur.Utilidade;

namespace Snebur.VisualStudio.Reflexao
{
    public partial class TipoUtil
    {

        #region "Comparadores"

        public static List<Type> RetornarSubTipos(List<Type> tipos, Type tipo)
        {
            //return tipos.Where(x => x.BaseType != null && TipoUtil.TipoIgualOuSubTipo(x, tipo)).ToList();
            return tipos.Where(x => x.BaseType != null && TipoSubTipo(x, tipo)).ToList();
        }

        public static List<Type> RetornarBaseIgualTipoBase(List<Type> tipos, Type tipo)
        {


            return tipos.Where(x => x.BaseType != null &&
                                    (x.BaseType.Equals(tipo) ||
                                    TipoUtil.TipoIgual(x.BaseType, tipo))).ToList();
        }

        public static bool TipoIgual(Type tipo1, Type tipo2)
        {
            return tipo1 != null && tipo2 != null && (
                   tipo1.Equals(tipo2) ||  (tipo1.Name == tipo2.Name && tipo1.Namespace == tipo2.Namespace));
        }

        public static bool TipoIgualOuSubTipo(Type tipo1, Type tipo2)
        {
            if (TipoIgual(tipo1, tipo2))
            {
                return true;
            }
            return TipoSubTipo(tipo1, tipo2);
        }

        /// <summary>
        /// tipo1.IsSubclassOf(tipo2)
        /// </summary>
        /// <param name="tipo1"></param>
        /// <param name="tipo2"></param>
        /// <returns></returns>
        public static bool TipoSubTipo(Type tipo1, Type tipo2)
        {

            if (tipo1.IsSubclassOf(tipo2))
            {
                return true;
            }

            var tipoBaseAtual = tipo1;
            while (tipoBaseAtual != null)
            {
                if (TipoIgual(tipoBaseAtual.BaseType, tipo2))
                {
                    return true;
                }
                tipoBaseAtual = tipoBaseAtual.BaseType;
            }
            return false;
        }
         
        public static void RemoverTipo(HashSet<Type> tipos, Type tipo)
        {
            var tiposRemover = tipos.Where(x => (TipoUtil.TipoIgual(x, tipo))).ToList();

            foreach (var tipoRemover in tiposRemover)
            {
                tipos.Remove(tipoRemover);
            }
        }

        public static void RemoverTipo(List<Type> tipos, Type tipo)
        {
            var tiposRemover = tipos.Where(x => (TipoUtil.TipoIgual(x, tipo))).ToList();

            foreach (var tipoRemover in tiposRemover)
            {
                tipos.Remove(tipoRemover);
            }
        }

        #endregion

        public static string RetornarNomeTipoTS(Type tipo)
        {

            if (tipo.Name.Contains("BaseListaTipoComplexo") ||
                (tipo.BaseType?.Name.Contains("BaseListaTipoComplexo") ?? false))
            {
                return tipo.Name.Split('`').First();
            }

            if (tipo.IsGenericType && tipo.Name.Contains(nameof(IConsultaEntidade)))
            {
                var nomeTipoGenerico = RetornarCaminhoTipoTS(tipo.GetGenericArguments().Single());
                return $"{nameof(IConsultaEntidade)}<{nomeTipoGenerico}>";
            }

            if (ReflexaoUtil.IsTipoNullable(tipo) && ReflexaoUtil.RetornarTipoSemNullable(tipo).IsEnum)
            {
                return $"{ReflexaoUtil.RetornarTipoSemNullable(tipo).Name} | null";
            }
            if (!tipo.IsEnum && ReflexaoUtil.TipoRetornaTipoPrimario(ReflexaoUtil.RetornarTipoSemNullable(tipo)))
            {

                return TipoUtil.RetornarNomeTipoPrimarioTypeScript(tipo);
            }
            else if (ReflexaoUtil.TipoRetornaColecao(tipo) && !tipo.IsInterface)
            {
                var tipoItemLista = ReflexaoUtil.RetornarTipoGenericoColecao(tipo);
                if (tipo.IsGenericType)
                {
                    var tipoDefinicacao = tipo.GetGenericTypeDefinition();
                    if (tipoDefinicacao.Name.Contains("BaseListaTipoComplexo"))
                    {
                        //return String.Format("BaseListaTipoComplexo<{0}>", RetornarCaminhoTipo(tipoItemLista));
                        return "BaseListaTipoComplexo";
                    }

                    if (tipoDefinicacao.Name.Contains("ListaEntidades"))
                    {
                        return String.Format("ListaEntidades<{0}>", RetornarCaminhoTipoTS(tipoItemLista));
                    }
                    else if (tipoDefinicacao == typeof(List<>))
                    {
                        return String.Format("Array<{0}>", RetornarCaminhoTipoTS(tipoItemLista));
                    }
                    else if (tipoDefinicacao == typeof(Dictionary<,>))
                    {
                        var tipoDaChave = tipo.GetGenericArguments().First();
                        if (tipoDaChave != typeof(string))
                        {
                            throw new ErroNaoSuportado("O tipo da chave não é suportado");
                        }
                        var tipoItemChave = tipo.GetGenericArguments().First();
                        tipoItemLista = tipo.GetGenericArguments().Last();

                        if (tipoItemChave.Name == typeof(string).Name)
                        {
                            return String.Format("DicionarioSimples<{0}>", TipoUtil.RetornarCaminhoTipoTS(tipoItemLista));
                        }
                        else
                        {
                            return String.Format("DicionarioTipado<{0}, {1}>", TipoUtil.RetornarCaminhoTipoTS(tipoItemChave), TipoUtil.RetornarCaminhoTipoTS(tipoItemLista));
                        }

                    }
                    else if (tipoDefinicacao == typeof(HashSet<>))
                    {
                        return String.Format("HashSet<{0}>", TipoUtil.RetornarCaminhoTipoTS(tipoItemLista));
                    }
                    else
                    {
                        throw new ErroNaoSuportado("O tipo da coleção  não é suportado");
                    }
                }
                else
                {
                    return String.Format("Array<{0}>", TipoUtil.RetornarCaminhoTipoTS(tipoItemLista));
                }
            }
            else
            {
                if (tipo.Namespace.StartsWith("System"))
                {
                    if (tipo == typeof(object))
                    {
                        return "any";
                    }

                    if (tipo.Name == "Void")
                    {
                        return "void";
                    }

                    if (tipo == typeof(System.IO.FileInfo))
                    {
                        return "SnBlob";
                    }

                    if(tipo == typeof(Type))
                    {
                        return "r.BaseTipo | string";
                    }

                    if(tipo == typeof(PropertyInfo))
                    {
                        return "r.Propriedade | string";
                    }

                    if (TipoUtil.TipoIgualOuSubTipo(tipo, typeof(ValidationAttribute)))
                    {
                        return AjudanteAssembly.NOME_TIPO_BASE_ATRIBUTO_VALIDACAO;
                    }
                    else if (TipoUtil.TipoIgualOuSubTipo(tipo, typeof(ForeignKeyAttribute)) ||
                             TipoUtil.TipoIgualOuSubTipo(tipo, typeof(ScaffoldColumnAttribute)))
                    {
                        return AjudanteAssembly.NomeTipoBaseAtributoDominio;
                    }
                    else
                    {
                        throw new NotImplementedException(tipo.Name);
                    }
                }
                return tipo.Name;
            }
        }

        internal static List<Type> IgnorarAtributo(List<Type> tipos, string nomeAtributo)
        {
            return tipos.Where(x => !x.GetCustomAttributes(false).Any(k => k.GetType().Name == nomeAtributo)).ToList();
        }

        internal static bool TipoPossuiAtributo(Type tipo, string nomeAtributo)
        {
            return tipo.GetCustomAttributes(false).Any(x => x.GetType().Name == nomeAtributo);
        }

        internal static bool TipoImplementaInterface(Type tipo, Type tipoInterface)
        {
            if (!tipoInterface.IsInterface)
            {
                throw new Erro($"O tipo {tipoInterface.Name}  não é interface");
            }
            if(tipo.Name == tipoInterface.Name && 
               tipo.Namespace == tipoInterface.Namespace &&
               tipo.IsInterface )
            {
                return true;
            }
            return tipo.GetInterfaces().Any(x => x.Name == tipoInterface.Name);
        }

        /// <summary>
        /// Se tipo possui o atributo ou atributo herda
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="nomeAtributo"></param>
        /// <returns></returns>
        internal static bool TipoPossuiAtributo(Type tipo, Type tipoAtributo, bool herda)
        {
            if (herda)
            {
                return tipo.GetCustomAttributes(false).Any(x => x.GetType().Name == tipoAtributo.Name || TipoUtil.TipoIgualOuSubTipo(x.GetType(), tipoAtributo));
            }
            else
            {
                return tipo.GetCustomAttributes(false).Any(x => x.GetType().Name == tipoAtributo.Name);
            }
        }

        internal static Attribute RetornarAtributo(Type tipo, Type tipoAtributo, bool atributoHerdadoTipo)
        {
            if (atributoHerdadoTipo)
            {
                return tipo.GetCustomAttributes(false).Cast<Attribute>().Where(x => x.GetType().Name == tipoAtributo.Name || TipoUtil.TipoIgualOuSubTipo(x.GetType(), tipoAtributo)).SingleOrDefault();
            }
            else
            {
                return tipo.GetCustomAttributes(false).Cast<Attribute>().Where(x => x.GetType().Name == tipoAtributo.Name).SingleOrDefault();
            }
        }


        internal static List<Attribute> RetornarAtributos(Type tipo, Type tipoAtributo, bool atributoHerdadoTipo)
        {
            if (atributoHerdadoTipo)
            {
                return tipo.GetCustomAttributes(false).Cast<Attribute>().Where(x => x.GetType().Name == tipoAtributo.Name || TipoUtil.TipoIgualOuSubTipo(x.GetType(), tipoAtributo)).ToList();
            }
            else
            {
                return tipo.GetCustomAttributes(false).Cast<Attribute>().Where(x => x.GetType().Name == tipoAtributo.Name).ToList();
            }
        }

        //internal static Boolean TipoImplementaInterface(Type tipo, Type tipoInterface)
        //{
        //    return ReflexaoUtil.TipoImplementaInterface(tipo, tipoInterface, false);
        //}

        public static string RetornarNameSpace(Type tipo)
        {
            if (ReflexaoUtil.IsTipoNullable(tipo) && ReflexaoUtil.RetornarTipoSemNullable(tipo).IsEnum)
            {
                return ReflexaoUtil.RetornarTipoSemNullable(tipo).Namespace;
            }

            if (tipo.IsGenericType && tipo.Name.Contains("BaseListaTipoComplexo"))
            {
                return tipo.Namespace;
            }

            if (!tipo.IsGenericType && tipo.BaseType != null && tipo.BaseType.Name.Contains("BaseListaTipoComplexo"))
            {
                return tipo.Namespace;
            }

            if (!tipo.IsEnum && ReflexaoUtil.TipoRetornaTipoPrimario(ReflexaoUtil.RetornarTipoSemNullable(tipo)))
            {
                return String.Empty;
            }
            else if (ReflexaoUtil.TipoRetornaColecao(tipo))
            {
                return String.Empty;

            }
            else
            {
                if (tipo.Namespace.StartsWith("System"))
                {
                    if (tipo == typeof(object) ||
                        tipo == typeof(System.IO.FileInfo) ||
                        tipo == typeof(Type) ||
                        tipo == typeof(PropertyInfo) ||
                        tipo.Name == "Void")
                    {
                        return String.Empty;
                    }

                    if (TipoUtil.TipoIgualOuSubTipo(tipo, typeof(ValidationAttribute)) ||
                        TipoUtil.TipoIgualOuSubTipo(tipo, typeof(ForeignKeyAttribute)) ||
                        TipoUtil.TipoIgualOuSubTipo(tipo, typeof(ScaffoldColumnAttribute)))
                    {
                        return "Snebur.Dominio.Atributos";
                    }
                    else
                    {
                        throw new NotImplementedException(tipo.Namespace);
                    }
                }
                return tipo.Namespace;
            }
        }

        public static string RetornarCaminhoTipoTS(Type tipo, bool isPromise)
        {
            var caminhoTipo = RetornarCaminhoTipoTS(tipo);
            if (isPromise)
            {
                return $"Promise<{caminhoTipo}>";
            }
            return caminhoTipo;
        }
        public static string RetornarCaminhoTipoTS(Type tipo)
        {
            var _namespace = TipoUtil.RetornarNameSpace(tipo);
            var nomeTipo = TipoUtil.RetornarNomeTipoTS(tipo);
            if (!String.IsNullOrEmpty(_namespace))
            {
                return String.Format("{0}.{1}", _namespace, nomeTipo);
            }
            else
            {
                return nomeTipo;
            }
        }

        public static object RetornarNomeAtributo(Type tipoAtributo)
        {
            if (tipoAtributo.Name.EndsWith("Attribute"))
            {
                var posicao = tipoAtributo.Name.LastIndexOf("Attribute");
                return tipoAtributo.Name.Substring(0, posicao);
            }
            return tipoAtributo.Name;
        }

        public static string RetornarNomeTipoPrimarioTypeScript(Type tipo)
        {
            var tipoSemNullable = ReflexaoUtil.RetornarTipoSemNullable(tipo);
            var tipoPrimarioEnum = ReflexaoUtil.RetornarTipoPrimarioEnum(tipoSemNullable);

            switch (tipoPrimarioEnum)
            {

                case EnumTipoPrimario.Boolean:
                    return "boolean";
                case EnumTipoPrimario.DateTime:
                    return "Date";
                case EnumTipoPrimario.Decimal:
                case EnumTipoPrimario.Double:
                case EnumTipoPrimario.Integer:
                case EnumTipoPrimario.Long:
                case EnumTipoPrimario.Byte:
                    return "number";
                case EnumTipoPrimario.String:
                case EnumTipoPrimario.Guid:
                    return "string";
                case EnumTipoPrimario.TimeSpan:
                    return "TimeSpan";
                case EnumTipoPrimario.Object:
                    return "any";
                default:
                    throw new NotSupportedException(String.Format("Tipo primário não suportado {0} ", tipoPrimarioEnum.ToString()));
            }


        }

        private static string RetornarNomeTipoPrimarioTypeScriptInterno(Type tipo)
        {
            var tipoSemNullable = ReflexaoUtil.RetornarTipoSemNullable(tipo);
            var tipoPrimarioEnum = ReflexaoUtil.RetornarTipoPrimarioEnum(tipoSemNullable);

            switch (tipoPrimarioEnum)
            {

                case EnumTipoPrimario.Boolean:
                    return "boolean";
                case EnumTipoPrimario.DateTime:
                    return "Date";
                case EnumTipoPrimario.Decimal:
                case EnumTipoPrimario.Double:
                case EnumTipoPrimario.Integer:
                case EnumTipoPrimario.Long:
                    return "number";
                case EnumTipoPrimario.String:
                case EnumTipoPrimario.Guid:
                    return "string";
                case EnumTipoPrimario.TimeSpan:
                    return "TimeSpan";
                case EnumTipoPrimario.Object:
                    return "any";
                default:
                    throw new NotSupportedException(String.Format("Tipo primário não suportado {0} ", tipoPrimarioEnum.ToString()));
            }


        }

    }
}
