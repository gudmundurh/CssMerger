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

        private readonly IFileManager _fileManager;

        public CssMerger() : this(new FileManager())
        {
        }

        public CssMerger(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public void MergeCss(string inputFilename, string outputFilename)
        {
            string outputPath = new FileInfo(outputFilename).DirectoryName;
            var inputFileInfo = new FileInfo(inputFilename);
            string startUrl = FilenameResolver.GetUrlRelativeToOutputPath(inputFileInfo.Name,
                                                                          inputFileInfo.DirectoryName, outputPath);

            _fileManager.WriteFile(outputFilename, Merge(startUrl, outputPath, outputPath));
        }

        private string Merge(string url, string currentPath, string outputPath)
        {
            string filename = FilenameResolver.GetFilenameFromUrl(url, currentPath);
            string filenamePath = new FileInfo(filename).DirectoryName;

            string css = _fileManager.ReadFile(filename);

            css = PropertyUrlRegex.Replace(css,
                                           match =>
                                           ResolvePropertyUrl(match.Groups["url"].Value, filenamePath, outputPath));

            return ImportRegex.Replace(css, match => Merge(match.Groups["url"].Value, filenamePath, outputPath));
        }


        private static string ResolvePropertyUrl(string url, string currentPath, string outputPath)
        {
            return string.Format("url({0})", FilenameResolver.GetUrlRelativeToOutputPath(url, currentPath, outputPath));
        }
    }
}