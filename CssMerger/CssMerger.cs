using System.IO;
using System.Text.RegularExpressions;

namespace CssMerger
{
    public class CssMerger
    {
        private static readonly Regex ImportRegex =
            new Regex(
                @"
                    @import \s+ (?:                           
                        url \( (['""]) (?<url>.*?) \1 \)  # Match url() with quoted contents
                      | url \(         (?<url>.*?)    \)  # Match url() without quotes
                      |        (['""]) (?<url>.*?) \2     # Match quoted url, without url()
                    ) \s* ;
                ",
                RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex PropertyUrlRegex =
            new Regex(
                @"
                    (?<= : [^:;}]* )         # Look-behind to assert that we are in a property
                    (?:
                        url \( (['""]) (?<url>.*?) \1 \)  # Match url() with quoted contents
                      | url \(         (?<url>.*?)    \)  # Match url() without quotes
                    )
                ",
                RegexOptions.IgnorePatternWhitespace);

        private IFileManager fileManager;

        public CssMerger(IFileManager fileManager)
        {
            this.fileManager = fileManager;
        }

        public bool RaiseErrors { get; set; }

        public static void MergeCss(string inputFilename, string outputFilename)
        {
            var cssMerger = new CssMerger(new FileManager());
            cssMerger.MergeAndWrite(inputFilename, outputFilename);
        }


        public static void MergeCss(string inputFilename, string outputFilename, IFileManager fileManager)
        {
            var cssMerger = new CssMerger(fileManager);
            cssMerger.MergeAndWrite(inputFilename, outputFilename);
        }


        public void MergeAndWrite(string inputFilename, string outputFilename)
        {
            string outputPath = new FileInfo(outputFilename).DirectoryName;
            var inputFileInfo = new FileInfo(inputFilename);
            string startUrl = FilenameResolver.GetUrlRelativeToOutputPath(inputFileInfo.Name,
                                                                          inputFileInfo.DirectoryName, outputPath);

            fileManager.WriteFile(outputFilename, Merge(startUrl, outputPath, outputPath));
        }


        private string Merge(string url, string currentPath, string outputPath)
        {
            string filename = FilenameResolver.GetFilenameFromUrl(url, currentPath);
            string filenamePath = new FileInfo(filename).DirectoryName;

            string css = null;

            try
            {
                css = fileManager.ReadFile(filename);
            }
            catch (FileNotFoundException exception)
            {
                if (RaiseErrors)
                    throw exception;
            }

            if (css == null)
                return string.Format("/* CssMerger: {0} missing */", url);

            css = PropertyUrlRegex.Replace(css, m => ResolvePropertyUrl(m.Groups["url"].Value, filenamePath, outputPath));

            return ImportRegex.Replace(css, m => Merge(m.Groups["url"].Value, filenamePath, outputPath));
        }


        private static string ResolvePropertyUrl(string url, string currentPath, string outputPath)
        {
            return string.Format("url({0})", FilenameResolver.GetUrlRelativeToOutputPath(url, currentPath, outputPath));
        }
    }
}