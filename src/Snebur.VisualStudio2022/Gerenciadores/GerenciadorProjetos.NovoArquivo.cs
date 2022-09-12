using EnvDTE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Utilidade;
using static Snebur.VisualStudio.ConstantesProjeto;

namespace Snebur.VisualStudio
{
    public partial class GerenciadorProjetos
    {
        //private bool _isBloqueioDocumentoAberto = false;
        //private bool _isBloqueioDocumentoSalvo = false;
        //private bool _isBloqueioMensagemArquivoAlterado = false;

        private void DocumentEvents_DocumentOpened(Document documento)
        {
            _ = this.ExecutarAsync(this.DocumentoAbertoAsync, documento);
        }

        private void DocumentEvents_DocumentSaved(Document documento)
        {
            _ = DocumentSavedAsync(documento);
        }

        private async Task DocumentSavedAsync(Document documento)
        {
            await this.ExecutarAsync(this.DocumentoSalvoAsync, documento);
            await this.ExecutarAsync(this.MensagemArquivoAlteradoAsync, documento);
        }

        #region Talvez Renomear

        private async Task DocumentoSalvoAsync(Document documento)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            this.InicializarGerenciadorSeNecessario();
            var nome = documento.Name;
            if (!String.IsNullOrWhiteSpace(nome))
            {

                var arquivo = new FileInfo(documento.FullName);
                if (arquivo.Extension == ".shtml")
                {
                    var projetItem = DTE_GLOBAL.Solution.FindProjectItem(arquivo.FullName);
                    if (projetItem != null)
                    {
                        var projectItensArquivo = ProjetoUtil.RetornarProjectItemsArquivo(projetItem.ProjectItems, true);
                        var diretorio = arquivo.Directory.FullName.ToLower();
                        var projetosTS = this.ProjetosTS.Values.Where(x => diretorio.Contains(x.CaminhoProjetoCaixaBaixa)).ToList();
                        var projetoTS = (projetosTS.Count == 1) ? projetosTS.Single() : null;

                        var isExisteAlteracao = this.AnalisarArquivoRenomeadoEmbutido(projetoTS,
                                                               projetItem,
                                                               arquivo, projectItensArquivo,
                                                               EXTENSAO_CONTROLE_SHTML_ESTILO);

                        isExisteAlteracao = this.AnalisarArquivoRenomeadoEmbutido(projetoTS,
                                                                                  projetItem,
                                                                                  arquivo,
                                                                                  projectItensArquivo,
                                                                                  EXTENSAO_CONTROLE_SHTML_TYPESCRIPT) ||
                                                                                  isExisteAlteracao;

                        if (isExisteAlteracao && projetoTS != null)
                        {
                            (projetoTS.ProjetoVS as Project)?.Save();
                            await projetoTS.NormalizarReferenciasAsync(false);

                        }
                    }
                }
            }
        }

        private bool AnalisarArquivoRenomeadoEmbutido(ProjetoTypeScript projetoTS,
                                                      ProjectItem projecItemMestre,
                                                      FileInfo arquivo,
                                                      List<(ProjectItem ProjectItem, List<string> Arquivos)> projectItensArquivo,
                                                     string extensao)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            var projectesItem = projectItensArquivo.Where(x => x.Arquivos.Any(f => f.EndsWith(extensao))).ToList();
            if (projectesItem.Count == 1)
            {

                var (projectItemEmbutido, arquivos) = projectesItem.Single();
                if (arquivos.Count == 1)
                {
                    var caminhoArquivoEmbutido = arquivos.Single();
                    //remove a extensão e a extensao shtml
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

                            projectItemEmbutido.Remove();
                            projecItemMestre.ProjectItems.AddFromFile(caminhoDestino);

                            projetoTS.TodosArquivos.Remove(caminhoArquivoEmbutido);
                            projetoTS.ArquivosTS.Remove(caminhoArquivoEmbutido);

                            projetoTS.TodosArquivos.Add(caminhoDestino);
                            if (Path.GetExtension(caminhoDestino) == EXTENSAO_TYPESCRIPT)
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
            var isForcar = !this._isInicializado;
            this.InicializarGerenciadorSeNecessario();
            var nome = documento.Name;
            if (!String.IsNullOrWhiteSpace(nome))
            {
                var arquivo = new FileInfo(documento.FullName);
                var projectItem = documento?.ProjectItem;
                if (projectItem != null)
                {
                    if (this.Extensoes.Contains(arquivo.Extension))
                    {
                        var diretorio = arquivo.Directory.FullName.ToLower();
                        var projetosTS = this.ProjetosTS.Values.Where(x => diretorio.Contains(x.CaminhoProjetoCaixaBaixa)).ToList();
                        if (projetosTS.Count == 0)
                        {
                            LogVSUtil.Alerta($"Nenhum gerenciador de projeto foi inicializado ainda, arquivo {arquivo.Name} ignorado, aguarde a solução carregada por completo");
                            return;
                        }
                        if (projetosTS.Count > 1)
                        {
                            projetosTS = projetosTS.Where(x => DiretorioUtil.IsDiretorioFilho(arquivo.Directory, x.DiretorioProjeto)).ToList();
                        }
                        if (projetosTS.Count > 1)
                        {
                            LogVSUtil.LogErro($"Mais de um gerenciador de projeto foi encontrado para o arquivo, arquivo {arquivo.Name}");
                            return;
                        }

                        var projetoTS = projetosTS.Single();
                        if (projetoTS.TodosArquivos != null)
                        {
                            var caminhoarArquivo = arquivo.FullName;

                            if (!projetoTS.TodosArquivos.Contains(caminhoarArquivo.ToLower()) || isForcar)
                            {
                                LogVSUtil.Log($"Novo arquivo {arquivo.Name}");

                                var projetItem = DTE_GLOBAL.Solution.FindProjectItem(caminhoarArquivo);
                                if (projetItem == null)
                                {
                                    LogVSUtil.Alerta($"O project item do arquivo não foi {arquivo.Name}, o arquivo pode não está no projeto");
                                    return;
                                }
                                var isAtualizarProjetoTS = false;
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

                                        var projectItemCodigo = DTE_GLOBAL.Solution.FindProjectItem(caminhoCodigo);

                                        if (projectItemCodigo == null)
                                        {
                                            LogVSUtil.LogErro($"Project item de código de {arquivo.Name} não foi encontrado");
                                        }
                                        else
                                        {
                                            projetoTS.TodosArquivos.Add(caminhoCodigo.ToLower());
                                            projetoTS.ArquivosTS.Add(caminhoCodigo);
                                            this.NormalizarNamespaceTS(projetoTS, caminhoCodigo);
                                            isAtualizarProjetoTS = true;

                                            LogVSUtil.Log($"Normalizando o arquivo do código {arquivo.Name}{EXTENSAO_TYPESCRIPT}");
                                        }

                                        var projetoItemEstilo = DTE_GLOBAL.Solution.FindProjectItem(caminhoEstilo);
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

                                            this.AgruparArquivos(projectItemLayout,
                                                                 projectItemCodigo,
                                                                 projetoItemEstilo,
                                                                 caminhoarArquivo,
                                                                 caminhoCodigo,
                                                                 caminhoEstilo);
                                        }
                                    }

                                }

                                if (arquivo.Extension == EXTENSAO_TYPESCRIPT)
                                {
                                    var projectItemCodigo = projetItem;
                                    if (projectItemCodigo != null)
                                    {
                                        LogVSUtil.Log($"Normalizando arquivo typescript {arquivo.Name}");

                                        projetoTS.TodosArquivos.Add(arquivo.FullName.ToLower());
                                        projetoTS.ArquivosTS.Add(arquivo.FullName);
                                        isAtualizarProjetoTS = true;
                                        this.InserirTemplateArquivoNovo(projetoTS, arquivo);

                                    }
                                }
                                if (isAtualizarProjetoTS)
                                {
                                    (projetoTS.ProjetoVS as Project)?.Save();
                                   await projetoTS.NormalizarReferenciasAsync(false);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AgruparArquivos(ProjectItem projectItemLayout,
                                     ProjectItem projectItemCodigo,
                                     ProjectItem projetoItemEstilo,
                                     string caminhoLayout,
                                     string caminhoCodigo,
                                     string caminhoEstilo)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (projectItemLayout.ProjectItems == null)
            {
                LogVSUtil.LogErro("A propriedade ProjectItems do arquivo layout não está definida");
                return;
            }

            projectItemCodigo.Remove();
            projetoItemEstilo?.Remove();

            if(projetoItemEstilo!= null)
            {
                projectItemLayout.ProjectItems.AddFromFile(caminhoEstilo);
            }
            
            projectItemLayout.ProjectItems.AddFromFile(caminhoCodigo);

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
                _ = this.ExecutarAsync(this.AtualizarProjetosAsync);
            }
        }
    }
}
