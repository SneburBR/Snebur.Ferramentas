using EnvDTE;
using EnvDTE80;
using Snebur.Depuracao;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
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
    /// <summary>
    /// Interaction logic for JanelaMigracaoControl.
    /// </summary>
    public partial class MigrationWindowControl : UserControl
    {
        private const string VERSAO = "Versao_";
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

        private static bool Inicializado = false;

        public MigrationWindowControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
            this.Loaded += this.This_Loaded;
        }

        private async void This_Loaded(object sender, RoutedEventArgs e)
        {
            await this.AtualizarProjetosAsync();
            this.PopularAmbientesServidor();

            if (!Inicializado)
            {
                Inicializado = true;
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
            if (this.CmbProjetoMigracao.SelectedItem == null)
            {
                this.CmbProjetoMigracao.SelectedItem = this.Projetos.Where(x => x.Name.Contains("Migracao") && x.Name != "Snebur.AcessoDados.Migracao").SingleOrDefault();
            }
            if (this.CmbProjetosEntidades.SelectedItem == null)
            {
                this.CmbProjetosEntidades.SelectedItem = this.Projetos.Where(x =>
                {
                    return x.Name.Contains("Entidades");
                }).LastOrDefault();
            }
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

                    ThreadUtil.ExecutarAsync(() => this.IniciarGeracaoScript(projetoMigracao, projetoEntidade));
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
            ThreadHelper.ThrowIfNotOnUIThread();
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
            return null;
        }

        private void IniciarGeracaoScript(Project projetoMigracao,
                                          Project projetoEntidades)
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
                throw new Exception($"Não foi encontrado a configuracao do 'DbMigrationsConfiguration' no projeto {projetoMigracao.Name}");
            }

            var instanciaConfiguracao = Activator.CreateInstance(tipoConfiguracao) as DbMigrationsConfiguration;
            var migrador = new DbMigrator(instanciaConfiguracao);



            this.Log(" -- Iniciando assistente de migração. isso pode deremorar um pouco. ");

            this.IniciarGeracaoScriptInterno(projetoMigracao, projetoEntidades, migrador, assemblyMigracao);
            this.Dispatcher.Invoke(this.Desocupar);
        }

        private void IniciarGeracaoScriptInterno(Project projetoMigracao, Project projetoEntidades, DbMigrator migrador, Assembly assemblyMigracao)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
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
                    this.Log(" -- Gerando scriptps");

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
                        throw new Exception(" A nomeclatura do migracao não é valida pelo padrão Snebur Versao_X.X.X.X");
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

                    this.Scripts.AddRange(scriptsGrupoArquivos);
                    this.ScriptsTransacao.AddRange(this.RetornarSripts(scriptAtualizado.Script));

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

        private void Desocupar()
        {
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

                    this.Dispatcher.Invoke(this.Desocupar);
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
                        this.Dispatcher.Invoke(this.Desocupar);
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
                throw new Exception($"Não foi encontrado a configuracao do 'DbMigrationsConfiguration' no projeto {projetoMigracao.Name}");
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

                this.Log(" -- Iniciando assistente de migração. isso pode deremorar um pouco. ");

                this.IniciarGeracaoScriptInterno(projetoMigracao, projetoEntidades, migrador, assemblyMigracao);
                this.ExecutarScripts(projetoMigracao, projetoEntidades);
            }

            if (this.Scripts.Count > 0)
            {
                this.AtualizarVersaoAssemby(projetoEntidades);
                this.AtualizarVersaoAssemby(projetoMigracao);
            }
        }

        private void ExecutarScripts(Project projetoMigracao, Project projetoEntidade)
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
            ThreadHelper.ThrowIfNotOnUIThread();

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
            var scriptNovoBancoDados = ScriptNovoBancoDados.RetornarScript(this.NomeConnectionString);
            var connectionScriptMastar = ScriptUtil.RetornarConnectionStringBancoMastar(this.NomeConnectionString);
            var conexao = new Conexao(connectionScriptMastar);
            conexao.ExecutarComando(scriptNovoBancoDados);
        }
        #region Validar a proxima migração

        private void BtnValidacaoAssembly_Click(object sender, RoutedEventArgs e)
        {
            this.ValidarMigracao();
        }

        private void BtnAdidioncarMigracao_Click(object sender, RoutedEventArgs e)
        {
            this.ValidarMigracao(true);
        }

        private void ValidarMigracao(bool adicionarMigracao = false)
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

                ThreadUtil.ExecutarAsync(() => this.IniciarValidacao(projetoMigracao, projetoEntidade, adicionarMigracao));
            }
        }
        private void IniciarValidacao(Project projetoMigracao, Project projetoEntidades, bool adicionarMigracao)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
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

                var assemblyEntidades = AjudanteAssemblyEx.RetornarAssembly(projetoEntidades);
                var assemblyMigracao = AjudanteAssemblyEx.RetornarAssembly(projetoMigracao);

                var tiposEntidade = assemblyEntidades.GetAccessibleTypes().Where(x => TipoUtil.TipoIgualOuSubTipo(x, typeof(Entidade))).ToList();

                var tiposSemAtributoTabela = tiposEntidade.Where(x => !TipoUtil.TipoPossuiAtributo(x, typeof(TabelaAttribute), true)).ToList();
                if (tiposSemAtributoTabela.Count > 0)
                {
                    throw new Exception($"O(s) tipo(s) não possui o atributo tabela  {String.Join(", ", tiposSemAtributoTabela)} ");
                }
                var tiposEntidadeNomeTabelaDiferenta = tiposEntidade.Where(x => x.Name != this.NomeTabela(x)).ToList();
                if (tiposEntidadeNomeTabelaDiferenta.Count > 0)
                {
                    throw new Exception($"O(s) tipo(s) possui o nome diferente do nome da tabela  {String.Join(", ", tiposEntidadeNomeTabelaDiferenta)}");
                }
                var tiposValidacaoUnicoComposta = tiposEntidade.Where(x => TipoUtil.TipoPossuiAtributo(x, typeof(ValidacaoUnicoCompostaAttribute), true)).ToList();
                foreach (var tipoEntidade in tiposValidacaoUnicoComposta)
                {
                    var atributos = TipoUtil.RetornarAtributos(tipoEntidade, typeof(ValidacaoUnicoCompostaAttribute), true);
                    foreach (var atributo in atributos)
                    {
                        var tipoEntidadeAtributo = (Type)ReflexaoUtil.RetornarValorPropriedade<ValidacaoUnicoCompostaAttribute>(x => x.TipoEntidade, atributo);

                        if (tipoEntidadeAtributo == null)
                        {
                            throw new Exception($"A {tipoEntidade.Name}, Construtor do atributo {nameof(ValidacaoUnicoCompostaAttribute)}, utilizar o construtor passando o tipo como primeiro argumento ");
                        }
                        if (tipoEntidade.Name != tipoEntidadeAtributo.Name)
                        {
                            throw new Exception($"O entidade {tipoEntidade.Name},atributo {nameof(ValidacaoUnicoCompostaAttribute)}, o tipo do construtor é diferente do tipo da entidade ");
                        }
                    }

                }
                var validacoes = new HashSet<string>();
                var alertas = new HashSet<string>();
                foreach (var tipoEntidade in tiposEntidade)
                {
                    var instancia = tipoEntidade.IsAbstract ? null : Activator.CreateInstance(tipoEntidade);
                    var nomeEntidade = tipoEntidade.Name;

                    foreach (var propriedade in tipoEntidade.GetProperties())
                    {
                        if (!PropriedadeUtil.PossuiAtributo(propriedade, typeof(IgnorarValidacaoRelacaoAttribute)))
                        {
                            this.ValidarPropriedade(tipoEntidade,
                                                    instancia,
                                                    propriedade,
                                                    validacoes,
                                                    alertas);
                        }
                    }

                    if (TipoUtil.TipoImplementaInterface(tipoEntidade, typeof(IDeletado)))
                    {
                        foreach (var propriedade in tipoEntidade.GetProperties().Where(x =>
                        PropriedadeUtil.PossuiAtributo(x, typeof(ValidacaoUnicoAttribute))))
                        {
                            if (!PropriedadeUtil.PossuiAtributo(propriedade, typeof(ValorDeletadoConcatenarGuidAttribute)))
                            {
                                throw new Exception($"A entidade {tipoEntidade.Name},  implementa a interface {nameof(IDeletado)}," +
                                                    $" a propriedade {propriedade.Name} posusi  {nameof(ValidacaoUnicoAttribute)}  " +
                                                    $"  adicioanar o atributo  {nameof(ValorDeletadoConcatenarGuidAttribute)}" +
                                                    $" ou  remover o atributo o {nameof(ValidacaoUnicoAttribute)} de {propriedade.Name}," +
                                                    $" e utilizar {nameof(ValidacaoUnicoCompostaAttribute)} com propriedade  a {nameof(IDeletado.DataHoraDeletado)} ");
                            }
                        }
                    }

                    //var propriedadesRelacaoNn = tipoEntidade.GetProperties().Where(x => PropriedadeUtil.PossuiAtributo(x, typeof(RelacaoNnAttribute))).ToList();
                    //foreach (var propriedade in propriedadesRelacaoNn)
                    //{
                    //    if (!PropriedadeUtil.PossuiAtributo(propriedade, typeof(NaoMapearAttribute)))
                    //    {
                    //        throw new Exception($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name} não possui o atributo {nameof(NaoMapearAttribute)} ");
                    //    }
                    //}
                }
                if (validacoes.Count > 0)
                {
                    throw new Exception(String.Join(Environment.NewLine, validacoes));
                }

                if (alertas.Count > 0)
                {
                    this.LogAlerta(String.Join(Environment.NewLine, alertas));
                }

                this.LogSucesso("Validação concluída com sucesso");

                if (adicionarMigracao)
                {
                    var proximaMigracao = this.RetornarProximaMigracao(assemblyMigracao);
                    this.LogSucesso($"Adicionando migração  {proximaMigracao}");

                    //dte.Solution.Properties.Item("StartupProject").Value = projetoMigracao.Name;

                    System.Threading.Thread.Sleep(2000);

                    dte.ExecuteCommand("View.PackageManagerConsole");
                    System.Threading.Thread.Sleep(600);
                    System.Windows.Forms.SendKeys.SendWait($"add-migration {proximaMigracao} -Project {projetoMigracao.Name} -StartupProject {projetoMigracao.Name} ");
                    System.Threading.Thread.Sleep(600);
                    System.Windows.Forms.SendKeys.SendWait("{ENTER}");
                }
            }
            catch (Exception ex)
            {
                var mensagem = ErroUtil.RetornarMensagem(ex);
                this.LogErro(mensagem);
            }
            finally
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.IsEnabled = true;
                    this.Cursor = Cursors.Arrow;
                });
            }
        }

        private void ValidarPropriedade(Type tipoEntidade,
                                        object instancia,
                                        PropertyInfo propriedade,
                                        HashSet<string> validacoes,
                                        HashSet<string> alertas)
        {

            if (!PropriedadeUtil.PossuiAtributo(propriedade, typeof(NaoMapearAttribute)))
            {
                if (TipoUtil.TipoIgualOuSubTipo(propriedade.PropertyType, typeof(Entidade)))
                {
                    if (!PropriedadeUtil.PossuiAtributo(propriedade, typeof(ChaveEstrangeiraAttribute)))
                    {
                        if (!propriedade.IsOverride())
                        {
                            validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name} não possui o atributo {nameof(ChaveEstrangeiraAttribute)} ");
                        }
                    }
                    else
                    {
                        var atributoChaveEstrangeira = PropriedadeUtil.RetornarAtributo(propriedade, typeof(ChaveEstrangeiraAttribute), true);
                        var nomePropriedade = ReflexaoUtil.RetornarNomePropriedade<ChaveEstrangeiraAttribute>(x => x.NomePropriedade);
                        var propriedadeNomePropriedade = atributoChaveEstrangeira.GetType().GetProperty(nomePropriedade);
                        var nomePropriedadeChaveEstrangeira = (string)propriedadeNomePropriedade.GetValue(atributoChaveEstrangeira);

                        //var nomePropriedadeChaveEstrangeira = (string)ReflexaoUtil.RetornarValorPropriedade<ChaveEstrangeiraAttribute>(x => x.NomePropriedade, atributo);

                        var propriedadeChaveEstrangeira = tipoEntidade.GetProperty(nomePropriedadeChaveEstrangeira);
                        if (propriedadeChaveEstrangeira == null)
                        {
                            validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name}, não foi encontrada a propriedade '{nomePropriedadeChaveEstrangeira}' da chave estrangeira ");
                        }
                        else
                        {
                            if (PropriedadeUtil.PossuiAtributo(propriedade, typeof(ValidacaoRequeridoAttribute)))
                            {
                                if (ReflexaoUtil.IsTipoNullable(propriedadeChaveEstrangeira.PropertyType))
                                {
                                    validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name} possui o atributo {nameof(ValidacaoRequeridoAttribute)}. Remover o Nullable? da propriedade chave estrangeira {propriedadeChaveEstrangeira.Name} ");
                                }
                            }
                            else
                            {
                                if (!ReflexaoUtil.IsTipoNullable(propriedadeChaveEstrangeira.PropertyType))
                                {
                                    validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name} não possui o atributo {nameof(ValidacaoRequeridoAttribute)}. Atricionar o Nullable? da propriedade chave estrangeira {propriedadeChaveEstrangeira.Name} ");
                                }
                            }
                        }
                    }

                    if (!propriedade.PropertyType.Name.Contains(propriedade.Name) &&
                        !propriedade.Name.Contains(propriedade.PropertyType.Name))
                    {
                        alertas.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name}, o nome da propriedade é diferente do seu tipo {propriedade.PropertyType.Name} ");
                    }

                    if (!PropriedadeUtil.PossuiAtributo(propriedade, typeof(RelacaoPaiAttribute)) &&
                        !PropriedadeUtil.PossuiAtributo(propriedade, typeof(RelacaoUmUmAttribute)))
                    {
                        if (!propriedade.IsOverride())
                        {
                            validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name}, não possui o atributo de relacao ");
                        }
                    }
                }
                if (TipoUtil.TipoIgualOuSubTipo(propriedade.PropertyType, typeof(string)))
                {
                    if (!PropriedadeUtil.PossuiAtributo(propriedade, typeof(ValidacaoTextoTamanhoAttribute)))
                    {
                        validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name} não possui o atributo {nameof(ValidacaoTextoTamanhoAttribute)} ");
                    }
                }
            }

            if (ReflexaoUtil.TipoRetornaColecao(propriedade.PropertyType) && propriedade.PropertyType.IsGenericType)
            {
                var tipoItemEntidade = propriedade.PropertyType.GetGenericArguments().First();
                if (TipoUtil.TipoIgualOuSubTipo(tipoItemEntidade, typeof(Entidade)))
                {
                    if (!PropriedadeUtil.PossuiAtributo(propriedade, typeof(RelacaoNnAttribute)) &&
                         !PropriedadeUtil.PossuiAtributo(propriedade, typeof(RelacaoFilhosAttribute)))
                    {
                        validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name} não possui o atributo de relação, esperado '{nameof(RelacaoFilhosAttribute)}' ou {nameof(RelacaoFilhosAttribute)} ");
                    }

                    if (propriedade.SetMethod == null || !propriedade.SetMethod.IsPublic)
                    {
                        validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name} não possui está implementado o método SET, necessário para a serialização");
                    }

                    if (PropriedadeUtil.PossuiAtributo(propriedade, typeof(RelacaoNnAttribute)) &&
                        PropriedadeUtil.PossuiAtributo(propriedade, typeof(RelacaoFilhosAttribute)))
                    {
                        validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name} possui ambos atributos, somente um é permitido '{nameof(RelacaoFilhosAttribute)}' ou {nameof(RelacaoFilhosAttribute)} ");
                    }
                    if (PropriedadeUtil.PossuiAtributo(propriedade, typeof(RelacaoNnAttribute)))
                    {
                        if (!PropriedadeUtil.PossuiAtributo(propriedade, typeof(NaoMapearAttribute)))
                        {
                            validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name} não possui o atributo {nameof(NaoMapearAttribute)} ");
                        }

                        var atributoRelacaoNn = PropriedadeUtil.RetornarAtributo(propriedade, typeof(RelacaoNnAttribute), true);
                        var nomePropriedadeTipoEntidadeRelacao = ReflexaoUtil.RetornarNomePropriedade<RelacaoNnAttribute>(x => x.TipoEntidadeRelacao);
                        var propriedadeTipoEntidadeRelacao = atributoRelacaoNn.GetType().GetProperty(nomePropriedadeTipoEntidadeRelacao);
                        var tipoEntidadeRelacao = (Type)propriedadeTipoEntidadeRelacao.GetValue(atributoRelacaoNn);

                        if (!tipoEntidadeRelacao.GetProperties().Any(x => TipoUtil.TipoIgualOuSubTipo(tipoEntidade, x.PropertyType) || TipoUtil.TipoIgualOuSubTipo(x.PropertyType, tipoEntidade)))
                        {
                            validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name}. A relação Nn. não foi encontrada a propriedade do tipo {tipoEntidade.Name} na entidade de relação {tipoEntidadeRelacao.Name}");
                        }

                        if (!tipoEntidadeRelacao.GetProperties().Any(x => TipoUtil.TipoIgualOuSubTipo(tipoItemEntidade, x.PropertyType) || TipoUtil.TipoIgualOuSubTipo(x.PropertyType, tipoItemEntidade)))
                        {
                            validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name}. A relação Nn. não foi encontrada a propriedade  do tipo {tipoItemEntidade.Name} na entidade de relação {tipoEntidadeRelacao.Name}");
                        }

                    }
                    if (PropriedadeUtil.PossuiAtributo(propriedade, typeof(RelacaoFilhosAttribute)))
                    {
                        if (!tipoItemEntidade.GetProperties().Any(x => TipoUtil.TipoIgualOuSubTipo(tipoEntidade, x.PropertyType) || TipoUtil.TipoIgualOuSubTipo(x.PropertyType, tipoEntidade)))
                        {
                            validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name}. A relação filhos. não foi encontrada a propriedade de relação na entidade {tipoItemEntidade.Name}");
                        }
                    }



                }
            }
            if (!tipoEntidade.IsAbstract)
            {
                if (ReflexaoUtil.TipoRetornaColecao(propriedade.PropertyType) && propriedade.PropertyType.IsGenericType)
                {
                    var tipoItem = propriedade.PropertyType.GetGenericArguments().First();
                    if (TipoUtil.TipoIgualOuSubTipo(tipoItem, typeof(Entidade)))
                    {
                        if (propriedade.GetValue(instancia) == null)
                        {
                            validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name},A coleção <'{tipoItem.Name}'> não foi instancia na declaração ");
                        }
                    }
                }
            }
        }

        private string RetornarProximaMigracao(Assembly assemblyMigracao)
        {
            var tipoConfiguracao = assemblyMigracao.GetAccessibleTypes().Where(x => x.IsSubclassOf(typeof(DbMigrationsConfiguration))).SingleOrDefault();
            var instanciaConfiguracao = Activator.CreateInstance(tipoConfiguracao) as DbMigrationsConfiguration;
            var migrador = new DbMigrator(instanciaConfiguracao);
            var pendentes = migrador.GetPendingMigrations().ToList();

            if (pendentes.Count > 0)
            {
                throw new Exception($"Migracao pendente {pendentes.Last()}");
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
            throw new Exception($"Não foi possivel convertrer a string '{versaoMigracao}' numa tipo {nameof(Version)}");
        }

        private string NomeTabela(Type tipoEntidade)
        {
            var atributoTabela = TipoUtil.RetornarAtributo(tipoEntidade, typeof(TabelaAttribute), true);
            return ReflexaoUtil.RetornarValorPropriedade<TabelaAttribute>(x => x.Name, atributoTabela).ToString(); ;
        }
        #endregion


    }
}