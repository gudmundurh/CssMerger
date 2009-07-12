using System.IO;

namespace CssMerger
{
    internal class FileManager : IFileManager
    {
        #region IFileManager Members

        public string ReadFile(string filename)
        {
            return File.ReadAllText(filename);
        }

        public void WriteFile(string filename, string contents)
        {
            File.WriteAllText(filename, contents);
        }

        #endregion
    }
}