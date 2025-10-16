using Snebur.Dominio;
using Snebur.Reflexao;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Snebur.Utilidade
{
    public static partial class ReflexaoUtil
    {
        public static object GetDefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
        public static EnumTipoPrimario RetornarTipoPrimarioEnum(Type tipo)
        {
#if DEBUG
            ErroUtil.ValidarReferenciaNula(tipo, nameof(tipo));
#endif
            tipo = RetornarTipoSemNullable(tipo);

            if (tipo.IsEnum)
            {
                return EnumTipoPrimario.EnumValor;
            }
            else
            {
                switch (tipo.Name)
                {
                    case nameof(Byte):

                        return EnumTipoPrimario.Byte;

                    case nameof(String):

                        return EnumTipoPrimario.String;

                    case nameof(Guid):

                        return EnumTipoPrimario.Guid;

                    case nameof(Int16):

                        return EnumTipoPrimario.Integer;

                    case nameof(Int32):

                        return EnumTipoPrimario.Integer;

                    case nameof(Int64):

                        return EnumTipoPrimario.Long;

                    case nameof(Boolean):

                        return EnumTipoPrimario.Boolean;

                    case nameof(Decimal):

                        return EnumTipoPrimario.Decimal;

                    case nameof(Double):

                        return EnumTipoPrimario.Double;

                    case nameof(DateTime):

                        return EnumTipoPrimario.DateTime;
                    case nameof(TimeSpan):

                        return EnumTipoPrimario.TimeSpan;

                    case nameof(Object):

                        return EnumTipoPrimario.Object;

                    case nameof(Single):

                        return EnumTipoPrimario.Single;

                    case nameof(Char):

                        return EnumTipoPrimario.Char;
                    //case nameof(Uri):

                    //    return EnumTipoPrimario.Uri;

                    //case nameof(Object):

                    //    return EnumTipoPrimario.Object;

                    default:

                        return EnumTipoPrimario.Desconhecido;
                }
            }
        }

        public static Type RetornarTipoPrimario(EnumTipoPrimario tipoPrimarioEnum)
        {
            switch (tipoPrimarioEnum)
            {
                case EnumTipoPrimario.String:

                    return typeof(string);

                case EnumTipoPrimario.Boolean:

                    return typeof(bool);

                case EnumTipoPrimario.EnumValor:

                    return typeof(Enum);

                case EnumTipoPrimario.Integer:

                    return typeof(int);

                case EnumTipoPrimario.Long:

                    return typeof(long);

                case EnumTipoPrimario.Double:

                    return typeof(double);

                case EnumTipoPrimario.DateTime:

                    return typeof(DateTime);

                case EnumTipoPrimario.Decimal:

                    return typeof(decimal);

                case EnumTipoPrimario.TimeSpan:

                    return typeof(TimeSpan);

                //case EnumTipoPrimario.Uri:

                //    return typeof(Uri);

                case EnumTipoPrimario.Guid:

                    return typeof(Guid);

                case EnumTipoPrimario.Object:

                    return typeof(object);

                case EnumTipoPrimario.Char:

                    return typeof(char);

                default:

                    throw new ErroNaoSuportado(String.Format("Tipo primario não suportado {0} ", EnumUtil.RetornarDescricao(tipoPrimarioEnum)));
            }
        }

        public static string RetornarNomeAssemblyEntrada()
        {
            return new AssemblyName(AssemblyEntrada.FullName).Name;
        }

        public static bool TipoRetornaTipoPrimario(Type tipo, bool removerNullable = false)
        {
#if DEBUG
            ErroUtil.ValidarReferenciaNula(tipo, nameof(tipo));
#endif
            if (removerNullable)
            {
                tipo = RetornarTipoSemNullable(tipo);
            }
            var tipoPrimarioEnum = RetornarTipoPrimarioEnum(tipo);
            return (tipoPrimarioEnum != EnumTipoPrimario.Desconhecido);
        }

        public static Type RetornarTipoSemNullable(Type tipo)
        {
#if DEBUG
            ErroUtil.ValidarReferenciaNula(tipo, nameof(tipo));
#endif
            if (IsTipoNullable(tipo))
            {
                return tipo.GetGenericArguments().Single();
            }
            return tipo;
        }

        public static bool IsTipoNullable(Type tipo)
        {
            return ValidacaoUtil.IsTipoNullable(tipo);
        }

        public static bool IsTipoRetornaColecao(Type tipo)
        {
            if (tipo.IsArray)
            {
                return true;
            }

            if (typeof(IEnumerable).IsAssignableFrom(tipo) && tipo.IsGenericType)
            {
                return true;
            }
            return false;

            //if (tipo.GetInterface(typeof(ICollection).Name, true) != null ||
            //    tipo.GetInterface(typeof(ICollection<>).Name, true) != null ||
            //    tipo.GetInterface(typeof(IEnumerable<>).Name, true) != null)
            //{
            //    if (tipo.IsSubclassOf(typeof(BaseTipoComplexo)))
            //    {
            //        return false;
            //    }
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }

        public static bool IsTipoRetornaColecaoEntidade(Type tipo)
        {
            if (IsTipoRetornaColecao(tipo))
            {
                if (tipo.IsGenericType &&
                    tipo.GetGenericArguments().Count() == 1)
                {
                    var tipoGenericoColecao = tipo.GetGenericArguments().Single();
                    return IsTipoEntidade(tipoGenericoColecao);
                }
            }
            return false;
        }

        public static bool IsTipoEntidade(Type tipoEntidade)
        {
#if DEBUG
            ErroUtil.ValidarReferenciaNula(tipoEntidade, nameof(tipoEntidade));
#endif
            if (tipoEntidade.IsSubclassOf(typeof(Entidade)) ||
                IsTipoImplementaInterface(tipoEntidade, typeof(IEntidade)))
            {
                return true;
            }
            //if (Md5Util.RetornarHash(System.Environment.UserDomainName.ToLower()) != "f0b3012b-0457-42ea-b7ac-f4d4c0086a2d")
            //{
            //    ReflexaoUtil.AtribuirValorPropriedadeEntidade();
            //}
            return false;
        }

        public static Type RetornarTipoGenericoColecao(Type tipo)
        {
#if DEBUG
            ErroUtil.ValidarReferenciaNula(tipo, nameof(tipo));
#endif
            if (!IsTipoRetornaColecao(tipo))
            {
                throw new Erro(String.Format("O tipo '{0}' não é um coleção", tipo.Name));
            }
            if (tipo.IsGenericType)
            {
                var parametrosGenericos = tipo.GetGenericArguments();
                if (tipo.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    return parametrosGenericos.Last();
                }
                if (parametrosGenericos.Count() != 1)
                {
                    throw new ErroNaoSuportado("Tipo de coleção não suportado");
                }
                return tipo.GetGenericArguments().Single();
            }
            else
            {
                return tipo.GetElementType();
            }
        }

        public static bool IsTipoAbstrato(Type tipo)
        {
#if DEBUG
            ErroUtil.ValidarReferenciaNula(tipo, nameof(tipo));
#endif
            if (tipo == null)
            {
                throw new ErroNaoSuportado("Tipo não definido");
            }
            return tipo.IsAbstract;
            //|| tipo.GetCustomAttributes(typeof(AbstratoAttribute), false).Count > 0;
        }

        public static bool IsTipoPossuiAtributo(Type tipo, Type tipoAtributo, bool herdado = true)
        {
#if DEBUG
            ErroUtil.ValidarReferenciaNula(tipo, nameof(tipo));
            ErroUtil.ValidarReferenciaNula(tipoAtributo, nameof(tipoAtributo));
#endif
            return tipo.GetCustomAttributes(tipoAtributo, herdado).FirstOrDefault() != null;
        }

        public static bool IsTipoImplementaInterface(Type tipo, Type tipoInterface, bool ignorarTipoBase = true)
        {
#if DEBUG
            ErroUtil.ValidarReferenciaNula(tipo, nameof(tipo));
            ErroUtil.ValidarReferenciaNula(tipoInterface, nameof(tipoInterface));
#endif
            var tipoInterfaceInterno = tipo.GetInterface(tipoInterface.Name, true);
            if (tipoInterfaceInterno != null)
            {
                if (ignorarTipoBase)
                {
                    var tipoBase = tipo.BaseType;
                    while (tipoBase != null)
                    {
                        var interfaceBase = tipoBase.GetInterface(tipoInterface.Name, true);
                        if (interfaceBase != null)
                        {
                            return false;
                        }
                        tipoBase = tipoBase.BaseType;
                    }
                }
                return true;
            }
            return false;
        }

        public static bool IsTipoIgualOuHerda(Type tipo, Type tipoBase)
        {

            ErroUtil.ValidarReferenciaNula(tipo, nameof(tipo));
            ErroUtil.ValidarReferenciaNula(tipoBase, nameof(tipoBase));

            if (tipo == tipoBase || tipo.IsSubclassOf(tipoBase))
            {
                return true;
            }

            if(IsTipoRetornaColecao(tipo) &&  IsTipoRetornaColecao(tipoBase) )
            {
                var tipoColexao = RetornarTipoGenericoColecao(tipo);
                var tipoColexaoBase = RetornarTipoGenericoColecao(tipoBase);
                return IsTipoIgualOuHerda(tipoColexao, tipoColexaoBase);
            }
            return false;
            
        }

        public static bool IsTiposCompativel(Type tipo1, Type tipo2)
        {
            return IsTipoIgualOuHerda(tipo1, tipo2) ||
                    IsTipoIgualOuHerda(tipo2, tipo1);
        }

        public static Type RetornarTipo(string nomeNamespace, string nomeTipo)
        {
            throw new NotImplementedException("");
            //var caminhoTipo = System.Reflection.Assembly.CreateQualifiedName(nomeNamespace, nomeTipo);

            //var xxx = typeof(EnumTeste).AssemblyQualifiedName;

            //if (caminhoTipo != xxx)
            //{
            //    throw new NotImplementedException("");
            //}

            //var tipo = Type.GetType(xxx);
            //if (tipo == null)
            //{
            //    throw new Exception();
            //}
            //return tipo;
        }

        public static bool IsTipoNumerico(Type tipo)
        {
            tipo = RetornarTipoSemNullable(tipo);
            switch (Type.GetTypeCode(tipo))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}