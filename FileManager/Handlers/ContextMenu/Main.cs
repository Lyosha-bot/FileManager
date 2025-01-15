using System.IO;
using System.Windows.Controls;
using System.Windows;

namespace FileManager;

// Context Menu Open Event

public partial class MainWindow
{
    public void ContextMenuOpened(object sender, RoutedEventArgs e)
    {
        UIElement? target = e.Source as UIElement;
        if (target == null)
            return;

        bool isButton = (target is Button);

        if (!isButton)
            UnselectItems();
        else
        {
            DirItem? item = GetItem(target as Button);
            if (item != null)
                SelectItem(item, !SelectItems.Contains(item), false);
        }

        OpenOption.Visibility = (SelectItems.Count == 1 ? Visibility.Visible : Visibility.Collapsed);
        CreateOption.Visibility = (isButton ? Visibility.Collapsed : Visibility.Visible);
        RenameOption.Visibility = (SelectItems.Count == 1 ? Visibility.Visible : Visibility.Collapsed);
        CopyOption.Visibility = (isButton ? Visibility.Visible : Visibility.Collapsed);
        PasteOption.Visibility = (isButton ? Visibility.Collapsed : Visibility.Visible);
        DeleteOption.Visibility = (SelectItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed);
    }
}