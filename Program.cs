using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LoLChatViewer.UI.Animations;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using NAudio.Wave;
using LoLChatViewer.UI.Panels;

namespace LoLChatViewer
{
    public class ChatViewerWindow
    {
        public static Dispatcher dispatcher = System.Windows.Application.Current.Dispatcher;

        // Ubicaciones de las carpetas que se usan, leagueFolderPath vendira siendo la carpeta League of Legends
        // leagueLogFolderPath es la ubicacion de la carpeta 'Logs' dentro de 'League of Legends'
        // y entireLeaguePath son esas dos de antes pero combinadas.
        private static String lolFolderPath = Properties.Settings.Default.MainLeaguePath, r3dlogFolderPath = "\\Logs\\GameLogs", entireLoLPath = "";

        // Grid principal en el que se muestra todo el programa.
        private static Grid mainGrid = new()
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(69, 41, 64)),
            IsHitTestVisible = true,
            Opacity = 0
        };

        // Grid delgado posicionado arriba que solo muestra texto.
        private static Grid topGrid = new()
        {
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Height = 100,
            Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0)),
            IsHitTestVisible = true,
        };

        // Grid que contiene a fileGrid y logGrid, usado mas que nada para seprar un poco todo.
        private static Grid bottomGrid = new Grid()
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(0, 40, 0, 0),
        };

        // Easing usado en las animaciones.
        private static QuadraticEase quadratic = new()
        {
            EasingMode = EasingMode.EaseOut
        };

        // Inicio
        public Grid Start()
        {
            if (lolFolderPath == "") // Vacio significa que es la primera vez que se inicia el prrograma.
            {
                if (Directory.Exists("C:\\Riot Games\\League of Legends")) // Si existe y tiene archivos adentro, entonces guardala como default y setearla para usarla en el momento.
                {
                    if (Directory.GetFiles("C:\\Riot Games\\League of Legends").Length > 0)
                    {
                        lolFolderPath = "C:\\Riot Games\\League of Legends";
                        Properties.Settings.Default.MainLeaguePath = lolFolderPath;
                    }
                }
                else
                {
                    MessageBox.Show("La carpeta 'League of Legends' parece estar en otro lado, selecciona la ubicacion correcta", "Caracoles", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                    PathCheck();
                }
            }

            // Crear path completo.
            entireLoLPath = lolFolderPath + r3dlogFolderPath;

            topGrid.Children.Add(TitleElements.Show());
            bottomGrid.Children.Add(LogViewerElements.Show(entireLoLPath));

            // Añadir todo el Grid principal
            mainGrid.Children.Add(topGrid);
            mainGrid.Children.Add(bottomGrid);

            // Setear eventos.
            mainGrid.PreviewKeyDown += SelectFileManually;

            Animate.Opacity(mainGrid, 1, quadratic, 1300, 0, 0);

            Animate.Position(bottomGrid, new TranslateTransform(0, 0), quadratic, 600, 0, new TranslateTransform(0, 50));
            Animate.Position(topGrid, new TranslateTransform(0, 0), quadratic, 600, 0, new TranslateTransform(0, -50));

            return mainGrid;
        }

        // Mostrar select file dialog y checkear que la ubicacion es valida.
        private static void PathCheck()
        {
            OpenFolderDialog ofd = new()
            {
                Title = "Seleccione la carpeta 'League of Legends'",
            };

            bool? DIALOG_RESULT = ofd.ShowDialog(); // Si el usuario presiona 'OK' este boolean da true como resultado.

            if (DIALOG_RESULT == true && !string.IsNullOrWhiteSpace(ofd.FolderName))
            {
                lolFolderPath = ofd.FolderName;
                entireLoLPath = lolFolderPath + r3dlogFolderPath;

                // Guardar ajustes
                Properties.Settings.Default.MainLeaguePath = lolFolderPath;
                Properties.Settings.Default.Save();

                if (!Directory.Exists(entireLoLPath))
                {
                    MessageBox.Show("Ubicacion invalida", "Frijoles", MessageBoxButton.OK, MessageBoxImage.Error);

                    PathCheck();
                }
            }
            else
            {
                Environment.Exit(0);
            }
        }

        // Funcion especial que hace que puedas abrir un archivo especifico presionando la tecla 'L'
        private static void SelectFileManually(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.L)
            {
                OpenFileDialog ofd = new()
                {
                    Title = "Seleccione un archivo 'r3dlog'",
                    Filter = "Archivos de texto (*.txt*)|*.txt|Todos los archivos (*.*)|*.*"
                };

                bool? result = ofd.ShowDialog();

                if (result == true)
                {
                    LogViewerElements.logGrid.Children.Clear();
                    LogViewerElements.logGrid.Children.Add(MessageList.ShowMessages(ofd.FileName));
                    LogViewerElements.pathLabel.Content = ofd.FileName;
                    FileListBuilder.DeselectFile();
                }
            }
        }        
    }
}
