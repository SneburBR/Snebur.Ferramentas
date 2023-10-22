using System.Windows.Controls;
using System.Windows.Media;

namespace Snebur.VisualStudio
{
    public static class ItensControlUtil
    {
        public static async Task ScrollToBottonAsync(ItemsControl itemsControl)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (VisualTreeHelper.GetChildrenCount(itemsControl) > 0)
            {
                try
                {
                    Border border = (Border)VisualTreeHelper.GetChild(itemsControl, 0);
                    if(VisualTreeHelper.GetChild(border, 0) is ScrollViewer scrollViewer)
                    {
                        scrollViewer.ScrollToBottom();
                    }
                    //ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                    
                }
                catch
                {

                }
            }
        }
    }
}
