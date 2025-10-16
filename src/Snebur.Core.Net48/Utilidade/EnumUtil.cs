using Snebur.Dominio.Atributos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Snebur.Utilidade
{
    public static class EnumUtil
    {
        public static bool IsFlagsEnumDefinida<TEnum>(Enum soma)
        {
            return IsFlagsEnumDefinida(typeof(TEnum), soma);
        }

        public static bool IsFlagsEnumDefinida(Type tipoEnum, Enum soma)
        {
            var flags = RetornarFlags(tipoEnum, soma);
            return (flags.Sum(x => Convert.ToInt32(x)) == Convert.ToInt32(soma)) &&
                   flags.All(x => Enum.IsDefined(tipoEnum, x));
        }

        public static TEnum[] RetornarValoresEnum<TEnum>() where TEnum : Enum
        {
            return RetornarValoresEnum(typeof(TEnum)).Cast<TEnum>().ToArray();
        }

        public static TEnum[] RetornarFlags<TEnum>(Enum soma)
        {
            return RetornarFlags(typeof(TEnum), soma).Cast<TEnum>().ToArray();
        }

        public static Enum[] RetornarFlags(Type tipoEnum, Enum soma)
        {
            if (!tipoEnum.IsEnum)
            {
                throw new Erro($"O tipo {tipoEnum.Name} não é um {nameof(Enum)}");
            }
            var enums = new List<Enum>();
            var flags = RetornarValoresEnum(tipoEnum);

            foreach (var valorEnum in flags)
            {
                if (soma.HasFlag(valorEnum))
                {
                    enums.Add(valorEnum);
                }
            }
            return enums.ToArray();
        }

        public static Enum[] RetornarValoresEnum(Type tipoEnum)
        {
            ErroUtil.ValidarReferenciaNula(tipoEnum, nameof(tipoEnum));
            if (!tipoEnum.IsEnum)
            {
                throw new Erro(String.Format("O tipo {0} não suportado, esperado Enum", tipoEnum.Name));
            }
            return tipoEnum.GetFields().Where(x => x.IsLiteral).Select(x => (Enum)x.GetValue(null)).ToArray();
        }

        public static string RetornarDescricao(Enum valor)
        {
            ErroUtil.ValidarReferenciaNula(valor, nameof(valor));

            var fi = valor.GetType().GetField(valor.ToString());
            if (fi != null)
            {
                var rotuloAtributo = fi.GetCustomAttribute<RotuloAttribute>(true);
                if (rotuloAtributo != null)
                {
                    return rotuloAtributo.Rotulo;
                }

                var atributoDescricao = fi.GetCustomAttribute<DescriptionAttribute>(true);
                if (atributoDescricao != null)
                {
                    return atributoDescricao.Description;
                }
                return fi.Name;
            }
            return valor.ToString();
        }

        public static TEnum RetornarValorDaDescricao<TEnum>(string descricao)
        {
            var tipoEnum = typeof(TEnum);
            if (!tipoEnum.IsEnum)
            {
                throw new ArgumentException($"O tipo {tipoEnum.Name} não é um enum");
            }
            return (TEnum)(RetornarValorDaDescricao(tipoEnum, descricao) as object);
        }

        public static Enum RetornarValorDaDescricao(Type tipoEnum, string descricao)
        {
            var campos = tipoEnum.GetFields();
            var camposDescricao = campos.SelectMany(f => f.GetCustomAttributes<DescriptionAttribute>(false),
                                        (campo, atributo) => new
                                        {
                                            Campo = campo,
                                            AtriburosDescricao = atributo
                                        });

            var campoDescricao = camposDescricao.Where(x => x.AtriburosDescricao.Description == descricao).
                                                 Select(x => x.Campo).SingleOrDefault();

            return campoDescricao == null ? default : (Enum)campoDescricao.GetRawConstantValue();
        }

        public static string RetornarNome(Enum valor)
        {
            ErroUtil.ValidarReferenciaNula(valor, nameof(valor));

            var nome = Enum.GetName(valor.GetType(), valor);
            if (!String.IsNullOrEmpty(nome))
            {
                return nome;
            }
            return valor.ToString();
        }

        public static Enum RetornarValorEnum(Type tipoEnum,
                                            int valorEnum)
        {
            if (!tipoEnum.IsEnum)
            {
                throw new Erro($"O tipo {tipoEnum.Name} não é enum ");
            }
            return (Enum)Enum.ToObject(tipoEnum, valorEnum);
        }

        public static TEnum RetornarValorEnum<TEnum>(string descricao,
                                                      bool isValidarEnumDefino = false)
        {
            var valor = RetornarValorEnum(typeof(TEnum), descricao);
            if (isValidarEnumDefino)
            {
                if (Enum.IsDefined(typeof(TEnum), valor))
                {
                    throw new Erro($"O valor {descricao} não está defino no enum {typeof(TEnum).Name}");
                }
            }
            return (TEnum)(valor as object);
        }

        public static TEnum RetornarValorEnum<TEnum>(int valorInt,
                                                     bool isValidarEnumDefino = false)
        {
            var valor = RetornarValorEnum(typeof(TEnum), valorInt);
            if (isValidarEnumDefino)
            {
                if (!Enum.IsDefined(typeof(TEnum), valor))
                {
                    throw new Erro($"O valor {valorInt} não está defino no enum {typeof(TEnum).Name}");
                }
            }
            return (TEnum)(valor as object);
        }

        public static Enum RetornarValorEnum(Type tipoEnum, string descricao)
        {
            if (!tipoEnum.IsEnum)
            {
                throw new Erro($"O tipo {tipoEnum.Name} não é enum ");
            }
            return (Enum)Enum.Parse(tipoEnum, descricao);
        }

        public static List<string> RetornarNomesEnum(Type tipoEnum)
        {
            return Enum.GetNames(tipoEnum).ToList();
        }

        public static bool PossuiFlag<TEnum>(Enum attributes, TEnum flag) where TEnum : Enum
        {
            var flags = RetornarFlags<TEnum>((Enum)attributes);
            return flags.Contains(flag);
        }
    }
}