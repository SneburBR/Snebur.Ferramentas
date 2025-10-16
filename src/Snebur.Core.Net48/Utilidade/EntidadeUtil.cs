using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Snebur.Utilidade
{
    public class EntidadeUtil
    {
        private static PropertyInfo _propriedadeNomeTipoEntidade;
        private static List<Type> _tiposInterfacesEntidade;
        public static PropertyInfo PropriedadeNomeTipoEntidade
        {
            get
            {
                if (_propriedadeNomeTipoEntidade == null)
                {
                    _propriedadeNomeTipoEntidade = ReflexaoUtil.RetornarPropriedade<Entidade>(x => x.__NomeTipoEntidade);
                }
                return _propriedadeNomeTipoEntidade;
            }
        }

        public static List<Type> TiposInterfaceEntidade
        {
            get
            {
                if (_tiposInterfacesEntidade == null)
                {
                    _tiposInterfacesEntidade = typeof(EntidadeUtil).Assembly.GetAccessibleTypes().Where(x => x.IsInterface && x.GetInterface(typeof(IEntidade).Name) != null).ToList();
                }
                return _tiposInterfacesEntidade;
            }
        }

        public static FieldInfo RetornarCampoPrivadoChaveEstrangeira(Type tipoEntidade,
                                                                     PropertyInfo propriedade)
        {
            var propriedadeChaveEstrangeira = RetornarPropriedadeChaveEstrangeira(tipoEntidade, propriedade, true);
            if (propriedadeChaveEstrangeira == null)
            {
                return null;
            }
            var filedName = $"_{TextoUtil.RetornarPrimeiraLetraMinusculo(propriedadeChaveEstrangeira.Name)}";
            var fi = tipoEntidade.GetField(filedName, BindingFlags.NonPublic | BindingFlags.Instance);
            return fi;
        }

        public static PropertyInfo RetornarPropriedadeChaveEstrangeiraRelacaoFilhos(Type tipoEntidade,
                                                                                    PropertyInfo propriedadeRelacoesFilhos)
        {
            var atributoRelacaoFilhos = propriedadeRelacoesFilhos.GetCustomAttribute<RelacaoFilhosAttribute>();
            if (atributoRelacaoFilhos == null)
            {
                throw new Erro($"Não foi encontrado o atributo RelacaoFilhosAttribute para a propriedade {propriedadeRelacoesFilhos.Name} em {propriedadeRelacoesFilhos.DeclaringType.Name} ");
            }

            if (!propriedadeRelacoesFilhos.PropertyType.IsGenericType ||
                 propriedadeRelacoesFilhos.PropertyType.GetGenericArguments().Count() != 1)
            {
                throw new Erro($"O da propriedade {propriedadeRelacoesFilhos.Name} em {propriedadeRelacoesFilhos.DeclaringType.Name} não é uma coleção de entidades");
            }

            var tipoEntidadeRelacao = propriedadeRelacoesFilhos.PropertyType.GetGenericArguments().Single();
            if (!String.IsNullOrWhiteSpace(atributoRelacaoFilhos.NomePropriedadeChaveEstrangeira))
            {
                var proriedade = tipoEntidadeRelacao.GetProperty(atributoRelacaoFilhos.NomePropriedadeChaveEstrangeira);
                if (proriedade == null)
                {
                    throw new Erro($"Não foi encontrado a propriedade {atributoRelacaoFilhos.NomePropriedadeChaveEstrangeira} em {tipoEntidade.Name} ");
                }
                return proriedade;
            }

            var propriedadesRelacaoPai = tipoEntidadeRelacao.GetProperties().
                                                               Where(x => x.PropertyType == tipoEntidade || x.PropertyType.IsSubclassOf(tipoEntidade)).
                                                               ToList();

            if (propriedadesRelacaoPai.Count == 1)
            {
                var propriedadeRelacaoPai = propriedadesRelacaoPai[0];
                return RetornarPropriedadeChaveEstrangeira(propriedadeRelacaoPai.DeclaringType,
                                                           propriedadeRelacaoPai);
            }
            if (propriedadesRelacaoPai.Count == 0)
            {
                throw new Erro($"Não foi encontrado uma propriedade do tipo {tipoEntidade.Name} em {tipoEntidadeRelacao.Name} ");
            }
            throw new Erro($"Foi encontrado mais de uma propriedade do tipo {tipoEntidade.Name} em {tipoEntidadeRelacao.Name} ");
        }

        public static PropertyInfo RetornarPropriedadeChaveEstrangeira(Type tipoEntidade,
                                                                       PropertyInfo propriedade,
                                                                       bool isIgnorarErro = false)
        {
            var atributoChaveEstrangeira = propriedade.RetornarAtributoChaveEstrangeira(isIgnorarErro);
            if (atributoChaveEstrangeira == null)
            {
                if (isIgnorarErro)
                {
                    return null;
                }
                throw new ErroNaoDefinido(String.Format("O atributo chave estrangeira não foi definido na propriedade {0}", propriedade.Name));
            }
            var nomePropriedade = atributoChaveEstrangeira.Name;
            var propriedadeChaveEstrangeira = tipoEntidade.GetProperties().Where(x => x.Name == nomePropriedade).SingleOrDefault();
            if (propriedadeChaveEstrangeira == null)
            {
                if (isIgnorarErro)
                {
                    return null;
                }
                throw new ErroNaoDefinido(String.Format("Não foi encontrada a propriedade chave estrangeira {0} para a propriedade {1} em {2} ", propriedadeChaveEstrangeira, propriedade.Name, tipoEntidade.Name));
            }
            return propriedadeChaveEstrangeira;
        }

        public static PropertyInfo RetornarPropriedadeRelacaoPai(Type tipoEntidade,
                                                                 PropertyInfo propriedadeChaveEstrangeira)
        {
            return ReflexaoUtil.RetornarPropriedades(tipoEntidade, false)
                        .Where(propriedade => IsPropriedadeRelacaoPai(propriedade, propriedadeChaveEstrangeira.Name))
                        .FirstOrDefault();
        }

        private static bool IsPropriedadeRelacaoPai(PropertyInfo propriedade,
                                                    string nomePropriedadeChaveEstrangeira)
        {
            var atributoChaveEstrangeira = propriedade.RetornarAtributoChaveEstrangeira(true);
            if (atributoChaveEstrangeira != null)
            {
                return atributoChaveEstrangeira.NomePropriedade == nomePropriedadeChaveEstrangeira;
            }
            return false;
        }

        public static PropertyInfo RetornarPropriedadeChavaPrimaria(Type tipoEntidade)
        {
            var propriedade = tipoEntidade.GetProperties().Where(x => x.GetCustomAttribute<KeyAttribute>() != null).SingleOrDefault();
            if (propriedade == null)
            {
                throw new Erro($"A propriedade chave primaria não foi encontrada para o tipo {tipoEntidade.Name}");
            }
            return propriedade;
        }

        public static long? RetornarValorIdChaveEstrangeira(Entidade entidade,
                                                           PropertyInfo propriedade,
                                                           bool isPreferirCampoPrivado = false)
        {
            if (isPreferirCampoPrivado)
            {
                var campo = RetornarCampoPrivadoChaveEstrangeira(entidade.GetType(), propriedade);
                if (campo != null)
                {
                    return ConverterUtil.Para<long?>(campo.GetValue(entidade));
                }
            }

            var valorEntidade = propriedade.GetValue(entidade);
            if (valorEntidade is Entidade)
            {
                return (valorEntidade as Entidade).Id;
            }
            else
            {
                var propriedadeChaveEstrangeira = RetornarPropriedadeChaveEstrangeira(entidade.GetType(), propriedade, true);
                if (propriedadeChaveEstrangeira == null)
                {
                    return null;
                }
                var valorPropriedade = propriedadeChaveEstrangeira.GetValue(entidade);
                return ConverterUtil.Para<long?>(valorPropriedade);
            }
        }

        public static List<PropertyInfo> RetornarPropridadesDescricao(Type tipoEntidade)
        {
            var propriedadesDescricao = ReflexaoUtil.RetornarPropriedadePossuiAtributo<PropriedadeDescricaoAttribute>(tipoEntidade);
            if (propriedadesDescricao.Count == 0)
            {
                var propriedade = ReflexaoUtil.RetornarPropriedade(tipoEntidade, "Nome", true);
                if (propriedade != null)
                {
                    propriedadesDescricao.Add(propriedade);
                }
                else
                {
                    propriedade = ReflexaoUtil.RetornarPropriedade(tipoEntidade, "Descricao", true);
                    if (propriedade != null)
                    {
                        propriedadesDescricao.Add(propriedade);
                    }
                }
            }
            return propriedadesDescricao;
        }

        public static List<PropertyInfo> RetornarPropriedadesCampos(Type tipoEntidade)
        {
            return RetornarPropriedadesCampos(tipoEntidade, EnumFiltroPropriedadeCampo.Todas);
        }

        public static TInterfaceAtributo RetornarAtributoImplementaInterface<TInterfaceAtributo>(PropertyInfo propriedade) where TInterfaceAtributo : class
        {
            var atributos = propriedade.GetCustomAttributes();
            var tipoIValorPadrao = typeof(TInterfaceAtributo);
            var atributo = atributos.Where(x => ReflexaoUtil.IsTipoImplementaInterface(x.GetType(), tipoIValorPadrao, true)).ToArray();
            if (atributo.Length > 1)
            {
                throw new Erro($"Existe mais de um atributo que implementa a interface {typeof(TInterfaceAtributo).Name}, Entidade: '{propriedade.DeclaringType.Name}', Propriedade: '{propriedade.Name}'");
            }
            return atributo.SingleOrDefault() as TInterfaceAtributo;
        }

        public static List<PropertyInfo> RetornarPropriedadesCampos(Type tipoEntidade,
                                                                    EnumFiltroPropriedadeCampo filtro,
                                                                    bool isIncluirTipoComplexos = false)
        {
            var filtros = EnumUtil.RetornarFlags<EnumFiltroPropriedadeCampo>(filtro)
                .ToHashSet();
            
            var ignorarTipoBase = filtros.Contains(EnumFiltroPropriedadeCampo.IgnorarTipoBase);
            var ignorarChavePrimaria = filtros.Contains(EnumFiltroPropriedadeCampo.IgnorarChavePrimaria);
            var ignorarPropriedadeProtegida = filtros.Contains(EnumFiltroPropriedadeCampo.IgnorarPropriedadeProtegida);
            var ignorarChaveEstrangeira = filtros.Contains(EnumFiltroPropriedadeCampo.IgnorarChaveEstrangeira);

            return RetornarPropriedadesCampos(tipoEntidade, ignorarTipoBase: ignorarTipoBase,
                                                            ignorarChavePrimaria: ignorarChavePrimaria,
                                                            ignorarPropriedadeProtegida: ignorarPropriedadeProtegida,
                                                            ignorarChaveEstrangeira: ignorarChaveEstrangeira,
                                                            isIncluirTipoComplexos);
        }
        //public static List<PropertyInfo> RetornarPropriedadesCampos(Type tipoEntidade, bool ignorarTipoBase)
        //{
        //    return EntidadeUtil.RetornarPropriedadesCampos(tipoEntidade, ignorarTipoBase, false, false);
        //}

        public static List<PropertyInfo> RetornarPropriedadesCampos(Type tipoEntidade,
                                                                    bool ignorarTipoBase = false,
                                                                    bool ignorarChavePrimaria = false,
                                                                    bool ignorarPropriedadeProtegida = false,
                                                                    bool ignorarChaveEstrangeira = false,
                                                                    bool ignorarSobreescritas = false,
                                                                    bool isIncluirTipoComplexo = false)
        {
            var propriedades = ReflexaoUtil.RetornarPropriedades(tipoEntidade, ignorarTipoBase);
            if (tipoEntidade.BaseType == typeof(Entidade))
            {
                if (!ignorarPropriedadeProtegida)
                {
                    propriedades.Insert(0, PropriedadeNomeTipoEntidade);
                }
            }

            HashSet<string> nomePrpriedadesChaveEstrangeiras = null;
            if (ignorarChaveEstrangeira)
            {
                nomePrpriedadesChaveEstrangeiras = propriedades.Select(x => x.RetornarAtributoChaveEstrangeira()).
                                                                Where(x => x != null).
                                                                Select(x => x.NomePropriedade).ToHashSet();
            }
            return propriedades.Where(propriedade =>
                                    {
                                        if (IsPropriedadeCampo(propriedade, isIncluirTipoComplexo))
                                        {
                                            var atributoChavePrimaria = propriedade.GetCustomAttribute<KeyAttribute>();
                                            var atributoPropriedadeProtegida = propriedade.GetCustomAttribute<PropriedadeProtegidaAttribute>();
                                            var getMethod = propriedade.GetGetMethod();
                                            bool resultado = true;

                                            // ignorar propriedades sobreescritas - override
                                            if (atributoChavePrimaria == null)
                                            {
                                                resultado = resultado && getMethod.GetBaseDefinition().DeclaringType == getMethod.DeclaringType;
                                            }
                                            if (ignorarChavePrimaria)
                                            {
                                                resultado = resultado && atributoChavePrimaria == null;
                                            }
                                            if (ignorarPropriedadeProtegida)
                                            {
                                                resultado = resultado && atributoPropriedadeProtegida == null;
                                            }
                                            if (ignorarChaveEstrangeira)
                                            {
                                                resultado = resultado && !nomePrpriedadesChaveEstrangeiras.Contains(propriedade.Name);
                                            }
                                            if (ignorarSobreescritas)
                                            {
                                                resultado = resultado && !nomePrpriedadesChaveEstrangeiras.Contains(propriedade.Name);
                                            }
                                            return resultado;
                                        }
                                        return false;

                                    }).ToList();
        }

        private static bool IsPropriedadeCampo(PropertyInfo propriedade,
                                               bool isIncluirTipoComplexo)
        {
            if (propriedade == PropriedadeNomeTipoEntidade)
            {
                return true;
            }

            var atrituboNaoMapear = propriedade.GetCustomAttribute<NaoMapearAttribute>();
            var atrituboNaoMapearInterno = propriedade.GetCustomAttribute<NaoMapearInternoAttribute>();
            if (atrituboNaoMapear == null && atrituboNaoMapearInterno == null)
            {
                if (propriedade.GetGetMethod() != null && propriedade.GetGetMethod().IsPublic &&
                    propriedade.GetSetMethod() != null && propriedade.GetSetMethod().IsPublic)
                {
                    if (propriedade.PropertyType.IsSubclassOf(typeof(BaseTipoComplexo)))
                    {
                        return isIncluirTipoComplexo;
                    }
                    return ReflexaoUtil.IsPropriedadeRetornaTipoPrimario(propriedade, true);
                }
            }
            return false;
        }

        public static string RetornarNomeCampo(PropertyInfo propriedade)
        {
            var atributoCampo = propriedade.GetCustomAttribute<CampoAttribute>();
            if (atributoCampo != null)
            {
                return atributoCampo.NomeCampo;
            }
            return propriedade.Name;
        }

        public static List<PropertyInfo> RetornarPropriedadesRelacaoPai(Type tipoEntidade, bool ignorarTipoBase = false)
        {
            var propriedades = ReflexaoUtil.RetornarPropriedades(tipoEntidade, ignorarTipoBase);
            propriedades.Where(x => x.RetornarAtributoChaveEstrangeira() != null);
            return propriedades.ToList();
        }

        public static Dictionary<string, PropertyInfo> RetornarDicionarioPropriedades(Type tipoEntidade)
        {
            var dicionario = new Dictionary<string, PropertyInfo>();
            var propriedades = RetornarPropriedadesCampos(tipoEntidade, EnumFiltroPropriedadeCampo.Todas);
            var propriedadesRelacao = RetornarPropriedadesRelacaoPai(tipoEntidade);

            foreach (var propriedade in propriedades)
            {
                dicionario.Add(propriedade.Name, propriedade);
            }
            return dicionario;
        }

        public static Dictionary<string, FieldInfo> RetornarDicionarioCamposPrivados(Type tipoEntidade, List<PropertyInfo> propriedades)
        {
            var dicionario = new Dictionary<string, FieldInfo>();

            foreach (var propriedade in propriedades)
            {
                var nomeCampoPrivado = RetornarNomeCampoPrivado(propriedade);
                var campo = tipoEntidade.GetField(nomeCampoPrivado);
                if (campo == null)
                {
                    throw new Exception($"Não foi encontrado  o campo privado para a propriedade {propriedade.Name}, nome do campo {nomeCampoPrivado}");
                }
                dicionario.Add(propriedade.Name, campo);
            }
            return dicionario;
        }

        private static string RetornarNomeCampoPrivado(PropertyInfo propriedade)
        {
            var nomePropriedade = TextoUtil.RetornarPrimeiraLetraMaiuscula(propriedade.Name);
            if (nomePropriedade.StartsWith("iD"))
            {
                nomePropriedade = "id" + nomePropriedade.Substring(2);
            }
            var nomeCampo = "_" + nomePropriedade;
            return nomeCampo;
        }

    }

    public enum EnumFiltroPropriedadeCampo
    {
        Todas = 1,
        IgnorarTipoBase = 2,
        IgnorarChavePrimaria = 4,
        IgnorarPropriedadeProtegida = 8,
        IgnorarChaveEstrangeira = 16
    }
}