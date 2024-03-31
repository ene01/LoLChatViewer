// A class that parses the lines of a file, set the Path and (if needed) the LineNumber property and get the line you want from the file.
using System.IO;

namespace LoLChatViewer.IO
{
    public static class Read
    {
        private static string[] lines;

        private static string _filePath = "";
        public static string FilePath
        {
            get
            {
                return _filePath;
            }

            set
            {
                if (value != _filePath)
                {
                    _filePath = value;

                    using StreamReader sr = new(_filePath);

                    if (File.Exists(_filePath))
                    {
                        lines = File.ReadAllLines(_filePath);
                    }
                    else
                    {
                        _filePath = "";
                    }
                }
            }
        }

        /// <summary>
        /// Returns a line from the file you set in 'Path', if no specific 'LineNumber' was selected, then it will just read from line 0. If this returns null, it means the line doesn't exist or you set an invalid path.
        /// </summary>
        /// <returns></returns>
        public static string Line(int line)
        {
            try
            {
                return lines[line];
            }
            catch
            {
                return null;
            }
        }
    }
}