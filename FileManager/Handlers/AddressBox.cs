
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

// Address box section
namespace FileManager;

public partial class MainWindow
{
    private void AddressBoxCopy(object sender, RoutedEventArgs e)
    {
        TextBox? textBox = sender as TextBox;
        if (textBox == null)
            return;

        textBox.Focus();
        textBox.SelectAll();
        e.Handled = true;
    }

    private void AddressBoxEnter(object sender, KeyEventArgs e)
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
}
