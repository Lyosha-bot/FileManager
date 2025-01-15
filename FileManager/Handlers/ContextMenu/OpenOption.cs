using System.Windows;

namespace FileManager;

// Open Option

public partial class MainWindow
{
    public void SelectOpenOption(object sender, RoutedEventArgs e)
    {
        if (SelectItems.Count != 1)
            return;

        string? dir = SelectItems[0].Dir;
        if (dir == null)
            return;

        OpenDir(dir);
    }
}