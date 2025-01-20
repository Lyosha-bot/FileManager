using System.IO;
using System.Windows;

namespace FileManager;

// Move Option
public partial class MainWindow
{
    private void SelectMoveOption(object sender, RoutedEventArgs e)
    {
        if (SelectItems.Count <= 0)
            return;

        MoveDirectory.Text = DirHistory.Peek();
        MoveWindow.Visibility = Visibility.Visible;
        ModalMode = true;
    }

    private void MoveItems()
    {
        if (SelectItems.Count <= 0)
            return;

        string finalDir = MoveDirectory.Text;
        if (finalDir == "")
            return;

        if (!Directory.Exists(finalDir))
        {
            MessageBox.Show($"Данной директории не существует", "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        foreach (DirItem item in SelectItems)
        {
            if (!item.Exists())
                return;

            if (item.Type == "folder")
                Directory.Move(item.Dir, Path.Combine(finalDir, item.Name));
            else
                File.Move(item.Dir, Path.Combine(finalDir, item.Name));
        }
    }

    private void HideMoveWindow()
    {
        MoveWindow.Visibility = Visibility.Collapsed;
        ModalMode = false;
    }

    private void MoveAccept(object sender, RoutedEventArgs e)
    {
        MoveItems();
        HideMoveWindow();
    }

    private void MoveCancel(object sender, RoutedEventArgs e)
    {
        HideMoveWindow();
    }
}
