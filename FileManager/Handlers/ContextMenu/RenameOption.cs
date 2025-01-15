using System.Windows.Controls;
using System.Windows;
using System.IO;

namespace FileManager;

// Rename Option

public partial class MainWindow
{
    public void SelectRenameOption(object sender, RoutedEventArgs e)
    {
        if (SelectItems.Count != 1)
            return;

        DirItem item = SelectItems[0];

        if (item.Type == "drive")
            return;

        RenameItem = item;

        Grid ButtonContent = (Grid)item.ItemButton.Content;

        ButtonContent.Children[1].Visibility = Visibility.Collapsed;

        RenameBox.Text = Path.GetFileName(item.Name);
        RenameBox.CaretIndex = RenameBox.Text.Length;
        ButtonContent.Children.Add(RenameBox);

        RenameBox.Focus();
    }
}
