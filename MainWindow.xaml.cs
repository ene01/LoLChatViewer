using System.Windows;

namespace LoLChatViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ChatViewer cv = new();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowGrid.Children.Add(cv.Start());
        }
    }
}