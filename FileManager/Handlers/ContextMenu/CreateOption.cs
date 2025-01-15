using System.IO;
using System.Windows;

namespace FileManager;

// Create Option
public partial class MainWindow
{
    // Create Folder Option

    public void SelectCreateFolderOption(object sender, RoutedEventArgs e)
    {
        if (SelectItems.Count > 0)
            return;

        string currentDir = DirHistory.Peek().ToString();
        if (currentDir == "")
            return;

        string name = "Новая папка";

        if (Directory.Exists(Path.Combine(currentDir, name)))
        {
            int counter = 1;
            while (Directory.Exists(Path.Combine(currentDir, $"{name} ({counter})")))
                counter++;

            name = $"{name} ({counter})";
        }

        Directory.CreateDirectory(Path.Combine(currentDir, name));
    }

    // Create File Option

    public void SelectCreateFileOption(object sender, RoutedEventArgs e)
    {
        if (SelectItems.Count > 0)
            return;

        string currentDir = DirHistory.Peek().ToString();
        if (currentDir == "")
            return;

        string name = "Новый файл";

        if (File.Exists(Path.Combine(currentDir, name)))
        {
            int counter = 1;
            while (Directory.Exists(Path.Combine(currentDir, $"{name} ({counter})")))
                counter++;

            name = $"{name} ({counter})";
        }

        FileStream file = File.Create(Path.Combine(currentDir, name));
        file.Close();
    }

}
