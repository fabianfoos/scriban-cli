using System.IO;

namespace ScribanCli
{
    internal static class FileTools
    {
        public static void Write(string filename, string content)
        {
            new FileInfo(filename).Directory.Create();
            File.WriteAllText(filename, content);
        }

        public static void WriteIfNotExist(string filename, string content)
        {
            if (!Exists(filename))
                Write(filename, content);
        }

        public static bool Exists(string filename)
        {
            return File.Exists(filename);
        }
    }
}