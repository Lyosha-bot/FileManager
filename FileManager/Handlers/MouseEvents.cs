using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FileManager;

// Mouse actions

public partial class MainWindow
{
    public void LeftMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (ModalMode)
            return;

        DirItem? target = GetItem(e.Source as Button);
        if (target == null)
        {
            UnselectItems();
            return;
        }

        bool holdsCtrl = Keyboard.IsKeyDown(Key.LeftCtrl);
        if (!SelectItems.Contains(target))
            SelectItem(target, !holdsCtrl, false);
        else if (holdsCtrl)
            SelectItem(target, !holdsCtrl, holdsCtrl && SelectItems.Contains(target));
    }

    public void RenameDir(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
            return;

        if (RenameItem == null) // Заглушка
            return;

        string newName = RenameBox.Text;
        string? parent = DirItem.GetParent(RenameItem.Dir);

        if (parent == null)
            return;

        if (RenameItem.Type == "folder")
            Directory.Move(RenameItem.Dir, @$"{parent}\{newName}");
        else
            File.Move(RenameItem.Dir, @$"{parent}\{newName}");

        HideRenameBox();
    }

    public void OpenItem(object sender, MouseButtonEventArgs e)
    {
        if (ModalMode)
            return;

        if (e.ChangedButton != MouseButton.Left)
            return;

        DirItem? item = GetItem(sender as Button);
        if (item == null)
            return;

        OpenDir(item.Dir);
    }
}
