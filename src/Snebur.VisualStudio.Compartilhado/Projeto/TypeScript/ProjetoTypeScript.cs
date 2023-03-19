using Snebur.Linq;
using Snebur.Publicacao;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Snebur.VisualStudio
{
    public class ProjetoTypeScript : BaseProjeto<ConfiguracaoProjetoTypeScript>
    {
        public const string NOME_ARQUIVO_HTML_REFERENCIA = "Html.Referencias.ts";
        public static string NOME_PROPRIEDADE_CAMINHO_TIPO { get; } = ReflexaoUtil.RetornarNomePropriedade<Snebur.Dominio.ICaminhoTipo>(x => x.__CaminhoTipo);
        public static string NOME_ARQUIVO_APLICACAO_CONFIG = "Aplicacao.Config.ts";
        private const string NOME_PROPRIEDADE_VERSAO_SCRIPT = "VersaoScript";


        public string CaminhosDiretorioTypeScripts { get; }
        public string CaminhoAplicacaoConfig { get; }
        public string CaminhoHtmlReferencias { get; }

        public string NomeArquivoSaida => $"{this.NomeProjeto}.js";
        public string CaminhoSaidaPadrao { get; }
        public string CaminhoSaidaAtual => this.RetornarCaminhoSaidaAtual();

        public FileInfo ArquivoScriptCompilado => new FileInfo(this.CaminhoSaidaPadrao);

        public FileInfo ArquivoReferenciaProjeto { get; set; }

        public HashSet<string> ArquivosTS { get; private set; }
        public List<BaseArquivoTypeScript> ArquivosTypeScriptOrdenados { get; private set; } = new List<BaseArquivoTypeScript>();
        public HashSet<BaseArquivoTypeScript> ArquivosTypeScript { get; private set; } = new HashSet<BaseArquivoTypeScript>();
        private Dictionary<string, ArquivoTypeScript> DicionariosArquivosTypeScript { get; } = new Dictionary<string, ArquivoTypeScript>();

        public HashSet<string> CaminhosTipoClassBase { get; private set; } = new HashSet<string>();
        public HashSet<string> NomesTipoClassBase { get; private set; } = new HashSet<string>();
        public HashSet<string> TodosArquivos { get;  set; }

        public Dictionary<string, int> ProjetosPrioridade { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, string> Dependencias => this.ConfiguracaoProjeto?.Depedencias;

        public string Namespace => this.NomeProjeto.Replace(".Typescript", String.Empty);


        public ProjetoTypeScript(ProjetoViewModel projetoVM, 
                                 ConfiguracaoProjetoTypeScript configuracaoProjeto,
                                 FileInfo arquivoProjeto,
                                 string caminhoConfiguracao) : base(projetoVM, 
                                                                    configuracaoProjeto,
                                                                   arquivoProjeto,
                                                                   caminhoConfiguracao)
        {
            this.CaminhosDiretorioTypeScripts = Path.Combine(this.CaminhoProjeto, ConstantesProjeto.PASTA_SRC);
            this.CaminhoAplicacaoConfig = Path.Combine(this.CaminhosDiretorioTypeScripts, NOME_ARQUIVO_APLICACAO_CONFIG);
            this.CaminhoSaidaPadrao = Path.Combine(this.CaminhoProjeto, ConstantesPublicacao.NOME_PASTA_BUILD, this.NomeArquivoSaida);
            this.CaminhoHtmlReferencias = Path.Combine(this.CaminhosDiretorioTypeScripts, NOME_ARQUIVO_HTML_REFERENCIA);
          
            this.LogCaminhoSaida();
        }

        private void PopularArquivosTS()
        {
            var todosArquivos = LocalProjetoUtil.RetornarTodosArquivos(this.ProjetoViewModel.ProjetoVS, 
                                                                       this.CaminhoProjeto, 
                                                                       false);

            var arquivosTypeScript = todosArquivos.Where(X => Path.GetExtension(X) == ConstantesProjeto.EXTENSAO_TYPESCRIPT).ToHashSet();

                //ProjetoTypeScriptUtil.RetornarArquivosTypeScript(this.CaminhoProjeto);
            LogVSUtil.Log($"Total de arquivos typescript {arquivosTypeScript.Count}");
            this.ArquivosTS = arquivosTypeScript;
        }

        protected override void AtualizarInterno()
        {
            var t = Stopwatch.StartNew();
            if (this.ConfiguracaoProjeto.IsIgnorar)
            {
                return;
            }
            this.CriarArquivoReferencia();
            this.PopularArquivosTS();
            this.ArquivosTypeScript.Clear();
            this.ArquivosTypeScriptOrdenados.Clear();
            this.CaminhosTipoClassBase.Clear();
            this.NomesTipoClassBase.Clear();
            this.DicionariosArquivosTypeScript.Clear();

            this.ArquivosTypeScript.AddRange(TipoArquivoTypeScriptUtil.RetornarArquivosTypeScript(this.ConfiguracaoProjeto,
                                                                                                  this.CaminhoProjeto,
                                                                                                  this.ArquivosTS,
                                                                                                  this.RetornarPrioridadeProjeto ));

            foreach (var arquivoTypescript in this.ArquivosTypeScript.OfType<ArquivoTypeScript>().Where(x => x.IsExisteTipo).ToList())
            {
                if (this.DicionariosArquivosTypeScript.TryGetValue(arquivoTypescript.CaminhoTipo, 
                                                                  out var arquivoTSAtual))
                {
                    if (!arquivoTSAtual.Equals(arquivoTypescript))
                    {
                        LogVSUtil.LogErro($"O tipo já existe  no dicionario. " +
                                                $"' atual: {arquivoTypescript.CaminhoTipo} -- {arquivoTypescript.CaminhoArquivo}" +
                                                $"  novo: {arquivoTSAtual.CaminhoTipo} -- {arquivoTSAtual.CaminhoArquivo}");
                    }
                    else
                    {
                        LogVSUtil.Alerta($"O tipo duplicado no dicionario . " +
                                                $"' atual: {arquivoTypescript.CaminhoTipo} -- {arquivoTypescript.CaminhoArquivo}" +
                                                $"  duplicado: {arquivoTSAtual.CaminhoTipo} -- {arquivoTSAtual.CaminhoArquivo}");
                    }
                    

                    this.DicionariosArquivosTypeScript.AddOrUpdate(arquivoTypescript.CaminhoTipo, arquivoTypescript);
                }
                this.DicionariosArquivosTypeScript.Add(arquivoTypescript.CaminhoTipo, arquivoTypescript);
            }

            if (!this.IsProjetoDebug)
            {
                this.CaminhosTipoClassBase.AddRange(TipoArquivoTypeScriptUtil.RetornarCaminhosClassesBase(this.ConfiguracaoProjeto, this.CaminhoProjeto));
                this.NomesTipoClassBase.AddRange(this.CaminhosTipoClassBase.Where(x => !String.IsNullOrEmpty(x)).Select(x => x.Split('.').Last()));
            }

            this.OrdenarArquivosTypeScript();

            this.AtualizarTSConfig();
            this.ConfigurarHtmlRerefencias();

            BaseGerenciadoProjetos.TryIntancia?.AtualizarProjetoTS(this);
            LogVSUtil.Sucesso($"Projeto TS {this.NomeProjeto} normalizado em {t.Elapsed.TotalSeconds}s", t);
        }

        private void AtualizarTSConfig()
        {
            var tsconfig = this.RetornarConfiguracaoProjetoTypescript();
            this.SalvarTSConfig(tsconfig);
        }

        private ConfiguracaoProjetoTypeScript RetornarConfiguracaoProjetoTypescript()
        {
            var arquivosTypescript = this.ArquivosTypeScriptOrdenados.Select(x => @x.Arquivo.FullName).ToList();
            var caminhoJavasriptSaida = CaminhoUtil.RetornarCaminhoRelativo(new FileInfo(this.CaminhoSaidaPadrao), this.CaminhoProjeto);
            var configuracaoProjetoAtual = ProjetoTypeScriptUtil.RetornarConfiguracaoProjetoTypeScript(this.CaminhoConfiguracao);

            caminhoJavasriptSaida = this.NormalizarCaminhos(caminhoJavasriptSaida, configuracaoProjetoAtual, arquivosTypescript);

            //if (this.ConfiguracaoProjeto.IsEditarApresentacao)
            //{
            //    return new ConfiguracaoProjetoTypeScriptApresentacao(configuracaoProjetoAtual,
            //                                                         arquivosTypescript,
            //                                                         this.CaminhoProjeto,
            //                                                         caminhoJavasriptSaida,
            //                                                         this.IsProjetoDebug);
            //}

            return new ConfiguracaoProjetoTypeScriptFramework(configuracaoProjetoAtual,
                                                             arquivosTypescript,
                                                             this.CaminhoProjeto,
                                                             caminhoJavasriptSaida,
                                                             this.IsProjetoDebug);
        }

        private string NormalizarCaminhos(string caminhoJavasriptSaida,
                                          ConfiguracaoProjetoTypeScript configuracaoProjetoAtual,
                                          List<string> arquivosTypescript)
        {
            if (GerenciadorProjetosUtil.DiretorioProjetoTypescriptInicializacao != null &&
                Directory.Exists(GerenciadorProjetosUtil.DiretorioProjetoTypescriptInicializacao))
            {
                var diretorioProjeto = GerenciadorProjetosUtil.DiretorioProjetoTypescriptInicializacao;
                var caminhoSaida = Path.Combine(diretorioProjeto, caminhoJavasriptSaida);
                var caminhoSaidaRelativo = CaminhoUtil.RetornarCaminhoRelativo(caminhoSaida, this.CaminhoProjeto);

                //var caminhoSaida = CaminhoUtil.RetornarCaminhoRelativo(diretorioProjeto, caminhoJavasriptSaida);
                this.NormalizarCaminhosDeclarecoes(configuracaoProjetoAtual, diretorioProjeto, arquivosTypescript);
                return caminhoSaidaRelativo;
            }
            return caminhoJavasriptSaida;
        }

        private void NormalizarCaminhosDeclarecoes(ConfiguracaoProjetoTypeScript configuracaoProjetoAtual,
                                                   string diretorioProjetoInicializacao,
                                                   List<string> arquivosTypescript)
        {
            var scriptsDepedentes = configuracaoProjetoAtual.Depedencias.Values.
                                                            Select(x => Path.GetFileName(x)).ToList();

            foreach (var arquivo in arquivosTypescript.ToList())
            {
                if (scriptsDepedentes.Count == 0)
                {
                    break;

                }

                var nomeArquivo = Path.GetFileName(arquivo);
                if (scriptsDepedentes.Contains(nomeArquivo))
                {
                    scriptsDepedentes.Remove(nomeArquivo);
                    arquivosTypescript.Remove(arquivo);
                }
            }

            foreach (var caminhoDepedencia in configuracaoProjetoAtual.Depedencias.Values.Reverse())
            {
                var nomeArquivo = Path.GetFileName(caminhoDepedencia);

                var caminhoDestino = Path.Combine(diretorioProjetoInicializacao, ConstantesProjeto.PASTA_BUILD, nomeArquivo);
                var caminhoDestinoRelativo = CaminhoUtil.RetornarCaminhoRelativo(caminhoDestino, this.CaminhoProjeto);
                var caminhoFinal = Path.GetFullPath(Path.Combine(this.CaminhoProjeto, caminhoDestinoRelativo));
                arquivosTypescript.Insert(0, caminhoFinal);
                scriptsDepedentes.Add(nomeArquivo);
            }



        }

        #region " Métodos "

        protected void OrdenarArquivosTypeScript()
        {
            while (this.ArquivosTypeScript.Count > 0)
            {
                var proximoAruqivo = this.RetornarProximoArquivoTypeScrit();
                if (proximoAruqivo != null)
                {
                    if (proximoAruqivo.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseBase ||
                    proximoAruqivo.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseBaseAbstrata ||
                    proximoAruqivo.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseExportAbstrata ||
                    proximoAruqivo.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.BaseClasseViewModel ||
                    proximoAruqivo.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.BaseClasseViewModelAbstrata ||
                    proximoAruqivo.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseViewModel ||
                    proximoAruqivo.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseViewModelAbstrataExport ||
                    proximoAruqivo.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseExport)
                    {
                        this.CaminhosTipoClassBase.Add(((ArquivoTypeScript)proximoAruqivo).CaminhoTipo);
                        this.NomesTipoClassBase.Add(((ArquivoTypeScript)proximoAruqivo).NomeTipo);
                    }
                    this.ArquivosTypeScriptOrdenados.Add(proximoAruqivo);
                }
                else
                {
                    break;
                }
            }
        }

        private BaseArquivoTypeScript RetornarProximoArquivoTypeScrit()
        {
            var proximo = this.RetornarProximoArquivoTypeScritInterno();
            if (proximo == null)
            {
                if (this.ArquivosTypeScript.Count > 0)
                {
                    var arquivosRestante = String.Join(System.Environment.NewLine, this.ArquivosTypeScript.Select(x => x.Arquivo.FullName));
                    var mensagem = String.Format("Não possível encontrar o próximo arquivo da fila, Conferir as dependências dos tipos bases e os padrões de nomeclaturas {0}. {1}{2}", this.NomeProjeto, Environment.NewLine, arquivosRestante);
                    LogVSUtil.LogErro(mensagem);
                    //throw new Exception(mensagem);
                    proximo = this.ArquivosTypeScript.First();
                }
            }
            if (proximo != null)
            {
                this.ArquivosTypeScript.Remove(proximo);
            }
            return proximo;
        }
        private BaseArquivoTypeScript RetornarProximoArquivoTypeScritInterno()
        {
            var proximoArquivo = this.RetornarProximoArquivoTypeScritInterno(false);
            if (proximoArquivo == null)
            {
                return this.RetornarProximoArquivoTypeScritInterno(true);
            }
            return proximoArquivo;

        }
        private BaseArquivoTypeScript RetornarProximoArquivoTypeScritInterno(bool ignorarNamespaceClasseSuperior)
        {
            return this.ArquivosTypeScript.FirstOrDefault(x =>
              {
                  switch (x.TipoArquivoTypeScript)
                  {
                      case EnumTipoArquivoTypeScript.Nativo:
                      case EnumTipoArquivoTypeScript.EnumBase:
                      case EnumTipoArquivoTypeScript.EnumExport:
                      case EnumTipoArquivoTypeScript.ClassePartial:
                      case EnumTipoArquivoTypeScript.Inteface:
                      case EnumTipoArquivoTypeScript.IntefaceExport:
                      case EnumTipoArquivoTypeScript.Vazio:
                      case EnumTipoArquivoTypeScript.Desconhecido:
                      case EnumTipoArquivoTypeScript.ClasseStatica:
                      case EnumTipoArquivoTypeScript.Teste:


                          return true;

                      case EnumTipoArquivoTypeScript.DominioAtributos:
                      case EnumTipoArquivoTypeScript.DominioClasses:
                      case EnumTipoArquivoTypeScript.DominioEnums:
                      case EnumTipoArquivoTypeScript.DominioReflexao:
                      case EnumTipoArquivoTypeScript.DominioInterfaces:
                      case EnumTipoArquivoTypeScript.DominioConstantes:

                          return true;

                      case EnumTipoArquivoTypeScript.SistemaInicio:
                      case EnumTipoArquivoTypeScript.SistemaAplicacao:
                      case EnumTipoArquivoTypeScript.SistemaAplicacaoConfiguracao:
                      case EnumTipoArquivoTypeScript.SistemaLocalConfig:
                      case EnumTipoArquivoTypeScript.SistemaReflexao:
                      
                      case EnumTipoArquivoTypeScript.SistemaVariaveis:
                      case EnumTipoArquivoTypeScript.SistemaDeclarationType:

                          return true;

                      case EnumTipoArquivoTypeScript.SistemaReferencias:

                          return true;

                      case EnumTipoArquivoTypeScript.SistemaExtensaoAplicacaoConfiguracao:
                      case EnumTipoArquivoTypeScript.SistemaExtensaoAplicacao:
                      case EnumTipoArquivoTypeScript.SistemaExtensaoReflexao:

                          return true;

                      case EnumTipoArquivoTypeScript.ClasseBase:
                      case EnumTipoArquivoTypeScript.ClasseBaseAbstrata:
                      case EnumTipoArquivoTypeScript.ClasseExport:
                      case EnumTipoArquivoTypeScript.ClasseExportAbstrata:
                      case EnumTipoArquivoTypeScript.BaseClasseViewModel:
                      case EnumTipoArquivoTypeScript.BaseClasseViewModelAbstrata:
                      case EnumTipoArquivoTypeScript.ClasseViewModel:
                      case EnumTipoArquivoTypeScript.ClasseViewModelAbstrataExport:

                          if (!ignorarNamespaceClasseSuperior)
                          {
                              return !x.IsExisteTipoBase ||
                                     x.CaminhoTipoBase == "Object" ||
                                     x.CaminhoTipoBase == "Error" ||
                                     x.CaminhoTipoBase == "Erro" ||
                                     x.CaminhoTipoBase == "Snebur.Dominio.BaseViewModel" ||
                                     x.CaminhoTipoBase == "Snebur.Dominio.EntidadeViewModel" ||
                                     this.CaminhosTipoClassBase.Contains(x.CaminhoTipoBase) ||
                                     this.CaminhosTipoClassBase.Contains(String.Format("{0}.{1}", x.Namespace, x.CaminhoTipoBase));
                          }
                          else
                          {
                              return x.IsExisteTipoBase && this.NomesTipoClassBase.Contains(x.NomeTipoBase);
                          }

                      default:
                          throw new NotSupportedException("TipoArquivo não suportado ");
                  }

              });
        }

        #endregion

        #region Métodos privados 

        private void SalvarTSConfig(ConfiguracaoProjetoTypeScript tsconfig)
        {
            //var js = new System.Web.Script.Serialization.JavaScriptSerializer();
            var json = JsonUtil.Serializar(tsconfig, true, true);

            ArquivoUtil.SalvarArquivoTexto(this.CaminhoConfiguracao, json);
            LogVSUtil.Log("Arquivo de tsconfig atualizado com sucesso.");
        }

        //private string RetornarCaminhoSaidaAtual(string caminhoProjeto)
        //{
        //    if (Directory.Exists(GerenciadorProjetos.DiretorioProjetoTypescriptInicializacao))
        //    {
        //        throw new NotImplementedException();


        //    }

        //    //return this.ConfiguracaoProjeto.RetornarCaminhoSaida(caminhoProjeto, this.NomeProjeto);
        //    //var caminhoRaiz = caminhoProjeto;
        //    ////if (Directory.Exists(Path.Combine(caminhoProjeto, "www")))
        //    ////{
        //    ////    caminhoRaiz = Path.Combine(caminhoProjeto, "www");
        //    ////}
        //    //var debug = this.IsProjetoDebug ? ".Debug" : String.Empty;

        //    //var isApresentacao = this.ConfiguracaoProjeto.IsEditarApresentacao;
        //    //var caminhoScripts = Path.Combine(caminhoRaiz, ConstantesPublicacao.NOME_PASTA_BUILD);
        //    ////caminhoScripts = this.ConfiguracaoProjeto.RetornarCaminhoScripts(caminhoRaiz);

        //    //if (isApresentacao)
        //    //{
        //    //    var diretorio = Path.Combine(caminhoScripts, "compilado");
        //    //    DiretorioUtil.CriarDiretorio(diretorio);
        //    //    return diretorio;
        //    //}

        //    ////var xxx = Path.Combine(caminhoScripts, $"{this.NomeProjeto}{debug}.js");

        //    //return Path.Combine(caminhoScripts, String.Format("{0}{1}.js", this.NomeProjeto, debug));

        //    return Path.Combine(caminhoProjeto, ConstantesPublicacao.NOME_PASTA_BUILD, $"{this.NomeProjeto}.js");
        //}

        private int RetornarPrioridadeProjeto(FileInfo arquivo)
        {
            if (arquivo.Directory.FullName.Contains(this.CaminhoProjeto))
            {
                return this.ConfiguracaoProjeto.PrioridadeProjeto;
            }
            else
            {
                var diretorioAtual = arquivo.Directory;

                string caminhoTSConfig = String.Empty;
                do
                {
                    if (diretorioAtual == null)
                    {
                        break;
                    }
                    caminhoTSConfig = Path.Combine(diretorioAtual.FullName, ConstantesProjeto.CONFIGURACAO_TYPESCRIPT);
                    diretorioAtual = diretorioAtual.Parent;
                } while (!File.Exists(caminhoTSConfig));

                if (!File.Exists(caminhoTSConfig))
                {
                    throw new Exception("Não foi encontrado o arquivo de configuração");
                }

                if (this.ProjetosPrioridade.ContainsKey(caminhoTSConfig))
                {
                    return this.ProjetosPrioridade[caminhoTSConfig];
                }
                else
                {
                    var prioridadeProjeto = this.RetornarPrioridade(caminhoTSConfig);
                    this.ProjetosPrioridade.Add(caminhoTSConfig, prioridadeProjeto);
                    return prioridadeProjeto;
                }
            }
        }

        private int RetornarPrioridade(string caminhoConfiguracao)
        {
            var configuracao = ProjetoTypeScriptUtil.RetornarConfiguracaoProjetoTypeScript(caminhoConfiguracao);
            return configuracao.PrioridadeProjeto;
        }
        #endregion

        #region Html  Referencias


        private void ConfigurarHtmlRerefencias()
        {
            var namespaceRaiz = ProjetoTypeScriptUtil.NamespaceRaiz(this.NomeProjeto);
            var namespaceReflexao = ProjetoTypeScriptUtil.NamespaceReflexao(this.NomeProjeto);

            var conteudoDeclaracaoTiposUI = this.RetornarDeclaracaoTiposUI(namespaceReflexao);
            var conteudoDeclaracaoHtmlReferencias = this.RetornarHtmlReferencias(namespaceRaiz);

            var sb = new StringBuilder();
            sb.AppendLine($"namespace {namespaceReflexao}");
            sb.AppendLine("{");
            sb.AppendLine();
            sb.AppendLine($"\t//#region Declarações dos tipo UI");
            sb.AppendLine();
            sb.AppendLine(conteudoDeclaracaoTiposUI);
            sb.AppendLine();
            sb.AppendLine("\t//#endregion");
            sb.AppendLine();

            sb.AppendLine();
            sb.AppendLine("\t//#region Html referencias");
            sb.AppendLine();
            sb.AppendLine(conteudoDeclaracaoHtmlReferencias);
            sb.AppendLine();
            sb.AppendLine("\t//#endregion");
            sb.AppendLine();
            sb.AppendLine("}");

            var conteudo = sb.ToString();

            var caminhoRepositorioHtmlReferencia = this.CaminhoHtmlReferencias;
            ArquivoUtil.SalvarArquivoTexto(caminhoRepositorioHtmlReferencia, conteudo);
        }

        private string RetornarDeclaracaoTiposUI(string namespaceReflexao)
        {
            var declaracoesTipos = new DeclaracaoTiposUI(this.ArquivosTypeScriptOrdenados,
                                                        this.DicionariosArquivosTypeScript,
                                                        namespaceReflexao);

            return declaracoesTipos.RetornarDeclaracao().TrimEnd();
        }


        private string RetornarHtmlReferencias(string namasceRaiz)
        {
            var sb = new StringBuilder();
            var arquivosTipoHtmlReferencia = this.ArquivosTypeScriptOrdenados.OfType<ArquivoTypeScript>().Where(x => x.IsTipoHtmlReferencia).ToHashSet();
            foreach (var arquivoTypeScript in arquivosTipoHtmlReferencia)
            {
                if (!arquivoTypeScript.Arquivo.Exists)
                {
                    continue;
                }

                var urlDesenvolvimentoAbsoluta = HtmlReferenciaUti.RetornarUrlDesenvolvimentoAbsoluta(arquivoTypeScript, arquivoTypeScript.ArquivoHtmlReferencia);
                var urlDesenvolvimentoRelativa = HtmlReferenciaUti.RetornarUrlDesenvolvimentoRelativa(arquivoTypeScript, arquivoTypeScript.ArquivoHtmlReferencia);

                var html = this.RetornarHtml(arquivoTypeScript.ArquivoHtmlReferencia);
                //var chave = this.RetornarCaminhoChaveHtmlReferencia(arquivoTypeScript.CaminhoTipo, arquivoHtmlReferencia);
                var dicionarAtribbutos = this.RetornarDicicionarAtributosCorpo(arquivoTypeScript.ArquivoHtmlReferencia);

                //var declaracaoTipo = this.RetornarDeclaracaoTipoUI(arquivoTypeScript, "TipoUIHtml");
                var declaracaoHtmlReferencia = $"\t$HtmlReferencias.Add({arquivoTypeScript.CaminhoTipo}.{NOME_PROPRIEDADE_CAMINHO_TIPO}, new {namasceRaiz}.UI.HtmlReferencia(\"{urlDesenvolvimentoAbsoluta}\", \"{urlDesenvolvimentoRelativa}\", \"{html}\", {dicionarAtribbutos} ));";

                //sb.AppendLine(declaracaoTipo.TrimEnd());
                sb.AppendLine(declaracaoHtmlReferencia.TrimEnd());

                //sb.AppendLine();
            }
            return sb.ToString().TrimEnd();
        }

        private string RetornarDicicionarAtributosCorpo(FileInfo arquivo)
        {
            var html = File.ReadAllText(arquivo.FullName, Encoding.UTF8);
            var documentoHtml = new HtmlAgilityPack.HtmlDocument();
            documentoHtml.LoadHtml(html);

            var corpo = documentoHtml.DocumentNode.SelectSingleNode("//body");
            if (corpo != null && corpo.Attributes.Count > 0)
            {
                var propriedades = new List<string>();
                foreach (var atributo in corpo.Attributes)
                {
                    var valorSerializado = JsonUtil.Serializar(atributo.Value, true);
                    propriedades.Add($"\"{atributo.Name}\" : {valorSerializado}");
                }

                var sb = new StringBuilder();
                sb.Append("{");
                sb.Append(String.Join(", ", propriedades));
                sb.Append("}");
                return $" new DicionarioSimples({sb.ToString()}) ";
            }
            return "null";

        }

        //private string RetornarCaminhoChaveHtmlReferencia(string caminhoArquivo, FileInfo arquivo)
        //{
        //    if (caminhoArquivo.Contains("."))
        //    {
        //        var posicaoFinal = caminhoArquivo.LastIndexOf(".");
        //        var _namespace = caminhoArquivo.Substring(0, posicaoFinal);
        //        var chave = String.Format("{0}.{1}", _namespace, Path.GetFileNameWithoutExtension(arquivo.Name));
        //        return chave;
        //    }
        //    return arquivo.Name;

        //}

        private string RetornarHtml(FileInfo arquivo)
        {
            var html = File.ReadAllText(arquivo.FullName, Encoding.UTF8);
            var documentoHtml = new HtmlAgilityPack.HtmlDocument();
            documentoHtml.LoadHtml(html);

            var corpo = documentoHtml.DocumentNode.SelectSingleNode("//body");
            if (corpo != null)
            {
                html = corpo.InnerHtml;
            }
            html = this.NormalizarHtml(html);
            var bytes = Encoding.Default.GetBytes(html);
            return Convert.ToBase64String(bytes);
        }

        private string NormalizarHtml(string html)
        {
            //html = Regex.Replace(html, @"\s+", " ");
            html = Regex.Replace(html, @"\s{2,}", " ");

            var regex = new Regex(@"[\s\>a-zA-Z0-9,\:]{{[a-zA-Z0-9,\s=]*}}[\s\<a-zA-Z0-9,\:]");

            return regex.Replace(html, (macth) =>
            {
                var substring = macth.Value;
                var inicio = substring.IndexOf("{{");
                var fim = substring.IndexOf("}}") + 2;

                var antes = substring.Substring(0, inicio);
                var depois = substring.Substring(fim);
                var bind = substring.Substring(inicio, fim - inicio);

                if (String.IsNullOrWhiteSpace(antes))
                {
                    antes = "&nbsp;";
                }
                if (String.IsNullOrWhiteSpace(depois))
                {
                    depois = "&nbsp;";

                }
                return $" {antes}<span sn-bind=\"{bind}\" ></span>{depois} ";

            });

        }

        private void CriarArquivoReferencia()
        {
            if (!File.Exists(this.CaminhoHtmlReferencias) && !this.ConfiguracaoProjeto.IsIgnorar)
            {
                DiretorioUtil.CriarDiretorio(Path.GetDirectoryName(this.CaminhoHtmlReferencias));
                using (File.CreateText(this.CaminhoHtmlReferencias))
                {
                }
            }
        }

        #endregion

        protected override void DispensarInerno()
        {
           
            this.ArquivosTS?.Clear();
            this.NomesTipoClassBase?.Clear();
            this.ArquivosTypeScript?.Clear();
            this.ArquivosTypeScriptOrdenados?.Clear();
            this.TodosArquivos?.Clear();

            this.ArquivosTS = null;
            this.NomesTipoClassBase = null;
            this.ArquivosTypeScript = null;
            this.ArquivosTypeScriptOrdenados = null;
            this.TodosArquivos = null;
        }
         
        public override void InscrementarVersao()
        {
            base.InscrementarVersao();

            var novoVersao = this.VersaoProjeto.ToString();
            var linhas = File.ReadAllLines(this.CaminhoAplicacaoConfig, Encoding.UTF8).ToList();

            var linhaVersaoScript = $"{this.NomeProjeto}.{NOME_PROPRIEDADE_VERSAO_SCRIPT} = \"{novoVersao}\";";

            var posicaoLinhaVersaoScript = this.RetornarPosicaoLinhaVersaoScrit(linhas);
            if (posicaoLinhaVersaoScript >= 0)
            {
                linhas[posicaoLinhaVersaoScript] = linhaVersaoScript;
            }
            else
            {
                linhas.Insert(0, linhaVersaoScript);
            }

            var linhaVersao = linhas.Where(x => x.Trim().StartsWith("Versao:")).Single();
            var linhaVersaoNormalizada = linhaVersao.Replace("'", "\"");

            //var versaoAtual = ExpressaoUtil.RetornarExpressaoAbreFecha(linhaVersao, true, '"', '"', false);
            var versaoAtual = ExpressaoUtil.RetornarTextoEntrarApas(linhaVersaoNormalizada);

            var novaLinhaVersao = linhaVersao.Replace(versaoAtual, novoVersao);

            var posicaoLinhaVersao = linhas.IndexOf(linhaVersao);
            linhas[posicaoLinhaVersao] = novaLinhaVersao;

            File.WriteAllLines(this.CaminhoAplicacaoConfig, linhas.ToArray(), Encoding.UTF8);
        }

        private int RetornarPosicaoLinhaVersaoScrit(List<string> linhas)
        {
            var linha = linhas.Where(x => x.Trim().StartsWith(this.NomeProjeto + "." + NOME_PROPRIEDADE_VERSAO_SCRIPT)).SingleOrDefault();
            if (linha != null && linha.Contains(NOME_PROPRIEDADE_VERSAO_SCRIPT))
            {
                return linhas.IndexOf(linha);
            }
            return -1;
        }

        #region Métodos privados

        private void LogCaminhoSaida()
        {
            var caminhoSaida = this.CaminhoSaidaAtual;
            var caminhoRelativo = CaminhoUtil.RetornarCaminhoRelativo(caminhoSaida, this.DiretorioProjeto.FullName);
            LogVSUtil.Log(caminhoRelativo);
        }

        private string RetornarCaminhoSaidaAtual()
        {
            if (this.ConfiguracaoProjeto.CompilerOptions is CompilerOptionsFramework configuracao)
            {
                var caminhoSaidaConfigurado = Path.GetFullPath(Path.Combine(this.CaminhoProjeto, configuracao.outFile));
                if (!this.ConfiguracaoProjeto.IsIgnorar)
                {
                    if (GerenciadorProjetosUtil.DiretorioProjetoTypescriptInicializacao != null)
                    {
                        var caminhoProjetoAtual = this.RetornarCaminhoSaidaProjetoAtual();
                        if (!CaminhoUtil.CaminhoIgual(caminhoProjetoAtual, caminhoSaidaConfigurado))
                        {
                            LogVSUtil.Alerta("Caminho da saída JS compilado é diferente do padrão ." +
                                             $"\r\nConfigurado: {caminhoSaidaConfigurado}" +
                                             $"\r\nPadrão: {caminhoProjetoAtual}," +
                                             $"\r\nNormalize os projetos e se problema persistir e reinicie o visual studio; ");
                        }
                    }
                }
                return caminhoSaidaConfigurado;
            }
            throw new Erro("Tipo de projeto não suportado");
        }
        private string RetornarCaminhoSaidaProjetoAtual()
        {
            var caminhoSaida = this.CaminhoSaidaPadrao;
            if (GerenciadorProjetosUtil.DiretorioProjetoTypescriptInicializacao != null)
            {
                var nomeArquivo = Path.GetFileName(caminhoSaida);
                return Path.Combine(GerenciadorProjetosUtil.DiretorioProjetoTypescriptInicializacao,
                                   ConstantesProjeto.PASTA_BUILD,
                                   nomeArquivo);
            }
            return caminhoSaida;
        }

        #endregion
    }
}
