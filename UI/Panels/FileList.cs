using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System.Windows;
using System.Windows.Input;
using LoLChatViewer.UI.Animations;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace LoLChatViewer.UI.Panels
{
    public class FileList
    {
        // Easing usado en las animaciones.
        public QuadraticEase quadratic = new()
        {
            EasingMode = EasingMode.EaseOut
        };

        // Grids temporales que se usan para almacenar informacion de cuales son los Grids apuntados y clickeados anteriormente, son usados para ayudar en las animaciones.
        private StackPanel oldHoveredSP = null, oldClickedSP = null;

        // Label temporal que se usa para almacenar informacion de cual es el Label clickeado anteriormente
        // no se nesesita saber cual fue apuntado anteriormente ya que los labels no cambian de color al apuntarlos.
        private TextBlock oldClickedTB = null, oldClickedTBTwo = null, oldHoveredTBTwo = null;

        // Lista de strings en donde se guardan las ubicaciones de los archivos 'r3dlog', este es el que despues es usado para saber que archivo se debe mostrar
        private List<string> files = new();

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
        public void ShowFilesIn(Dispatcher dispatcher, LogViewerElements logViewerElements, Panel element, string path, bool doStrictNameSearch, bool doSubDirectorySearch, string nameContains = "", string extensions = "")
        {
            ScrollViewer fileScrollViewer = new ScrollViewer()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
            };

            StackPanel fileStackPanel = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
            };

            // Separar cada extension dada y guardarlo en una List.
            List<string> extensionList = TextProcessing.Text.SeparateWords(extensions, '|');

            // Contador de posiciones, se inicia en '-1' debido a que es usado mas tarde como un index de array para saber que StackPanel fue seleccionado.
            int filePosition = -1;

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

                                // Ya que tenemos todos los datos que queremos del archivo, comenzamos a crear los elementos de UI y los añadimos.
                                TextBlock fileNameTextBlock = new()
                                {
                                    VerticalAlignment = VerticalAlignment.Top,
                                    HorizontalAlignment = HorizontalAlignment.Stretch,
                                    Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
                                    Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)),
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
                                    Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
                                    Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)),
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
                                    Background = new SolidColorBrush(Color.FromArgb(255, 26, 18, 24)),
                                    Name = "f" + filePosition
                                };

                                // Eventos de animaciones.
                                fileMiniStackPanel.MouseEnter += HoverAnimationEnter;
                                fileMiniStackPanel.MouseLeave += HoverAnimationLeave;

                                // Evento de Click.
                                fileMiniStackPanel.PreviewMouseUp += ClickFile;

                                fileMiniStackPanel.Children.Add(fileNameTextBlock);
                                fileMiniStackPanel.Children.Add(fileInfoTextBlock);

                                fileStackPanel.Children.Add(fileMiniStackPanel);
                            }
                        }

                        // Evento que se ejecuta cuando el mouse sale del contenedor.
                        void HoverAnimationLeave(object sender, MouseEventArgs e)
                        {
                            StackPanel currentSP = (StackPanel)sender; // Conseguir a que elemento se apunto el mouse.

                            TextBlock currentSecondaryTB = (TextBlock)currentSP.Children[1];

                            if (currentSP != oldClickedSP) // No animar en caso de que el elemento este seleccionado.
                            {
                                Animate.Color(currentSP, Animate.ColorProperty.Background, Color.FromArgb(255, 26, 18, 24), quadratic, 300, 0);

                                Animate.Opacity(currentSecondaryTB, 0.6, quadratic, 250, 0);
                            }
                        }

                        // Evento que se ejecuta cuando el mouse entra al contenedor.
                        void HoverAnimationEnter(object sender, MouseEventArgs e)
                        {
                            StackPanel currentSP = (StackPanel)sender;

                            TextBlock currentSecondaryTB = (TextBlock)currentSP.Children[1];

                            if (currentSP != oldClickedSP) // No animar en caso de que el elemento este seleccionado.
                            {
                                Animate.Color(currentSP, Animate.ColorProperty.Background, Color.FromArgb(255, 66, 48, 62), quadratic, 100, 0);
                                Animate.Opacity(currentSecondaryTB, 0.8, quadratic, 250, 0);
                            }   
                        }

                        // Evento que se ejecuta cuando se hace click al contenedor
                        void ClickFile(object sender, MouseButtonEventArgs e)
                        {
                            StackPanel currentSP = (StackPanel)sender;

                            if (currentSP != oldClickedSP) // Si el elemento al que le hiciste click ya estaba seleccionado, entonces no hacer nada para no repetir la aniamcion.
                            {
                                TextBlock currentTB = (TextBlock)currentSP.Children[0];

                                TextBlock currentSecondaryTB = (TextBlock)currentSP.Children[1];

                                if (oldClickedSP != null) // Si habia ya un elemento seleccionado, deseleccionarlo, sino, no hacer nada.
                                {
                                    oldClickedTB.FontWeight = FontWeights.Regular;
                                    oldClickedTBTwo.FontWeight = FontWeights.Regular;
                                    Animate.Color(oldClickedSP, Animate.ColorProperty.Background, Color.FromArgb(255, 26, 18, 24), quadratic, 250, 0);
                                    Animate.Color(oldClickedTB, Animate.ColorProperty.Foreground, Color.FromArgb(255, 255, 255, 255), quadratic, 250, 0);
                                    Animate.Color(oldClickedTBTwo, Animate.ColorProperty.Foreground, Color.FromArgb(255, 255, 255, 255), quadratic, 250, 0);
                                }

                                currentTB.FontWeight = FontWeights.Bold;
                                currentSecondaryTB.FontWeight = FontWeights.Bold;
                                Animate.Color(currentSP, Animate.ColorProperty.Background, Color.FromArgb(255, 255, 209, 244), quadratic, 100, 0);
                                Animate.Color(currentTB, Animate.ColorProperty.Foreground, Color.FromArgb(255, 0, 0, 0), quadratic, 100, 0);
                                Animate.Color(currentSecondaryTB, Animate.ColorProperty.Foreground, Color.FromArgb(255, 0, 0, 0), quadratic, 250, 0);

                                string indexPosition = currentSP.Name.Substring(1);
                                string pathSelected = files[Convert.ToInt32(indexPosition)];

                                // Los elementos que se animaron ahora pasan a ser los elementos 'viejos' ya seleccionados.
                                oldClickedSP = currentSP;
                                oldClickedTB = currentTB;
                                oldClickedTBTwo = currentSecondaryTB;

                                // A partir de aca es donde podes poner lo que sea que queres hacer con el path seleccionaado.
                                MessageList.ShowMessages(dispatcher, logViewerElements.logGrid, pathSelected);
                                logViewerElements.pathLabel.Content = pathSelected;
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

            element.Children.Add(fileScrollViewer);
        }

        // Quita la seleccion del elemento.
        public void DeselectFile()
        {
            if (oldClickedSP != null)
            {
                oldClickedTB.FontWeight = FontWeights.Regular;
                oldClickedTBTwo.FontWeight = FontWeights.Regular;
                Animate.Color(oldClickedSP, Animate.ColorProperty.Background, Color.FromArgb(255, 26, 18, 24), quadratic, 250, 0);
                Animate.Color(oldClickedTB, Animate.ColorProperty.Foreground, Color.FromArgb(255, 255, 255, 255), quadratic, 250, 0);
                Animate.Color(oldClickedTBTwo, Animate.ColorProperty.Foreground, Color.FromArgb(255, 255, 255, 255), quadratic, 250, 0);

                oldClickedSP = null;
                oldClickedTB = null;
                oldClickedTBTwo = null;
            }
        }
    }
}
