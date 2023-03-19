namespace Snebur.VisualStudio
{
    public partial class GerenciadorProjetos
    {
        

        //private void ObservaarProjeto( Project projetoVS)
        //      {
        //	if (this.ProjetosTS.TryGetValue(identificador, out ProjetoTypeScript projetoTS))
        //	{
        //		var tempo = System.Diagnostics.Stopwatch.StartNew();
        //		projetoTS.Normalizar(false);
        //		tempo.Stop();
        //		LogVSUtil.Sucesso($"Projeto typescript  {projetoTS.NomeProjeto} atualizado", tempo);
        //		this.ServicoDepuracao.SalvarPorta(projetoTS.CaminhoProjeto);


        //		//this.ObservarArquivo(identificador, arquivoScriptCompildo);
        //		this.ObservarArquivo(identificador, projetoTS.CaminhoProjeto, $"*{EXTENSAO_SCRIPT}", false);
        //		this.ObservarArquivo(identificador, projetoTS.CaminhoProjeto, $"*{EXTENSAO_CONTROLE_SHTML}", true);
        //		this.ObservarArquivo(identificador, projetoTS.CaminhoProjeto, $"*{EXTENSAO_TYPESCRIPT}", true);
        //	}

        //	if (this.ProjetosSass.TryGetValue(projetoVS.UniqueName, out ProjetoEstilo projetoSass))
        //	{
        //		var tempo = System.Diagnostics.Stopwatch.StartNew();
        //		projetoSass.Normalizar(false);
        //		this.ObservarArquivo(identificador, projetoSass.ArquivoEstiloCompilado);

        //		tempo.Stop();
        //		LogVSUtil.Sucesso($"Projeto sass  {projetoSass.NomeProjeto} atualizado", tempo);
        //	}
        //}
        //private void ObservarArquivo(string identificador, FileInfo arquivo)
        //{
        //	var direotrio = arquivo.DirectoryName;
        //	var filtro = $"*.{ arquivo.Extension}";
        //	this.ObservarArquivo(identificador, direotrio, arquivo.Name);
        //}

        //private void ObservarArquivo(string identificador, string caminhoDiretorio, string filtro, bool incluirSubDiretorios = false)
        //{
        //	var observadorProjeto = this.RetornarObservadorArquivoProjeto(identificador);
        //	if (!observadorProjeto.IsObservandoDiretorioFiltro(caminhoDiretorio, filtro))
        //	{
        //		var observador = new FileSystemWatcher(caminhoDiretorio, filtro);
        //		GerenciadorProjetos.EvitarGarbaCollection(observador);
        //		observador.IncludeSubdirectories = incluirSubDiretorios;
        //		observador.BeginInit();
        //		observador.Changed += this.ObservarArquivo_Alterado;
        //		observador.Error += this.ObservadorArquivo_Error;
        //		observador.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Attributes | NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.CreationTime | NotifyFilters.Security;
        //		observador.EnableRaisingEvents = true;
        //		observador.EndInit();
        //		observadorProjeto.AdicionarNovoObservador(observador);
        //	}
        //}

        //private void ObservarArquivo_Alterado(object sender, FileSystemEventArgs e)
        //{
        //    LogVSUtil.Log($"Arquivo alterado {e.Name}");

        //    if (this.IsCompilando)
        //    {
        //        this.ArquivosNotificacaoPendente.Add(e.FullPath);
        //    }
        //    else
        //    {
        //        this.NotificarArquivoAlterado(new FileInfo(e.FullPath));

        //    }
        //}

        //public static FileSystemWatcher Obsevador1 { get; private set; }
        //public static FileSystemWatcher Obsevador2 { get; private set; }
        //public static FileSystemWatcher Obsevador3 { get; private set; }
        //public static FileSystemWatcher Obsevador4 { get; private set; }
        //public static FileSystemWatcher Obsevador5 { get; private set; }
        //public static FileSystemWatcher Obsevador6 { get; private set; }
        //public static FileSystemWatcher Obsevador7 { get; private set; }
        //public static FileSystemWatcher Obsevador8 { get; private set; }
        //public static FileSystemWatcher Obsevador9 { get; private set; }
        //public static FileSystemWatcher Obsevador10 { get; private set; }
        //public static FileSystemWatcher Obsevador11 { get; private set; }
        //public static FileSystemWatcher Obsevador12 { get; private set; }
        //public static FileSystemWatcher Obsevador13 { get; private set; }
        //public static FileSystemWatcher Obsevador14 { get; private set; }
        //public static FileSystemWatcher Obsevador15 { get; private set; }
        //public static FileSystemWatcher Obsevador16 { get; private set; }
        //public static FileSystemWatcher Obsevador17 { get; private set; }
        //public static FileSystemWatcher Obsevador18 { get; private set; }
        //public static FileSystemWatcher Obsevador19 { get; private set; }
        //public static FileSystemWatcher Obsevador20 { get; private set; }

        //private static void EvitarGarbaCollection(FileSystemWatcher observador)
        //{
        //    if (GerenciadorProjetos.Obsevador1 == null)
        //    {
        //        GerenciadorProjetos.Obsevador1 = observador;
        //        return;
        //    }

        //    if (GerenciadorProjetos.Obsevador2 == null)
        //    {
        //        GerenciadorProjetos.Obsevador2 = observador;
        //        return;
        //    }

        //    if (GerenciadorProjetos.Obsevador3 == null)
        //    {
        //        GerenciadorProjetos.Obsevador3 = observador;
        //        return;
        //    }

        //    if (GerenciadorProjetos.Obsevador4 == null)
        //    {
        //        GerenciadorProjetos.Obsevador4 = observador;
        //        return;
        //    }
        //    if (GerenciadorProjetos.Obsevador5 == null)
        //    {
        //        GerenciadorProjetos.Obsevador5 = observador;
        //        return;
        //    }
        //    if (GerenciadorProjetos.Obsevador6 == null)
        //    {
        //        GerenciadorProjetos.Obsevador6 = observador;
        //        return;
        //    }
        //    if (GerenciadorProjetos.Obsevador7 == null)
        //    {
        //        GerenciadorProjetos.Obsevador7 = observador;
        //        return;
        //    }
        //    if (GerenciadorProjetos.Obsevador8 == null)
        //    {
        //        GerenciadorProjetos.Obsevador8 = observador;
        //        return;
        //    }
        //    if (GerenciadorProjetos.Obsevador9 == null)
        //    {
        //        GerenciadorProjetos.Obsevador9 = observador;
        //        return;
        //    }
        //    if (GerenciadorProjetos.Obsevador10 == null)
        //    {
        //        GerenciadorProjetos.Obsevador10 = observador;
        //        return;
        //    }
        //    if (GerenciadorProjetos.Obsevador11 == null)
        //    {
        //        GerenciadorProjetos.Obsevador11 = observador;
        //        return;
        //    }
        //    if (GerenciadorProjetos.Obsevador12 == null)
        //    {
        //        GerenciadorProjetos.Obsevador12 = observador;
        //        return;
        //    }
        //    if (GerenciadorProjetos.Obsevador13 == null)
        //    {
        //        GerenciadorProjetos.Obsevador13 = observador;
        //        return;
        //    }
        //    if (GerenciadorProjetos.Obsevador14 == null)
        //    {
        //        GerenciadorProjetos.Obsevador14 = observador;
        //        return;
        //    }
        //    if (GerenciadorProjetos.Obsevador15 == null)
        //    {
        //        GerenciadorProjetos.Obsevador15 = observador;
        //        return;
        //    }
        //    if (GerenciadorProjetos.Obsevador16 == null)
        //    {
        //        GerenciadorProjetos.Obsevador16 = observador;
        //        return;
        //    }
        //    if (GerenciadorProjetos.Obsevador17 == null)
        //    {
        //        GerenciadorProjetos.Obsevador17 = observador;
        //        return;
        //    }

        //    if (GerenciadorProjetos.Obsevador18 == null)
        //    {
        //        GerenciadorProjetos.Obsevador18 = observador;
        //        return;
        //    }

        //    if (GerenciadorProjetos.Obsevador19 == null)
        //    {
        //        GerenciadorProjetos.Obsevador19 = observador;
        //        return;
        //    }

        //    if (GerenciadorProjetos.Obsevador20 == null)
        //    {
        //        GerenciadorProjetos.Obsevador20 = observador;
        //        return;
        //    }


        //    LogVSUtil.LogErro("Nao existe local statico para observador");
        //}
    }
}
