using Snebur.Linq;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Snebur.VisualStudio
{
    public class ProjetoSass : BaseProjeto<ConfiguracaoProjetoSass>
    {
        private const string INICIO_SHTML = "/*#region shtml*/";
        private const string FIM_SHTML = "/*#endregion shtml*/";

        private const string INICIO_ARQUIVOS = "/*#region arquivos*/";
        private const string FIM_ARQUIVOS = "/*#endregion arquivos*/";

        private const string NOME_ARQUIVO_MIXINS_SIMPLES = "mixins.scss";
        private const string NOME_ARQUIVO_VARIAVEIS = "variaveis.scss";
        private const string NOME_ARQUIVO_PERSONALIZADO = "personalizado.scss";

        public FileInfo ArquivoEstiloCompilado { get; }
        public FileInfo ArquivoEstiloCompiladoMinificado { get; }
        public FileInfo ArquivoEstiloReferenciaSass { get; }
        private FileInfo ArquivoVariaveis { get; }
        private FileInfo ArquivoMixins { get; }
        private string CaminhoRepositorioSass { get; }
         
        private const string PREFIXO_SEM_IMPORTACAO = "_";
        private const string PREFIXO_CORES = "--";

        private HashSet<string> PrefixosReservado { get; } = new HashSet<string> { PREFIXO_SEM_IMPORTACAO, PREFIXO_SEM_IMPORTACAO };
         
        public ProjetoSass(ProjetoViewModel projetoVM, 
                             ConfiguracaoProjetoSass configuracaoProjeto,
                             FileInfo arquivoProjeto, string caminhoConfiguracao) :
                             base(projetoVM, 
                                  configuracaoProjeto,
                                  arquivoProjeto, 
                                  caminhoConfiguracao)
        {
             
            this.ArquivoEstiloReferenciaSass = new FileInfo(Path.GetFullPath(Path.Combine(this.CaminhoProjeto, this.ConfiguracaoProjeto.inputFile)));
            this.ArquivoEstiloCompilado = new FileInfo(Path.GetFullPath(Path.Combine(this.CaminhoProjeto, this.ConfiguracaoProjeto.outputFile)));
            this.ArquivoEstiloCompiladoMinificado = this.RetornarArquivoMinificado();
            this.CaminhoRepositorioSass = this.ArquivoEstiloReferenciaSass.DirectoryName;
            this.ArquivoMixins = new FileInfo(Path.Combine(this.CaminhoRepositorioSass, NOME_ARQUIVO_MIXINS_SIMPLES));
            this.ArquivoVariaveis = new FileInfo(Path.Combine(this.CaminhoRepositorioSass, NOME_ARQUIVO_VARIAVEIS));

            this.AtualizarArquivosSass();
        }

        //private void ArquivoEstiloCompiadorWaitcher_Alterado(object sender, FileSystemEventArgs e)
        //{
        //    this.EventoArquivoEstiloCompiadorAlterado?.Invoke(sender, this.ArquivoEstiloCompilado);
        //}

        protected override void AtualizarInterno()
        {
            this.AtualizarInterno(this.ArquivoEstiloReferenciaSass, INICIO_ARQUIVOS, FIM_ARQUIVOS, true, this.IsArquivoSassNormal);
            this.AtualizarInterno(this.ArquivoEstiloReferenciaSass, INICIO_SHTML, FIM_SHTML, true, this.IsArquivoSassShtml);

            this.AtualizarInterno(this.ArquivoVariaveis, INICIO_ARQUIVOS, FIM_ARQUIVOS, false, this.IsArquivoSassVariaveis);
            this.AtualizarInterno(this.ArquivoMixins, INICIO_ARQUIVOS, FIM_ARQUIVOS, false, this.IsArquivoSassMixins);

            this.AtualizarLinkEstiloProjeto();

            BaseGerenciadoProjetos.TryIntancia?.AtualizarProjetoSass(this);
        }

        public override void InscrementarVersao()
        {
            this.NotificarPropriedadeAlterada(nameof(this.VersaoProjeto));
        }

        public HashSet<string> ArquivosScss { get; } = new HashSet<string>();
        private void AtualizarLinkEstiloProjeto()
        {
            var arquivosScss = this.RetornaArquivosScss(this.IsArquivoSassShtml);

            var arquivosEstilo = this.RetornarArquivosEstilo();
            foreach (var arquivo in arquivosScss)
            {
                var caminhoArquivoShtml = Path.Combine(arquivo.DirectoryName, Path.GetFileNameWithoutExtension(arquivo.FullName));
                if (File.Exists(caminhoArquivoShtml))
                {
                    var conteudo = File.ReadAllText(caminhoArquivoShtml, Encoding.UTF8);
                    var linhas = conteudo.ToLines();
                    var linhasCabecalho = TextoUtil.RetornarIntervaloLinhas(linhas, "<head>", "</head>");
                    if (linhasCabecalho.Count > 1)
                    {
                        var linhasRemover = linhasCabecalho.Where(x => x.Contains("<link") && this.IsLinhaEstiloRemover(arquivosEstilo, x)).ToList();

                        linhas.RemoveRange(linhasRemover);

                        foreach (var arquivoEstilo in arquivosEstilo)
                        {
                            var caminhoRelativo = CaminhoUtil.RetornarCaminhoRelativo(arquivoEstilo.FullName, arquivo.DirectoryName);


                            var primeiraLinha = linhasCabecalho.First();
                            var tabulacao = primeiraLinha.Substring(0, primeiraLinha.IndexOf("<head>")) + "    ";

                            var linhaEstilo = $"{tabulacao}<link href=\"{caminhoRelativo}\" rel=\"stylesheet\" />";

                            var posicaoCabecalhoFim = linhas.IndexOf(linhasCabecalho.Last());
                            linhas.Insert(posicaoCabecalhoFim, linhaEstilo);
                        }

                        var novoConteudo = String.Join(System.Environment.NewLine, linhas);
                        if (novoConteudo.Trim() != conteudo.Trim())
                        {
                            ArquivoUtil.SalvarArquivoTexto(caminhoArquivoShtml, novoConteudo);
                        }
                    }
                }
            }


            this.AtualizarArquivosSass();


        }

        private void AtualizarArquivosSass()
        {
            this.ArquivosScss.Clear();
            var arquivosScss = this.RetornaArquivosScss((f) => true);
            foreach (var arquivoScss in arquivosScss)
            {
                this.ArquivosScss.Add(arquivoScss.Name.ToLower());
            }
        }

        private bool IsLinhaEstiloRemover(List<FileInfo> arquivosEstilo, string linha)
        {
            var nomesArquivos = arquivosEstilo.Select(x => x.Name.ToLower()).ToList();
            const string procurarhref = "href=";
            var parcial = linha.Substring(linha.IndexOf(procurarhref) + procurarhref.Length);
            var aspa = parcial[0];
            parcial = parcial.Substring(1);
            var caminho = parcial.Substring(0, Math.Max( parcial.IndexOf(aspa), 0));
            if (Uri.TryCreate(caminho, UriKind.RelativeOrAbsolute, out Uri url))
            {
                caminho = url.IsAbsoluteUri ? url.AbsolutePath : caminho;

                var nomeArquivo = Path.GetFileName(caminho).ToLower();
                var resultado = nomesArquivos.Contains(nomeArquivo);
                return resultado;
            }
            return true;
        }

        private List<FileInfo> RetornarArquivosEstilo()
        {
            var arquivosEstilo = new List<FileInfo>();
            foreach (var caminhoParcial in this.ConfiguracaoProjeto.Depedencias.Values)
            {
                var caminhoCompleto = Path.GetFullPath(Path.Combine(this.CaminhoProjeto, caminhoParcial));
                var arquivoEstilo = new FileInfo(caminhoCompleto);
                if (arquivoEstilo.Exists)
                {
                    arquivosEstilo.Add(arquivoEstilo);
                }
            }
            arquivosEstilo.Add(this.ArquivoEstiloCompilado);
            return arquivosEstilo;
        }

        private void AtualizarInterno(FileInfo arquivoDestino, string procurarInicoRegion, string procurarFimRegion, bool isImportarVariaveisMixins, Func<FileInfo, bool> funcaoFiltroArquivoScsss)
        {
            var caminhoArquivoDestino = arquivoDestino.FullName;
            var arquivosScss = this.RetornaArquivosScss(funcaoFiltroArquivoScsss, arquivoDestino);
            var linhas = this.RetornarLinhas(caminhoArquivoDestino);

            var repositorioArquivoScss = this.CaminhoRepositorioSass;
            var arquivosScssCaminhoParcial = this.RetornarCaminhosParcial(arquivosScss, repositorioArquivoScss);

            var linhasArquivo = arquivosScssCaminhoParcial.Select(x => String.Format("@import \"{0}\";", x.Replace("\\", "\\\\"))).ToList();
            foreach (var linha in linhasArquivo)
            {
                linhas.Remove(linha);
            }

            //var linhasArquivo = arquivosScss.Select(x => String.Format("@import \"{0}\";", x.Replace("\\", "\\\\"))).ToList();
            var inicio = this.RetornarPosicao(linhas, procurarInicoRegion);
            var fim = this.RetornarPosicao(linhas, procurarFimRegion);

            linhas.RemoveRange(inicio + 1, fim - inicio - 1);
            linhas.InsertRange(inicio + 1, linhasArquivo);

            var conteudo = String.Join("\n", linhas);
            ArquivoUtil.SalvarArquivoTexto(caminhoArquivoDestino, conteudo);

            if (isImportarVariaveisMixins)
            {
                this.AtualziarImportacaoMixinsVariaveis(arquivosScss, repositorioArquivoScss);
            }

        }

        private List<string> RetornarLinhas(string caminhoArquivoDestino)
        {
            if (File.Exists(caminhoArquivoDestino))
            {
                return File.ReadAllLines(caminhoArquivoDestino, Encoding.UTF8).ToList();
            }
            return new List<string>();
        }

        private void AtualziarImportacaoMixinsVariaveis(List<FileInfo> arquivosSass, string caminhoRepositorio)
        {
            foreach (var arquivoSass in arquivosSass)
            {
                if (!CaminhoUtil.CaminhoIgual(arquivoSass.DirectoryName, caminhoRepositorio))
                {
                    var linhas = File.ReadAllLines(arquivoSass.FullName, Encoding.UTF8).ToList();
                    var existeAlteracao = false;

                    if (!arquivoSass.Name.StartsWith(PREFIXO_SEM_IMPORTACAO))
                    {
                        existeAlteracao = existeAlteracao | this.ResetarLinhasAntigas(linhas, "mixins.funcoes.scss");
                        existeAlteracao = existeAlteracao | this.ResetarLinhasAntigas(linhas, "mixins.simples.scss");

                        existeAlteracao = existeAlteracao | this.InserirLinhaImportacao(linhas, arquivoSass, this.ArquivoMixins);
                        existeAlteracao = existeAlteracao | this.InserirLinhaImportacao(linhas, arquivoSass, this.ArquivoVariaveis);
                    }

                    if (existeAlteracao)
                    {
                        var conteudo = String.Join("\n", linhas);
                        ArquivoUtil.SalvarArquivoTexto(arquivoSass.FullName, conteudo);
                    }
                }
            }
        }

        private bool ResetarLinhasAntigas(List<string> linhas, string nomeArquivoImportacao)
        {
            foreach (var itemLinha in linhas.ToList())
            {
                var linha = itemLinha.Trim();
                if (linha.StartsWith("@import") && linha.Contains(nomeArquivoImportacao))
                {
                    linhas.Remove(linha);
                    return true;
                }
            }
            return false;
        }



        private bool InserirLinhaImportacao(List<string> linhas, FileInfo arquivoSass, FileInfo arquivoImportacao)
        {
            if (arquivoImportacao.Exists)
            {
                var caminhoRelativo = CaminhoUtil.RetornarCaminhoRelativo(arquivoImportacao, arquivoSass.DirectoryName);
                var linhaImportacao = $"@import \"{caminhoRelativo}\";";

                for (var i = 0; i < linhas.Count; i++)
                {
                    var linha = linhas[i].Trim();
                    if (linha.StartsWith("@import") && linha.Contains(arquivoImportacao.Name))
                    {
                        if (linha != linhaImportacao)
                        {
                            linhas[i] = linhaImportacao;
                            return true;

                        }
                        return false;
                    }

                }
                linhas.Insert(0, linhaImportacao);
                return true;
            }
            return false;
        }

        private List<string> RetornarCaminhosParcial(List<FileInfo> arquivos, string caminhoBase)
        {
            var caminhosParcial = new List<string>();
            foreach (var caminho in arquivos)
            {
                var caminhoRelativo = CaminhoUtil.RetornarCaminhoRelativo(caminho, caminhoBase);
                caminhosParcial.Add(caminhoRelativo);
            }
            return caminhosParcial;
        }

        private List<FileInfo> RetornaArquivosScss(Func<FileInfo, bool> funcaoFiltroArquivoScsss,
                                                   FileInfo arquivoDestino = null)
        {
            
            var diretorioProjeto = new DirectoryInfo(this.CaminhoProjeto);

            //var arquivos =   diretorioProjeto.GetFiles("*.scss", SearchOption.AllDirectories);
            var xxx = LocalProjetoUtil.RetornarTodosArquivosAsync(this.ProjetoViewModel.ProjetoVS,
                                                                  this.CaminhoProjeto, true).
                                                                  GetAwaiter().
                                                                  GetResult();

            var arquivos = xxx.Select(x => new FileInfo(x)).
                                             Where(x => x.Extension == ConstantesProjeto.EXTENSAO_SASS);
             
            if (arquivoDestino != null)
            {
                return arquivos.Where(x => funcaoFiltroArquivoScsss.Invoke(x) && !CaminhoUtil.CaminhoIgual(x.FullName, arquivoDestino.FullName)
                                                && !x.Name.StartsWith(PREFIXO_CORES)).Select(x => x).ToList();
            }
            return arquivos.Where(x => funcaoFiltroArquivoScsss.Invoke(x) && !x.Name.StartsWith(PREFIXO_CORES)).Select(x => x).ToList();
        }

        private bool IsArquivoSassNormal(FileInfo arquivo)
        {
            return !arquivo.Name.Equals(this.ArquivoEstiloReferenciaSass.Name,
                StringComparison.InvariantCultureIgnoreCase) &&
                   !this.IsArquivoSassShtml(arquivo) &&
                   !this.IsArquivoSassPersonalizado(arquivo) &&
                   !this.IsArquivoSassMixins(arquivo) &&
                   !this.IsArquivoSassVariaveis(arquivo);
        }

        private bool IsArquivoSassShtml(FileInfo arquivo)
        {
            return arquivo.Name.EndsWith(ConstantesProjeto.EXTENSAO_CONTROLE_SHTML_SCSS);
        }

        private bool IsArquivoSassPersonalizado(FileInfo arquivo)
        {
            return arquivo.Name.ToLower().EndsWith(NOME_ARQUIVO_PERSONALIZADO);  //&& arquivo.Name != NOME_ARQUIVO_MIXINS_SIMPLES;
        }

        private bool IsArquivoSassMixins(FileInfo arquivo)
        {
            return arquivo.Name.EndsWith(NOME_ARQUIVO_MIXINS_SIMPLES);  //&& arquivo.Name != NOME_ARQUIVO_MIXINS_SIMPLES;
        }

        private bool IsArquivoSassVariaveis(FileInfo arquivo)
        {
            return arquivo.Name.EndsWith(NOME_ARQUIVO_VARIAVEIS); // && arquivo.Name != NOME_ARQUIVO_VARIAVEIS;
        }

        public static ConfiguracaoProjetoSass RetornarConfiguracao(string caminhoConfiguracao)
        {
            var json = File.ReadAllText(caminhoConfiguracao, UTF8Encoding.UTF8);
            var configuracoes = JsonUtil.Deserializar<List<ConfiguracaoProjetoSass>>(json).
                                 Where(x => Path.GetExtension(x.inputFile) == ".scss").ToList();

            if (configuracoes.Count == 0)
            {   
                LogVSUtil.LogErro("--------------  compilerconfig.json----------------------");
                throw new Exception("Nenhuma configuração encontrada");
            }

            configuracoes = configuracoes.Where(x => !x.IsIgnorar).ToList();
            if (configuracoes.Count == 0)
            {
                LogVSUtil.LogErro("--------------  compilerconfig.json----------------------");
                LogVSUtil.Alerta(($"Nenhuma configuração encontrada, Precisa de um projeto definido como {nameof(ConfiguracaoProjetoSass.IsIgnorar)} = false"));
            }

            if (configuracoes.Count > 1)
            {
                LogVSUtil.Alerta("--------------  compilerconfig.json----------------------");
                throw new Exception("Mais de uma configuracao de Estilo no projeto, verificar compilerconfig.json, outros projetos IsPersonalizado");
            }
            return configuracoes.SingleOrDefault();
        }

        private int RetornarPosicao(List<string> linhas, string procurar)
        {
            if (String.IsNullOrEmpty(procurar))
            {
                throw new ArgumentException($"O paramentro {nameof(procurar)} não está definido  ");
            }

            for (var i = 0; i < linhas.Count; i++)
            {
                var linha = linhas[i];
                if (linha.Trim().Contains(procurar))
                {
                    return i;
                }
            }
            linhas.Add(String.Empty);
            linhas.Add(procurar);
            return this.RetornarPosicao(linhas, procurar);
        }

        private FileInfo RetornarArquivoMinificado()
        {
            var arquivoCompilado = this.ArquivoEstiloCompilado;
            var nomeArquivoSemExtensao = Path.GetFileNameWithoutExtension(arquivoCompilado.FullName);
            var nomeArquivoMinificado = $"{nomeArquivoSemExtensao}.min{arquivoCompilado.Extension}";
            var caminho = Path.Combine(arquivoCompilado.DirectoryName, nomeArquivoMinificado);
            return new FileInfo(caminho);
        }



        protected override void DispensarInerno()
        {


        }
    }
}


