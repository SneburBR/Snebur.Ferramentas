using Community.VisualStudio.Toolkit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Snebur.VisualStudio.ToolWindows.Output
{
    internal class AjudanteNormalizarProjetos
    {
        private static bool _isGerenciadorReiniciado;

        public static async Task NormalizarInternoAsync(bool isCompilar = false)
        {
            //isCompilar = false;
            var isSucesso = false;
            var tempo = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                if (isCompilar)
                {
                    GerenciadorProjetos.Instancia.DesativarEventosBuild();
                }

                LogVSUtil.Clear();
                LogVSUtil.Log("Normalizando");

                if (isCompilar)
                {
                    AjudanteAssembly.Clear();
                }
                AjudanteAssembly.Inicializar(isCompilar);
                 
                await VS.Solutions.SaveAllProjetsAsync();
                 
                var projetos = await AjudanteNormalizarProjetos.RetornarProjetosAsync();
                if (projetos?.Count > 0)
                {
                    
                    await SolutionUtil.DefinirProjetosInicializacaoAsync();

                    LogVSUtil.Log($"Total de projetos encontrados {projetos.Count}");

                    var projetosTypeScript = projetos.OfType<ProjetoTypeScript>().ToList();
                    var projetosDominio = projetos.OfType<ProjetoDominio>().OrderBy(x => x.ConfiguracaoProjeto.PrioridadeDominio).ToList();

                    foreach (var projeto in projetosDominio)
                    {
                        await projeto.NormalizarReferenciasAsync(isCompilar);
                    }

                    var projetosContextoDados = projetos.OfType<ProjetoContextoDados>().ToList();
                    foreach (var projeto in projetosContextoDados)
                    {
                        await projeto.NormalizarReferenciasAsync(isCompilar);
                    }

                    var projetosRegrasNegocioTS = projetos.OfType<ProjetoRegrasNegocioTypeScript>().ToList();
                    foreach (var projeto in projetosRegrasNegocioTS)
                    {
                        await projeto.NormalizarReferenciasAsync(isCompilar);
                    }

                    var projetosRegrasNegocioCSharp = projetos.OfType<ProjetoRegrasNegocioCSharp>().ToList();
                    foreach (var projeto in projetosRegrasNegocioCSharp)
                    {
                        await projeto.NormalizarReferenciasAsync(isCompilar);
                    }

                    var projetosServicosTS = projetos.OfType<ProjetoServicosTypescript>().ToList();
                    foreach (var projeto in projetosServicosTS)
                    {
                        await projeto.NormalizarReferenciasAsync(isCompilar);
                    }

                    var projetosServicosDotNet = projetos.OfType<ProjetoServicosDotNet>().ToList();
                    foreach (var projeto in projetosServicosDotNet)
                    {
                        await projeto.NormalizarReferenciasAsync(isCompilar);
                    }

                    foreach (var projeto in projetosTypeScript)
                    {
                        await projeto.NormalizarReferenciasAsync(isCompilar);
                    }

                    var projetosSass = projetos.OfType<ProjetoSass>().ToList();
                    foreach (var projeto in projetosSass)
                    {
                        await projeto.NormalizarReferenciasAsync(isCompilar);
                    }

                    //GerenciadorProjetos.Reiniciar();
                    foreach (var projeto in projetos)
                    {
                        //projeto.Dispose();
                    }

                    if (!AjudanteNormalizarProjetos._isGerenciadorReiniciado &&
                        GerenciadorProjetos.Instancia.IsReiniciarGerenciadorPendente)
                    {
                        AjudanteNormalizarProjetos._isGerenciadorReiniciado = true;
                        LogVSUtil.Alerta("Reiniciando gerenciador");
                        await GerenciadorProjetos.Instancia.ReiniciarAsync();
                        return;
                    }
                    isSucesso = true;
                    ProjetoTypeScriptUtil.AtualizarScriptsDebug(projetos.OfType<ProjetoTypeScript>().ToList());
                }
            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro(ex);
            }
            finally
            {
                GerenciadorProjetos.Instancia.TtempoCompilacao?.Stop();
                if (isSucesso)
                {
                    tempo.Stop();
                    LogVSUtil.Sucesso("Normalização finalizada.", tempo);
                }
                else
                {
                    //if (!isCompilar)
                    //{
                    //    _ = this.NormalizarInternoAsync(true);
                    //}
                }
                GerenciadorProjetos.Instancia.AtivarEventosBuild();
            }
        }

        public static async Task<List<BaseProjeto>> RetornarProjetosAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var projetos = new List<BaseProjeto>();
            //var dte = await DteEx.GetDTEAsync();
            //if (dte.Solution.Count > 0)
            //{
            //var projetosVS = await ProjetoUtil.RetornarProjetosVisualStudioAsync();
            var todosProjetosVS = await VS.Solutions.GetAllProjectsAsync();
            var projetosVS = todosProjetosVS.Where(x => x.IsLoaded).ToList();
            //var UIH = (EnvDTE.UIHierarchy)dte.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer).Object;

            foreach (var projetoVS in projetosVS)
            {
                try
                {

                    //projetoVS.IsKindAsync
                    //var tipo = await projetoVS.is
                    var referecias = projetoVS.References.ToList();
                    var atibutos = await projetoVS.GetAttributeAsync("Version");
                    atibutos = await projetoVS.GetAttributeAsync("version");
                    atibutos = await projetoVS.GetAttributeAsync("debug");

                    var caminhoProjeto = projetoVS.FullPath;
                    //var caminhoProjeto = projetoVS.FullName;
                    var nomeArquivoProjeto = Path.GetFileName(caminhoProjeto);
                    //var nomeArquivoProjeto = projetoVS.FileName;
                    //var propriedadesVM = new List<PropriedadeViewModel>();

                    //projetoVS.Properties.RetornarPropriedadesViewModel();
                    var projetoVM = new ProjetoViewModel(caminhoProjeto,
                                                         projetoVS);

                     
                    if (!String.IsNullOrWhiteSpace(nomeArquivoProjeto) &&
                        (File.Exists(Path.GetFullPath(caminhoProjeto))))
                    {
                        var arquivoProjeto = new FileInfo(caminhoProjeto);
                        var diretorioProjeto = arquivoProjeto.Directory;
                        if (!arquivoProjeto.Exists)
                        {
                            throw new DirectoryNotFoundException($"O arquivo do projeto não foi encontrado {arquivoProjeto.FullName}");
                        }

                        var caminhoConfiguracaoDominio = Path.Combine(diretorioProjeto.FullName, ConstantesProjeto.CONFIGURACAO_DOMINIO);
                        var caminhoConfiguracaoTypeScript = Path.Combine(diretorioProjeto.FullName, ConstantesProjeto.CONFIGURACAO_TYPESCRIPT);
                        var caminhoConfiguracaoContextoDados = Path.Combine(diretorioProjeto.FullName, ConstantesProjeto.CONFIGURACAO_CONTEXTO_DADOS);
                        var caminhoConfiguracaoRegrasNegocio = Path.Combine(diretorioProjeto.FullName, ConstantesProjeto.CONFIGURACAO_REGRIAS_NEGOCIO);
                        var caminhoConfiguracaoSass = Path.Combine(diretorioProjeto.FullName, ConstantesProjeto.CONFIGURACAO_SASS);
                        var caminhoConfiguracaoServicos = Path.Combine(diretorioProjeto.FullName, ConstantesProjeto.CONFIGURACAO_SERVICOS);
                        var caminhoConfiguracaoWebConfig = Path.Combine(diretorioProjeto.FullName, ConstantesProjeto.CONFIGURACAO_WEB_CONFIG);
                        var caminhoConfiguracaoAppSettings = Path.Combine(diretorioProjeto.FullName, ConstantesProjeto.CONFIGURACAO_APP_SETTINGS);

                        //Domínio
                        if (File.Exists(caminhoConfiguracaoDominio) &&
                            File.Exists(caminhoConfiguracaoTypeScript))
                        {
                            throw new NotSupportedException(String.Format("Não é suportado no mesmo projetos arquivos de configuração de dominio.json e tsconfig.json juntos: Projeto {0}", projetoVS.Name));
                        }

                        if (File.Exists(caminhoConfiguracaoDominio))
                        {
                            var configuracaoDominio = ProjetoDominio.RetornarConfiguracaoDominio(caminhoConfiguracaoDominio);
                            projetos.Add(new ProjetoDominio(projetoVM,
                                                            configuracaoDominio,
                                                            arquivoProjeto,
                                                            caminhoConfiguracaoDominio));
                            LogVSUtil.Log($"Compilando o projeto {projetoVS.Name}");
                        }

                        if (File.Exists(caminhoConfiguracaoTypeScript))
                        {
                            //TypeScript
                            LogVSUtil.Log($"Projeto TypeScript encontrado : {projetoVS.Name} ");
                            var configuracao = ProjetoTypeScriptUtil.RetornarConfiguracaoProjetoTypeScript(caminhoConfiguracaoTypeScript);
                            projetos.Add(ProjetoTypeScriptUtil.RetornarProjetoTypeScript(configuracao,
                                                                                         projetoVM,
                                                                                         arquivoProjeto,
                                                                                         caminhoConfiguracaoTypeScript));
                        }

                        //ContextoDados
                        if (File.Exists(caminhoConfiguracaoContextoDados))
                        {
                            LogVSUtil.Log(String.Format("Projeto ContextoDados encontrado : {0} ", projetoVS.Name));
                            var configuracao = ProjetoContextoDados.RetornarConfiguracao(caminhoConfiguracaoContextoDados);
                            projetos.Add(new ProjetoContextoDados(projetoVM, configuracao, arquivoProjeto, caminhoConfiguracaoContextoDados));
                        }

                        //RegrasNegocio
                        if (File.Exists(caminhoConfiguracaoRegrasNegocio))
                        {
                            LogVSUtil.Log(String.Format("Projeto regra de negócios encontrado : {0} ", projetoVS.Name));

                            var configuracao = ProjetoRegrasNegocioUtil.RetornarConfiguracao(caminhoConfiguracaoRegrasNegocio);
                            var caminhoBase = Path.GetDirectoryName(caminhoConfiguracaoRegrasNegocio);
                            var caminhoExtensaoTS = configuracao.RetornarCaminhoExtensaoTypeScriptCompleto(caminhoBase);
                            var caminhoExtensaoCS = configuracao.RetornarCaminhoExtensaoCSharpCompleto(caminhoBase);
                            if (File.Exists(caminhoExtensaoTS))
                            {
                                projetos.Add(new ProjetoRegrasNegocioTypeScript(projetoVM,
                                                                                configuracao,
                                                                                arquivoProjeto,
                                                                                caminhoConfiguracaoContextoDados));
                            }

                            if (File.Exists(caminhoExtensaoCS))
                            {
                                projetos.Add(new ProjetoRegrasNegocioCSharp(projetoVM,
                                                                            configuracao,
                                                                            arquivoProjeto,
                                                                            caminhoConfiguracaoContextoDados));
                            }

                        }

                        //Serviço
                        if (File.Exists(caminhoConfiguracaoServicos))
                        {
                            LogVSUtil.Log(String.Format("Projeto serviços encontrado : {0} ", projetoVS.Name));
                            var configuracaoServicoes = ProjetoServicosTypescript.RetornarConfiguracao(caminhoConfiguracaoServicos);
                            projetos.Add(new ProjetoServicosTypescript(projetoVM,
                                                                       configuracaoServicoes,
                                                                       arquivoProjeto,
                                                                       caminhoConfiguracaoServicos));

                            projetos.Add(new ProjetoServicosDotNet(projetoVM,
                                                                    configuracaoServicoes,
                                                                    arquivoProjeto,
                                                                    caminhoConfiguracaoServicos));
                        }

                        //Sass
                        if (File.Exists(caminhoConfiguracaoSass))
                        {
                            LogVSUtil.Log(String.Format("Projeto sass encontrado : {0} ", projetoVS.Name));
                            var configuracaoSass = ProjetoSass.RetornarConfiguracao(caminhoConfiguracaoSass);
                            if (configuracaoSass != null && !configuracaoSass.IsIgnorar)
                            {
                                projetos.Add(new ProjetoSass(projetoVM,
                                                               configuracaoSass,
                                                               arquivoProjeto,
                                                               caminhoConfiguracaoSass));
                            }
                        }

                        if (File.Exists(caminhoConfiguracaoWebConfig) &&
                            File.Exists(caminhoConfiguracaoAppSettings))
                        {
                            LogVSUtil.Log(String.Format("Projeto web service : {0} ", projetoVS.Name));

                            var configuracaoWeb = ProjetoWeb.RetornarConfiguracao(caminhoConfiguracaoWebConfig,
                                                                                  caminhoConfiguracaoTypeScript);
                            switch (configuracaoWeb)
                            {
                                case ConfiguracaoProjetoWebApresentacao configuracaoProjetoWebApresentacao:

                                    var projetoTypescript = projetos.OfType<ProjetoTypeScript>().Where(x => x.ProjetoViewModel.CaminhoProjetoCsProj == projetoVS.FullPath).Single();

                                    projetos.Add(new ProjetoWebApresentacao(projetoVM,
                                                                            configuracaoProjetoWebApresentacao,
                                                                            projetoTypescript,
                                                                            arquivoProjeto,
                                                                            caminhoConfiguracaoWebConfig));

                                    break;

                                case ConfiguracaoProjetoWebService configuracaoWebService:

                                    projetos.Add(new ProjetoWebService(projetoVM,
                                                                       configuracaoWebService,
                                                                       arquivoProjeto,
                                                                       caminhoConfiguracaoWebConfig));
                                    break;
                                default:
                                    break;
                            }

                        }
                    }

                }
                catch (Exception ex)
                {
                    LogVSUtil.LogErro(ex.Message);
                }
            }

            //foreach (var projeto in projetos)
            //{
            //    projeto.UniqueName = projeto.UniqueName;
            //}
            //}
            //else
            //{
            //    LogVSUtil.Log("Nenhum projeto encontrado");
            //}
            return projetos;

        }
    }
}
