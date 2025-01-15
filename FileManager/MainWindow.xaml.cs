using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace FileManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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
        public MainWindow()
        {
            InitializeComponent();

            RenameBox.KeyDown += RenameDir;
            RenameBox.Style = UI.Styles.Name();
            Grid.SetRow(RenameBox, 1);

            LoadFiles("");
        }

        public DirItem? GetItem(Button? button)
        {
            string? dir = DirItem.GetDir(button);
            if (dir == null)
                return null;

            return CurrentItems[dir];
        }

        public void LoadFiles(string? dir)
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

        public void DirChanged(object sender, FileSystemEventArgs e)
        {
            Dispatcher.Invoke(() => LoadFiles(null));
        }

        // Mouse actions
        public void HideRenameBox()
        {
            if (RenameItem == null)
                return;

            Grid ButtonContent = (Grid) RenameItem.ItemButton.Content;
            ButtonContent.Children.Remove(RenameBox);
            ButtonContent.Children[1].Visibility = Visibility.Visible;

            RenameItem = null;
        }

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

        public void OpenDir(string dir)
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

        public void UnselectItems()
        {
            foreach (DirItem button in SelectItems)
                button.ItemButton.BorderBrush = Brushes.Transparent;
            SelectItems.Clear();
        }

        public void UnselectItem(DirItem item)
        {
            SelectItems.Remove(item);
            item.ItemButton.BorderBrush = Brushes.Transparent;
        }

        public void SelectItem(DirItem newItem, bool newList, bool removeOnExist)
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

        // Address box section

        public void AddressBoxCopy(object sender, RoutedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox == null)
                return;

            textBox.Focus();
            textBox.SelectAll();
            e.Handled = true;
        }

        public void AddressBoxEnter(object sender, KeyEventArgs e)
        {
            if (ModalMode)
                return;

            TextBox? textBox = sender as TextBox;
            if (textBox == null)
                return;

            if (e.Key != Key.Enter)
                return;

            if (textBox.Text != "" && !Directory.Exists(textBox.Text))
            {
                MessageBox.Show($"Директория {textBox.Text} не найдена или не существует");
                return;
            }

            Keyboard.ClearFocus();

            if (textBox.Text != "" && !DirItem.CanAccess(textBox.Text))
                return;

            if (textBox.Text == DirHistory.Peek())
                return;

            DirNextHistory.Clear();
            LoadFiles(textBox.Text);
        }

        // History buttons section

        public void RevertAction(object sender, RoutedEventArgs e)
        {
            if (ModalMode)
                return;

            if (DirHistory.Count <= 1)
                return;

            DirNextHistory.Push(DirHistory.Pop());
            LoadFiles(null);
        }

        public void ForwardAction(object sender, RoutedEventArgs e)
        {
            if (ModalMode)
                return;

            if (DirHistory.Count <= 0)
                return;

            LoadFiles(DirNextHistory.Pop());
        }

        public void ParentAction(object sender, RoutedEventArgs e)
        {
            if (ModalMode)
                return;

            string? parent = DirItem.GetParent(DirHistory.Peek());
            if (parent == null)
                return;

            LoadFiles(parent);
        }

        // Context Menu

        // Context Menu Options
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

        // Open Option

        public void SelectOpenOption(object sender, RoutedEventArgs e)
        {
            if (SelectItems.Count != 1)
                return;

            string? dir = SelectItems[0].Dir;
            if (dir == null)
                return;

            OpenDir(dir);
        }

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

        // Rename Option

        public void Rename(DirItem item)
        {
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

        public void SelectRenameOption(object sender, RoutedEventArgs e)
        {
            if (SelectItems.Count != 1)
                return;

            Rename(SelectItems[0]);
        }

        // Move Option

        public void SelectMoveOption(object sender, RoutedEventArgs e)
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
                //MessageBox.Show(item.Dir);

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
                    RecursivePaste(item.Dir, Path.Combine(currentDir,item.Name));
                else
                    File.Copy(item.Dir, Path.Combine(currentDir, item.Name), true);
            }
        }

        // Delete Option

        public void SelectDeleteOption(object sender, RoutedEventArgs e)
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
}