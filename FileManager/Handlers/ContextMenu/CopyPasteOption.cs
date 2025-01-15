using System.IO;
using System.Windows;

namespace FileManager;

// Copy And Paste Option

public partial class MainWindow
{
    // Copy Option

    public void SelectCopyOption(object sender, RoutedEventArgs e)
    {
        if (SelectItems.Count <= 0)
            return;

        CopyItems = new List<DirItem>(SelectItems);
    }

    // Paste Option

    public void RecursivePaste(string source_dir, string end_dir)
    {
        if (!Directory.Exists(source_dir))
            return;

        string[] dirs = Directory.GetDirectories(source_dir); // Получаем папки ДО ТОГО как создаем новую папку.

        Directory.CreateDirectory(end_dir);

        foreach (string subDirectoryPath in dirs)
        {
            string subDirectoryName = Path.GetFileName(subDirectoryPath);
            string destSubDirectoryPath = Path.Combine(end_dir, subDirectoryName);
            RecursivePaste(subDirectoryPath, Path.Combine(end_dir, subDirectoryName));
        }

        foreach (string filePath in Directory.GetFiles(source_dir))
        {
            string fileName = Path.GetFileName(filePath);
            string destFilePath = Path.Combine(end_dir, fileName);
            File.Copy(filePath, Path.Combine(end_dir, fileName), true);
        }
    }

    public void SelectPasteOption(object sender, RoutedEventArgs e)
    {
        if (CopyItems.Count < 1)
            return;

        string currentDir = DirHistory.Peek();

        foreach (DirItem item in CopyItems)
        {
            if (!item.Exists())
                return;

            if (item.Type == "folder")
                RecursivePaste(item.Dir, Path.Combine(currentDir, item.Name));
            else
                File.Copy(item.Dir, Path.Combine(currentDir, item.Name), true);
        }
    }
}
