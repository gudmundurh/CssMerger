namespace CssMerger
{
    public interface IFileManager
    {
        string ReadFile(string filename);
        void WriteFile(string filename, string contents);
    }
}