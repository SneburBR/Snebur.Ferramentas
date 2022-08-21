using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.VisualStudio.Utilidade;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Snebur.Reflexao;
using Snebur.Utilidade;
using System.ComponentModel.DataAnnotations.Schema;

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
            return tipo1 != null && tipo2 != null && (tipo1.Equals(tipo2) || (tipo1.Name == tipo2.Name && tipo1.Namespace == tipo2.Namespace));
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



        public static void RemoverTipo(List<Type> tipos, Type tipo)
        {
            var tiposRemover = tipos.Where(x => (TipoUtil.TipoIgual(x, tipo))).ToList();

            foreach (var tipoRemover in tiposRemover)
            {
                tipos.Remove(tipoRemover);
            }
        }

        #endregion

        public static string RetornarNomeTipo(Type tipo)
        {
            if (!tipo.IsEnum && ReflexaoUtil.TipoRetornaTipoPrimario(ReflexaoUtil.RetornarTipoSemNullable(tipo)))
            {

                return TipoUtil.RetornarNomeTipoPrimario(tipo);
            }
            else if (ReflexaoUtil.TipoRetornaColecao(tipo) && !tipo.IsInterface)
            {

                var itemTipoLista = ReflexaoUtil.RetornarTipoGenericoColecao(tipo);
                if (tipo.IsGenericType)
                {
                    var tipoDefinicacao = tipo.GetGenericTypeDefinition();
                    if (tipoDefinicacao.Name.Contains("ListaEntidades"))
                    {
                        return String.Format("ListaEntidades<{0}>", RetornarCaminhoTipo(itemTipoLista));
                    }
                    else if (tipoDefinicacao == typeof(List<>))
                    {
                        return String.Format("Lista<{0}>", RetornarCaminhoTipo(itemTipoLista));
                    }
                    else if (tipoDefinicacao == typeof(Dictionary<,>))
                    {
                        var tipoDaChave = tipo.GetGenericArguments().First();
                        if (tipoDaChave != typeof(string))
                        {
                            throw new ErroNaoSuportado("O tipo da chave não é suportado");
                        }
                        itemTipoLista = tipo.GetGenericArguments().Last();
                        return String.Format("Dicionario<{0}>", TipoUtil.RetornarCaminhoTipo(itemTipoLista));
                    }
                    else
                    {
                        throw new ErroNaoSuportado("O tipo da coleção  não é suportado");
                    }
                }
                else
                {
                    return String.Format("Array<{0}>", TipoUtil.RetornarCaminhoTipo(itemTipoLista));
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

                    if (TipoUtil.TipoIgualOuSubTipo(tipo, typeof(ValidationAttribute)))
                    {
                        return AjudanteAssembly.NomeTipoBaseAtributoValidacao;
                    }
                    else if (TipoUtil.TipoIgualOuSubTipo(tipo, typeof(ForeignKeyAttribute)) ||
                             TipoUtil.TipoIgualOuSubTipo(tipo, typeof(ScaffoldColumnAttribute)))
                    {
                        return AjudanteAssembly.NomeTipoBaseAtributoDominio ;
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
            return tipos.Where(x => !x.GetCustomAttributes().Any(k => k.GetType().Name == nomeAtributo)).ToList();
        }

        internal static Boolean TipoPossuiAtributo(Type tipo, string nomeAtributo)
        {
            return tipo.GetCustomAttributes().Any(k => k.GetType().Name == nomeAtributo);
        }

        internal static Boolean TipoImplementaInterface(Type tipo, Type tipoInterface)
        {
            return ReflexaoUtil.TipoImplementaInterface(tipo, tipoInterface, false);
        }

        public static string RetornarNameSpace(Type tipo)
        {
            if (!tipo.IsEnum && ReflexaoUtil.TipoRetornaTipoPrimario(ReflexaoUtil.RetornarTipoSemNullable(tipo)))
            {
                return string.Empty;
            }
            else if (ReflexaoUtil.TipoRetornaColecao(tipo))
            {
                return String.Empty;

            }
            else
            {
                if (tipo.Namespace.StartsWith("System"))
                {
                    if (tipo == typeof(object))
                    {
                        return string.Empty;
                    }

                    if (tipo.Name == "Void")
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

        public static string RetornarCaminhoTipo(Type tipo)
        {
            var _namespace = TipoUtil.RetornarNameSpace(tipo);
            var nomeTipo = TipoUtil.RetornarNomeTipo(tipo);
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

        public static string RetornarNomeTipoPrimario(Type tipo)
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
