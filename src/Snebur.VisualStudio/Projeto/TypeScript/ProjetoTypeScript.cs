using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Snebur.VisualStudio.Utilidade;
using EnvDTE;
using Snebur.Utilidade;

namespace Snebur.VisualStudio.Projeto.TypeScript
{
    public class ProjetoTypeScript : BaseProjeto
    {

        public ConfiguracaoProjetoTypeScript ConfiguracaoProjetoTypeScript { get; set; }

        public string CaminhosDiretorioTypeScripts { get; set; }

        public string CaminhoJavascriptSaida { get; set; }

        public List<FileInfo> Arquivos { get; set; }

        public FileInfo ArquivoReferenciaProjeto { get; set; }

        public List<BaseArquivoTypeScript> ArquivosTypeScript { get; set; } = new List<BaseArquivoTypeScript>();

        public List<BaseArquivoTypeScript> ArquivosTypeScriptOrdenados { get; set; } = new List<BaseArquivoTypeScript>();

        public HashSet<string> CaminhosClasseSuperior { get; set; } = new HashSet<string>();

        public Dictionary<string, int> ProjetosPrioridade { get; set; } = new Dictionary<string, int>();

        public ProjetoTypeScript(Project projetoVS,
                                 string caminhoProjeto,
                                 string caminhoConfiguracao,
                                 List<FileInfo> arquivos) : base(projetoVS,
                                                                 caminhoProjeto,
                                                                 caminhoConfiguracao)
        {

            if (!this.NomeProjeto.EndsWith(".TypeScript"))
            {
                this.NomeProjeto = this.NomeProjeto.Replace(".TypeScript", String.Empty);
                //throw new Exception(String.Format("O nome do projeto {0} não termina com .TypeScript", projetoVS.Name));
            }

            this.NomeProjeto = this.NomeProjeto.Replace(".TypeScript", String.Empty);

            this.CaminhosDiretorioTypeScripts = Path.Combine(caminhoProjeto, "TypeScript");
            this.CaminhoJavascriptSaida = this.RetornarCaminhoSaidaConfiguracao(caminhoProjeto);
            this.ConfiguracaoProjetoTypeScript = this.RetornarConfiguracaoProjetoTypeScript(caminhoConfiguracao);

            this.Arquivos = arquivos;
        }

        public override void Configurar()
        {
            this.ArquivosTypeScript = this.RetornarArquivosTypeScript();
            this.OrdenarArquivosTypeScript();

            this.ConfigurarTSConfig();
            this.ConfigurarHtmlRerefencias();
        }

        private void ConfigurarTSConfig()
        {
            var arquivosReferencias = this.ArquivosTypeScriptOrdenados.Select(x => @x.Arquivo.FullName).ToList();
            //var arquivosReferencias = this.ArquivosTypeScriptOrdenados.Select(x => FormatacaoUtils.RetornarCaminhoRelativo(x.Arquivo, this.CaminhoProjeto)).ToList();
            var caminhoJSRelativo = FormatacaoUtil.RetornarCaminhoRelativo(new FileInfo(this.CaminhoJavascriptSaida), this.CaminhoProjeto);
            var tsconfig = new ConfiguracaoProjetoTypeScript(this.ConfiguracaoProjetoTypeScript, caminhoJSRelativo, arquivosReferencias);
            this.SalvarTSConfig(tsconfig);
        }

        #region " Métodos "

        protected void OrdenarArquivosTypeScript()
        {
            while (this.ArquivosTypeScript.Count > 0)
            {
                var proximoAruqivo = this.RetornarProximoArquivoTypeScrit();
                if (proximoAruqivo.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseBase ||
                    proximoAruqivo.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseBaseAbstrata ||
                    proximoAruqivo.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseExportAbstrata ||
                    proximoAruqivo.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseExport)
                {
                    this.CaminhosClasseSuperior.Add(((ArquivoTypeScript)proximoAruqivo).CaminhoTipo);
                }
                this.ArquivosTypeScriptOrdenados.Add(proximoAruqivo);
            }
        }

        private BaseArquivoTypeScript RetornarProximoArquivoTypeScrit()
        {
            var proximo = this.ArquivosTypeScript.FirstOrDefault(
                x =>
                {
                    switch (x.TipoArquivoTypeScript)
                    {
                        case EnumTipoArquivoTypeScript.Nativo:
                        case EnumTipoArquivoTypeScript.EnumBase:
                        case EnumTipoArquivoTypeScript.EnumExport:
                        case EnumTipoArquivoTypeScript.ClassePartial:
                        case EnumTipoArquivoTypeScript.Inteface:
                        case EnumTipoArquivoTypeScript.Vazio:
                        case EnumTipoArquivoTypeScript.ClasseStatica:


                            return true;

                        case EnumTipoArquivoTypeScript.DominioAtributos:
                        case EnumTipoArquivoTypeScript.DominioClasses:
                        case EnumTipoArquivoTypeScript.DominioEnums:
                        case EnumTipoArquivoTypeScript.DominioReflexao:
                        case EnumTipoArquivoTypeScript.DominioInterfaces:
                        case EnumTipoArquivoTypeScript.DominioConstantes:

                            return true;

                        case EnumTipoArquivoTypeScript.SistemaAplicacao:
                        case EnumTipoArquivoTypeScript.SistemaAplicacaoConfig:
                        case EnumTipoArquivoTypeScript.SistemaReflexao:
                        case EnumTipoArquivoTypeScript.SistemaReferencias:
                        case EnumTipoArquivoTypeScript.SistemaVariaveis:
                        case EnumTipoArquivoTypeScript.SistemaDeclarationType:

                            return true;

                        case EnumTipoArquivoTypeScript.ClasseBase:
                        case EnumTipoArquivoTypeScript.ClasseBaseAbstrata:
                        case EnumTipoArquivoTypeScript.ClasseExport:
                        case EnumTipoArquivoTypeScript.ClasseExportAbstrata:
                        case EnumTipoArquivoTypeScript.ClasseViewModel:


                            return (String.IsNullOrWhiteSpace(x.CaminhoClasseSuperior)) ||
                                    x.CaminhoClasseSuperior == "Object" ||
                                    x.CaminhoClasseSuperior == "Error" ||
                                    x.CaminhoClasseSuperior == "Erro" ||
                                    x.CaminhoClasseSuperior == "Snebur.Dominio.BaseViewModel" ||
                                    x.CaminhoClasseSuperior == "Snebur.Dominio.EntidadeViewModel" ||
                                    this.CaminhosClasseSuperior.Contains(x.CaminhoClasseSuperior) ||
                                    this.CaminhosClasseSuperior.Contains(String.Format("{0}.{1}", x.Namespace, x.CaminhoClasseSuperior));

                        default:
                            throw new NotSupportedException("TipoArquivo não suportado ");
                    }

                });

            if (proximo == null)
            {
                var arquivosRestante = String.Join(System.Environment.NewLine, this.ArquivosTypeScript.Select(x => x.Arquivo.FullName));
                throw new Exception(String.Format("Não foi encontrado o proximo arquivo para ordenação dos arquivos TypeScript {0}. {1}{2}", this.NomeProjeto, Environment.NewLine, arquivosRestante));
            }
            this.ArquivosTypeScript.Remove(proximo);
            return proximo;

        }

        #endregion

        #region Métodos privados 

        private ConfiguracaoProjetoTypeScript RetornarConfiguracaoProjetoTypeScript(string caminhoConfiguracao)
        {
            var json = File.ReadAllText(caminhoConfiguracao, UTF8Encoding.UTF8);
            return JsonUtil.Deserializar<ConfiguracaoProjetoTypeScript>(json);
        }

        private List<BaseArquivoTypeScript> RetornarArquivosTypeScript()
        {
            var arquivosTS = new List<BaseArquivoTypeScript>();
            foreach (var arquivo in this.Arquivos)
            {

                if (arquivo.Name.EndsWith(".bkp.ts") || arquivo.Name.EndsWith(".backup.ts") || arquivo.Name.EndsWith(".copia.ts"))
                {
                    //ignorar
                    continue;
                }

                var prioridadeProjeto = this.RetornarPrioridadeProjeto(arquivo);
                if (arquivo.Name.Contains(".Dominio."))
                {
                    arquivosTS.Add(new ArquivoTSDominio(this, arquivo, prioridadeProjeto));
                }
                else if (ArquivoTSSistema.IsArquivoSistema(arquivo.Name))
                {
                    arquivosTS.Add(new ArquivoTSSistema(this, arquivo, prioridadeProjeto));
                }
                else
                {
                    arquivosTS.Add(new ArquivoTypeScript(this, arquivo, prioridadeProjeto));
                }
            }

            return arquivosTS.OrderBy(x => Convert.ToInt32(x.TipoArquivoTypeScript)).
                              ThenBy(x => x.PrioridadeProjeto).
                              ThenBy(x =>
                              {
                                  if (x is ArquivoTSDominio)
                                  {
                                      return ((ArquivoTSDominio)x).PrioridadeDominio;
                                  }
                                  return 0;
                              }).
                              ThenBy(x => x.Namespace).ToList();
        }

        private void SalvarTSConfig(ConfiguracaoProjetoTypeScript tsconfig)
        {
            var js = new System.Web.Script.Serialization.JavaScriptSerializer();
            var json = JsonUtil.Serializar(tsconfig);

            ArquivoUtil.SalvarArquivoTexto(this.CaminhoConfiguracao, json, false);
            LogMensagemUtil.Log("Arquivo de tsconfig atualizado com sucesso.");
        }

        private string RetornarCaminhoSaidaConfiguracao(string caminhoProjeto)
        {
            var caminhoRaiz = caminhoProjeto;
            if (Directory.Exists(Path.Combine(caminhoProjeto, "www")))
            {
                caminhoRaiz = Path.Combine(caminhoProjeto, "www");
            }
            return Path.Combine(caminhoRaiz, "Scripts", String.Format("{0}.js", this.NomeProjeto));
        }

        private int RetornarPrioridadeProjeto(FileInfo arquivo)
        {
            if (arquivo.Directory.FullName.Contains(this.CaminhoProjeto))
            {
                return this.ConfiguracaoProjetoTypeScript.PrioridadeProjeto;
            }
            else
            {
                DirectoryInfo diretorioAtual = arquivo.Directory;
                string caminhoTSConfig;
                do
                {
                    caminhoTSConfig = Path.Combine(diretorioAtual.FullName, ProjetoUtil.CONFIGURACAO_TYPESCRIPT);
                    diretorioAtual = diretorioAtual.Parent;
                } while (!File.Exists(caminhoTSConfig) || !(diretorioAtual != null));

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

        private int RetornarPrioridade(string CaminhoTSConfig)
        {
            var configuracao = this.RetornarConfiguracaoProjetoTypeScript(CaminhoTSConfig);
            return configuracao.PrioridadeProjeto;
        }
        #endregion

        #region Html  Referencias

        private void ConfigurarHtmlRerefencias()
        {

            var arquivosTypeSscriptPossuiHtmlRefencia = this.ArquivosTypeScriptOrdenados.OfType<ArquivoTypeScript>().Where(x => x.PossuiReferenciaHtml).ToList();
            var grupos = arquivosTypeSscriptPossuiHtmlRefencia.GroupBy(x => HtmlReferenciaUti.RetornarRepositorioHtmlsReferencias(x.Arquivo)).ToList();

            foreach (var grupo in grupos)
            {

                var caminhoRepositorioHtmlReferencia = grupo.Key;
                var arquivosTypescript = grupo.ToList();

                var sb = new StringBuilder();

                foreach (var arquivoTypeScript in arquivosTypescript)
                {
                    if (!String.IsNullOrEmpty(arquivoTypeScript.CaminhoTipo))
                    {
                        var arquivosHtmlReferencia = arquivoTypeScript.RetornarArquivosHtmlReferencia();
                        foreach (var arquivoHtmlReferencia in arquivosHtmlReferencia)
                        {
                            var urlDesenvolvimentoAbsoluta = HtmlReferenciaUti.RetornarUrlDesenvolvimentoAbsoluta(arquivoTypeScript, arquivoHtmlReferencia);
                            var urlDesenvolvimentoRelativa = HtmlReferenciaUti.RetornarUrlDesenvolvimentoRelativa(arquivoTypeScript, arquivoHtmlReferencia);
                            var html = HtmlReferenciaUti.RetornarHtml(arquivoHtmlReferencia.FullName);
                            var chave = this.RetornarCaminhoChaveHtmlReferencia(arquivoTypeScript.CaminhoTipo, arquivoHtmlReferencia);
                            sb.AppendLine(String.Format("$HtmlReferencias.Add(\"{0}\", new Snebur.UI.HtmlReferencia(\"{1}\", \"{2}\", \"{3}\" ));", chave, urlDesenvolvimentoAbsoluta, urlDesenvolvimentoRelativa, html));
                        }
                    }

                }

                sb.AppendLine();

                var nomePropriedadeCaminhoTipo = ReflexaoUtil.RetornarNomePropriedade<Snebur.Dominio.ICaminhoTipo>(x => x.__CaminhoTipo);
                foreach (var arquivoTypeScript in arquivosTypescript)
                {
                    sb.AppendLine(String.Format("{0}.{1} = \"{0}\";", arquivoTypeScript.CaminhoTipo, nomePropriedadeCaminhoTipo));
                }

                sb.AppendLine();

                foreach (var arquivoTypeScript in arquivosTypescript)
                {
                    var nome = arquivoTypeScript.NomeTipo;
                    sb.AppendLine(String.Format("$Reflexao.Tipos.Add(\"{0}\", new Snebur.Reflexao.TipoUIHtml(\"{1}\", \"{2}\"));", arquivoTypeScript.CaminhoTipo, arquivoTypeScript.NomeTipo, arquivoTypeScript.Namespace));
                }

                var conteudo = sb.ToString();

                ArquivoUtil.SalvarArquivoTexto(caminhoRepositorioHtmlReferencia, conteudo, true);

            }
        }

        private string RetornarCaminhoChaveHtmlReferencia(string caminhoArquivo, FileInfo arquivo)
        {
            if (caminhoArquivo.Contains("."))
            {
                var posicaoFinal = caminhoArquivo.LastIndexOf(".");
                var _namespace = caminhoArquivo.Substring(0, posicaoFinal);
                var chave = String.Format("{0}.{1}", _namespace, Path.GetFileNameWithoutExtension(arquivo.Name));
                return chave;
            }
            return arquivo.Name;

        }

        #endregion

    }
}
