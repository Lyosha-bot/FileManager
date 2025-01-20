using System.Windows;

namespace FileManager;

// History buttons section

public partial class MainWindow
{
    private void RevertAction(object sender, RoutedEventArgs e)
    {
        if (ModalMode)
            return;

        if (DirHistory.Count <= 1)
            return;

        DirNextHistory.Push(DirHistory.Pop());
        LoadFiles(null);
    }

    private void ForwardAction(object sender, RoutedEventArgs e)
    {
        if (ModalMode)
            return;

        if (DirHistory.Count <= 0)
            return;

        LoadFiles(DirNextHistory.Pop());
    }

    private void ParentAction(object sender, RoutedEventArgs e)
    {
        if (ModalMode)
            return;

        string? parent = DirItem.GetParent(DirHistory.Peek());
        if (parent == null)
            return;

        LoadFiles(parent);
    }
}
