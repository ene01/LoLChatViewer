using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LoLChatViewer.UI.Animations;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using LoLChatViewer.UI.Builders;
using LoLChatViewer.TextProcessing;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace LoLChatViewer
{
    public class ChatViewer
    {
        public static Dispatcher dispatcher = System.Windows.Application.Current.Dispatcher;

        // Grids temporales que se usan para almacenar informacion de cuales son los Grids apuntados y clickeados anteriormente, son usados para ayudar en las animaciones.
        private static StackPanel oldHoveredSP = null, oldClickedSP = null;

        // Label temporal que se usa para almacenar informacion de cual es el Label clickeado anteriormente
        // no se nesesita saber cual fue apuntado anteriormente ya que los labels no cambian de color al apuntarlos.
        private static TextBlock oldClickedTB = null, oldClickedTBTwo = null, oldHoveredTBTwo = null;

        // Lista de strings en donde se guardan las ubicaciones de los archivos 'r3dlog', este es el que despues es usado para saber que archivo se debe mostrar
        private static List<string> files = new();

        // Ubicaciones de las carpetas que se usan, leagueFolderPath vendira siendo la carpeta League of Legends
        // leagueLogFolderPath es la ubicacion de la carpeta 'Logs' dentro de 'League of Legends'
        // y entireLeaguePath son esas dos de antes pero combinadas.
        private String lolFolderPath = Properties.Settings.Default.MainLeaguePath, r3dlogFolderPath = "\\Logs\\GameLogs", entireLoLPath = "";

        // Grid principal en el que se muestra todo el programa.
        private static Grid mainGrid = new()
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(69, 69, 69)),
            IsHitTestVisible = true,
            Opacity = 0
        };

        // Grid que se usa para acomodar el ScrollView que muestra los archivos leidos.
        private static Grid fileGrid = new()
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Left,
            Width = 240,
            Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 30, 30, 30)),
            Margin = new Thickness(0, 20, 0, 0),
            ClipToBounds = true
        };

        private TextBox searchBar = new()
        {
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Left,
            FontStyle = FontStyles.Italic,
            Text = "Escribe aqui para buscar",
            Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(180, 0, 0, 0)),
            Height = 20,
            Width = 240
        };

        // Grid que se usa para acomodar el ScrollView que muestrra los mensajes del archivo elegido.
        private static Grid logGrid = new()
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 60, 60, 60)),
            Margin = new Thickness(250, 0, 0, 0),
            ClipToBounds = true
        };

        // Grid delgado posicionado arriba que solo muestra texto.
        private Grid titleGrid = new()
        {
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Height = 100,
            Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0)),
            IsHitTestVisible = true,
        };

        // Label fijo que se usa dentro del titleGrid.
        private Label titleLabel = new()
        {
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Left,
            Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0)),
            Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 255, 255, 255)),
            Margin = new Thickness(10, 6, 0, 0),
            FontSize = 14,
            Content = "🡶 Estas viendo: ",
        };

        // Label que muestra el 'Path' actual del archivo elegido a mostrar.
        private static Label pathLabel = new()
        {
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Left,
            Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0)),
            Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(120, 255, 255, 255)),
            Margin = new Thickness(110, 6, 0, 0),
            FontSize = 14,
            FontStyle = FontStyles.Italic,
            Content = "Nada...",
        };

        // Grid que contiene a fileGrid y logGrid, usado mas que nada para seprar un poco todo.
        private Grid middleGrid = new Grid()
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

            // Mostrar archivos 'r3dlog'
            fileGrid.Children.Add(ShowFiles(entireLoLPath, false, true, "r3dlog", ".txt"));

            // Añadir Grids de log y file.
            middleGrid.Children.Add(logGrid);
            middleGrid.Children.Add(fileGrid);
            middleGrid.Children.Add(searchBar);

            // Añadir labels.
            titleGrid.Children.Add(titleLabel);
            titleGrid.Children.Add(pathLabel);

            // Añadir todo el Grid principal
            mainGrid.Children.Add(titleGrid);
            mainGrid.Children.Add(middleGrid);

            // Setear eventos.
            middleGrid.PreviewKeyDown += SelectFileManually;
            searchBar.KeyDown += SearchBar_KeyDown;
            searchBar.PreviewMouseLeftButtonDown += SearchBar_PreviewMouseLeftButtonDown;
            searchBar.LostFocus += SearchBar_LostFocus;

            Animate.Opacity(mainGrid, 1, quadratic, 1300, 0, 0);

            Animate.Position(middleGrid, new TranslateTransform(0, 0), quadratic, 600, 0, new TranslateTransform(0, 50));
            Animate.Position(titleGrid, new TranslateTransform(0, 0), quadratic, 600, 0, new TranslateTransform(0, -50));

            return mainGrid;
        }

        // Cambia ciertos atributos de la barra de bsuqueda cuando pierde focus.
        private void SearchBar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchBar.Text == "")
            {
                searchBar.Text = "Escribe aqui para buscar";

                searchBar.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(180, 0, 0, 0));

                searchBar.FontStyle = FontStyles.Italic;
            }
        }

        // Cambia ciertos atributos de la barra de bsuquedaa cuando el usuario presiona para escribir texto.
        private void SearchBar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (searchBar.FontStyle == FontStyles.Italic)
            {
                searchBar.FontStyle = FontStyles.Normal;

                searchBar.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0));

                searchBar.Text = "";
            }
        }

        // Buscar palabra en los archivos cuando se presiona enter.
        private void SearchBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                fileGrid.Children.Clear();

                fileGrid.Children.Add(ShowFiles(entireLoLPath, false, true, searchBar.Text, ".txt"));
            }
        }

        // Mostrar select file dialog y checkear que la ubicacion es valida.
        private void PathCheck()
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
        private void SelectFileManually(object sender, KeyEventArgs e)
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
                    logGrid.Children.Clear();
                    logGrid.Children.Add(MessageListBuilder.ShowMessages(ofd.FileName));
                    pathLabel.Content = ofd.FileName;
                    DeselectFile();
                }
            }
        }

        // Recibe una ubicacion de una carpeta y retorna un ScrollViewer que contiene un StackPanel con mini StackPanels clickeables y animados con los nombres de cada archivo leido.
        //
        // path = Ubicacion de la carpeta a revisar.
        // doStrictNameSearch = Asegura que los archivos leidos tengan el titulo exacto dado en 'nameContains'.
        // doSubDirectorySearch = Busca en todos los subdirectorios.
        // nameContains = String que el titulo de los archivos debe contener para que sean mostrados.
        // extensions = Extensiones que los archivos deben tener para ser mostrados.
        //
        // El parametro 'nameContains' no debe tener extensiones de archivo a menos que el nombre tenga varios, ej:
        // 'File.xaml.cs' <- ('.cs' no va en 'nameContains', pero el '.xaml' si ya que es parte del nombre)', siempre poner
        // las extensiones del final en 'extensions'.
        //
        // Las extensiones deben ser especificados de esta forma: extensions = ".txt|.pdf|.jpeg|.cs".
        private static ScrollViewer ShowFiles(string path, bool doStrictNameSearch, bool doSubDirectorySearch, string nameContains = "", string extensions = "")
        {
            // Separar cada extension dada y guardarlo en una List.
            List<string> extensionList = TextProcessing.Text.SeparateWords(extensions, '|');

            // Contador de posiciones, se inicia en '-1' debido a que es usado mas tarde como un index de array para saber que StackPanel fue seleccionado.
            int filePosition = -1;

            ScrollViewer fileScrollViewer = new ScrollViewer()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(40, 40, 40)),
            };

            StackPanel fileStackPanel = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(40, 40, 40)),
            };

            // Thread nuevo para evitar interrumpir el thread de UI.
            Thread thread = new(() =>
            {
                // Usar dispatcher de la aplicacion debido a que hacemos cambios a la UI.
                dispatcher.Invoke(() =>
                {
                    // Funcion local que busca los archivos del la ubicacion especificada en dPath (significa delegatePath).
                    void AddFiles(string dPath)
                    {
                        string[] subFiles = Directory.GetFiles(dPath);

                        foreach (string subFile in subFiles)
                        {
                            if (subFile.Contains(nameContains))
                            {
                                FileInfo fileInfo = new FileInfo(subFile);

                                bool canSkipLoop = false;

                                // Ver si la List de extensiones esta vacia para saber si se tienen que verificar extensiones de los archivos.
                                if (extensionList.Count != 0)
                                {
                                    foreach (string ext in extensionList)
                                    {
                                        if (!(fileInfo.Extension == ext))
                                        {
                                            canSkipLoop = true;
                                        }
                                    }
                                }

                                if (canSkipLoop)
                                {
                                    continue;
                                }

                                string fileNameNoExt = fileInfo.Name; // Hasta aqui, el nombre del archivo es completo, con extension y todo.

                                // Separar nombre y extension, aveces los archivos pueden no tener extension, asi que verificamos si ese es el caso antes de intentar quitar la extension del nombre.
                                if (fileNameNoExt.Contains('.'))
                                {
                                    fileNameNoExt = fileNameNoExt.Substring(0, fileNameNoExt.LastIndexOf('.'));
                                }


                                // Si se activo la busqueda estricta, verificamos que el nombre dado concuerda con el nombre del archivo.
                                if (doStrictNameSearch && !(fileNameNoExt == nameContains))
                                {
                                    continue;
                                }

                                // Si se paso por todo lo de arriba, significa que el archivo es valido para ver mostrado y contado.
                                files.Add(subFile);
                                filePosition += 1;

                                // Conseguir datos del archivo valido.
                                string currentFileInfo = fileInfo.CreationTime + " Size: " + (fileInfo.Length / 1000).ToString() + " Kb";

                                TextBlock fileNameTextBlock = new()
                                {
                                    VerticalAlignment = VerticalAlignment.Top,
                                    HorizontalAlignment = HorizontalAlignment.Stretch,
                                    Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0)),
                                    Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255)),
                                    Margin = new Thickness(10, 5, 10, 5),
                                    TextAlignment = TextAlignment.Center,
                                    TextWrapping = TextWrapping.Wrap,
                                    FontSize = 11,
                                    Text = "➤ " + fileNameNoExt + fileInfo.Extension
                                };

                                TextBlock fileInfoTextBlock = new()
                                {
                                    VerticalAlignment = VerticalAlignment.Top,
                                    HorizontalAlignment = HorizontalAlignment.Stretch,
                                    Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0)),
                                    Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255)),
                                    Opacity = 0.6,
                                    Margin = new Thickness(0, 0, 0, 5),
                                    FontSize = 9,
                                    TextWrapping = TextWrapping.WrapWithOverflow,
                                    Text = currentFileInfo,
                                    TextAlignment = TextAlignment.Center
                                };

                                StackPanel fileMiniStackPanel = new()
                                {
                                    VerticalAlignment = VerticalAlignment.Top,
                                    HorizontalAlignment = HorizontalAlignment.Stretch,
                                    Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0)),
                                    Name = "f" + filePosition
                                };

                                // Eventos de animaciones.
                                fileMiniStackPanel.MouseEnter += HoverAnimationEnter;
                                fileMiniStackPanel.MouseLeave += HoverAnimationLeave;

                                // Evento de Click.
                                fileMiniStackPanel.PreviewMouseDown += ClickFile;

                                fileMiniStackPanel.Children.Add(fileNameTextBlock);
                                fileMiniStackPanel.Children.Add(fileInfoTextBlock);

                                fileStackPanel.Children.Add(fileMiniStackPanel);
                            }
                        }

                        // Evento que se ejecuta cuando el mouse sale del contenedor
                        static void HoverAnimationLeave(object sender, MouseEventArgs e)
                        {
                            oldHoveredSP = null;

                            oldHoveredSP = null;

                            StackPanel currentSP = (StackPanel)sender;

                            TextBlock currentSecondaryTB = (TextBlock)currentSP.Children[1];

                            if (currentSP != oldClickedSP)
                            {
                                Animate.Color(currentSP, Animate.ColorProperty.Background, System.Windows.Media.Color.FromArgb(255, 0, 0, 0), quadratic, 300, 0);

                                Animate.Opacity(currentSecondaryTB, 0.6, quadratic, 250, 0);
                            }
                        }

                        // Evento que se ejecuta cuando el mouse entra al contenedor
                        static void HoverAnimationEnter(object sender, MouseEventArgs e)
                        {
                            StackPanel currentSP = (StackPanel)sender;

                            TextBlock currentSecondaryTB = (TextBlock)currentSP.Children[1];

                            if (currentSP != oldClickedSP)
                            {
                                if (oldHoveredSP != null)
                                {
                                    Animate.Color(oldHoveredSP, Animate.ColorProperty.Background, System.Windows.Media.Color.FromArgb(255, 0, 0, 0), quadratic, 100, 0);
                                    Animate.Opacity(currentSecondaryTB, 0.6, quadratic, 250, 0);
                                }

                                oldHoveredSP = currentSP;
                                oldHoveredTBTwo = currentSecondaryTB;

                                Animate.Color(currentSP, Animate.ColorProperty.Background, System.Windows.Media.Color.FromArgb(255, 40, 40, 40), quadratic, 100, 0);
                                Animate.Opacity(currentSecondaryTB, 0.8, quadratic, 250, 0);
                            }
                        }

                        // Evento que se ejecuta cuando se hace click al contenedor
                        static void ClickFile(object sender, MouseButtonEventArgs e)
                        {
                            StackPanel currentSP = (StackPanel)sender;

                            if (currentSP != oldClickedSP)
                            {
                                TextBlock currentTB = (TextBlock)currentSP.Children[0];

                                TextBlock currentSecondaryTB = (TextBlock)currentSP.Children[1];

                                if (oldClickedSP != null)
                                {
                                    oldClickedTB.FontWeight = FontWeights.Regular;
                                    oldClickedTBTwo.FontWeight = FontWeights.Regular;
                                    Animate.Color(oldClickedSP, Animate.ColorProperty.Background, System.Windows.Media.Color.FromArgb(255, 0, 0, 0), quadratic, 250, 0);
                                    Animate.Color(oldClickedTB, Animate.ColorProperty.Foreground, System.Windows.Media.Color.FromArgb(255, 255, 255, 255), quadratic, 250, 0);
                                    Animate.Color(oldClickedTBTwo, Animate.ColorProperty.Foreground, System.Windows.Media.Color.FromArgb(255, 255, 255, 255), quadratic, 250, 0);
                                }

                                oldClickedSP = currentSP;
                                oldClickedTB = currentTB;
                                oldClickedTBTwo = currentSecondaryTB;

                                currentTB.FontWeight = FontWeights.Bold;
                                currentSecondaryTB.FontWeight = FontWeights.Bold;
                                Animate.Color(currentSP, Animate.ColorProperty.Background, System.Windows.Media.Color.FromArgb(255, 255, 255, 255), quadratic, 100, 0);
                                Animate.Color(currentTB, Animate.ColorProperty.Foreground, System.Windows.Media.Color.FromArgb(255, 0, 0, 0), quadratic, 100, 0);
                                Animate.Color(currentSecondaryTB, Animate.ColorProperty.Foreground, System.Windows.Media.Color.FromArgb(255, 0, 0, 0), quadratic, 250, 0);

                                string indexPosition = currentSP.Name.Substring(1);
                                string pathSelected = files[Convert.ToInt32(indexPosition)];

                                // A partir de aca es donde podes poner lo que sea que queres hacer con el path seleccionaado.
                                logGrid.Children.Clear();
                                logGrid.Children.Add(MessageListBuilder.ShowMessages(pathSelected));

                                pathLabel.Content = pathSelected;
                            }
                        }

                        // Si se habilito la busqueda a los subdirectorios, entonces conseguir los subdirectorios del 'dPath' actual y seguir asi sucesivamente.
                        // Con esta recursion, nos permite buscar los subdirectorios de un subdirectorio dentro de un.....
                        if (doSubDirectorySearch)
                        {
                            string[] subDirectories = Directory.GetDirectories(dPath);

                            if (subDirectories.Length != 0)
                            {
                                foreach (string subDirectory in subDirectories)
                                {
                                    AddFiles(subDirectory);
                                }
                            }
                        }
                    }

                    // Comenzar funcion local por primera vez.
                    AddFiles(path);
                });
            });
            // Iniciar thread que añade los elementos al StackPanel.
            thread.Start();

            fileScrollViewer.Content = fileStackPanel;

            return fileScrollViewer;
        }

        // Quita la seleccion del elemento.
        private static void DeselectFile()
        {
            if (oldClickedSP != null)
            {
                Animate.Color(oldClickedSP, Animate.ColorProperty.Background, System.Windows.Media.Color.FromArgb(255, 0, 0, 0), quadratic, 250, 0);
                Animate.Color(oldClickedTB, Animate.ColorProperty.Foreground, System.Windows.Media.Color.FromArgb(255, 255, 255, 255), quadratic, 250, 0);
            }
        }

    }
}
