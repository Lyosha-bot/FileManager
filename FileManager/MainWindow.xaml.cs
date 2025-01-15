using System.IO;
using System.Windows;
using System.Windows.Controls;


namespace FileManager;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        RenameBox.KeyDown += RenameDir;
        RenameBox.Style = UI.Styles.Name();
        Grid.SetRow(RenameBox, 1);

        LoadFiles("");
    }
}