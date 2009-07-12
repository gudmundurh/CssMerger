using System.Collections.Generic;
using System.IO;

namespace CssMerger
{
    internal static class FilenameResolver
    {
        public static string ResolveFilename(string filename, string relativeToFilename)
        {
            return Resolve(relativeToFilename, filename, Path.DirectorySeparatorChar);
        }

        public static string ResolveUrl(string url, string relativeToUrl)
        {
            return Resolve(relativeToUrl, url, '/');
        }

        private static string Resolve(string relativeToFilename, string filename, char outputPathSeparator)
        {
            var baseParts = new List<string>(relativeToFilename.Split(outputPathSeparator));
            var filenameParts = new List<string>(filename.Split('/'));

            if (baseParts.Count <= 1)
                return filename;

            baseParts.RemoveAt(baseParts.Count - 1);

            foreach (string part in filenameParts)
            {
                if (part == "..")
                {
                    if (baseParts.Count > 0)
                        baseParts.RemoveAt(baseParts.Count - 1);
                }
                else
                {
                    baseParts.Add(part);
                }
            }

            return string.Join(new string(outputPathSeparator, 1), baseParts.ToArray());
        }
    }
}