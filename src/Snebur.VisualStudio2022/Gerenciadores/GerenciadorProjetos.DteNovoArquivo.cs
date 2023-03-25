using EnvDTE;
using Snebur.Utilidade;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Snebur.VisualStudio.ConstantesProjeto;

namespace Snebur.VisualStudio
{
    public partial class GerenciadorProjetos
    {
        private async Task<ProjetoTypeScript> ArquivoRenomeadoAsync(FileInfo arquivo)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (!this.ExtensoesWeb.Contains(arquivo.Extension.ToLower()))
            {
                return null;
            }
            var projetoTS = await this.RetornarProjetoTSAsync(arquivo);
            if (projetoTS == null)
            {
                return null;
            }

            if (arquivo.Extension == ".shtml")
            {
                await projetoTS.SaveAsync();

                var dte = await DteEx.GetDTEAsync();
                var projetItem = dte.Solution.FindProjectItem(arquivo.FullName);
                if (projetItem != null)
                {
                    var projectItensArquivo = ProjetoDteUtil.RetornarProjectItemsArquivo(projetItem.ProjectItems, true);
                    await this.AnalisarArquivoRenomeadoEmbutidoAsync(projetoTS,
                                                               projetItem,
                                                               arquivo, projectItensArquivo,
                                                               EXTENSAO_CONTROLE_SHTML_ESTILO);

                    await this.AnalisarArquivoRenomeadoEmbutidoAsync(projetoTS,
                                                                     projetItem,
                                                                     arquivo,
                                                                     projectItensArquivo,
                                                                     EXTENSAO_CONTROLE_SHTML_TYPESCRIPT);



                }
            }
            return projetoTS;
        }


        private async Task<bool> AnalisarArquivoRenomeadoEmbutidoAsync(ProjetoTypeScript projetoTS,
                                                                       ProjectItem projecItemMestre,
                                                                       FileInfo arquivo,
                                                                       List<(ProjectItem ProjectItem, List<string> Arquivos)> projectItensArquivo,
                                                                       string extensao)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var projectesItem = projectItensArquivo.Where(x => x.Arquivos.Any(f => f.EndsWith(extensao))).ToList();
            if (projectesItem.Count == 1)
            {

                var (projectItemEmbutido, arquivos) = projectesItem.Single();
                if (arquivos.Count == 1)
                {
                    var caminhoArquivoEmbutido = arquivos.Single();
                    //remove a extensão e a extensão shtml
                    var nomeArquivoEmbutidoSemExtensao = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(caminhoArquivoEmbutido));
                    var nomeArquivoSemExtensao = Path.GetFileNameWithoutExtension(arquivo.Name);
                    if (!nomeArquivoSemExtensao.Equals(nomeArquivoEmbutidoSemExtensao, StringComparison.InvariantCultureIgnoreCase))
                    {
                        LogVSUtil.Alerta($"Arquivo renomeado {arquivo.Name}");

                        var nomeArquivoDestino = $"{nomeArquivoSemExtensao}{extensao}";
                        var caminhoDestino = Path.Combine(arquivo.DirectoryName, nomeArquivoDestino);
                        if (!File.Exists(caminhoDestino))
                        {
                            //projetoTS.TodosArquivos.Add(caminhoCodigo.ToLower());
                            //projetoTS.Arquivos.Add(caminhoCodigo);


                            ArquivoUtil.MoverArquivo(caminhoArquivoEmbutido,
                                                     caminhoDestino);

                            //caminhoArquivoEmbutido = caminhoArquivoEmbutido.ToLower();
                            //caminhoDestino = caminhoDestino.ToLower();

                            await this.RemoverArquivoAsync(projectItemEmbutido);
                            await this.AdicionaarArquivoAsync(projecItemMestre.ProjectItems, caminhoDestino);


                            projetoTS.TodosArquivos.Remove(caminhoArquivoEmbutido.ToLower());
                            projetoTS.ArquivosTS.Remove(caminhoArquivoEmbutido.ToLower());

                            projetoTS.TodosArquivos.Add(caminhoDestino);
                            if (Path.GetExtension(caminhoDestino.ToLower()) == EXTENSAO_TYPESCRIPT)
                            {
                                projetoTS.ArquivosTS.Add(caminhoDestino);
                            }
                            return true;
                        }
                    }
                }
            }
            return false;

        }

        private bool _isAdicionarArquivo = false;
        private async Task AdicionaarArquivoAsync(ProjectItems projectItems, string caminhoDestino)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            this._isAdicionarArquivo = true;
            projectItems.AddFromFile(caminhoDestino);
            this._isAdicionarArquivo = false;

        }

        private bool _isRemovendoArquivo = false;
        private async Task RemoverArquivoAsync(ProjectItem projectItemEmbutido)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            this._isRemovendoArquivo = true;
            projectItemEmbutido?.Remove();
            this._isRemovendoArquivo = false;
        }
        #region Talvez Renomear
        #endregion

        #region Talvez novo arquivo 
        private async Task DocumentoAbertoAsync(Document documento)
        {
            try
            {
                await this.AgruparDocumentoAsync(documento);
            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro(ex);
            }

        }

        private async Task AgruparDocumentoAsync(Document documento)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            this.InicializarGerenciadorSeNecessario();
            var nome = documento.Name;
            if (!String.IsNullOrWhiteSpace(nome))
            {
                var fileInfo = new FileInfo(documento.FullName);
                if (fileInfo.Exists)
                {
                    await this.AgruparArquivoAsync(fileInfo);
                }
            }
        }

        private async Task<ProjetoTypeScript> AgruparArquivoAsync(FileInfo arquivo)
        {
            if (!this.ExtensoesWeb.Contains(arquivo.Extension.ToLower()))
            {
                return null;
            }
            var projetoTS = await this.RetornarProjetoTSAsync(arquivo);
            if (projetoTS != null)
            {
                await this.AgruparArquivoAsync(projetoTS, arquivo);
            }
            return projetoTS;
        }

        private async Task AgruparArquivoAsync(ProjetoTypeScript projetoTS, FileInfo arquivo)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (projetoTS.TodosArquivos != null)
            {
                if (!projetoTS.IsNormalizado)
                {
                    await projetoTS.NormalizarReferenciasAsync();
                }

                var caminhoarArquivo = arquivo.FullName;
                LogVSUtil.Log($"Novo arquivo {arquivo.Name}");

                var dte = await DteEx.GetDTEAsync();
                var projetItem = dte.Solution.FindProjectItem(caminhoarArquivo);
                if (projetItem == null)
                {
                    LogVSUtil.Alerta($"O project item do arquivo não foi {arquivo.Name}, o arquivo pode não está no projeto");
                    return;
                }

                if (arquivo.Extension == EXTENSAO_CONTROLE_SHTML)
                {
                    var caminhoCodigo = caminhoarArquivo + EXTENSAO_TYPESCRIPT;
                    var caminhoEstilo = caminhoarArquivo + EXTENSAO_SASS;

                    var projectItemLayout = projetItem;

                    if (projectItemLayout != null)
                    {
                        projetoTS.TodosArquivos.Add(caminhoarArquivo.ToLower());
                        projetoTS.ArquivosTS.Add(caminhoarArquivo.ToLower());

                        LogVSUtil.Log($"Arquivo de layout encontrado {arquivo.Name}");

                        var projectItemCodigo = dte.Solution.FindProjectItem(caminhoCodigo);

                        if (projectItemCodigo == null)
                        {
                            LogVSUtil.LogErro($"Project item de código de {arquivo.Name} não foi encontrado");
                        }
                        else
                        {
                            projetoTS.TodosArquivos.Add(caminhoCodigo.ToLower());
                            projetoTS.ArquivosTS.Add(caminhoCodigo);
                            this.NormalizarNamespaceTS(projetoTS, caminhoCodigo);

                            LogVSUtil.Log($"Normalizando o arquivo do código {arquivo.Name}{EXTENSAO_TYPESCRIPT}");
                        }

                        var projetoItemEstilo = dte.Solution.FindProjectItem(caminhoEstilo);
                        if (projetoItemEstilo != null)
                        {
                            projetoTS.TodosArquivos.Add(caminhoEstilo.ToLower());
                            LogVSUtil.Log($"Normalizando o arquivo do estilo {arquivo.Name}{EXTENSAO_SASS}");
                        }


                        var isAgrupar = (projectItemLayout != null) &&
                                        (projectItemCodigo != null);
                        if (isAgrupar)
                        {
                            LogVSUtil.Log($"Agrupando arquivo {arquivo.Name}, {arquivo.Name}.ts, {arquivo.Name}{EXTENSAO_SASS}");

                            await this.AgruparArquivosAsync(projectItemLayout,
                                                            projectItemCodigo,
                                                            projetoItemEstilo,
                                                            caminhoarArquivo,
                                                            caminhoCodigo,
                                                            caminhoEstilo);
                        }
                    }

                }


                //if (isAtualizarProjetoTS)
                //{
                //    (projetoTS.ProjetoVS as Project)?.Save();
                //    await projetoTS.NormalizarReferenciasAsync();
                //}
            }
        }

        private async Task<ProjetoTypeScript> RetornarProjetoTSAsync(FileInfo arquivo)
        {
            if (this.ProjetosTS.Count == 0)
            {
                await this.AtualizarProjetosAsync(true);
            }

            var diretorio = arquivo.Directory.FullName.ToLower();
            var projetosTS = this.ProjetosTS.Values.Where(x => diretorio.Contains(x.CaminhoProjetoCaixaBaixa)).ToList();

            if (projetosTS.Count > 1)
            {
                projetosTS = projetosTS.Where(x => DiretorioUtil.IsDiretorioFilho(arquivo.Directory, x.DiretorioProjeto)).ToList();
            }

            if (projetosTS.Count == 1)
            {
                return projetosTS.Single();
            }

            if (projetosTS.Count > 1)
            {
                LogVSUtil.LogErro($"Mais de um gerenciador de projeto foi encontrado para o arquivo, arquivo {arquivo.Name}");
                return projetosTS.First();
            }
            LogVSUtil.Alerta($"Nenhum gerenciador de projeto foi inicializado ainda, arquivo {arquivo.Name} ignorado, aguarde a solução carregada por completo");
            return null;
        }

        private async Task AgruparArquivosAsync(ProjectItem projectItemLayout,
                                         ProjectItem projectItemCodigo,
                                         ProjectItem projetoItemEstilo,
                                         string caminhoLayout,
                                         string caminhoCodigo,
                                         string caminhoEstilo)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (projectItemLayout.ProjectItems == null)
            {
                LogVSUtil.LogErro("A propriedade ProjectItems do arquivo layout não está definida");
                return;
            }

            await this.RemoverArquivoAsync(projectItemCodigo);
            await this.RemoverArquivoAsync(projetoItemEstilo);

            if (projetoItemEstilo != null)
            {
                await this.AdicionaarArquivoAsync(projectItemLayout.ProjectItems, caminhoEstilo);
            }
            await this.AdicionaarArquivoAsync(projectItemLayout.ProjectItems, caminhoCodigo);

        }

        private void NormalizarNamespaceTS(ProjetoTypeScript projetoTS, string caminhoArquivo)
        {
            var arquivo = new FileInfo(caminhoArquivo);
            if (arquivo.Exists)
            {
                var conteudo = ArquivoUtil.LerTexto(caminhoArquivo);

                var linhas = conteudo.ToLines();
                var atualizar = true;
                for (var i = 0; i < linhas.Count; i++)
                {
                    var linha = linhas[i];
                    if (linha.TrimStart().StartsWith("namespace ") && linha.EndsWith("." + arquivo.Directory.Name))
                    {
                        if (linha.Contains(projetoTS.NomeProjeto))
                        {
                            linha = linha.Substring(0, linha.IndexOf(projetoTS.NomeProjeto) + projetoTS.NomeProjeto.Length);
                            linhas[i] = linha;
                            atualizar = true;
                        }
                        break;
                    }
                }
                if (atualizar)
                {
                    var novoConteudo = String.Join(Environment.NewLine, linhas);
                    try
                    {
                        ArquivoUtil.SalvarTexto(caminhoArquivo, novoConteudo);
                    }
                    catch
                    {

                    }
                }
            }
        }

        private async Task<ProjetoTypeScript> InserirTemplateAsync(FileInfo arquivo)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var projetoTS = await this.RetornarProjetoTSAsync(arquivo);
            if (projetoTS != null)
            {
                if (arquivo.Extension == EXTENSAO_TYPESCRIPT)
                {
                    var dte = await DteEx.GetDTEAsync();
                    var projectItemCodigo = dte.Solution.FindProjectItem(arquivo.FullName);
                    if (projectItemCodigo != null)
                    {
                        LogVSUtil.Log($"Normalizando arquivo typescript {arquivo.Name}");

                        projetoTS.TodosArquivos.Add(arquivo.FullName.ToLower());
                        projetoTS.ArquivosTS.Add(arquivo.FullName);
                        this.InserirTemplateArquivoNovo(projetoTS, arquivo);
                    }
                }
            }
            return projetoTS;
        }

        private void InserirTemplateArquivoNovo(ProjetoTypeScript projetoTS, FileInfo arquivo)
        {
            if (arquivo.Exists)
            {
                var conteudo = ArquivoUtil.LerTexto(arquivo.FullName);
                if (String.IsNullOrWhiteSpace(conteudo))
                {
                    var nomeTipo = arquivo.Name.ToLower().StartsWith("enum") ? "enum" : "class";
                    var nomeSemExtensao = arquivo.Name.Split('.').First();

                    var sb = new StringBuilder();
                    sb.AppendLine($"namespace {projetoTS.NomeProjeto}");
                    sb.AppendLine("{");
                    sb.AppendLine($"\texport {nomeTipo} {nomeSemExtensao}");
                    sb.AppendLine("\t{");
                    sb.AppendLine("");
                    sb.AppendLine("\t}");
                    sb.AppendLine("}");

                    ArquivoUtil.SalvarTexto(arquivo.FullName, sb.ToString());
                }
            }
        }

        #endregion

        private void InicializarGerenciadorSeNecessario()
        {
            if (!this._isProjetosAtualizados)
            {
                _ = this.AtualizarProjetosAsync();
            }
        }
    }
}
