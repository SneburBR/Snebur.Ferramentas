using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using Snebur.VisualStudio.Reflexao;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Snebur.VisualStudio
{
    public partial class ProjetoDominio : BaseProjeto<ConfiguracaoProjetoDominio>
    {
        public const string INICIO_REGION_PRIVADOS = "#region Campos Privados";
        public const string FIM_REGION = "#endregion";

        public static readonly string NOME_METODO_NOTIFICAR_PROPRIEDADE_ALTERADA = "NotificarValorPropriedadeAlterada";
        public static readonly string NOME_METODO_NOTIFICAR_PROPRIEDADE_ALTERADA_IS_ATIVO = "NotificarValorPropriedadeAlteradaIsAtivo";
        public static readonly string NOME_METODO_NOTIFICAR_PROPRIEDADE_ALTERADA_RELACAO = "NotificarValorPropriedadeAlteradaRelacao";
        public static readonly string NOME_METODO_NOTIFICAR_PROPRIEDADE_ALTERADA_CHAVE_ESTRANGEIRA = "NotificarValorPropriedadeAlteradaChaveEstrangeiraAlterada";
        public static readonly string NOME_METODO_NOTIFICAR_PROPRIEDADE_ALTERADA_TIPO_COMPLEXO = "NotificarValorPropriedadeAlteradaTipoCompleto";
        public static readonly string NOME_METODO_RETORNAR_VALOR_PROPRIEDADE = "RetornarValorPropriedade";
        public static readonly string NOME_METODO_RETORNAR_VALOR_PROPRIEDADE_IS_ATIVO = "RetornarValorPropriedadeIsAtivo";
        public static readonly string NOME_METODO_RETORNAR_VALOR_PROPRIEDADE_CHAVE_ESTRANGEIRA = "RetornarValorPropriedadeChaveEstrangeira";

        public void AtualizarAtribuirPropriedades()
        {
            if (this.IsProjetoSneburDominio)
            {
                return;
            }

            var filtros = EnumFiltroPropriedadeCampo.IgnorarChavePrimaria |
                          EnumFiltroPropriedadeCampo.IgnorarPropriedadeProtegida |
                          EnumFiltroPropriedadeCampo.IgnorarTipoBase;



            var dicionarioArquivos = Directory.GetFiles(this.CaminhoProjeto, "*.cs", SearchOption.AllDirectories).
                                        GroupBy(x => Path.GetFileNameWithoutExtension(x).ToLower()).
                                        ToDictionary(x => x.Key);


            var tiposBaseDominio = this.TodosTipos.Where(x => TipoUtil.TipoSubTipo(x, typeof(BaseDominio))).ToList();

            foreach (var tipoBaseDominio in tiposBaseDominio)
            {
                var chaveArquivo = tipoBaseDominio.Name.ToLower();
                if (!dicionarioArquivos.ContainsKey(chaveArquivo))
                {
                    LogVSUtil.LogErro($"Propriedades não atualizadas, nenhum encontrado nenhum arquivo para o tipo '{tipoBaseDominio.Name}'. O nome do tipo precisa ser igual para nome do arquivo");
                    continue;
                }

                var arquivos = dicionarioArquivos[chaveArquivo];
                if (arquivos.Count() > 1)
                {
                    LogVSUtil.LogErro($"Existem mais de um arquivo para o tipo '{tipoBaseDominio.Name}'. as propriedades  não foram atualizadas");
                    continue;
                }
                var caminhoArquivo = arquivos.Single();
                var conteuto = File.ReadAllText(caminhoArquivo, Encoding.UTF8);
                //var linhas = conteuto.ToLines();
                var linhas = File.ReadAllLines(caminhoArquivo, Encoding.UTF8).ToList();

                this.NormalizarLinhas(tipoBaseDominio, linhas);

                var linhaClass = linhas.Where(x => x.Trim().StartsWith("public") && x.Contains("class")).FirstOrDefault();
                if (linhaClass == null)
                {
                    throw new Erro($"A linha da declaração da classe não foi encontrada no tipo {tipoBaseDominio.Name}");
                }

                var linhaRegionPrivados = linhas.Where(x => x.Contains(INICIO_REGION_PRIVADOS)).SingleOrDefault();

                if (linhaRegionPrivados == null)
                {
                    var posicaoClass = linhas.IndexOf(linhaClass) + 2;
                    var novasLinhas = new List<string>();

                    novasLinhas.Add("");
                    novasLinhas.Add("\t\t" + INICIO_REGION_PRIVADOS);
                    novasLinhas.Add("");
                    novasLinhas.Add("");
                    novasLinhas.Add("\t\t" + FIM_REGION);
                    novasLinhas.Add("");

                    linhas.InsertRange(posicaoClass, novasLinhas);

                    linhaRegionPrivados = linhas.Where(x => x.Contains(INICIO_REGION_PRIVADOS)).Single();
                }

                var posicaoPrivadas = linhas.IndexOf(linhaRegionPrivados) + 2;

                var linhasCampoPrivado = new List<string>();
                var linhasCampoPrivadoRemover = new List<string>();

                var linhasPropriedade = linhas.Where(x => x.Contains("public") &&
                                                          x.Contains("get") &&
                                                          x.Contains("set") &&
                                                          !x.Trim().StartsWith(@"//")).ToList();

                var linhasPropriedadePesquisa = linhasPropriedade.Select(x => x.Substring(0, x.LastIndexOf("set"))).ToList();


                var propriedadesCampos = EntidadeUtil.RetornarPropriedadesCampos(tipoBaseDominio, filtros).
                                                             Where(x => !PropriedadeUtil.PossuiAtributo(x, nameof(IgnorarNormalizacaoAttribute))).ToList();

                var prorpiedadesTiposCompleto = ReflexaoUtil.RetornarPropriedades(tipoBaseDominio, true, true).
                                                             Where(x => TipoUtil.TipoIgualOuSubTipo(x.PropertyType, typeof(BaseTipoComplexo)) &&
                                                                        !PropriedadeUtil.PossuiAtributo(x, nameof(IgnorarNormalizacaoAttribute))).ToList();

                var propriedadesRelacao = ReflexaoUtil.RetornarPropriedades(tipoBaseDominio, true).
                                                             Where(x => TipoUtil.TipoSubTipo(x.PropertyType, (typeof(Entidade)))).
                                                             Where(x => TipoUtil.TipoSubTipo(x.DeclaringType, (typeof(Entidade)))).
                                                             Where(x => !PropriedadeUtil.PossuiAtributo(x, nameof(NaoMapearAttribute)) &&
                                                                        !PropriedadeUtil.PossuiAtributo(x, nameof(IgnorarNormalizacaoAttribute)) &&
                                                                        x.GetMethod.DeclaringType == x.GetMethod.GetBaseDefinition().DeclaringType).ToList();

                var propriedadesRelacaoNaoEntidade = ReflexaoUtil.RetornarPropriedades(tipoBaseDominio, true).
                                                             Where(x => TipoUtil.TipoSubTipo(x.PropertyType, (typeof(Entidade)))).
                                                             Where(x => !TipoUtil.TipoSubTipo(x.DeclaringType, (typeof(Entidade)))).
                                                             Where(x => !PropriedadeUtil.PossuiAtributo(x, nameof(NaoMapearAttribute)) &&
                                                                        !PropriedadeUtil.PossuiAtributo(x, nameof(IgnorarNormalizacaoAttribute)) &&
                                                                        x.GetMethod.DeclaringType == x.GetMethod.GetBaseDefinition().DeclaringType).ToList();


                List<(PropertyInfo propriedadeRelacao, PropertyInfo propriedadeChavaEtrangeira)> proproriedadesChaveEstrangeiras = propriedadesRelacao.Select(x => (x, PropriedadeUtil.RetornarPropriedadeChaveEstrangeira(x))).ToList();

                proproriedadesChaveEstrangeiras = proproriedadesChaveEstrangeiras.Where(x => x.propriedadeChavaEtrangeira != null).ToList();

                var isImplementarIAtivo = TipoUtil.TipoSubTipo(tipoBaseDominio, typeof(Entidade)) &&
                                           TipoUtil.TipoImplementaInterface(tipoBaseDominio, typeof(IAtivo));

                var propriedadesNormal = new List<PropertyInfo>();
                propriedadesNormal.AddRange(propriedadesCampos);
                propriedadesNormal.AddRange(propriedadesRelacaoNaoEntidade);

                //normal, campos ou relacao entidade em classes não entidade
                foreach (var propriedadeNormal in propriedadesNormal)
                {
                    var nomeCampoPrivado = this.RetornarNomeCampoPrivado(propriedadeNormal.Name);
                    var linhaNormalAtual = linhas.Where(x => x.Contains(" " + nomeCampoPrivado + ";") && !x.Contains("get")).SingleOrDefault();


                    var linhaPropriedadePesquisa = this.RetornarLinhaPropridade(linhasPropriedadePesquisa, propriedadeNormal);
                    if (linhaPropriedadePesquisa != null)
                    {

                        var itens = proproriedadesChaveEstrangeiras.
                            Where(x => x.propriedadeChavaEtrangeira == propriedadeNormal).ToList();

                        if (itens.Count > 1)
                        {
                            throw new Exception($"Existe mais um propriedade  estrangeira para {propriedadeNormal.Name} em {propriedadeNormal.DeclaringType.Name}");
                        }



                        var (propriedadeRelacao, propriedadeChavaEtrangeira) = itens.SingleOrDefault();

                        var nomeMetodoRetornarValorPropriedade = this.RetornarNomeMetodoRetornarValorPropriedade(isImplementarIAtivo, propriedadeChavaEtrangeira, propriedadeNormal);
                        var nomeMetodoNotificarValorPropriedadeAlterada = this.RetornarNomeMetodoNotificarValorPropriedadeAlterada(isImplementarIAtivo, propriedadeNormal, propriedadeChavaEtrangeira);
                        // campo comum
                        if (!linhaPropriedadePesquisa.Contains(NOME_METODO_NOTIFICAR_PROPRIEDADE_ALTERADA) ||
                            !linhaPropriedadePesquisa.Contains(nomeMetodoRetornarValorPropriedade))
                        {
                            var posicaoLinhaPesquisa = linhasPropriedadePesquisa.IndexOf(linhaPropriedadePesquisa);
                            var linhaPropriedade = linhasPropriedade[posicaoLinhaPesquisa];

                            var posicaoLinha = linhas.IndexOf(linhaPropriedade);
                            var fim = linhaPropriedadePesquisa.IndexOf("get");
                            var inicioPropriedade = linhaPropriedadePesquisa.Substring(0, fim).TrimEnd();

                            string linhaPropriedadeAtribuir;

                            if (propriedadeChavaEtrangeira != null)
                            {

                                linhaPropriedadeAtribuir = $"{inicioPropriedade} get => this.{NOME_METODO_RETORNAR_VALOR_PROPRIEDADE_CHAVE_ESTRANGEIRA}" +
                                                                                        $"(this.{nomeCampoPrivado}, this.{propriedadeRelacao.Name}); " +
                                                                                        $"set => this.{NOME_METODO_NOTIFICAR_PROPRIEDADE_ALTERADA_CHAVE_ESTRANGEIRA}" +
                                                                                         $"(this.{nomeCampoPrivado}, this.{nomeCampoPrivado} = value, \"{propriedadeRelacao.Name}\", this.{propriedadeRelacao.Name}); }}";

                            }
                            else
                            {

                                linhaPropriedadeAtribuir = $"{inicioPropriedade} get => this.{nomeMetodoRetornarValorPropriedade}(this.{nomeCampoPrivado}); " +
                                                                                 $"set => this.{nomeMetodoNotificarValorPropriedadeAlterada}(this.{nomeCampoPrivado}, this.{nomeCampoPrivado} = value); }}";
                            }

                            linhas[posicaoLinha] = linhaPropriedadeAtribuir;


                            var linhaCampo = linhaPropriedadePesquisa.Substring(0, linhaPropriedadePesquisa.LastIndexOf(" " + propriedadeNormal.Name + " "));
                            if (!linhaCampo.EndsWith(" "))
                            {
                                linhaCampo += " ";
                            }
                            linhaCampo += nomeCampoPrivado;
                            linhaCampo = linhaCampo.Replace("public", "private");

                            if (PropriedadeUtil.PossuiAtributo(propriedadeNormal, typeof(ValorPadraoCampoPrivadoAttribute)))
                            {
                                var atributo = PropriedadeUtil.RetornarAtributo(propriedadeNormal, typeof(ValorPadraoCampoPrivadoAttribute), false);
                                var valorPadrao = ReflexaoUtil.RetornarValorPropriedade<ValorPadraoCampoPrivadoAttribute>(x => x.ValorPadrao, atributo);
                                var valorPadraoConvertido = ConverterUtil.Converter(valorPadrao, propriedadeNormal.PropertyType);

                                var tipoEnum = ReflexaoUtil.RetornarValorPropriedade<ValorPadraoCampoPrivadoAttribute>(x => x.TipoEnum, atributo) as Type;
                                string valorString;
                                if (tipoEnum != null)
                                {
                                    if (!tipoEnum.IsEnum)
                                    {
                                        throw new Exception("O tipoEnum nao é suportado");
                                    }
                                    var valorEnum = Enum.Parse(tipoEnum, valorPadrao.ToString());
                                    valorString = $"{tipoEnum.Name}.{valorEnum}";
                                }
                                else
                                {
                                    valorString = this.RetornarValorString(valorPadraoConvertido);
                                }

                                linhaCampo += " = " + valorString;


                            }
                            linhaCampo += ";";

                            if (linhaNormalAtual != linhaCampo)
                            {
                                linhasCampoPrivado.Add(linhaCampo);

                                if (!String.IsNullOrEmpty(linhaNormalAtual))
                                {
                                    linhasCampoPrivadoRemover.Add(linhaNormalAtual);
                                }
                            }
                        }


                    }
                }

                //TIPO COMLEXO
                foreach (var propriedadeTipoComplexo in prorpiedadesTiposCompleto)
                {
                    var nomeCampoPrivado = this.RetornarNomeCampoPrivado(propriedadeTipoComplexo.Name);
                    var linhaCampoPrivado = linhas.Where(x => x.Contains(" " + nomeCampoPrivado + ";") && !x.Contains("get")).SingleOrDefault();

                    var linhaPropriedadePesquisa = this.RetornarLinhaPropridade(linhasPropriedadePesquisa, propriedadeTipoComplexo);
                    if (linhaPropriedadePesquisa != null)
                    {
                        if (!linhaPropriedadePesquisa.Contains(NOME_METODO_NOTIFICAR_PROPRIEDADE_ALTERADA_TIPO_COMPLEXO) &&
                            !linhaPropriedadePesquisa.Contains(NOME_METODO_RETORNAR_VALOR_PROPRIEDADE)
                            || !linhaPropriedadePesquisa.Contains("value?.Clone() as "))
                        {
                            var posicaoLinhaPesquisa = linhasPropriedadePesquisa.IndexOf(linhaPropriedadePesquisa);
                            var linhaPropriedade = linhasPropriedade[posicaoLinhaPesquisa];
                            var posicaoLinha = linhas.IndexOf(linhaPropriedade);

                            var fim = linhaPropriedadePesquisa.IndexOf("get");
                            var inicioPropriedade = linhaPropriedadePesquisa.Substring(0, fim);
                            inicioPropriedade = inicioPropriedade.TrimEnd();

                            var linhaPropriedadeAtribuir = String.Format("{0} get => this.{1}(this.{2}); set => this.{3}(this.{2}, this.{2} = value?.Clone() as {4} ?? new {4}()); }}",
                                                           inicioPropriedade,
                                                           NOME_METODO_RETORNAR_VALOR_PROPRIEDADE,
                                                           nomeCampoPrivado,
                                                           NOME_METODO_NOTIFICAR_PROPRIEDADE_ALTERADA_TIPO_COMPLEXO,
                                                           propriedadeTipoComplexo.PropertyType.Name);

                            linhas[posicaoLinha] = linhaPropriedadeAtribuir;

                            if (String.IsNullOrWhiteSpace(linhaCampoPrivado))
                            {
                                linhaCampoPrivado = linhaPropriedadePesquisa.Substring(0, linhaPropriedadePesquisa.LastIndexOf(" " + propriedadeTipoComplexo.Name + " "));
                                if (!linhaCampoPrivado.EndsWith(" "))
                                {
                                    linhaCampoPrivado += " ";
                                }
                                linhaCampoPrivado += nomeCampoPrivado + $" = new {propriedadeTipoComplexo.PropertyType.Name}();";
                                linhaCampoPrivado = linhaCampoPrivado.Replace("public", "private");
                                linhasCampoPrivado.Add(linhaCampoPrivado);
                            }
                        }
                    }
                }

                //TIPO RELACO PAI, UM UM
                foreach (var propriedadeRelacao in propriedadesRelacao)
                {
                    var nomeCampoPrivado = this.RetornarNomeCampoPrivado(propriedadeRelacao.Name);
                    var linhaCampo = linhas.Where(x => x.Contains(" " + nomeCampoPrivado + ";") && !x.Contains("get")).SingleOrDefault();

                    var linhaPropriedade = this.RetornarLinhaPropridade(linhasPropriedade, propriedadeRelacao);
                    if (linhaPropriedade != null)
                    {
                        if (!linhaPropriedade.Contains(NOME_METODO_NOTIFICAR_PROPRIEDADE_ALTERADA_RELACAO) &&
                            !linhaPropriedade.Contains(NOME_METODO_RETORNAR_VALOR_PROPRIEDADE))
                        {
                            var posicaoLinha = linhas.IndexOf(linhaPropriedade);
                            var fim = linhaPropriedade.IndexOf("get");
                            var inicioPropriedade = linhaPropriedade.Substring(0, fim);

                            var linhaPropriedadeAtribuir = String.Format("{0} get => this.{1}(this.{2}); set => this.{3}(this.{2}, this.{2} = value); }}",
                                                           inicioPropriedade,
                                                           NOME_METODO_RETORNAR_VALOR_PROPRIEDADE,
                                                           nomeCampoPrivado,
                                                           NOME_METODO_NOTIFICAR_PROPRIEDADE_ALTERADA_RELACAO);

                            linhas[posicaoLinha] = linhaPropriedadeAtribuir;

                            if (String.IsNullOrWhiteSpace(linhaCampo))
                            {
                                linhaCampo = linhaPropriedade.Substring(0, linhaPropriedade.LastIndexOf(" " + propriedadeRelacao.Name + " "));
                                if (!linhaCampo.EndsWith(" "))
                                {
                                    linhaCampo += " ";
                                }
                                linhaCampo += nomeCampoPrivado + ";";
                                linhaCampo = linhaCampo.Replace("public", "private");
                                linhasCampoPrivado.Add(linhaCampo);
                            }
                        }
                    }
                }

                linhasCampoPrivadoRemover.AddRange(linhasCampoPrivado);
                foreach (var linhaRemover in linhasCampoPrivadoRemover)
                {
                    linhas.Remove(linhaRemover);
                }


                linhas.InsertRange(posicaoPrivadas, linhasCampoPrivado);

                var novoConteudo = String.Join("\n", linhas);
                if (novoConteudo != conteuto)
                {
                    File.WriteAllText(caminhoArquivo, novoConteudo, Encoding.UTF8);
                    //return;
                }
            }
        }

        private string RetornarNomeMetodoRetornarValorPropriedade(bool isImplementarIAtivo,
                                                                  PropertyInfo propriedadeChavaEtrangeira,
                                                                  PropertyInfo propriedadeCampo)
        {
            if (isImplementarIAtivo && propriedadeCampo.Name == nameof(IAtivo.IsAtivo))
            {
                return NOME_METODO_RETORNAR_VALOR_PROPRIEDADE_IS_ATIVO;
            }

            return (propriedadeChavaEtrangeira != null) ? NOME_METODO_RETORNAR_VALOR_PROPRIEDADE_CHAVE_ESTRANGEIRA :
                                                          NOME_METODO_RETORNAR_VALOR_PROPRIEDADE;
        }

        private string RetornarNomeMetodoNotificarValorPropriedadeAlterada(
            bool isImplementarIAtivo,
            PropertyInfo propriedadeCampo,
            PropertyInfo propriedadeChavaEtrangeira)
        {
            if (propriedadeChavaEtrangeira != null)
            {
                return NOME_METODO_NOTIFICAR_PROPRIEDADE_ALTERADA_CHAVE_ESTRANGEIRA;
            }
            if (isImplementarIAtivo && propriedadeCampo.Name == nameof(IAtivo.IsAtivo))
            {
                return NOME_METODO_NOTIFICAR_PROPRIEDADE_ALTERADA_IS_ATIVO;
            }
            return NOME_METODO_NOTIFICAR_PROPRIEDADE_ALTERADA;
        }

        private void NormalizarLinhas(Type tipoBaseDominio, List<string> linhas)
        {
            foreach (var (linha, index) in linhas.ToTupleItemIndex())
            {
                var linhaNormalizada = this.NormalizarNameOf(tipoBaseDominio, linha);
                if (linhaNormalizada != linha)
                {
                    linhas[index] = linhaNormalizada;
                }
            }
        }

        private string NormalizarNameOf(Type tipoBaseDominio, string linha)
        {
            if ((linha.TrimStart().StartsWith("[Tabela") ||
                linha.TrimStart().StartsWith("[ChaveEstrangeira")) &&
                !linha.Contains("nameof("))
            {
                var isChaveEstrangeira = linha.TrimStart().StartsWith("[ChaveEstrangeira");
                var conteudo = ExpressaoUtil.RetornarExpressaoAbreFecha(linha, true);
                if (isChaveEstrangeira || conteudo.Equals(tipoBaseDominio.Name))
                {
                    var posicaoInicio = linha.IndexOf("(");
                    var posicaoFim = linha.LastIndexOf(")");

                    var inicio = linha.Substring(0, posicaoInicio + 1);
                    var fim = linha.Substring(posicaoFim);
                    var conteudoSemAspa = conteudo.Substring(1, conteudo.Length - 2);

                    return $"{inicio}nameof({conteudoSemAspa}){fim}";
                }
            }
            return linha;
        }

        private string RetornarValorString(object valorPadraoConvertido)
        {
            if (valorPadraoConvertido is null)
            {
                return "null";
            }

            if (valorPadraoConvertido.GetType().IsEnum)
            {
                return $"{valorPadraoConvertido.GetType().Name}.{valorPadraoConvertido}";
            }

            switch (valorPadraoConvertido)
            {
                case null:

                    return "null";

                case char catacter:

                    return $"'{catacter}'";

                case string texto:

                    return $"\"{texto}\"";

                case bool logico:

                    return logico.ToString().ToLower();

                case int interiro:

                    return interiro.ToString();

                case double duplo:

                    return duplo.ToString(CultureInfo.InvariantCulture);

                case float simples:

                    return simples.ToString(CultureInfo.InvariantCulture);

                case Guid guid:

                    return $"new Guid({guid.ToString()})";

                default:

                    throw new Exception("Tipo não suportado");

            }

        }

        private string RetornarLinhaPropridade(List<string> linhasPropriedade, PropertyInfo propriedadeCampo)
        {
            var procuraNomePropriedade = " " + propriedadeCampo.Name + " ";
            var possiveisLinhaPropriedade = linhasPropriedade.Where(x => x.Contains(procuraNomePropriedade)).ToList();
            if (possiveisLinhaPropriedade.Count == 0)
            {
                throw new Exception($"Não foi encontrada a linha da propriedade '{propriedadeCampo.Name}' em  {propriedadeCampo.DeclaringType.Name}");
            }
            if (possiveisLinhaPropriedade.Count == 1)
            {
                return possiveisLinhaPropriedade.Single();
            }
            var procuraTipo = " " + propriedadeCampo.Name + " ";

            // remove a parte do nome da propriedade procura pelo tipo, esse filtro é importante pq a propriedade pode ter o mesmo nome do tipo
            var linhasTipo = possiveisLinhaPropriedade.Where(x => (x.Substring(0, x.LastIndexOf(procuraNomePropriedade)) + " ").
                                                                 Contains(procuraTipo)).ToList();

            if (linhasTipo.Count != 1)
            {
                throw new Exception($"Não foi encontrada a linha da propriedade '{propriedadeCampo.Name}' em  {propriedadeCampo.DeclaringType.Name}");
            }
            return linhasTipo.Single();

        }

        private string RetornarNomeCampoPrivado(string nomePropriedade)
        {
            if (nomePropriedade.StartsWith("ID") || nomePropriedade.StartsWith("IP"))
            {
                var inicio = nomePropriedade.Substring(0, 2).ToLower();
                nomePropriedade = $"{inicio}{nomePropriedade.Substring(2)}";
            }

            nomePropriedade = TextoUtil.RetornarPrimeiraLetraMinusculo(nomePropriedade);
            return $"_{nomePropriedade}";
        }

        protected override void DispensarInerno()
        {

        }



    }
}
