//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Snebur.Publicacao;
//using Snebur.Utilidade;
//using Snebur.VisualStudio.Utilidade;

//namespace Snebur.VisualStudio
//{
//    public class CompilarTypescriptRuntime
//    {
//        private const string CAMINHO_TSC = @"C:\Program Files (x86)\Microsoft SDKs\TypeScript\3.1\tsc.exe";
//        private const string NOME_TS_CONFIG_RUNTIME = "tsconfig.runtime.json";

//        private string CaminhoProjeto { get; }
//        public ProjetoTypeScript ProjetoTS { get; }
//        public string CaminhoSaida { get; }
//        //public ConfiguracaoProjetoTypeScriptRuntime ConfiguracaoProjetoTypeScriptRuntime { get; }
//        private string CaminhoTsconfig { get; }
//        private FileInfo Arquivo { get; }
//        private string CaminhoDeclaracao { get; }
//        private string CaminhoDeclaracaoRuntime { get; }
//        public string UrlScriptRuntime { get; }
//        public string CaminhoTipo { get; }

//        //private string 

//        public CompilarTypescriptRuntime(ProjetoTypeScript projetoTS, FileInfo arquivo)
//        {
//            this.Arquivo = arquivo;
//            this.CaminhoProjeto = projetoTS.CaminhoProjeto;
//            this.ProjetoTS = projetoTS;
//            this.CaminhoDeclaracao = this.RetornarCaminhoDeclarationFile();
//            this.CaminhoDeclaracaoRuntime = this.RetornarCaminhoDeclaracaoRuntime();
//            this.UrlScriptRuntime = this.RetornarUrlScriptRuntime();
//            this.CaminhoTipo = this.RetornarCaminhoTipoInterno();
//        }

//        private string RetornarCaminhoTipoInterno()
//        {
//            var arquivoTS = new ArquivoTypeScript(this.ProjetoTS.ConfiguracaoProjeto, this.ProjetoTS.CaminhoProjeto, this.Arquivo, 0, false);
//            return arquivoTS.CaminhoTipo;
//        }

//        private string RetornarUrlScriptRuntime()
//        {
//            return UriUtil.CombinarCaminhos(this.ProjetoTS.ConfiguracaoProjeto.UrlDesenvolvimento, "Scripts/Runtime/" + this.Arquivo.Name.Replace(".ts", ".js"));
//        }



//        private string RetornarCaminhoDeclarationFile()
//        {
//            var nomeArquivo = $"{this.ProjetoTS.NomeProjeto}.d.ts";
//            return Path.Combine(this.CaminhoProjeto,
//                                ConstantesPublicacao.NOME_PASTA_BUILD,
//                                nomeArquivo);
//        }

//        private List<string> RetornarArquivosTypescript()
//        {
//            var retorno = new List<string>();
//            retorno.Add(this.CaminhoDeclaracaoRuntime);
//            retorno.Add(this.Arquivo.FullName);

//            return retorno;
//        }

//        private bool ExisteArquivoTsconfig(DirectoryInfo d)
//        {
//            return d.EnumerateFiles().Any(x => x.Name == ProjetoUtil.CONFIGURACAO_TYPESCRIPT);
//        }

//        internal Task<bool> CompilarAsync()
//        {
//            return Task.Factory.StartNew<bool>(() =>
//          {
//              try
//              {
//                  return this.CompilarInterno();
//              }
//              catch (Exception ex)
//              {
//                  LogVSUtil.LogErro(ex);
//                  return false;
//              }

//          });

//        }

//        private bool CompilarInterno()
//        {
//            if (Directory.Exists(this.CaminhoProjeto) && File.Exists(this.CaminhoDeclaracao))
//            {
//                if (this.SalvarDeclaracaoRuntime())
//                {
//                    this.SalvarConfiguracaoTypescriptRuntime();
//                    return this.ExecuteTSCCompilar();
//                }
//            }
//            return false;
//        }

//        private bool SalvarDeclaracaoRuntime()
//        {


//            if (File.Exists(this.CaminhoDeclaracaoRuntime))
//            {
//                ArquivoUtil.DeletarArquivo(this.CaminhoDeclaracaoRuntime, false, true);
//            }

//            var linhas = File.ReadAllLines(this.CaminhoDeclaracao, Encoding.UTF8).ToList();
//            var linhasDeclaracaoRuntime = linhas.ToList();
            
//            var declaracaoClass = "class " + Path.GetFileNameWithoutExtension(this.Arquivo.Name.Replace(ExtensaoContantes.EXTENSAO_CONTROLE_SHTML_TYPESCRIPT, String.Empty));

//            var linhasClass = linhas.Where(x => x.Contains(declaracaoClass)).ToList();
//            if (linhasClass.Count == 1)
//            {
//                var linhaClass = linhasClass.Single();
//                if (!linhaClass.TrimStart().StartsWith("abstract"))
//                {
//                    var posicaoLinha = linhas.IndexOf(linhasClass.Single());
//                    var conteudo = String.Join(Environment.NewLine, linhas.Skip(posicaoLinha));
//                    var conteudoAbreFeche = ExpressaoUtil.RetornarExpressaoAbreFecha(conteudo, false, '{', '}', false);
//                    var totalLinhas = conteudoAbreFeche.TotalLinhas();

//                    linhasDeclaracaoRuntime.RemoveRange(posicaoLinha, totalLinhas);
//                    var conteudoDeclaracaoRuntime = String.Join(Environment.NewLine, linhasDeclaracaoRuntime);
//                    ArquivoUtil.SalvarTexto(this.CaminhoDeclaracaoRuntime, conteudoDeclaracaoRuntime);
//                    return true;

//                }


//            }
//            return false;
//        }

//        private string RetornarCaminhoDeclaracaoRuntime()
//        {
//            var diretorioRuntime = Path.Combine(this.CaminhoProjeto,
//                                                ConstantesPublicacao.NOME_PASTA_BUILD,
//                                                "Runtime");
//            DiretorioUtil.CriarDiretorio(diretorioRuntime);
//            var nomeArquivo = this.ProjetoTS.NomeProjeto + ".d.ts";
//            return Path.Combine(diretorioRuntime, nomeArquivo);
//        }

//        private void SalvarConfiguracaoTypescriptRuntime()
//        {
//            var diretorioRuntime = Path.Combine(this.CaminhoProjeto, ExtensaoContantes.PASTA_BUILD, "Runtime");
//            var diretorioRuntimeRelativo = CaminhoUtil.RetornarCaminhoRelativo(diretorioRuntime, this.CaminhoProjeto);
//            var caminhoTsConfigRuntime = Path.Combine(this.CaminhoProjeto, NOME_TS_CONFIG_RUNTIME);
//            var arquivosTypescript = this.RetornarArquivosTypescript();
//            var configuracaoRuntime = new ConfiguracaoProjetoTypeScriptRuntime(this.ProjetoTS.ConfiguracaoProjeto, arquivosTypescript, this.CaminhoProjeto, diretorioRuntimeRelativo, true);
//            var json = JsonUtil.Serializar(configuracaoRuntime, true);
//            ArquivoUtil.SalvarTexto(caminhoTsConfigRuntime, json);

//        }

//        private bool ExecuteTSCCompilar()
//        {
//            var caminhoTsConfigRuntime = Path.Combine(this.CaminhoProjeto, NOME_TS_CONFIG_RUNTIME);
//            var parametros = new Dictionary<string, string>();

//            parametros.Add("--project", caminhoTsConfigRuntime);



//            var p = new Process();
//            var psi = new ProcessStartInfo(CAMINHO_TSC, String.Join(" ", parametros.Select(o => o.Key + " " + o.Value)));

//            // run without showing console windows
//            psi.CreateNoWindow = true;
//            psi.UseShellExecute = false;

//            // redirects the compiler error output, so we can read
//            // and display errors if any
//            psi.RedirectStandardError = true;

//            p.StartInfo = psi;

//            p.Start();

//            // reads the error output
//            var msg = p.StandardError.ReadToEnd();
//            LogVSUtil.LogErro(msg);

//            p.WaitForExit();

//            return String.IsNullOrEmpty(msg);


//        }
//    }
//}