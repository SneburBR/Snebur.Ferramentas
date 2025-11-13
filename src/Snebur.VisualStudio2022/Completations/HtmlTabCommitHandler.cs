using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;

namespace Snebur.VisualStudio.Completations;

[Export(typeof(ICommandHandler<TabKeyCommandArgs>))]
[Name("Snebur Html Tab Commit Handler")]
public sealed class HtmlTabCommitHandler : ICommandHandler<TabKeyCommandArgs>
{
    [Import]
    private ICompletionBroker CompletionBroker { get; set; } = null!;

    public string DisplayName
        => "Snebur Html Tab Commit Handler";

    public HtmlTabCommitHandler()
    {
        LogVSUtil.Alerta("Inicializando SHtmlTabCommitHandl sem injeção de dependencia");
    }

    public bool ExecuteCommand(TabKeyCommandArgs args, CommandExecutionContext executionContext)
    {
        if (CompletionBroker is null)
        {
            Debugger.Break();
            return false;
        }

        var view = args.TextView;
        var sessions = CompletionBroker.GetSessions(view);
        var active = sessions?.FirstOrDefault();
        if (active != null && !active.IsDismissed)
        {
            // commit the selected item, then consume the Tab
            active.Commit();
            return true;
        }

        // no active completion, let Tab insert indentation
        return false;
    }

    public CommandState GetCommandState(TabKeyCommandArgs args)
    {
        return CommandState.Available;
    }
}