using System.IO;
using System.Windows;

namespace FileManager;

// Delete Option

public partial class MainWindow
{
    private void SelectDeleteOption(object sender, RoutedEventArgs e)
    {
        if (SelectItems.Count < 1)
            return;

        foreach (DirItem item in SelectItems)
        {
            if (item.Type == "drive")
                continue;

            if (item.Type == "folder")
                Directory.Delete(item.Dir, true);
            else
                File.Delete(item.Dir);
        }
    }
}
