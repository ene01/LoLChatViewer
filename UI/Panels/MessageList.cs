using LoLChatViewer.IO;
using LoLChatViewer.TextProcessing.Seekers;
using LoLChatViewer.UI.Animations;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace LoLChatViewer.UI.Panels
{
    public class MessageList : LogViewerElements
    {
        private static QuadraticEase quadratic = new() // Usado en las animaciones.
        {
            EasingMode = EasingMode.EaseOut
        };

        public static ScrollViewer ShowMessages(string path)
        {
            StackPanel logStackPanel = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new SolidColorBrush(Color.FromRgb(54, 34, 54)),
            };

            ScrollViewer logScrollViewer = new ScrollViewer()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new SolidColorBrush(Color.FromRgb(54, 34, 54)),
            };

            Read.FilePath = path;

            bool alternateColor = false, isDisconnected = false;

            int counter = -1, messageCounter = 0, messagesToIgnore = 0, fileLineAmount = File.ReadAllLines(path).Length;

            Thread thread = new Thread(() =>
            {
                while (fileLineAmount != counter)
                {
                    string currentLine = Read.Line(counter);

                    if (counter == -1 || currentLine != null)
                    {
                        // Verificar si la linea corresponde a un desconexion
                        if (counter != -1 && Seek.Disconnection(currentLine))
                        {
                            isDisconnected = true;

                            messagesToIgnore = messageCounter;
                        }

                        // Si 'i' es '-1', significa que solo hay que mostrar los TextBox que muestran que datos hay en las columnas
                        if (counter == -1 || Seek.Message(currentLine) != null && messagesToIgnore == 0)
                        {
                            isDisconnected = false;

                            string time;

                            string type;

                            string user;

                            string msg;

                            Color tc = new();

                            if (counter != -1)
                            {
                                messageCounter += 1; // Mensaje valido = un mensaje al contador

                                // Conseguir partes del mensaje
                                time = " " + Seek.Timestamp(currentLine);

                                type = Seek.MessageType(currentLine);

                                user = Seek.User(currentLine);

                                msg = Seek.Message(currentLine);

                                tc = Seek.TeamColor(currentLine);
                            }
                            else
                            {
                                time = " Time";

                                type = "Type";

                                user = "Username";

                                msg = "Message";
                            }

                            Dispatcher.Invoke(() =>
                            {
                                // Crear TextBoxes
                                TextBlock timeTB = new TextBlock
                                {
                                    FontSize = 15,
                                    Height = 25,
                                    HorizontalAlignment = HorizontalAlignment.Stretch,
                                    TextAlignment = TextAlignment.Center,
                                    TextWrapping = TextWrapping.WrapWithOverflow,
                                    Foreground = new SolidColorBrush(Colors.White),
                                    Text = time
                                };

                                TextBlock typeTB = new TextBlock
                                {
                                    FontSize = 15,
                                    HorizontalAlignment = HorizontalAlignment.Stretch,
                                    TextAlignment = TextAlignment.Center,
                                    Width = 80,
                                    TextWrapping = TextWrapping.WrapWithOverflow,
                                    Text = type
                                };

                                TextBlock userTB = new TextBlock
                                {
                                    FontSize = 15,
                                    HorizontalAlignment = HorizontalAlignment.Stretch,
                                    Width = 210,
                                    TextWrapping = TextWrapping.WrapWithOverflow,
                                    Text = user
                                };

                                TextBlock msgTB = new()
                                {
                                    FontSize = 15,
                                    HorizontalAlignment = HorizontalAlignment.Stretch,
                                    TextAlignment = TextAlignment.Left,
                                    TextWrapping = TextWrapping.WrapWithOverflow,
                                    Foreground = new SolidColorBrush(Colors.White),
                                    Text = msg
                                };

                                // Establecer color del User y Type dependiendo del color del equipo.
                                if (counter != -1)
                                {
                                    typeTB.Foreground = new SolidColorBrush(tc);
                                    userTB.Foreground = new SolidColorBrush(tc);
                                }
                                else if (counter == -1)
                                {
                                    timeTB.Foreground = new SolidColorBrush(Colors.White);
                                    typeTB.Foreground = new SolidColorBrush(Colors.White);
                                    userTB.Foreground = new SolidColorBrush(Colors.White);
                                    msgTB.Foreground = new SolidColorBrush(Colors.White);
                                    timeTB.FontWeight = FontWeights.Bold;
                                    typeTB.FontWeight = FontWeights.Bold;
                                    userTB.FontWeight = FontWeights.Bold;
                                    msgTB.FontWeight = FontWeights.Bold;
                                }
                                else
                                {
                                    typeTB.Foreground = new SolidColorBrush(Colors.Yellow);
                                    userTB.Foreground = new SolidColorBrush(Colors.Yellow);
                                }

                                Grid msgGrid = new Grid();

                                // Alternar el color de los fondos para que sea mas legible.
                                if (alternateColor && counter != -1)
                                {
                                    msgGrid.Background = new SolidColorBrush(Color.FromArgb(80, 0, 0, 0)); // Color 1

                                    alternateColor = !alternateColor;
                                }
                                else if (counter == -1)
                                {
                                    msgGrid.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0)); // Color de la primerra baarra que describe los valores
                                }
                                else
                                {
                                    msgGrid.Background = new SolidColorBrush(Color.FromArgb(10, 0, 0, 0)); // Color 2

                                    alternateColor = !alternateColor;
                                }

                                // Crear 4 columnas en el grid y asignarles un tamaño automatico, a excepcion de la ultima columna que es la del mensaje, esa se deja que ocupe todo el espacio horizontal restante.
                                ColumnDefinition column1 = new ColumnDefinition
                                {
                                    Width = GridLength.Auto,
                                };

                                ColumnDefinition column2 = new ColumnDefinition
                                {
                                    Width = GridLength.Auto
                                };

                                ColumnDefinition column3 = new ColumnDefinition
                                {
                                    Width = GridLength.Auto
                                };

                                ColumnDefinition column4 = new ColumnDefinition();

                                // Añadir definiciones al grid.
                                msgGrid.ColumnDefinitions.Add(column1);
                                msgGrid.ColumnDefinitions.Add(column2);
                                msgGrid.ColumnDefinitions.Add(column3);
                                msgGrid.ColumnDefinitions.Add(column4);

                                // Asignar los TextBoxes a sus respectivas columnas.
                                Grid.SetRow(timeTB, 0);
                                Grid.SetColumn(timeTB, 0);
                                Grid.SetRow(typeTB, 0);
                                Grid.SetColumn(typeTB, 1);
                                Grid.SetRow(userTB, 0);
                                Grid.SetColumn(userTB, 2);
                                Grid.SetRow(msgTB, 0);
                                Grid.SetColumn(msgTB, 3);

                                // Mostrarlos en el grid.
                                msgGrid.Children.Add(timeTB);
                                msgGrid.Children.Add(typeTB);
                                msgGrid.Children.Add(userTB);
                                msgGrid.Children.Add(msgTB);

                                Random rnd = new();

                                Animate.Position(msgGrid, new TranslateTransform(0, 0), quadratic, 300, 0, new TranslateTransform(20, 0));
                                Animate.Opacity(msgGrid, 1, quadratic, 60, 0, 0);

                                // Mostrar grid.
                                logStackPanel.Children.Add(msgGrid);
                            });
                        }
                        else if (Seek.Message(currentLine) != null && isDisconnected)
                        {
                            messagesToIgnore -= 1;
                        }
                    }
                    counter += 1;
                }
            });
            thread.Start();

            logScrollViewer.Content = logStackPanel;

            return logScrollViewer;
        }
    }
}
