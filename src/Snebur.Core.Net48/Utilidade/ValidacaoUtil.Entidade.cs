using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Publicacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Snebur.Utilidade
{
    public static partial class ValidacaoUtil
    {
        public static bool IsValidacaoRequerido(PropertyInfo propriedade,
                                                object valorPropriedade,
                                                object paiPropriedade = null)
        {
            //var valorString = Convert.ToString(valorPropriedade);
            //if (String.IsNullOrWhiteSpace(valorString))
            if (!IsPropriedadeRequerida(propriedade))
            {
                return true;
            }
       
            if (propriedade.PropertyType.IsIdType() &&
                paiPropriedade is Entidade entidade )
            {
                var propriedadeChaveEstrangeira = EntidadeUtil.RetornarPropriedadeRelacaoPai(entidade.GetType(),
                                                                                             propriedade);

                if(propriedadeChaveEstrangeira != null)
                {
                    if ((long)valorPropriedade  > 0 )
                    {
                        return true;
                    }

                    var valorRelacaoPai = propriedadeChaveEstrangeira.GetValue(entidade);
                    if(valorRelacaoPai is Entidade)
                    {
                        return true;
                    }

                    return false;
                }

            }

            if (ReflexaoUtil.IsTipoIgualOuHerda(propriedade.PropertyType, typeof(Entidade)))
            {
                if (valorPropriedade is Entidade)
                {
                    return true;
                }

                if (paiPropriedade is Entidade baseEntidade)
                {
                    var idChaveEstrangeira = EntidadeUtil.RetornarValorIdChaveEstrangeira(baseEntidade, propriedade);
                    if (idChaveEstrangeira > 0)
                    {
                        return true;
                    }
                }
            }
           
            if (valorPropriedade == null)
            {
                return false;
            }
            if (valorPropriedade is BaseTipoComplexo)
            {
                return IsValidacaoRequeridoTipoComplexo(valorPropriedade as BaseTipoComplexo);
            }
            return IsValorPropriedadeRequerido(propriedade, valorPropriedade);
        }

        public static bool IsJson(string json)
        {
            if (!String.IsNullOrWhiteSpace(json))
            {
                json = json.Trim();
                return (json.StartsWith("{") && json.EndsWith("}")) ||
                       (json.StartsWith("[") && json.EndsWith("]"));
            }
            return false;
        }

        internal static bool ValidarPalavraTamanho(string texto,
                                                   int tamanhoMinimo,
                                                   int tamanhoMaximo)
        {
            if (!String.IsNullOrWhiteSpace(texto))
            {
                var reg = new Regex(@"\s+");
                var partes = reg.Split(texto);

                if (partes.Any(x => !ValidarTextoTamanho(x, tamanhoMinimo, tamanhoMaximo)))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool ValidarTextoTamanho(string texto, int tamanhoMinimo, int tamanhoMaximo)
        {
            if (tamanhoMinimo > 0 && texto.Length <= tamanhoMinimo)
            {
                return false;
            }
            if (tamanhoMaximo > 0 && texto.Length >= tamanhoMaximo)
            {
                return false;
            }
            return true;
        }

        public static bool IsUrlLocalhost(Uri url)
        {
            return IsLocalhost(url.Host);
        }

        public static bool IsLocalhost(string host)
        {
            return NormalizacaoUtil.NormlizarHost(host) == ConstantesPublicacao.LOCALHOST;
        }

        public static bool IsUrl(string caminho)
        {
            if (Uri.TryCreate(caminho, UriKind.Absolute, out var uri))
            {
                return uri.Scheme == Uri.UriSchemeHttp ||
                       uri.Scheme == Uri.UriSchemeHttps;
            }
            return false;
        }
        public static bool IsHostOuIp(string servidor)
        {
            return IsDominioDns(servidor) ||
                   IsIp(servidor);
        }

        public static bool IsDefinido(object valorPropriedade)
        {
            if (valorPropriedade != null)
            {
                if (valorPropriedade is string str)
                {
                    return !String.IsNullOrWhiteSpace(str);
                }
                return true;
            }
            return false;
        }

        public static bool IsPropriedadeRequerida(PropertyInfo propriedade)
        {
            //return !propriedade.PropertyType.IsValueType || propriedade.GetCustomAttribute<ValidacaoRequeridoAttribute>() != null;
            var atributo = propriedade.GetCustomAttribute<ValidacaoRequeridoAttribute>();
            if(atributo!= null)
            {
                return (atributo.OpcoesComparacaoAuxiliar == null ||
                        atributo.OpcoesComparacaoAuxiliar == EnumOpcoesComparacaoAuxiliar.Nenhuma);
                 
            }
            return false;
        }
        private static bool IsIntervaloValido(double valor, double inicio, double fim)
        {
            return (valor >= inicio && valor <= fim);
        }

        public static List<IAtributoValidacao> RetornarAtributosValidacao(PropertyInfo propriedade)
        {
            var atributos = propriedade.GetCustomAttributes();
            var tipoIAtributoValidacao = typeof(IAtributoValidacao);
            var atributosValidacao = atributos.Where(x => ReflexaoUtil.IsTipoImplementaInterface(x.GetType(), tipoIAtributoValidacao, false));
            return atributosValidacao.Cast<IAtributoValidacao>().ToList();
        }

        public static List<BaseValidacaoEntidadeAttribute> RetornarAtributosValidacaoEntidade(Type tipoEntidade)
        {
            var atributos = tipoEntidade.GetCustomAttributes(typeof(BaseValidacaoEntidadeAttribute), true);
            return atributos.OfType<BaseValidacaoEntidadeAttribute>().ToList();
        }

        private static bool IsValorPropriedadeRequerido(PropertyInfo propriedade, object valorPropriedade)
        {
            if (valorPropriedade == null)
            {
                return false;
            }
            switch (valorPropriedade)
            {
                case bool _:
                    return true;
                case char ch:
                    return ch != '\0';
                case string str:
                    return !String.IsNullOrWhiteSpace(str);
                case int int32:
                    return int32 > 0;
                case long int64:
                    return int64 > 0;
                case decimal _decimal:
                    return _decimal > 0;
                case double _double:
                    return _double > 0;
                case float _single:
                    return _single > 0;
                case short int16:
                    return int16 > 0;
                case byte _byte:
                    return _byte > 0;
                case Guid guid:
                    return guid != Guid.Empty;
                case DateTime data:
                    return IsDataValida(data);
                case TimeSpan _:
                    return true;

                case Enum _:

                    return Enum.IsDefined(ReflexaoUtil.RetornarTipoSemNullable(propriedade.PropertyType),
                                         (int)valorPropriedade);
                default:
                    break;
            }
            var tipoPropriedade = ReflexaoUtil.RetornarTipoSemNullable(propriedade.PropertyType);
            if (tipoPropriedade.IsEnum)
            {
                return Enum.IsDefined(tipoPropriedade, (int)valorPropriedade);
            }
            throw new ErroNaoSuportado("O tipo de propriedade não é suportado");
        }

        private static bool IsDataValida(DateTime data)
        {
            return data.Year >= 1900 && data.Year < 2100;
        }

        private static bool IsValidacaoRequeridoTipoComplexo(BaseTipoComplexo valor)
        {
            var tipo = valor.GetType();
            var propriedades = ReflexaoUtil.RetornarPropriedadesEntidade(valor.GetType());
            foreach (var propriedade in propriedades)
            {
                var valorPropriedade = propriedade.GetValue(valor);
                if (!IsValorPropriedadeRequerido(propriedade, valorPropriedade))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsTipoNullable(Type tipo)
        {
#if DEBUG
            ErroUtil.ValidarReferenciaNula(tipo, nameof(tipo));
#endif
            return Nullable.GetUnderlyingType(tipo) != null;
            //return (tipo.IsGenericType && tipo.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static bool IsDataNascimentoValida(DateTime dataNascimento)
        {
            var inico = DateTime.Now.AddYears(-120);
            return dataNascimento > inico && dataNascimento < DateTime.Now;
        }

        public static void ValidarProgresso(double progresso)
        {
            if (!IsProgressoValido(progresso))
            {
                throw new Erro($"A valor progresso {progresso.ToString()} não é valido");
            }
        }

        private static bool IsProgressoValido(double progresso)
        {
            return !Double.IsNaN(progresso) &&
                    progresso >= 0 &&
                    progresso <= 100;
        }

    }
}