using Community.VisualStudio.Toolkit;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Utilidade;

namespace Snebur.VisualStudio.Commands
{
    [Command(PackageIds.GoToDefinitionCommand)]
    internal sealed class GoToDefinitionCommand : BaseCommand<GoToDefinitionCommand>
    {
        private const string NOME_TIPO = "_NOME_TIPO_";

        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            try
            {
                if (SneburVisualStudio2022Package.IsVsixInialized)
                {
                    await this.ExecuteInternalAsync();
                }
            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro("GoToDefintion fail");
                LogVSUtil.LogErro(ex);
            }
        }
        private async Task ExecuteInternalAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var dte = await DteEx.GetDTEAsync();
            var documento = dte.ActiveDocument;
            if (documento != null)
            {
                var nomeArquivo = documento.Name;
                if (!nomeArquivo.EndsWith(".shtml"))
                {
                    this.GoToDefinitionNative(dte);
                    return;
                }

                var selecao = documento.Selection as TextSelection;
                if (selecao.BottomLine == selecao.TopLine && selecao.IsEmpty)
                {
                    var contador = 0;
                    string ultimaTextoSelecionado = null;

                    while (true)
                    {

                        var textoSelecionado = selecao.Text;
                        if (this.IsContinuar_EndsWith(textoSelecionado) &&
                            textoSelecionado != ultimaTextoSelecionado)
                        {
                            contador -= 1;
                            ultimaTextoSelecionado = textoSelecionado;
                            selecao.CharRight(true);
                        }
                        else
                        {
                            break;
                        }
                    }

                    var divisorFinal = selecao.Text.Contains("'") ? "'" : "\"";
                    var direita = selecao.Text.Split(divisorFinal.ToArray()).First();
                    ultimaTextoSelecionado = null;
                    while (true)
                    {
                        var textoSelecionado = selecao.Text;
                        if (this.IsContinuar_StartsWith(textoSelecionado) &&
                            textoSelecionado != ultimaTextoSelecionado)
                        {
                            ultimaTextoSelecionado = textoSelecionado;
                            contador += 1;
                            selecao.CharLeft(true);
                        }
                        else
                        {
                            break;
                        }
                    }
                    var esquerda = selecao.Text;
                    var conteudo = (esquerda + direita).Trim();
                    if (conteudo.Contains(" "))
                    {
                        conteudo = conteudo.Split(' ').Last();
                    }

                    conteudo = conteudo.Replace(">", String.Empty).
                                        Replace(" ", String.Empty).
                                        Replace("<", String.Empty).
                                        Replace("/", String.Empty).
                                        //Replace(",", String.Empty).
                                        Replace("\r\n", String.Empty);

                    if (!conteudo.Contains("="))
                    {
                        if (this.IsBind(conteudo))
                        {
                            await this.IrParaArquivoBindAsync(dte, documento, String.Empty, conteudo);
                        }
                        return;
                    }

                    //voltar ao normal
                    for (var i = 0; i < contador; i++)
                    {
                        selecao.CharRight(true);
                    }
                    selecao.Collapse();

                    var nomeAtributo = conteudo.Split('=').First().Trim();

                    var valorAtributo = conteudo.Substring(nomeAtributo.Length).
                                                  Replace("=", String.Empty).
                                                  Replace("'", String.Empty).
                                                  Replace("\"", String.Empty);



                    if (this.IsBind(valorAtributo))
                    {
                        if (conteudo.Trim().EndsWith(","))
                        {
                            ultimaTextoSelecionado = null;
                            while (true)
                            {

                                var textoSelecionado = selecao.Text;
                                if (!textoSelecionado.EndsWith("\"") &&
                                    textoSelecionado != ultimaTextoSelecionado)
                                {
                                    ultimaTextoSelecionado = textoSelecionado;
                                    selecao.CharRight(true);
                                }
                                else
                                {
                                    break;
                                }
                            }

                            var auxilares = selecao.Text;
                            if (auxilares.Contains(','))
                            {
                                auxilares = auxilares.Substring(auxilares.IndexOf(',') + 1);
                                valorAtributo += auxilares;
                            }
                        }

                        await this.IrParaArquivoBindAsync(dte, documento, nomeAtributo, valorAtributo);
                        return;
                    }

                    if (nomeAtributo == "Converter")
                    {
                        this.IrParaConverter(dte, documento, nomeAtributo, valorAtributo);
                        return;
                    }

                    nomeAtributo = nomeAtributo.ToLower();
                    if (this.AtributosArgumento.TryGetValue(nomeAtributo, out AtributoArgumento atributoArgumento))
                    {
                        var outerHTML = this.RetornarOuterHTMLElemento(selecao);

                        switch (atributoArgumento.Acao)
                        {
                            case EnumAcao.NomeControle:
                            case EnumAcao.NomeElemento:

                                this.IrParaPropriedadeDeclarada(dte,
                                                                documento.FullName,
                                                                outerHTML,
                                                                valorAtributo,
                                                                atributoArgumento);

                                break;


                            case EnumAcao.Evento:


                                this.IrParaDeclaracao(dte,
                                                      documento.FullName,
                                                      outerHTML,
                                                      valorAtributo,
                                                      atributoArgumento);
                                break;

                            case EnumAcao.ProcurarControle:

                                await this.ProcurarControleAsync(dte, valorAtributo);
                                break;

                            case EnumAcao.Metodo:

                                this.DeclararMetodo(dte,
                                                    documento.Selection as TextSelection,
                                                    valorAtributo,
                                                    atributoArgumento);
                                break;

                            default:

                                throw new Exception($"A ação {atributoArgumento.Acao.ToString()} não é suportada");
                        }
                    }
                }
            }
        }

        private void GoToDefinitionNative(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                var serviceProvider = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)dte);
                var shell = serviceProvider.GetService(typeof(SVsUIShell)) as IVsUIShell;
                if (shell != null)
                {
                    var cmd = dte.Commands.Item("Edit.GoToDefinition", 0);
                    var guid = new Guid(cmd.Guid);
                    object args = null;
                    shell.PostExecCommand(guid, (uint)cmd.ID, 0, args);
                }
                else
                {
                    dte.ExecuteCommand("Edit.GoToDefinition");
                }
            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro(ex);
            }
        }

        private bool IsContinuar_StartsWith(string textoSelecionado)
        {
            return !textoSelecionado.StartsWith(" ") &&
                   !textoSelecionado.StartsWith("\t") &&
                   !textoSelecionado.StartsWith(">") &&
                   !textoSelecionado.StartsWith("<") &&
                   !textoSelecionado.StartsWith(",") &&
                   !textoSelecionado.StartsWith("\r\n");
        }

        private bool IsContinuar_EndsWith(string textoSelecionado)
        {
            return !textoSelecionado.EndsWith(" ") &&
                   !textoSelecionado.EndsWith("\t") &&
                   !textoSelecionado.EndsWith(">") &&
                   !textoSelecionado.EndsWith("<") &&
                   !textoSelecionado.EndsWith(",") &&
                   !textoSelecionado.EndsWith("\r\n");
        }
        private void IrParaPropriedadeDeclarada(DTE2 dte,
                                                string caminhoLayout,
                                                string outerHtml,
                                                string nomeMembro,
                                                AtributoArgumento atributoArgumento)
        {

            ThreadHelper.ThrowIfNotOnUIThread();

            var caminhoCodigo = caminhoLayout + ".ts";
            this.AbrirDocumentoCodigo(dte, caminhoCodigo);

            var documentoCodigo = dte.ActiveDocument;



            if (documentoCodigo.Selection is TextSelection selecao)
            {
                this.ExisteDeclaracao(selecao,
                                      nomeMembro,
                                      false);

            }
        }

        private void IrParaDeclaracao(DTE2 dte,
                                      string caminhoLayout,
                                      string outerHtml,
                                      string valorAtributo,
                                      AtributoArgumento atributoArgumento)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var caminhoCodigo = caminhoLayout + ".ts";
            if (File.Exists(caminhoCodigo))
            {
                this.AbrirDocumentoCodigo(dte, caminhoCodigo);

                var documento = dte.ActiveDocument;
                if (documento.Selection is TextSelection selecao)
                {
                    var isMetodo = (atributoArgumento.Acao == EnumAcao.Evento) ? true : false;
                    if (!this.ExisteDeclaracao(selecao, valorAtributo, isMetodo))
                    {
                        switch (atributoArgumento.Acao)
                        {
                            case EnumAcao.Evento:

                                this.DeclararEvento(selecao, outerHtml, valorAtributo, atributoArgumento);
                                break;

                            case EnumAcao.NomeControle:

                                this.DeclararnNomeControle(selecao, outerHtml, valorAtributo, atributoArgumento);
                                break;

                            case EnumAcao.NomeElemento:

                                this.DeclararnNomeElemento(selecao, outerHtml, valorAtributo, atributoArgumento);
                                break;

                            default:

                                throw new Exception($"A ação {atributoArgumento.Acao.ToString()} não é suportada");
                        }
                    }
                }
            }
        }

        private void DeclararEvento(TextSelection selecao,
                                    string outerHTML,
                                    string nomeMetodo,
                                    AtributoArgumento atributoArgumento)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            selecao.StartOfDocument(true);

            if (selecao.FindText("export class") ||
                selecao.FindText("export abstract class"))
            {
                while (!selecao.Text.Contains("{"))
                {
                    selecao.LineDown(true);
                    selecao.SelectLine();
                }

                var tagName = outerHTML.Split(' ').First().Substring(1);
                var tipoControle = this.RetornarTipoControle(outerHTML);
                var nome = TextoUtil.RetornarPrimeiraLetraMinusculo(tipoControle.Split('.').Last());

                var selecaoAtual = selecao.Text;
                var declaracaoMetodo = $"private {nomeMetodo}({nome}: {tipoControle}, e:{atributoArgumento.NomeArgumentoEvento})";
                var sb = new StringBuilder();
                sb.AppendLine(selecaoAtual);
                sb.AppendLine($"\t\t{declaracaoMetodo}");
                sb.AppendLine("\t\t{");

                if (atributoArgumento.IsAsync)
                {
                    sb.AppendLine("\t\t\treturn new Promise<void>(async resolver =>");
                    sb.AppendLine("\t\t\t{");
                    sb.AppendLine("\t\t\t\tif(resolver) throw new ErroNaoImplementado();");
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.AppendLine("\t\t\t\tresolver();");
                    sb.AppendLine("\t\t\t});");
                }
                else
                {
                    sb.AppendLine("\t\t\tthrow new ErroNaoImplementado(this);");
                }
                 
                sb.AppendLine("\t\t}");
                //sb.AppendLine("");

                selecao.Insert(sb.ToString());

                selecao.StartOfDocument(true);
                selecao.FindText(declaracaoMetodo);
                selecao.LineDown();
                selecao.LineDown();
                selecao.StartOfLine(vsStartOfLineOptions.vsStartOfLineOptionsFirstText);
            }
        }

        private void DeclararnNomeControle(TextSelection selecao, string outerHTML, string nomeControle, AtributoArgumento atributoArgumento)
        {
            var tagName = outerHTML.Split(' ').First().Substring(1);
            var tipoControle = this.RetornarTipoControle(outerHTML);
            var declaracao = String.Format("private readonly {0}: {1};", nomeControle, tipoControle);
            this.InserirDeclaracao(selecao, declaracao);
        }

        private void DeclararnNomeElemento(TextSelection selecao, string outerHTML, string nomeElemento, AtributoArgumento atributoArgumento)
        {
            var tagName = outerHTML.Split(' ').First().Substring(1);
            var tipoElemento = this.RetornarTipoElemento(tagName);
            var declaracao = String.Format("private readonly {0}: {1};", nomeElemento, tipoElemento);
            this.InserirDeclaracao(selecao, declaracao);
        }

        private string RetornarTipoControle(string outerHTML)
        {
            var atributoZsControle = "sn-controle=";
            if (outerHTML.Contains(atributoZsControle))
            {
                outerHTML = outerHTML.Replace(System.Environment.NewLine, " ");
                var parcial = outerHTML.Substring(outerHTML.IndexOf(atributoZsControle) + atributoZsControle.Length);
                var valorAtributo = parcial.Split(' ').First().Split('>').First();
                //removendo aspa inicial e final
                return valorAtributo.Substring(1, valorAtributo.Length - 2);
            }
            var tagName = outerHTML.Split(' ').First().Substring(1).ToLower();
            return TagElementoUtil.RetornarTipo(tagName);

        }

        private object RetornarTipoElemento(string tag)
        {
            switch (tag)
            {
                case "a":

                    return "HTMLAnchorElement";

                case "input":

                    return "HTMLInputElement";

                case "div":

                    return "HTMLDivElement";

                case "span":

                    return "HTMLSpanElement";

                default:

                    return "HTMLElement";
            }
        }

        private void InserirDeclaracao(TextSelection selecao, string declaracao)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            selecao.StartOfDocument(true);

            if (selecao.FindText("export class") ||
                selecao.FindText("export abstract class"))
            {
                while (!selecao.Text.Contains("{"))
                {
                    selecao.LineDown(true);
                    selecao.SelectLine();
                }
                var selecaoAtual = selecao.Text;
                var sb = new StringBuilder();
                sb.AppendLine(selecaoAtual);
                sb.AppendLine($"\t\t{declaracao}");
                selecao.Insert(sb.ToString());

                selecao.StartOfDocument(true);
                selecao.FindText(declaracao);
                selecao.LineDown();
                selecao.LineDown();
                selecao.StartOfLine(vsStartOfLineOptions.vsStartOfLineOptionsFirstText);
            }
        }

        private void InserirDeclaracaoConstrutor(TextSelection selecao, string declaracao)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            selecao.StartOfDocument(true);

            if (selecao.FindText("constructor"))
            {
                while (!selecao.Text.Contains("{"))
                {
                    selecao.LineDown(true);
                    selecao.SelectLine();
                }

                while (!selecao.Text.Contains("super("))
                {
                    selecao.LineDown(true);

                    if (selecao.Text.Contains("}"))
                    {
                        selecao.LineUp(true);
                        break;
                    }
                }
                selecao.SelectLine();

                var selecaoAtual = selecao.Text;
                var sb = new StringBuilder();
                sb.AppendLine(selecaoAtual);
                sb.AppendLine($"\t\t\t{declaracao}");
                selecao.Insert(sb.ToString());

                selecao.StartOfDocument(true);
                selecao.FindText(declaracao);
                selecao.LineDown();
                selecao.LineDown();
                selecao.StartOfLine(vsStartOfLineOptions.vsStartOfLineOptionsFirstText);
            }
        }

        private bool ExisteDeclaracao(TextSelection selecao, string nomeMembro, bool isMetodo)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string[] moficadores = { "private", "public", "protected" };
            selecao.StartOfDocument(true);
            foreach (var modificador in moficadores)
            {
                if (isMetodo)
                {
                    var procurar = $"{modificador} {nomeMembro}(";
                    var procurarAsync = $"{modificador} async {nomeMembro}(";

                    if (selecao.FindText(procurar) || selecao.FindText(procurarAsync))
                    {
                        selecao.LineDown();
                        selecao.StartOfLine(vsStartOfLineOptions.vsStartOfLineOptionsFirstText);
                        return true;
                    }
                }
                else
                {
                    var procurar = $"{modificador} {nomeMembro}:";

                    var procurarPropriedade = $"{modificador} get {nomeMembro}():";
                    var procurarMoficadorReadonly = $"{modificador} readonly {nomeMembro}:";
                    var procurarIgual = $"{modificador} {nomeMembro} =";
                    var procurarReadonlyIgual = $"{modificador} readonly {nomeMembro} =";
                    var procurarReadonly = $"readonly {nomeMembro}:";

                    if (selecao.FindText(procurar) ||
                        selecao.FindText(procurarPropriedade) ||
                        selecao.FindText(procurarMoficadorReadonly) ||
                        selecao.FindText(procurarIgual) ||
                        selecao.FindText(procurarReadonlyIgual) ||
                        selecao.FindText(procurarReadonly))
                    {
                        selecao.StartOfLine(vsStartOfLineOptions.vsStartOfLineOptionsFirstText);
                        return true;
                    }
                }
            }
            return false;
        }

        private async Task ProcurarControleAsync(DTE2 dte, string nomeControle)
        {
            var projetos = await ProjetoDteUtil.RetornarProjetosVisualStudioAsync();
            var projetoAtivo = dte.RetornarProjetoAtivo();
            if (projetoAtivo != null)
            {
                projetos.Remove(projetoAtivo);
                projetos.Insert(0, projetoAtivo);
            }
            foreach (var projeto in projetos)
            {
                var caminhosArquivos = ProjetoDteUtil.RetornarTodosArquivos(projetoAtivo, false);
                var nomesCaminhoArquivo = caminhosArquivos.Select(x => new FileInfo(x)).Where(x => x.Extension == ".ts").
                                                                     Select(x => new NomeCaminhoArquivo(x));

                var nomeCaminhoArquivo = nomesCaminhoArquivo.Where(x => x.NomeArquivo == nomeControle).FirstOrDefault();
                if (nomeCaminhoArquivo != null)
                {
                    this.AbrirDocumentoCodigo(dte, nomeCaminhoArquivo.Arquivo.FullName);
                    break;
                }
            }
        }

        private string RetornarOuterHTMLElemento(TextSelection selecao)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            while (!selecao.Text.Trim().StartsWith("<"))
            {
                selecao.WordLeft(true);
            }
            var inicio = selecao.Text;

            while (!selecao.Text.Trim().EndsWith(">"))
            {
                selecao.WordRight(true);
            }
            var fim = selecao.Text;
            var outerHtml = inicio + fim;
            if (outerHtml.StartsWith("<"))
            {
                return outerHtml;
            }
            return String.Empty;
        }


        #region Atributos argumento - temp

        private Dictionary<string, AtributoArgumento> AtributosArgumento
        {
            get
            {
                return GoToDefinitionCommand.AtributosArgumentoStatico;
            }
        }

        private static Dictionary<string, AtributoArgumento> _atributosArgumento;
        private static Dictionary<string, AtributoArgumento> AtributosArgumentoStatico
        {
            get
            {
                if (GoToDefinitionCommand._atributosArgumento == null)
                {
                    GoToDefinitionCommand._atributosArgumento = new Dictionary<string, AtributoArgumento>
                    {
                        { "sn-click", new AtributoArgumento("sn-click", "ui.UIEventArgs") },
                        { "sn-enter", new AtributoArgumento("sn-enter", "ui.UIEventArgs") },
                        { "sn-click-async", new AtributoArgumento("sn-click-async", "ui.UIEventArgs", true) },

                        { "sn-item-click", new AtributoArgumento("sn-click", "ui.UIEventArgs") },
                        { "sn-valor-modificando", new AtributoArgumento("sn-valor-modificando", "ui.UIValorAlteradoEventArgs") },
                        { "sn-valor-alterado", new AtributoArgumento("sn-valor-alterado", "ui.UIValorAlteradoEventArgs") },
                        { "sn-selecionar-arquivos", new AtributoArgumento("sn-selecionar-arquivo", "ui.SelecionarArquivosEventoArgs") },
                        { "sn-item-selecionado-alterado", new AtributoArgumento("sn-item-selecionado-alterado", "ui.UIValorAlteradoEventArgs") },
                        { "sn-texto-pesquisa", new AtributoArgumento("sn-texto-pesquisa", "ui.TextoPesquisaEventArgs") },
                        { "sn-linha-detalhes-expandida", new AtributoArgumento("sn-linha-datalhes-expandida", "ui.LinhaDetalhesExpandidaEventArgs") },
                        { "sn-conteudo-expandido", new AtributoArgumento("sn-conteudo-expandido", "ui.ConteudoExpandidoEventArgs") },

                        { "sn-navegar", new AtributoArgumento("sn-navegar",EnumAcao.ProcurarControle) },
                        { "sn-controle", new AtributoArgumento("sn-controle",EnumAcao.ProcurarControle) },
                        { "sn-pagina-inicial", new AtributoArgumento("sn-pagina-inicial",EnumAcao.ProcurarControle) },

                        { "sn-nome", new AtributoArgumento("sn-nome",EnumAcao.NomeControle) },
                        { "sn-nome-elemento", new AtributoArgumento("sn-nome-elemento",EnumAcao.NomeElemento) },

                        { "sn-consulta-await", new AtributoArgumento("sn-consulta-async", "Promise<__TIPO__>",  true,
                            new Dictionary<string, string> { { "valor", "any" } }  ) },
                        { "sn-consulta", new AtributoArgumento("sn-consulta", "IConsultaEntidade<__TIPO__>",  true,
                            new Dictionary<string, string>()) },

                        { "sn-normalizar", new AtributoArgumento("sn-normalizar", "__TIPO__",  true, new Dictionary<string, string>{ { "valor", "any"  } } ) }
                    };
                }
                return GoToDefinitionCommand._atributosArgumento;
            }
        }

        private class AtributoArgumento
        {

            internal string NomeAtributo { get; }
            internal string NomeArgumentoEvento { get; }
            internal EnumAcao Acao { get; }
            internal bool IsAsync { get; }

            internal string NomeRetorno { get; }
            internal Dictionary<string, string> Parametros { get; }

            public AtributoArgumento(string nomeAtributo, string nomeArgumento) :
                this(nomeAtributo, nomeArgumento, EnumAcao.Evento)
            {
            }

            public AtributoArgumento(string nomeAtributo, string nomeArgumento, bool isAsync) :
                this(nomeAtributo, nomeArgumento, EnumAcao.Evento, isAsync)
            {
            }

            public AtributoArgumento(string nomeAtributo, EnumAcao acao) :
                this(nomeAtributo, null, acao)
            {
            }

            private AtributoArgumento(string nomeAtributo, string nomeArgumento, EnumAcao acao) :
                this(nomeAtributo, nomeArgumento, acao, false)

            {

            }

            private AtributoArgumento(string nomeAtributo, string nomeArgumento, EnumAcao acao, bool isAsync)
            {
                this.NomeAtributo = nomeAtributo;
                this.NomeArgumentoEvento = nomeArgumento;
                this.Acao = acao;
                this.IsAsync = isAsync;
            }

            public AtributoArgumento(string nomeAtributo, string nomeRetornor, bool isAsync, Dictionary<string, string> parametros)
            {
                this.Acao = EnumAcao.Metodo;
                this.NomeAtributo = nomeAtributo;
                this.NomeRetorno = nomeRetornor;
                this.IsAsync = isAsync;
                this.Parametros = parametros;
            }
        }

        private enum EnumAcao
        {
            Evento,
            ProcurarControle,
            NomeControle,
            NomeElemento,
            Metodo
        }

        private class NomeCaminhoArquivo
        {
            internal FileInfo Arquivo { get; }
            internal string NomeArquivo { get; }

            internal NomeCaminhoArquivo(FileInfo arquivo)
            {
                this.Arquivo = arquivo;
                this.NomeArquivo = Path.GetFileNameWithoutExtension(arquivo.Name);
                if (this.NomeArquivo.EndsWith(".shtml"))
                {
                    this.NomeArquivo = this.NomeArquivo.Substring(0, this.NomeArquivo.Length - ".shtml".Length);
                }
            }
        }

        #endregion

        #region Ir para declaração do Bind

        private bool IsBind(string valorAtributo)
        {
            return valorAtributo.Trim().StartsWith("{{");
        }

        private async Task IrParaArquivoBindAsync(DTE2 dte,
                                            Document documento,
                                           string nomeAtributo,
                                          string valorAtributo)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var inicio = valorAtributo.IndexOf("{{") + 2;
            var fim = valorAtributo.LastIndexOf("}}");

            var nomePropriedade = valorAtributo.Substring(inicio, fim - inicio);
            nomePropriedade = nomePropriedade.Split('.').First();
            var isOrigemThis = valorAtributo.Contains("Origem=this");

            var (caminhoArquivoTS, isCriarDefinicao) = await this.RetornarCaminhoArquivoViewModelAsync(dte, documento, nomeAtributo, nomePropriedade, isOrigemThis);
            if (caminhoArquivoTS != null)
            {
                this.AbrirDocumentoCodigo(dte, caminhoArquivoTS.Caminho);

                var documentoAtual = dte.ActiveDocument;
                var selecao = documentoAtual.Selection as TextSelection;
                if (!this.ExisteDeclaracao(selecao, nomePropriedade, false))
                {
                    if (isCriarDefinicao)
                    {
                        this.DeclararPropriedade(selecao, nomePropriedade, nomeAtributo);
                        documentoAtual.Save();
                    }
                }

            }
        }

        private void AbrirDocumentoCodigo(DTE2 dte, string caminhoDestino)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                var caminhoApresentacao = dte.ActiveDocument?.FullName;
                if (caminhoApresentacao != null && caminhoApresentacao.EndsWith(".shtml"))
                {
                    var caminhoCodigo = caminhoApresentacao + ".ts";

                    if (File.Exists(caminhoCodigo))
                    {
                        var htmlApresentacao = dte.ActiveDocument.RetornarTexto();
                        dte.AbrirArquivo(caminhoCodigo);

                        var documentoCodigo = dte.ActiveDocument;
                        if (documentoCodigo.Name.EndsWith(ConstantesProjeto.EXTENSAO_CONTROLE_SHTML_TYPESCRIPT))
                        {
                            var declaracao = new DeclaracaoComponentes(dte,
                                                                       caminhoApresentacao,
                                                                       htmlApresentacao,
                                                                       documentoCodigo);
                            declaracao.Declarar();

                            if (CaminhoUtil.CaminhoIgual(caminhoDestino, caminhoCodigo))
                            {
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro(ex);
            }
            dte.AbrirArquivo(caminhoDestino);
        }



        private void DeclararPropriedade(TextSelection selecao, string nomePropriedade, string nomeAtributo)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var isLista = nomeAtributo == "sn-bind-lista";
            var declaracaoPropriedade = this.RetornarDeclaracaoPropriedade(nomePropriedade, isLista);
            this.InserirDeclaracao(selecao, declaracaoPropriedade);
            if (!isLista)
            {
                var declaracaoPropriedadeConstrutor = this.RetornarDeclaracaoPropriedadeConstrutor(nomePropriedade, isLista);
                this.InserirDeclaracaoConstrutor(selecao, declaracaoPropriedadeConstrutor);
            }
            selecao.StartOfDocument(true);
            selecao.FindText(declaracaoPropriedade);
            selecao.StartOfLine(vsStartOfLineOptions.vsStartOfLineOptionsFirstText);
            selecao.FindText(NOME_TIPO);
            //selecao.EndOfLine();
            //selecao.WordLeft();
            //selecao.LineDown();

            //selecao.StartOfLine(vsStartOfLineOptions.vsStartOfLineOptionsFirstText);
        }

        private string RetornarDeclaracaoPropriedade(string nomePropriedade, bool isLista)
        {
            if (isLista)
            {
                return $"private readonly {nomePropriedade} = new ListaObservacao<{NOME_TIPO}>();";
            }
            return $"private {nomePropriedade}: {NOME_TIPO};";
        }

        private string RetornarDeclaracaoPropriedadeConstrutor(string nomePropriedade, bool isLista)
        {
            return $"this.DeclararPropriedade(x=> x.{nomePropriedade}, {NOME_TIPO}, \"{nomePropriedade}\");";
        }

        private async Task<(ArquivoTypescriptDefinicao, bool)> RetornarCaminhoArquivoViewModelAsync(DTE2 dte, Document documento, string nomeAtributo, string nomePropriedade, bool isOrigemThis)
        {
            var arquivosTS = this.RetornarArquivosTS(dte, documento, isOrigemThis);
            var arquivosTSEncontrrados = new List<(ArquivoTypescriptDefinicao ArquivoDefinicao, int NumeroLinha)>();
            foreach (var arquivoTS in arquivosTS)
            {
                var linhas = File.ReadAllLines(arquivoTS.Caminho, Encoding.UTF8);
                string[] moficadores = { "private", "public", "protected" };
                foreach (var modificador in moficadores)
                {

                    var procurar = $"{modificador} {nomePropriedade}:";
                    var procurarPropriedade = $"{modificador} get {nomePropriedade}():";
                    var procurarReadonly = $"{modificador} readonly {nomePropriedade}";
                    var len = linhas.Count();
                    for (var i = 0; i < len; i++)
                    {
                        var linha = linhas[i];
                        var linhaSemEspacoInicial = linha.TrimStart();
                        if (linhaSemEspacoInicial.StartsWith(procurar) ||
                            linhaSemEspacoInicial.StartsWith(procurarPropriedade) ||
                            linhaSemEspacoInicial.StartsWith(procurarReadonly))
                        {
                            var numeroLinha = i + 1;
                            arquivosTSEncontrrados.Add((arquivoTS, numeroLinha));
                        }
                    }
                }
            }

            if (arquivosTSEncontrrados.Count == 1)
            {
                return (arquivosTSEncontrrados.Single().ArquivoDefinicao, false);
            }

            if (arquivosTSEncontrrados.Count > 1)
            {
                await this.LogMultiplasDefinicoesAsync(dte, arquivosTSEncontrrados, nomePropriedade);
            }
            else
            {
                var arquivoCriarDefinicao = this.RetornarArquivosCriarDefinicao(arquivosTS);
                if (arquivoCriarDefinicao != null)
                {
                    return (arquivoCriarDefinicao, true);
                }
                else
                {
                    await this.EscolherLocalDeclararDefinicaoAsync(dte, arquivosTS, nomePropriedade, nomeAtributo);
                }
            }
            return (null, false);
        }

        private async Task LogMultiplasDefinicoesAsync(DTE2 dte, List<(ArquivoTypescriptDefinicao, int)> arquivosTSEncontrrados, string nomePropriedade)
        {

            await OutputWindow.ShowAsync();
            OutputWindow.Instance?.LimparLogs();
            var nomeBind = "{{" + nomePropriedade + "}}";

            LogVSUtil.Log($"Foram encontrados {arquivosTSEncontrrados.Count} referencias para o caminho do bind {nomeBind}");

            foreach (var (arquivoTS, numeroLinha) in arquivosTSEncontrrados)
            {
                var descricao = $"{nomeBind}  encontrada em {arquivoTS.NomeArquivo} na linha {numeroLinha}";
                LogVSUtil.LogAcaoLink(descricao, () =>
                {
                    ThreadHelper.ThrowIfNotOnUIThread();

                    this.AbrirDocumentoCodigo(dte, arquivoTS.Caminho);
                    var documentoAtual = dte.ActiveDocument;
                    this.ExisteDeclaracao(documentoAtual.Selection as TextSelection, nomePropriedade, false);
                });

            }
        }

        private async Task EscolherLocalDeclararDefinicaoAsync(DTE2 dte, List<ArquivoTypescriptDefinicao> arquivosTS, string nomePropriedade, string nomeAtributo)
        {
            await OutputWindow.ShowAsync();
            OutputWindow.Instance?.LimparLogs();

            var nomeBind = "{{" + nomePropriedade + "}}";

            LogVSUtil.Alerta($"A declaração {nomeBind}  não foi encontrada");

            foreach (var arquivoTS in arquivosTS)
            {
                var descricao = $"Declarar {nomeBind}  em {arquivoTS.NomeArquivo}";
                LogVSUtil.LogAcaoLink(descricao, () =>
                {
                    ThreadHelper.ThrowIfNotOnUIThread();

                    this.AbrirDocumentoCodigo(dte, arquivoTS.Caminho);

                    var documentoAtual = dte.ActiveDocument;
                    var selecao = documentoAtual.Selection as TextSelection;
                    this.DeclararPropriedade(selecao, nomePropriedade, nomeAtributo);
                    documentoAtual.Save();
                });

            }
        }

        private ArquivoTypescriptDefinicao RetornarArquivosCriarDefinicao(List<ArquivoTypescriptDefinicao> arquivosTS)
        {
            var arquivosNestingViewModel = arquivosTS.Where(x => x.IsViewModel);
            if (arquivosNestingViewModel.Count() == 1)
            {
                return arquivosNestingViewModel.Single();
            }
            if (arquivosNestingViewModel.Count() > 1)
            {
                return null;
            }

            var arquivosNestingControle = arquivosTS.Where(x => x.Local == EnumLocalArquivo.Nesting && x.IsControle);
            if (arquivosNestingControle.Count() == 1)
            {
                return arquivosNestingControle.Single();
            }
            return null;
        }

        private List<ArquivoTypescriptDefinicao> RetornarArquivosTS(DTE2 dte, Document documento, bool isOrigemThis)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var projectItem = documento.ProjectItem;
            var projectItemPasta = projectItem.RetornarProjectItemPai();
            var arquivosNesting = this.RetornarArquivosTypeScript(dte, projectItem.ProjectItems);
            var retorno = new List<ArquivoTypescriptDefinicao>();


            var arquivosTSNesting = arquivosNesting.Select(x => new ArquivoTypescriptDefinicao(documento, x, EnumLocalArquivo.Nesting));
            retorno.AddRange(arquivosTSNesting.OrderBy(x => x.Ordenacao));
            if (isOrigemThis)
            {
                return retorno.Where(x => x.IsControle).ToList();
            }

            var arquivosPasta = this.RetornarArquivosTypeScript(dte, projectItemPasta.ProjectItems);
            var arquivosTSPasta = arquivosPasta.Select(x => new ArquivoTypescriptDefinicao(documento, x, EnumLocalArquivo.Pasta));
            retorno.AddRange(arquivosTSPasta.Where(x => x.IsViewModel).OrderBy(x => x.Ordenacao));
            return retorno;
        }

        private List<string> RetornarArquivosTypeScript(DTE2 dte, ProjectItems items)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var arquivos = new List<string>();
            foreach (ProjectItem item in items)
            {

                if (item.Name.ToLower().EndsWith(".ts"))
                {
                    if (item.FileCount > 0)
                    {
                        var caminho = item.FileNames[0];
                        arquivos.Add(caminho);
                        if (item.IsOpen)
                        {
                            item.Document?.Save();
                        }
                    }
                }
            }
            return arquivos;
        }

        private class ArquivoTypescriptDefinicao
        {
            public EnumLocalArquivo Local { get; }
            public string Caminho { get; }
            public string NomeArquivo { get; }
            public bool IsViewModel { get; }
            public int Ordenacao { get; }
            public bool IsControle { get; }
            public Document Documento { get; }

            public ArquivoTypescriptDefinicao(Document document, string caminho, EnumLocalArquivo local)
            {
                this.Documento = document;
                this.Caminho = caminho;
                this.Local = local;

                this.NomeArquivo = Path.GetFileName(caminho);
                this.IsViewModel = this.NomeArquivo.ToLower().Contains("viewmodel");
                this.IsControle = this.RetornarIsControle();
                this.Ordenacao = this.IsViewModel ? -1 : 1;


            }

            private bool RetornarIsControle()
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                var nomeArquivoControle = this.Documento.ProjectItem.Name + ".ts";
                return this.NomeArquivo == nomeArquivoControle;
            }
        }

        private enum EnumLocalArquivo
        {
            Nesting = 1,
            Pasta = 2
        }

        #endregion

        #region Ir para Converter

        private void IrParaConverter(DTE2 dte,
                                     Document documento,
                                     string nomeAtributo,
                                     string valorAtributo)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (valorAtributo.StartsWith("this"))
            {
                var nomeMetodo = valorAtributo.Substring(valorAtributo.IndexOf(".") + 1);

                var caminhoCodigo = documento.FullName + ".ts";
                this.AbrirDocumentoCodigo(dte, caminhoCodigo);
                var selecao = dte.ActiveDocument.Selection as TextSelection;

                if (!this.ExisteDeclaracao(selecao, nomeMetodo, true))
                {
                    this.DeclararFuncaoConverter(selecao, nomeMetodo);
                }
            }
        }

        private void DeclararFuncaoConverter(TextSelection selecao, string nomeMetodo)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            selecao.StartOfDocument(true);

            if (selecao.FindText("export class") ||
                selecao.FindText("export abstract class"))
            {
                while (!selecao.Text.Contains("{"))
                {
                    selecao.LineDown(true);
                    selecao.SelectLine();
                }
                var selecaoAtual = selecao.Text;
                var declaracaoMetodo = $"private {nomeMetodo}(valor: any, dataContext: any): any";
                var sb = new StringBuilder();
                sb.AppendLine(selecaoAtual);
                sb.AppendLine($"\t\t{declaracaoMetodo}");
                sb.AppendLine("\t\t{");


                sb.AppendLine("\t\t\tthrow new ErroNaoImplementado(this);");
                sb.AppendLine("\t\t}");
                //sb.AppendLine("");
                selecao.Insert(sb.ToString());
                selecao.StartOfDocument(true);
                selecao.FindText(declaracaoMetodo);
                selecao.LineDown();
                selecao.LineDown();
                selecao.StartOfLine(vsStartOfLineOptions.vsStartOfLineOptionsFirstText);
            }
        }

        private void DeclararMetodo(DTE2 dte, TextSelection selecao, string nomeMetodo, AtributoArgumento atributoArgumento)
        {
            this.DeclararMetodo(dte, selecao, nomeMetodo, atributoArgumento.IsAsync, atributoArgumento.Parametros, atributoArgumento.NomeRetorno);
        }

        private void DeclararMetodo(DTE2 dte, TextSelection selecao, string nomeMetodo, bool isAsync, Dictionary<string, string> parametros, string retorno)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            selecao.StartOfDocument(true);


            if (selecao.FindText("export class") ||
                selecao.FindText("export abstract class"))
            {
                while (!selecao.Text.Contains("{"))
                {
                    selecao.LineDown(true);
                    selecao.SelectLine();
                }
                var selecaoAtual = selecao.Text;

                var conteudoParametros = String.Join(", ", parametros.Select(x => $"{x.Key}: {x.Value}"));
                var async = isAsync ? "async " : "";
                var declaracaoMetodo = $"private {async} {nomeMetodo}({conteudoParametros}): {retorno}";
                var sb = new StringBuilder();
                sb.AppendLine(selecaoAtual);
                sb.AppendLine($"\t\t{declaracaoMetodo}");
                sb.AppendLine("\t\t{");

                sb.AppendLine("\t\t\tthrow new ErroNaoImplementado(this);");

                sb.AppendLine("\t\t}");
                //sb.AppendLine("");
                selecao.Insert(sb.ToString());
                selecao.StartOfDocument(true);
                selecao.FindText(declaracaoMetodo);
                selecao.LineDown();
                selecao.LineDown();
                selecao.StartOfLine(vsStartOfLineOptions.vsStartOfLineOptionsFirstText);
            }
        }

        #endregion
    }
}