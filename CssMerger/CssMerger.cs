using System.IO;
using System.Text.RegularExpressions;

namespace CssMerger
{
    public class CssMerger
    {
        private static readonly Regex importRegex = new Regex(
            " @import \\s+ (?:                           " +
            "    url \\( (['\"]) (?<url>[^\\1]*) \\1 \\) " + // Match url() with quoted contents
            "  | url \\(         (?<url>[^\\)]*)     \\) " + // Match url() without quotes
            "  |         (['\"]) (?<url>[^\\2]*) \\2     " + // Match quoted url, without url()
            " ) \\s* ;                                   ", RegexOptions.IgnorePatternWhitespace);

        private IFileManager fileManager;

        public CssMerger(IFileManager fileManager)
        {
            this.fileManager = fileManager;
        }

        public bool RaiseErrors { get; set; }


        public static void MergeCss(string inputFilename, string outputFilename, IFileManager fileManager)
        {
            var cssMerger = new CssMerger(fileManager);
            cssMerger.MergeAndWrite(inputFilename, outputFilename);
        }


        public void MergeAndWrite(string inputFilename, string outputFilename)
        {
            fileManager.WriteFile(outputFilename, Merge(inputFilename, null));
        }


        private string Merge(string inputFilename, string relativeToPath)
        {
            if (relativeToPath != null)
                inputFilename = FilenameResolver.ResolveFilename(inputFilename, relativeToPath);

            string css = null;

            try
            {
                css = fileManager.ReadFile(inputFilename);
            }
            catch (FileNotFoundException exception)
            {
                if (RaiseErrors)
                    throw exception;
            }

            if (css == null)
                return string.Format("/* CssMerger: {0} missing */", inputFilename);

            return importRegex.Replace(css, m => Merge(m.Groups["url"].Value, inputFilename));
        }
    }
}