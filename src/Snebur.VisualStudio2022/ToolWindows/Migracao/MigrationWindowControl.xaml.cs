using EnvDTE;
using EnvDTE80;
using Snebur.BancoDados;
using Snebur.Depuracao;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Linq;
using Snebur.Utilidade;
using Snebur.VisualStudio.Reflexao;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;

namespace Snebur.VisualStudio
{
    public partial class MigrationWindowControl : UserControl
    {
        private const string VERSAO = "Versao_";
        private static bool IsInicializado = false;

        public static readonly DependencyProperty ProjetoSelecionadoProperty = DependencyProperty.Register("ProjetoSelecionado", typeof(Project), typeof(MigrationWindowControl), new PropertyMetadata(MigrationWindowControl.ProjetoSelecionadoAlterado));

        public ObservableCollection<Project> Projetos { get; set; } = new ObservableCollection<Project>();
        public ObservableCollection<FileInfo> ArquivosSql { get; set; } = new ObservableCollection<FileInfo>();
        public ObservableCollection<LogMensagemViewModel> Logs { get; set; } = new ObservableCollection<LogMensagemViewModel>();

        public Project ProjetoSelecionado
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                return (Project)this.GetValue(ProjetoSelecionadoProperty);
            }
            set { this.SetValue(ProjetoSelecionadoProperty, value); }
        }

        internal static void ProjetoSelecionadoAlterado(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //(d as JanelaMigracaoControl).AtualizarProjetoSelecionado();
        }

        public List<string> Scripts { get; } = new List<string>();
        public List<string> ScriptsTransacao { get; } = new List<string>();
        public Version VersaoMigracao { get; private set; }
        public Version VersaoAtual { get; set; }
        private Project ProjetoEntidadesSelecionado;
        private string NomeConnectionString { get; set; }

        public AmbienteViewModel AmbienteSelecionado { get; set; }
        public ObservableCollection<AmbienteViewModel> Ambientes { get; set; } = new ObservableCollection<AmbienteViewModel>();

        public MigrationWindowControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
            this.Loaded += this.This_Loaded;

            //this.Foreground = Repositorio.BrushWindowText;
            //this.Background = Repositorio.BrushBackground;
        }

        private async void This_Loaded(object sender, RoutedEventArgs e)
        {
            await this.AtualizarProjetosAsync();
            this.PopularAmbientesServidor();

            if (!IsInicializado)
            {
                IsInicializado = true;
                GerenciadorProjetos.SoluacaoAberta += this.GerenciadorProjeto_SolucaoAberta;
                var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
                dte.ExecuteCommand("View.PackageManagerConsole");
            }
        }

        private void PopularAmbientesServidor()
        {
            this.Ambientes.Clear();
            var ambientes = EnumUtil.RetornarValoresEnum<EnumAmbienteServidor>().ToList();
            //ambientes.Remove(EnumAmbienteServidor.Localhost);
            foreach (var ambiente in ambientes)
            {
                this.Ambientes.Add(new AmbienteViewModel(ambiente));
            }
        }

        private void GerenciadorProjeto_SolucaoAberta(object sender, EventArgs e)
        {
            _ = this.AtualizarProjetosAsync();
        }

        private void BtnAtualizarProjeots_Click(object sender, RoutedEventArgs e)
        {
            _ = this.AtualizarProjetosAsync();
        }

        private async Task AtualizarProjetosAsync()
        {
            this.Projetos.Clear();
            var projetos = await ProjetoUtil.RetornarProjetosVisualStudioAsync();
            foreach (var p in projetos)
            {
                this.Projetos.Add(p);
            }

            this.CmbProjetoMigracao.SelectedItem ??= this.Projetos.Where(x => x.Name.Contains("Migracao") && x.Name != "Snebur.AcessoDados.Migracao").FirstOrDefault();
            this.CmbProjetosEntidades.SelectedItem ??= this.Projetos.Where(x => x.Name.Contains("Entidades")).LastOrDefault();

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (this.CmbProjetoMigracao.SelectedItem is Project projetoSelecionado)
                {
                    try
                    {
                        var caminhoProjeto = new FileInfo(projetoSelecionado.FileName).Directory.FullName;
                        var diretorioSqls = new DirectoryInfo(Path.Combine(caminhoProjeto, "Migrations", "sqls"));
                        this.ArquivosSql.Clear();
                        if (diretorioSqls.Exists)
                        {
                            var arquivos = diretorioSqls.GetFiles("*.sql");
                            foreach (var arquivo in arquivos)
                            {
                                this.ArquivosSql.Add(arquivo);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogVSUtil.LogErro(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                this.LogErro(ex.Message);
            }

        }

        private void BtnGerarScript_Click(object sender, RoutedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                this.AtualizandoConnectionStringEmTempoExecucao();

                AjudanteAssembly.Inicializar();
                AppDomain.CurrentDomain.AssemblyResolve += this.CurrentDomain_AssemblyResolve;
                if (this.CmbProjetoMigracao.SelectedItem is Project projetoMigracao &&
                    this.CmbProjetosEntidades.SelectedItem is Project projetoEntidade &&
                    this.AmbienteSelecionado != null)
                {
                    this.Logs.Clear();
                    this.IsEnabled = false;
                    this.BtnExecutar.IsEnabled = false;
                    this.Cursor = Cursors.Wait;
                    this.ScriptsTransacao.Clear();
                    this.Scripts.Clear();
                    this.VersaoAtual = null;
                    this.ProjetoEntidadesSelecionado = projetoEntidade;
                    var isCompilar = this.ChkIsCompilar.IsChecked ?? false;

                    ThreadUtil.ExecutarAsync(() => this.IniciarGeracaoScript(projetoMigracao, projetoEntidade, isCompilar));
                }

            }
            catch (Exception ex)
            {
                this.LogErro(ex.Message);
            }
        }

        private void AtualizandoConnectionStringEmTempoExecucao()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                typeof(ConfigurationElementCollection).GetField("bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic)
                                               .SetValue(System.Configuration.ConfigurationManager.ConnectionStrings, false);

                if (this.CmbProjetoMigracao.SelectedItem is Project projeto)
                {
                    var caminhoProjeto = new FileInfo(projeto.FileName).Directory.FullName;
                    var nomeArquivoConfiguracao = this.RetornarNomeCaminhoAmbiente();
                    var caminhoArquivoConfiugracao = Path.Combine(caminhoProjeto, nomeArquivoConfiguracao);

                    using (var fs = StreamUtil.OpenRead(caminhoArquivoConfiugracao))
                    {
                        using (var leitor = new XmlTextReader(fs))
                        {
                            var xdoc = new XmlDocument();
                            xdoc.Load(leitor);
                            var appSettings = xdoc.SelectNodes("/connectionStrings/add");
                            foreach (XmlNode node in appSettings)
                            {
                                var name = node.Attributes["name"].Value.ToString();
                                var connectionString = node.Attributes["connectionString"].Value.ToString();
                                var providerName = node.Attributes["providerName"].Value.ToString();

                                if (System.Configuration.ConfigurationManager.ConnectionStrings[name] != null)
                                {
                                    System.Configuration.ConfigurationManager.ConnectionStrings[name].ConnectionString = connectionString;
                                }
                                else
                                {
                                    System.Configuration.ConfigurationManager.ConnectionStrings.Add(new ConnectionStringSettings(name, connectionString, providerName));
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                this.LogErro(ex.Message);
            }
        }

        private string RetornarNomeCaminhoAmbiente()
        {
            switch (this.AmbienteSelecionado.AmbienteServidor)
            {
                case EnumAmbienteServidor.Localhost:

                    //throw new NotSupportedException("O ambiente localhost não é suportado para a migração do banco de dados");
                    return "connectionStrings.Localhost.config";

                case EnumAmbienteServidor.Interno:

                    return "connectionStrings.Interno.config";

                case EnumAmbienteServidor.Teste:

                    return "connectionStrings.Teste.config";

                case EnumAmbienteServidor.Producao:

                    return "connectionStrings.Producao.config";

                default:

                    throw new Erro("O ambiente não é suportado");
            }
            throw new NotImplementedException();
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                var projeto = this.ProjetoEntidadesSelecionado;
                if (projeto != null)
                {
                    var nomeProjeto = projeto.Name;
                    if (args.Name.StartsWith(nomeProjeto, StringComparison.OrdinalIgnoreCase))
                    {
                        var caminhoProjeto = new FileInfo(projeto.FileName).Directory.FullName;
                        var caminhoDllAssembly = AjudanteAssemblyEx.RetornarCaminhoAssembly(projeto);
                        return AjudanteAssembly.RetornarAssembly(caminhoDllAssembly);
                    }
                }
            }
            catch (Exception ex)
            {
                LogVSUtil.Alerta(ex.Message);
            }
            return null;
        }

        private void IniciarGeracaoScript(Project projetoMigracao,
                                          Project projetoEntidades,
                                          bool isCompilar)
        {
            try
            {
                //ThreadHelper.ThrowIfNotOnUIThread();
                if (isCompilar)
                {
                    var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
                    this.Log($"Limpando a solução");
                    dte.Solution.SolutionBuild.Clean(true);
                    this.Log($"Compilando a solução");
                    dte.Solution.SolutionBuild.Build(true);

                    this.Log($"Compilando o projeto {projetoEntidades.Name}");
                    ProjetoUtil.CompilarProjeto(dte, projetoEntidades);
                    this.Log($"Compilando o projeto {projetoMigracao.Name}");
                    ProjetoUtil.CompilarProjeto(dte, projetoMigracao);
                    System.Threading.Thread.Sleep(1500);
                }

                var assemblyMigracao = AjudanteAssemblyEx.RetornarAssembly(projetoMigracao);
                var assemblyEntidade = AjudanteAssemblyEx.RetornarAssembly(projetoEntidades);
                var tipoConfiguracao = assemblyMigracao.GetAccessibleTypes().Where(x => x.IsSubclassOf(typeof(DbMigrationsConfiguration))).SingleOrDefault();
                if (tipoConfiguracao == null)
                {
                    throw new Exception($"Não foi encontrado a configuração do 'DbMigrationsConfiguration' no projeto {projetoMigracao.Name}");
                }

                var instanciaConfiguracao = Activator.CreateInstance(tipoConfiguracao) as DbMigrationsConfiguration;
                var migrador = new DbMigrator(instanciaConfiguracao);

                this.Log(" -- Iniciando assistente de migração. isso pode demorar um pouco. ");

                this.IniciarGeracaoScriptInterno(projetoMigracao, projetoEntidades, migrador, assemblyMigracao);

            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro(ex);
            }
            finally
            {
                _ = this.DesocuparAsync();
            }

        }

        private void IniciarGeracaoScriptInterno(Project projetoMigracao,
                                                 Project projetoEntidades,
                                                 DbMigrator migrador,
                                                 Assembly assemblyMigracao)
        {
            //ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                this.Scripts.Clear();
                this.ScriptsTransacao.Clear();
                var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;

                var caminhoProjetoMigracao = new FileInfo(projetoMigracao.FileName).Directory.FullName;

                var pendentes = migrador.GetPendingMigrations().ToList();

                if (pendentes.Count == 0)
                {
                    this.NaoExisteMigracaoPendente();
                }
                else
                {
                    this.Log(" -- Gerando scripts");

                    var tipoContexto = assemblyMigracao.GetAccessibleTypes().Where(x => x.IsSubclassOf(typeof(DbContext))).SingleOrDefault();
                    if (tipoContexto == null)
                    {
                        throw new Exception($"Não foi encontrado o contexto do 'BaseContextoEntity' no projeto {projetoMigracao.Name}");
                    }
                    var instanciaContexto = Activator.CreateInstance(tipoContexto) as DbContext;
                    this.NomeConnectionString = (string)instanciaContexto.GetType().GetProperty("NomeConnectionString").GetValue(instanciaContexto);

                    var migracaoExecutas = migrador.GetDatabaseMigrations().ToList();

                    var ultimaMigracaoExecuta = this.RetornarNomeMigracaoFormatado(migracaoExecutas.LastOrDefault());
                    var migracaoPendente = this.RetornarNomeMigracaoFormatado(migrador.GetPendingMigrations().First());

                    if (!this.ValidarNomeclaturaMigracao(migracaoPendente))
                    {
                        throw new Exception(" A nomenclatura do migração não é valida pelo padrão Snebur Versao_X.X.X.X");
                    }

                    this.VersaoMigracao = Version.Parse(migracaoPendente.Substring(VERSAO.Length));
                    this.VersaoAtual = new Version(this.VersaoMigracao.Major, this.VersaoMigracao.Minor, this.VersaoMigracao.MinorRevision, 0);

                    var ultimaVersao = this.RetornarUltimaVersao(migrador);
                    if (ultimaVersao != null && !this.ValidarSequenciaMigracao(ultimaVersao, this.RetornarVersaoMigracao(migracaoPendente)))
                    {
                        throw new Exception($" A sequencia da migração não é valida {ultimaMigracaoExecuta}, {migracaoPendente}");
                    }
                    this.Log($"Migração pendente -- {migracaoPendente}");

                    var scripter = new MigratorScriptingDecorator(migrador);
                    var script = scripter.ScriptUpdate(null, migracaoPendente);

                    var nomeArquivo = $"{migracaoPendente.Replace("_", ".")}.sql";
                    var caminhoPastaScripts = Path.Combine(caminhoProjetoMigracao, "Migrations", "Scripts");
                    DiretorioUtil.CriarDiretorio(caminhoPastaScripts);
                    var caminhoScript = Path.Combine(caminhoPastaScripts, nomeArquivo);

                    var scriptAtualizado = this.RetornarScriptAtualizado(script);
                    var scriptsGrupoArquivos = ScriptGrupoArquivos.RetonarScripts(scriptAtualizado.ListaGrupoArquivos, this.NomeConnectionString);
                    var scriptFinal = String.Concat(String.Join(Environment.NewLine, scriptsGrupoArquivos), Environment.NewLine, scriptAtualizado.Script);

                    File.WriteAllText(caminhoScript, scriptFinal);

                    var pastaMigrations = ProjetoUtil.RetornarProjetoItem(projetoMigracao.ProjectItems, "Migrations");
                    var pastaScripts = ProjetoUtil.RetornarProjetoItem(pastaMigrations.ProjectItems, "Scripts");
                    if (pastaScripts == null)
                    {
                        pastaScripts = pastaMigrations.ProjectItems.AddFromDirectory(caminhoPastaScripts);
                    }
                    var item = ProjetoUtil.RetornarProjetoItem(pastaScripts.ProjectItems, nomeArquivo);
                    if (item == null)
                    {
                        pastaScripts.ProjectItems.AddFromFileCopy(caminhoScript);
                    }
                    dte.AbrirArquivo(caminhoScript);

                    var scriptsTransacao = this.RetornarSripts(scriptAtualizado.Script);
                    var scriptsAlterDatabase = scriptsTransacao.Where(x => x.ToUpper().Trim().StartsWith("ALTER DATABASE")).ToList();
                    scriptsTransacao.RemoveRange(scriptsAlterDatabase);
                    scriptsAlterDatabase.Reverse();
                    foreach (var s in scriptsAlterDatabase)
                    {
                        this.Scripts.Insert(0, s);
                        this.ScriptsTransacao.Remove(s);
                    }

                    this.Scripts.AddRange(scriptsGrupoArquivos);
                    this.ScriptsTransacao.AddRange(scriptsTransacao);

                    this.Log(this.Scripts);
                    this.Log(this.ScriptsTransacao);

                    this.Dispatcher.Invoke(() =>
                    {
                        this.BtnExecutar.IsEnabled = true;
                    });
                }
            }
            catch (Exception ex)
            {
                this.LogErro(ex.Message);
            }
            finally
            {

            }
        }

        private string RetornarNomeMigracaoFormatado(string nomeMigracao)
        {
            if (nomeMigracao != null)
            {
                return nomeMigracao.Substring(nomeMigracao.IndexOf('_') + 1);
            }
            return null;
        }

        private bool ValidarSequenciaMigracao(Version ultimaVersao, Version versaoPendente)
        {
            return ((ultimaVersao.Revision + 1) == versaoPendente.Revision);
        }

        private bool ValidarNomeclaturaMigracao(string migracao)
        {
            if (migracao.StartsWith(VERSAO))
            {
                return Version.TryParse(migracao.Substring(VERSAO.Length), out Version versao);
            }
            return false;
        }

        private List<string> RetornarSripts(string script)
        {
            var scripts = new List<string>();
            var linhas = script.ToLines();

            var separadores = new string[] { "CREATE", "ALTER", "INSERT" };

            var linhasScriptAtual = new List<string>();
            for (var i = 0; i < linhas.Count; i++)
            {
                var linha = linhas[i];
                if (i == (linhas.Count - 1))
                {
                    linhasScriptAtual.Add(linha);
                }
                if (separadores.Any(x => linha.StartsWith(x)) || i == (linhas.Count - 1))
                {
                    var scriptIndividual = String.Join(Environment.NewLine, linhasScriptAtual);
                    if (!String.IsNullOrWhiteSpace(scriptIndividual))
                    {
                        scripts.Add(scriptIndividual);
                    }
                    linhasScriptAtual.Clear();
                }
                linhasScriptAtual.Add(linha);
            }
            return scripts;
        }

        private void NaoExisteMigracaoPendente()
        {
            this.Dispatcher.Invoke(() =>
            {
                this.LogAlerta("Não existe migração pendente");
            });
        }

        private async Task DesocuparAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            this.IsEnabled = true;
            this.Cursor = Cursors.Arrow;
            AppDomain.CurrentDomain.AssemblyResolve -= this.CurrentDomain_AssemblyResolve;
        }

        #region Atualizando script

        private ScriptAtualizado RetornarScriptAtualizado(string script)
        {
            AjudanteAssembly.Inicializar();

            var assembly = AjudanteAssemblyEx.RetornarAssembly(this.ProjetoEntidadesSelecionado);
            var todosTipos = assembly.GetAccessibleTypes().ToList();
            var tiposEntidade = todosTipos.Where(x => TipoUtil.TipoIgualOuSubTipo(x, typeof(Entidade))).ToList();

            var linhas = script.ToLines();
            var createTableEncondado = false;
            var linhaGrupoArquivoDados = String.Empty;
            var listaGrupoArquivos = new HashSet<string>();

            for (var i = 0; i < linhas.Count(); i++)
            {
                var linha = linhas[i];
                if (linha.Trim().StartsWith("CREATE TABLE"))
                {
                    var nomeGrupoArquivosDados = this.RetornarNomeGrupoArquivo(tiposEntidade, EnumTipoGrupo.Dados, linha);
                    listaGrupoArquivos.Add(nomeGrupoArquivosDados);
                    if (nomeGrupoArquivosDados != null)
                    {
                        linhaGrupoArquivoDados = $"ON [{nomeGrupoArquivosDados}] ";
                        createTableEncondado = true;
                    }
                }
                if (createTableEncondado && (linha.Trim() == ")" || linha.Contains(linhaGrupoArquivoDados)))
                {
                    createTableEncondado = false;
                    linhas[i] = $"{linha} {linhaGrupoArquivoDados}";
                }
                if (linha.Contains("CREATE INDEX"))
                {
                    var nomeGrupoArquivosIndices = this.RetornarNomeGrupoArquivo(tiposEntidade, EnumTipoGrupo.Indices, linha);
                    listaGrupoArquivos.Add(nomeGrupoArquivosIndices);
                    if (nomeGrupoArquivosIndices != null)
                    {
                        if (!linha.Contains(nomeGrupoArquivosIndices))
                        {
                            var novaLinha = $" {linha} ON [{nomeGrupoArquivosIndices}] ";
                            linhas[i] = novaLinha;
                        }
                    }
                }
            }
            var scriptAtualizado = String.Join(Environment.NewLine, linhas);
            return new ScriptAtualizado
            {
                ListaGrupoArquivos = listaGrupoArquivos,
                Script = scriptAtualizado
            };
        }

        private string RetornarNomeGrupoArquivo(List<Type> tiposEntidade, EnumTipoGrupo tipoGrupo, string linha)
        {
            var nomeTabela = linha.Split('.')[1].Split(']')[0].Replace("[", String.Empty);
            var tipo = tiposEntidade.Where(x => x.Name == nomeTabela).SingleOrDefault();
            if (tipo != null)
            {
                var atributoTabela = tipo.GetCustomAttributes(false).Where(x => TipoUtil.TipoIgualOuSubTipo(x.GetType(), typeof(TabelaAttribute))).Single();
                var nomePropriedade = this.RetornarNomePropriedadeGrupoArquivo(tipoGrupo);

                var propriedadeGrupoArquivos = atributoTabela.GetType().GetProperty(nomePropriedade);
                if (propriedadeGrupoArquivos.GetValue(atributoTabela) is string nomeGrupoArquivosDados)
                {
                    return TextoUtil.RetornarPrimeiraLetraMaiuscula(nomeGrupoArquivosDados);
                }
            }
            return null;
        }

        private string RetornarNomePropriedadeGrupoArquivo(EnumTipoGrupo tipoGrupo)
        {
            if (tipoGrupo == EnumTipoGrupo.Dados)
            {
                return ReflexaoUtil.RetornarNomePropriedade<TabelaAttribute>(x => x.GrupoArquivoDados);
            }
            if (tipoGrupo == EnumTipoGrupo.Indices)
            {
                return ReflexaoUtil.RetornarNomePropriedade<TabelaAttribute>(x => x.GrupoArquivoIndices);
            }
            throw new Exception("Tipo de grupo não suportado");
        }

        public enum EnumTipoGrupo
        {
            Dados = 1,
            Indices = 2
        }

        private class ScriptAtualizado
        {
            public string Script { get; set; }

            public HashSet<string> ListaGrupoArquivos { get; set; }
        }
        #endregion

        #region Log

        private void Log(List<string> mensagens)
        {
            foreach (var mensagem in mensagens)
            {
                this.Log(mensagem);
            }
        }

        private void Log(string mensagem)
        {
            this.Log(mensagem, EnumTipoLog.Normal);
        }

        private void LogErro(string mensagem)
        {
            var linhas = mensagem.ToLines();
            foreach (var linha in linhas)
            {
                this.Log(linha, EnumTipoLog.Erro);
            }

        }

        private void LogAlerta(string mensagem)
        {
            this.Log(mensagem, EnumTipoLog.Alerta);
        }

        private void LogSucesso(string mensagem)
        {
            this.Log(mensagem, EnumTipoLog.Sucesso);
        }

        private void Log(string mensagem, EnumTipoLog tipo)
        {
            _ = this.Dispatcher.BeginInvoke((Action)(() =>
            {
                this.Logs.Add(new LogMensagemViewModel(mensagem, tipo));
                this.ScrollLog.ScrollToBottom();
            }));

            //this.Logs.Add(new LogMensagemViewModel(mensagem, tipo));
        }

        #endregion

        private void BtnExecutar_Click(object sender, RoutedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (this.CmbProjetoMigracao.SelectedItem is Project projetoMigracao && this.CmbProjetosEntidades.SelectedItem is Project projetoEntidades)
            {
                this.Logs.Clear();
                this.Cursor = Cursors.Wait;
                this.IsEnabled = false;
                ThreadUtil.ExecutarAsync(() =>
                {
                    this.ExecutarScripts(projetoMigracao, projetoEntidades);

                    if (this.Scripts.Count > 0)
                    {
                        this.AtualizarVersaoAssemby(projetoMigracao);
                        this.AtualizarVersaoAssemby(projetoEntidades);
                    }

                    _ = this.DesocuparAsync();
                });
            };
        }

        private void BtnAtualizar_Click(object sender, RoutedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                this.AtualizandoConnectionStringEmTempoExecucao();

                AjudanteAssembly.Inicializar();
                AppDomain.CurrentDomain.AssemblyResolve += this.CurrentDomain_AssemblyResolve;
                if (this.CmbProjetoMigracao.SelectedItem is Project projetoMigracao &&
                    this.CmbProjetosEntidades.SelectedItem is Project projetoEntidade &&
                    this.AmbienteSelecionado != null)
                {
                    this.IsEnabled = false;
                    this.Logs.Clear();
                    this.ProjetoEntidadesSelecionado = projetoEntidade;

                    this.IsEnabled = false;
                    //this.BtnExecutar.IsEnabled = false;
                    this.Cursor = Cursors.Wait;

                    ThreadUtil.ExecutarAsync(() =>
                    {
                        this.ExecutarAtualizarPendente(projetoMigracao, projetoEntidade);
                        _ = this.DesocuparAsync();
                    });
                }

            }
            catch (Exception ex)
            {
                this.LogErro(ex.Message);
            }
        }

        private void ExecutarAtualizarPendente(Project projetoMigracao, Project projetoEntidades)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;

            this.Log($"Limpando a solução");
            dte.Solution.SolutionBuild.Clean(true);
            this.Log($"Compilando a solução");
            dte.Solution.SolutionBuild.Build(true);

            this.Log($"Compilando o projeto {projetoEntidades.Name}");
            ProjetoUtil.CompilarProjeto(dte, projetoEntidades);
            this.Log($"Compilando o projeto {projetoMigracao.Name}");
            ProjetoUtil.CompilarProjeto(dte, projetoMigracao);

            System.Threading.Thread.Sleep(1500);

            var assemblyMigracao = AjudanteAssemblyEx.RetornarAssembly(projetoMigracao);
            var assemblyEntidade = AjudanteAssemblyEx.RetornarAssembly(projetoEntidades);
            var tipoConfiguracao = assemblyMigracao.GetAccessibleTypes().Where(x => x.IsSubclassOf(typeof(DbMigrationsConfiguration))).SingleOrDefault();
            if (tipoConfiguracao == null)
            {
                throw new Exception($"Não foi encontrado a configuração do 'DbMigrationsConfiguration' no projeto {projetoMigracao.Name}");
            }

            var instanciaConfiguracao = Activator.CreateInstance(tipoConfiguracao) as DbMigrationsConfiguration;

            while (true)
            {

                var migrador = new DbMigrator(instanciaConfiguracao);
                var pendecias = migrador.GetPendingMigrations().ToList();

                if (pendecias.Count == 0)
                {
                    break;
                }


                this.Dispatcher.Invoke(() =>
                {
                    this.ScriptsTransacao.Clear();
                    this.Scripts.Clear();
                    this.VersaoAtual = null;
                });

                this.Log(" -- Iniciando assistente de migração. isso pode demorar um pouco. ");

                this.IniciarGeracaoScriptInterno(projetoMigracao, projetoEntidades, migrador, assemblyMigracao);
                this.ExecutarScripts(projetoMigracao, projetoEntidades);
            }

            if (this.Scripts.Count > 0)
            {
                this.AtualizarVersaoAssemby(projetoEntidades);
                this.AtualizarVersaoAssemby(projetoMigracao);
            }
        }

        private void ExecutarScripts(Project projetoMigracao,
                                     Project projetoEntidade)
        {
            try
            {
                if (!ScriptUtil.ExisteBancoDados(this.NomeConnectionString))
                {
                    this.CriarNovoBancoDados();
                }
                var conexao = new Conexao(this.NomeConnectionString);

                foreach (var script in this.Scripts)
                {
                    conexao.ExecutarComando(script);
                    this.Log(script);
                    System.Threading.Thread.Sleep(50);
                }

                conexao.ExecutarComandos(this.ScriptsTransacao, (cmd) =>
                {
                    this.Log(cmd.CommandText);
                    System.Threading.Thread.Sleep(15);
                });

                this.LogSucesso("Migração realizada com sucesso.");

            }
            catch (Exception ex)
            {
                this.LogErro(ex.Message);
            }
            finally
            {

            }
        }

        private void AtualizarVersaoAssemby(Project projeto)
        {
            //ThreadHelper.ThrowIfNotOnUIThread();

            var caminhoProjeto = new FileInfo(projeto.FileName).Directory.FullName;
            var arquivoAssemblyInfo = Path.Combine(caminhoProjeto, "Properties/AssemblyInfo.cs");
            if (File.Exists(arquivoAssemblyInfo))
            {
                var linhas = File.ReadAllLines(arquivoAssemblyInfo, Encoding.UTF8);

                for (var i = 0; i < linhas.Count(); i++)
                {
                    var linha = linhas[i];
                    if (linha.Trim().StartsWith("[assembly: AssemblyVersion"))
                    {
                        var novaLinha = $"[assembly: AssemblyVersion(\"{this.VersaoAtual.Major}.{this.VersaoAtual.Minor}.{this.VersaoAtual.Build}.{this.VersaoAtual.Revision}\")]";
                        linhas[i] = novaLinha;
                    }
                    if (linha.Trim().StartsWith("[assembly: AssemblyFileVersion"))
                    {
                        linhas[i] = $"[assembly: AssemblyFileVersion(\"{this.VersaoAtual.Major}.{this.VersaoAtual.Minor}.{this.VersaoAtual.Build}.{this.VersaoAtual.Revision}\")]";
                    }
                }
                File.WriteAllLines(arquivoAssemblyInfo, linhas, Encoding.UTF8);
            }
        }

        private void CriarNovoBancoDados()
        {
            var scripts = ScriptNovoBancoDados.RetornarScripts(this.NomeConnectionString);
            var connectionScriptMastar = ScriptUtil.RetornarConnectionStringBancoMastar(this.NomeConnectionString);
            var conexao = new Conexao(connectionScriptMastar);
            foreach (var script in scripts)
            {
                conexao.ExecutarComando(script);
            }

        }
        #region Validar a próxima migração

        private void BtnValidacaoAssembly_Click(object sender, RoutedEventArgs e)
        {
            this.ValidarMigracao();
        }

        private void BtnAdidioncarMigracao_Click(object sender, RoutedEventArgs e)
        {
            this.ValidarMigracao(true);
        }

        private void ValidarMigracao(bool isAdicionarMigracao = false)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            this.Logs.Clear();
            if (this.AmbienteSelecionado == null)
            {
                this.LogErro("Selecione o ambiente do servidor");
                return;
            }
            AjudanteAssembly.Inicializar();
            this.AtualizandoConnectionStringEmTempoExecucao();

            AppDomain.CurrentDomain.AssemblyResolve += this.CurrentDomain_AssemblyResolve;
            if (this.CmbProjetoMigracao.SelectedItem is Project projetoMigracao &&
                this.CmbProjetosEntidades.SelectedItem is Project projetoEntidade &&
                this.AmbienteSelecionado != null)
            {
                this.IsEnabled = false;
                this.BtnExecutar.IsEnabled = false;
                this.Cursor = Cursors.Wait;
                this.ScriptsTransacao.Clear();
                this.VersaoAtual = null;
                this.Scripts.Clear();
                this.ProjetoEntidadesSelecionado = projetoEntidade;
                var isCompilar = this.ChkIsCompilar.IsChecked ?? false;

                ThreadUtil.ExecutarAsync(() => this.IniciarValidacao(projetoMigracao,
                                                                     projetoEntidade,
                                                                     isAdicionarMigracao,
                                                                     isCompilar));
            }
        }


        //private string RetornarProximaMigracao(Assembly assemblyMigracao)
        //{
        //    this.Log("Verificando migrações pendente e retornando a próxima versão");


        //}

        private string RetornarProximaMigracao(Assembly assemblyMigracao)
        {
            this.Log("Verificando migrações pendentes");
            assemblyMigracao.ModuleResolve += (s, e) =>
            {
                if (e.Name == "")
                {

                }
                return null;
            };

            var tipoConfiguracao = assemblyMigracao.GetAccessibleTypes().Where(x => x.IsSubclassOf(typeof(DbMigrationsConfiguration))).SingleOrDefault();
            if (tipoConfiguracao == null)
            {
                throw new Exception($"Não foi encontrada a class de configuração das migrações " +
                                     $"Verifique versão do EntityFramework 6.4.4" +
                                     $"Verifique se o projeto {new AssemblyName(assemblyMigracao.FullName).Name} possui uma classe que herda de {nameof(DbMigrationsConfiguration)}");
            }
            var instanciaConfiguracao = Activator.CreateInstance(tipoConfiguracao) as DbMigrationsConfiguration;
            var migrador = new DbMigrator(instanciaConfiguracao);
            var pendentes = migrador.GetPendingMigrations().ToList();

            if (pendentes.Count > 0)
            {
                throw new Exception($"Migração pendente {pendentes.Last()}");
            }
            Version ultimaVersao = this.RetornarUltimaVersao(migrador);
            if (ultimaVersao != null)
            {
                var proximaVersao = new Version(ultimaVersao.Major, ultimaVersao.Minor, ultimaVersao.Build, ultimaVersao.Revision + 1);
                return $"Versao_{proximaVersao.Major}.{proximaVersao.Minor}.{proximaVersao.Build}.{proximaVersao.Revision}";
            }
            return "Versao_1.0.0.0";
        }

        private Version RetornarUltimaVersao(DbMigrator migrador)
        {
            var migracoesExecuta = migrador.GetDatabaseMigrations();
            if (migracoesExecuta.Count() > 0)
            {
                var versoes = migracoesExecuta.Select(x => this.RetornarVersaoMigracao(x)).ToList();
                return versoes.OrderBy(x => x.Major).ThenBy(x => x.Minor).ThenBy(x => x.Build).ThenBy(x => x.Revision).Last();
            }
            return null;
        }

        private Version RetornarVersaoMigracao(string versaoMigracao)
        {
            if (Version.TryParse(versaoMigracao.Substring(versaoMigracao.LastIndexOf('_') + 1), out Version versao))
            {
                return versao;
            }
            throw new Exception($"Não foi possível converter a string '{versaoMigracao}' numa tipo {nameof(Version)}");
        }

        private string NomeTabela(Type tipoEntidade)
        {
            var atributoTabela = TipoUtil.RetornarAtributo(tipoEntidade, typeof(TabelaAttribute), true);
            return ReflexaoUtil.RetornarValorPropriedade<TabelaAttribute>(x => x.Name, atributoTabela).ToString(); ;
        }

        #endregion


    }
}