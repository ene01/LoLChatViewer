using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;

namespace LoLChatViewer.UI.Panels
{
    public class LogViewerElements : ChatViewerWindow
    {
        // Grid que se usa para acomodar el ScrollView que muestra los archivos leidos.
        public Grid fileGrid = new()
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Left,
            Width = 240,
            Background = new SolidColorBrush(Color.FromArgb(255, 41, 16, 37)),
            Margin = new Thickness(0, 20, 0, 25),
            ClipToBounds = true
        };

        public TextBox searchBar = new()
        {
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Left,
            FontStyle = FontStyles.Italic,
            Text = "Escribe aqui para buscar",
            Foreground = new SolidColorBrush(Color.FromArgb(180, 0, 0, 0)),
            Height = 20,
            Width = 240
        };

        // Grid que se usa para acomodar el ScrollView que muestrra los mensajes del archivo elegido.
        public Grid logGrid = new()
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Background = new SolidColorBrush(Color.FromArgb(255, 56, 26, 55)),
            Margin = new Thickness(250, 0, 0, 25),
            ClipToBounds = true
        };

        // Label que muestra el 'Path' actual del archivo elegido a mostrar.
        public Label pathLabel = new()
        {
            VerticalAlignment = VerticalAlignment.Bottom,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
            Foreground = new SolidColorBrush(Color.FromArgb(180, 255, 255, 255)),
            FontSize = 14,
            FontWeight = FontWeights.Thin,
            Content = "No estas viendo nada",
        };

        // Grid que contiene a fileGrid y logGrid, usado mas que nada para seprar un poco todo.
        protected Grid handlerGrid = new Grid()
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        public Grid Show(string path)
        {
            // Mostrar archivos 'r3dlog'
            fileGrid.Children.Add(FileList.ShowFiles(path, false, true, "r3dlog", ".txt"));

            // Añadir Grids de log y file.
            handlerGrid.Children.Add(logGrid);
            handlerGrid.Children.Add(fileGrid);
            handlerGrid.Children.Add(searchBar);
            handlerGrid.Children.Add(pathLabel);

            searchBar.KeyDown += SearchBar_KeyDown;
            searchBar.PreviewMouseLeftButtonDown += SearchBar_PreviewMouseLeftButtonDown;
            searchBar.LostFocus += SearchBar_LostFocus;

            // Cambia ciertos atributos de la barra de bsuqueda cuando pierde focus.
            void SearchBar_LostFocus(object sender, RoutedEventArgs e)
            {
                if (searchBar.Text == "")
                {
                    searchBar.Text = "Escribe aqui para buscar";

                    searchBar.Foreground = new SolidColorBrush(Color.FromArgb(180, 0, 0, 0));

                    searchBar.FontStyle = FontStyles.Italic;
                }
            }

            // Cambia ciertos atributos de la barra de bsuquedaa cuando el usuario presiona para escribir texto.
            void SearchBar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                if (searchBar.FontStyle == FontStyles.Italic)
                {
                    searchBar.FontStyle = FontStyles.Normal;

                    searchBar.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

                    searchBar.Text = "";
                }
            }

            // Buscar palabra en los archivos cuando se presiona enter.
            void SearchBar_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.Key == Key.Enter)
                {
                    fileGrid.Children.Clear();

                    fileGrid.Children.Add(FileList.ShowFiles(path, false, true, searchBar.Text, ".txt"));
                }
            }

            return handlerGrid;
        }
    }
}
