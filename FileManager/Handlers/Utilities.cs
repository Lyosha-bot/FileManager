using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FileManager;

public partial class MainWindow
{
    Dictionary<string, DirItem> CurrentItems = new Dictionary<string, DirItem>();

    Stack<string> DirHistory = new Stack<string>();
    Stack<string> DirNextHistory = new Stack<string>();

    bool ModalMode = false;
    List<DirItem> SelectItems = new List<DirItem>();

    List<DirItem> CopyItems = new List<DirItem>();

    TextBox RenameBox = new TextBox();
    DirItem? RenameItem;

    FileSystemWatcher? watcher;

    private void HideRenameBox()
    {
        if (RenameItem == null)
            return;

        Grid ButtonContent = (Grid)RenameItem.ItemButton.Content;
        ButtonContent.Children.Remove(RenameBox);
        ButtonContent.Children[1].Visibility = Visibility.Visible;

        RenameItem = null;
    }

    private DirItem? GetItem(Button? button)
    {
        string? dir = DirItem.GetDir(button);
        if (dir == null)
            return null;

        return CurrentItems[dir];
    }

    private void LoadFiles(string? dir)
    {
        string currentDir = (dir == null ? DirHistory.Peek() : dir);

        if (watcher != null)
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }

        AddressBox.Text = currentDir;
        Objects.Children.Clear();
        SelectItems.Clear();
        CurrentItems.Clear();

        if (currentDir == "")
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                DirItem item = new DirItem(drive.Name);
                CurrentItems.Add(drive.Name, item);

                Objects.Children.Add(item.ItemButton);
                item.ItemButton.MouseDoubleClick += OpenItem;
            }
        else
        {
            if (!Directory.Exists(currentDir))
            {
                DirHistory.Clear();
                DirNextHistory.Clear();
                MessageBox.Show("Текущая директория была изменена", "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadFiles("");
                return;
            }

            foreach (string folder in Directory.GetDirectories(currentDir))
            {
                DirItem item = new DirItem(folder);
                CurrentItems.Add(folder, item);

                Objects.Children.Add(item.ItemButton);
                item.ItemButton.MouseDoubleClick += OpenItem;
            }

            foreach (string file in Directory.GetFiles(currentDir))
            {
                DirItem item = new DirItem(file);
                CurrentItems.Add(file, item);

                Objects.Children.Add(item.ItemButton);
                item.ItemButton.MouseDoubleClick += OpenItem;
            }

            watcher = new FileSystemWatcher(currentDir);
            watcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite;

            watcher.Changed += DirChanged;
            watcher.Renamed += DirChanged;
            watcher.Created += DirChanged;
            watcher.Deleted += DirChanged;

            watcher.EnableRaisingEvents = true;
        }

        if (dir != null)
            DirHistory.Push(dir);

        BackButton.IsEnabled = DirHistory.Count > 1;
        ForwardButton.IsEnabled = DirNextHistory.Count > 0;
        ParentButton.IsEnabled = (DirItem.GetParent(currentDir) != null);
    }

    private void DirChanged(object sender, FileSystemEventArgs e)
    {
        Dispatcher.Invoke(() => LoadFiles(null));
    }

    private void OpenDir(string dir)
    {
        DirItem item = CurrentItems[dir];

        if (item.Type == "drive" || item.Type == "folder" || item.Type == "system")
        {
            if (item.Type != "system" && !item.CanAccess())
                return;

            DirNextHistory.Clear();
            LoadFiles(item.Dir);
        }
        else
            item.Open();
    }

    private void UnselectItems()
    {
        foreach (DirItem button in SelectItems)
            button.ItemButton.BorderBrush = Brushes.Transparent;
        SelectItems.Clear();
    }

    private void UnselectItem(DirItem item)
    {
        SelectItems.Remove(item);
        item.ItemButton.BorderBrush = Brushes.Transparent;
    }

    private void SelectItem(DirItem newItem, bool newList, bool removeOnExist)
    {
        if (newList)
            UnselectItems();
        else
            if (SelectItems.Contains(newItem))
        {
            if (removeOnExist)
                UnselectItem(newItem);
            return;
        }

        SelectItems.Add(newItem);
        newItem.ItemButton.BorderBrush = Brushes.Blue;
    }
}
