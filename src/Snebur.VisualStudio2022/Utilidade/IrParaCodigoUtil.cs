using Community.VisualStudio.Toolkit;
using EnvDTE;
using Microsoft.VisualStudio.Text;
using Snebur.Depuracao;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Snebur.VisualStudio.Utilidade;
public static class IrParaCodigoUtil
{
    public static async Task EventoIrParaCodigoAsync(
        MensagemIrParaCodigo mensagem,
        List<ProjetoTypeScript> projetoTS)
    {
        try
        {
            await EventoIrParaCodigoInternalAsync(mensagem, projetoTS);
        }
        catch (Exception ex)
        {
            LogVSUtil.LogErro("Erro ao executar o evento IrParaCodigo.", ex);
        }
    }

    private static async Task EventoIrParaCodigoInternalAsync(
        MensagemIrParaCodigo mensagem,
        List<ProjetoTypeScript> projetoTS)
    {
        var nomeControle = mensagem.NomeControle;
        var possiveisArquivos = RetornarPossiveisArquivosControle(projetoTS, nomeControle, mensagem.Namespace);
        if (possiveisArquivos.Count == 0)
        {
            LogVSUtil.LogErro($"Não foi possível localizar o controle '{nomeControle}' na solução.");
            return;
        }

        var logMessage = $"Ir para código controle: {nomeControle} - Arquivo(s) encontrado(s): {string.Join(", ", possiveisArquivos)}";
        LogVSUtil.Log(logMessage, EnumTipoLog.Normal);
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        var (caminhoArquivo, nomeProjeto) = possiveisArquivos.First();
        var projeto = await VS.Solutions.FindProjectsAsync(nomeProjeto);
        if (projeto is null)
        {
            LogVSUtil.LogErro($"Não foi possível localizar o projeto '{nomeProjeto}' na solução.");
            return;
        }

        var file = await projeto.GetPhysicalFileAsync(caminhoArquivo);
        if (file is null)
        {
            LogVSUtil.LogErro($"Não foi possível localizar o arquivo do controle '{caminhoArquivo}' no projeto '{nomeProjeto}'");
            return;
        }
        var searchPatters = mensagem.SearchElementPattern;
        var windowFrame = await file.OpenAsync();
        if (windowFrame is null)
        {
            await AbrirUsandoDteAsync(caminhoArquivo, searchPatters, mensagem.TagElemento);
            LogVSUtil.LogErro($"Não foi possível abrir o arquivo do controle '{caminhoArquivo}' no projeto '{nomeProjeto}'");
            return;
        }


        if (string.IsNullOrEmpty(searchPatters))
        {
            return;
        }
        var documentView = await windowFrame.GetDocumentViewAsync();
        if (documentView is null)
        {
            LogVSUtil.LogErro($"Não foi possível obter a GetDocumentViewAsync o arquivo do controle '{caminhoArquivo}' no projeto '{nomeProjeto}'");
            await AbrirUsandoDteAsync(caminhoArquivo, searchPatters, mensagem.TagElemento);

            return;
        }
        await HighlightHtmlPatternAsync(documentView, searchPatters);

    }
    private static async Task AbrirUsandoDteAsync(
        string caminhoArquivo,
        string searchPatters,
        string tagElemento)
    {
        LogVSUtil.Log($"Abrndo arquivo usando Dte {caminhoArquivo} ");
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        var dte = await DteUtil.GetDTEAsync();
        var documento = dte.AbrirArquivo(caminhoArquivo);
        documento.Activate();
        if (string.IsNullOrEmpty(searchPatters))
        {
            return;
        }

        if (documento.Selection is TextSelection selecao)
        {
            selecao.StartOfDocument(true);
            if (selecao.FindText(searchPatters))
            {
                return;
            }
            selecao.FindText(tagElemento);
        }
    }
    private static async Task HighlightHtmlPatternAsync(DocumentView documentView, string searchPattern)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        if (documentView?.TextBuffer is null || string.IsNullOrEmpty(searchPattern))
        {
            return;
        }

        ITextBuffer buffer = documentView.TextBuffer;
        ITextSnapshot snapshot = buffer.CurrentSnapshot;
        string text = snapshot.GetText();

        int index = text.IndexOf(searchPattern, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
        {
            // opcional log
            LogVSUtil.Log($"Padrão '{searchPattern}' não encontrado no documento.", EnumTipoLog.Normal);
            return;
        }

        // Calcula linha e coluna
        ITextSnapshotLine line = snapshot.GetLineFromPosition(index);
        int lineStart = line.Start.Position;
        int column = index - lineStart;

        var view = documentView.TextView;
        if (view is null)
        {
            LogVSUtil.LogErro("Não foi possível obter o TextView do documento.");
            return;
        }

        var span = new SnapshotSpan(snapshot, index, searchPattern.Length);

        // Seleciona o texto e move o cursor
        view.Selection.Select(span, isReversed: false);
        view.Caret.MoveTo(span.Start);
        view.ViewScroller.EnsureSpanVisible(span);
    }

    private static List<(string, string)> RetornarPossiveisArquivosControle(
        List<ProjetoTypeScript> projetoTS,
        string nomeControle,
        string @namespace)
    {
        try
        {
            var arquivos = new List<(string, string)>();
            foreach (var projetoTs in projetoTS)
            {
                foreach (var arquivo in projetoTs.ArquivosTS)
                {
                    if (arquivo.EndsWith(ConstantesProjeto.EXTENSAO_CONTROLE_SHTML_TYPESCRIPT))
                    {
                        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(arquivo);
                        if (fileNameWithoutExtension.Equals(nomeControle, StringComparison.OrdinalIgnoreCase))
                        {
                            var caminhoControle = arquivo.TrimEnd(".ts", StringComparison.OrdinalIgnoreCase);
                            arquivos.Add((caminhoControle, projetoTs.NomeProjeto));
                            if (projetoTs.Namespace.Equals(@namespace))
                            {
                                return new List<(string, string)> { (caminhoControle, projetoTs.NomeProjeto) };
                            }
                        }
                    }
                }
            }
            return arquivos;
        }
        catch (Exception ex)
        {
            LogVSUtil.LogErro($"Erro ao retornar possíveis arquivos do controle {nomeControle}", ex);
            return new List<(string, string)>();
        }
    }

}
