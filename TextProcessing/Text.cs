using System.Diagnostics;

namespace LoLChatViewer.TextProcessing
{
    public class Text
    {
        public static List<string> SeparateWords(string line, char separator)
        {
            line += separator.ToString();

            List<string> words = new();

            string? word = null;

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] != separator)
                {
                    word += line[i].ToString();
                }
                else if (word != null)
                {
                    words.Add(word);
                    word = null;
                }
            }
            return words;
        }
    }
}
