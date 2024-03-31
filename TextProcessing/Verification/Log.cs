using LoLChatViewer.TextProcessing;

namespace LoLChatViewer.TextProcessing.Verification
{
    public class Log
    {
        /// <summary>
        /// Verifies if the lines comes from a 'r3dlog' file.
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        public static bool Verify(string line)
        {
            if (line.Contains("ALWAYS|") || line.Contains("ERROR|")) // Un archivo 'r3dlog' simepre tiene estas dos palabras en la segunda posicion, si no las tiene, entonces el archivo no es un log de LoL
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
