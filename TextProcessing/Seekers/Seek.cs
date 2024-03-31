using LoLChatViewer.IO;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;

namespace LoLChatViewer.TextProcessing.Seekers
{
    public class Seek
    {
        /// <summary>
        /// Busca el tipo de mensaje, puede ser Team, All o Party.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static string MessageType(string line)
        {
            List<string> words = Text.SeparateWords(line, ' ');

            string messageType = "";

            for (int i = words.Count; i > 0; i--) // Lee la lista de atras para adelante.
            {
                if (i != words.Count) // Ignorar ultima palabra.
                {
                    if (words[i].Contains("Team"))
                    {
                        messageType = "Team";
                        break;
                    }
                    else if(words[i].Contains("All"))
                    {
                        messageType = "All";
                        break;
                    }
                    else if (words[i].Contains("Party"))
                    {
                        messageType = "Party";
                        break;
                    }
                }
            }
            return messageType;
        }

        /// <summary>
        /// Busca el usuario que envio el mensaje
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static string User(string line)
        {
            List<string> words = Text.SeparateWords(line, ' ');

            words.Reverse();

            List<string> usernameWords = [];

            foreach (string word in words) // Leer archivo alreves
            {
                if (!word.StartsWith("</font>")) // Ignorar primera palabra encontrada porque ya se sabe que no sirve de nada
                {
                    if (word.StartsWith("color=")) // Si encontramos una palabra que empieza con 'color=', entonces ya terminamos de leer el username
                    {
                        break;
                    }
                    usernameWords.Add(word);
                }
            }

            usernameWords.Reverse(); // Como leeimos el archivo de atras para adelante, lo damos vuelta

            string usernameFull = string.Join(" ", usernameWords); // Juntar y guardarlo en un string, cada palabra separada por un espacio

            usernameFull.Remove(usernameFull.Length - 2);

            return usernameFull;
        }

        /// <summary>
        /// Busca y toma los numeros de tiempo al principio del 'r3dlog' y los retorna en un formato mas entendible.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static string Timestamp(string line)
        {
            List<string> words = Text.SeparateWords(line, ' ');

            string rawTimeString = "";

            for (int i = 0; i <= 5; i++)
            {
                rawTimeString += words[0][i]; // Leer cada numero del tiempo
            }

            int minutes = int.Parse(rawTimeString) / 60; // Dividir por sesenta para tener los minutos.
            int seconds = int.Parse(rawTimeString) % 60; // Conseguir el resto de la misma division para tener los segundos.

            string stringMinutes = "", stringSeconds = "";

            // Añadir un cero en caso de que el resultado haya sido un solo digito.
            if (minutes < 10)
            {
                stringMinutes = "0" + minutes.ToString();
            }
            else
            {
                stringMinutes = minutes.ToString();
            }

            if (seconds < 10)
            {
                stringSeconds = "0" + seconds.ToString();
            }
            else
            {
                stringSeconds = seconds.ToString();
            }

            string formatedTime = stringMinutes + ":" + stringSeconds;

            return formatedTime;
        }

        /// <summary>
        /// Deternima el color de equipo del usuario que envio el mensaje, puede ser 'Blue o 'Red'
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static System.Windows.Media.Color TeamColor(string line)
        {
            if (line.Contains("color="))
            {
                List<string> words = Text.SeparateWords(line, ' ');

                System.Windows.Media.Color color = new();

                for (int i = words.Count - 1; i > 0; i--)
                {
                    if (words[i].StartsWith("color="))
                    {
                        if (words[i].Contains("#40C1FF"))
                        {
                            color = System.Windows.Media.Color.FromArgb(255, 97, 204, 255);
                        }
                        else if (words[i].Contains("#FF3333"))
                        {
                            color = System.Windows.Media.Color.FromArgb(255, 255, 105, 105);
                        }
                    }
                }
                return color;
            }
            return System.Windows.Media.Color.FromArgb(255, 0, 0, 0);
        }

        /// <summary>
        /// Busca el mensaje del usuario.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static string Message(string line)
        {
            if (line.Contains("Chat received valid message:"))
            {
                const int MESSAGE_STARTING_INDEX = 6;

                List<string> words = Text.SeparateWords(line, ' ');

                List<string> messageWords = [];

                List<string> endKeyWords = ["with", "speaker", "DisplayName"]; // y terminan con esto.

                for (int i = MESSAGE_STARTING_INDEX; i < words.Count; i++)
                {
                    messageWords.Add(words[i]);

                    if (words[i + 1].Contains(endKeyWords[0]) && words[i + 2].Contains(endKeyWords[1]) && words[i + 3].Contains(endKeyWords[2])) // Lo mismo que antes pero en este caso se buscan las palabras que terminan el mensaje.
                    {
                        break;
                    }
                }

                string message = string.Join(" ", messageWords); // Quitamos la palabras del final ya que sabemos que no son parte del mensaje.

                return message;
            }
            return null;
        }

        /// <summary>
        /// Revisa si hubo una desconexion.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool Disconnection(string line)
        {
            if (line.Contains("Disconnected:"))
            {
                return true;
            }

            return false;
        }
    }
}
