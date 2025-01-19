using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Diagnostics;

using File = System.IO.File;

namespace FileManager;

public class DirItem
{
    public string Name { get; }
    public string DisplayName { get; }
    public string Type { get; }

    public string Dir { get; }

    public Button ItemButton { get; }

    public DirItem(string dir)
    {
        Name = Path.GetFileName(dir);
        Type = GetType(dir);
        DisplayName = (Type == "drive" ? $"Диск {dir[0].ToString()}" : Name);

        Dir = dir;

        ItemButton = new Button()
        {
            Tag = dir,
            Style = UI.Styles.Button(),
            ToolTip = DisplayName
        };

        Grid grid = new Grid() 
        { 
            Style = UI.Styles.Wrapper(),
        };

        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(8, GridUnitType.Star) });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });

        Image img = new Image()
        {
            Style = UI.Styles.Image(),
            Stretch = Stretch.Uniform,
            Source = new BitmapImage(GetIcon(dir)),
            IsHitTestVisible = false
        };

        Grid.SetRow(img, 0);
        grid.Children.Add(img);

        TextBlock textBlock = new TextBlock() 
        { 
            Style = UI.Styles.Name(),
            TextWrapping = TextWrapping.NoWrap,
            TextTrimming = TextTrimming.CharacterEllipsis,
            Text = DisplayName,
            IsHitTestVisible = false
        };

        Grid.SetRow(textBlock, 1);
        grid.Children.Add(textBlock);

        ItemButton.Content = grid;
    }

    public bool Exists()
    {
        return Directory.Exists(Dir) || File.Exists(Dir);
    }

    public bool CanAccess()
    {
        try
        {
            if (Type == "folder" || Type == "drive")
                Directory.GetFiles(Dir);
            else
            {
                FileStream file = File.OpenRead(Dir);
                file.Close();
            }
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            MessageBox.Show("Нет доступа к данной директории.", "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (DirectoryNotFoundException)
        {
            MessageBox.Show("Данной директории не существует.", "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка доступа директории: {ex.ToString()}.", "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        return false;
    }

    public void Open()
    {
        if (Type == "folder" || Type == "shortcut")
            return;

        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = Dir,
                Verb = "open",
                UseShellExecute = true
            };

            Process.Start(startInfo);
        }
        catch (FileNotFoundException)
        {
            MessageBox.Show("Данного файла не существует.", "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка открытия файла {Dir}: {ex.Message}");
        }
    }

    // Static

    static public bool CanAccess(string dir)
    {
        try
        {
            if (GetType(dir) == "folder" || GetType(dir) == "drive")
                Directory.GetFiles(dir);
            else
            {
                FileStream file = File.OpenRead(dir);
                file.Close();
            }
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            MessageBox.Show($"Нет доступа к данной директории.", "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (DirectoryNotFoundException)
        {
            MessageBox.Show("Данной директории не существует.", "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка доступа директории: {ex.ToString()}.", "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        return false;
    }

    static public string GetType(string dir)
    {
        if (dir == "")
            return "system";

        string[] split = dir.Split('\\');
        if (split[1] == "")
            return "drive";

        FileAttributes attr = File.GetAttributes(dir);

        if (attr.HasFlag(FileAttributes.Directory))
            return "folder";

        switch (Path.GetExtension(dir).ToUpperInvariant())
        {
            case ".PNG":
            case ".JPG":
            case ".JPEG":
                return "image";

            //case ".LNK":
            //    return "shortcut";

            default:
                return "unknown";
        }
    }

    static public string? GetParent(string dir)
    {
        if (dir == "")
            return null;

        if (GetType(dir) == "drive")
            return "";

        DirectoryInfo? parent = Directory.GetParent(dir);
        if (parent == null)
            return null;

        return parent.FullName;
    }

    static public Uri GetIcon(string dir)
    {
        string type = GetType(dir);

        if (type == "image")
            return new Uri(dir, UriKind.Absolute);

        return new Uri(@$"Assets\{type}.png", UriKind.Relative);
    }

    static public string? GetDir(Button? item)
    {
        if (item == null)
            return null;

        var tag = item.Tag;
        if (tag == null)
            return null;

        return tag.ToString();
    }
}
