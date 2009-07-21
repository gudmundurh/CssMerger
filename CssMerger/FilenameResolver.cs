using System;
using System.Collections.Generic;
using System.IO;

namespace CssMerger
{
    internal static class FilenameResolver
    {
        public static string GetFilenameFromUrl(string url, string currentPath)
        {
            if (!Path.IsPathRooted(currentPath))
                throw new ArgumentException("currentPath must be rooted");

            var pathParts = new List<string>(SplitPath(currentPath));

            foreach (string part in url.Split('/'))
            {
                if (part == "..")
                {
                    if (pathParts.Count == 0)
                        throw new Exception(string.Format("Can't get filename from url={0} for path={1}, too many ../", url, currentPath));
                    
                    pathParts.RemoveAt(pathParts.Count - 1);
                }
                else
                {
                    pathParts.Add(part);
                }
            }

            return string.Join(new string(Path.DirectorySeparatorChar, 1), pathParts.ToArray());
        }

        private static string[] SplitPath(string path)
        {
            return path.Trim(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
        }


        public static string GetUrlRelativeToOutputPath(string url, string currentPath, string outputPath)
        {
            if (!Path.IsPathRooted(currentPath))
                throw new ArgumentException("currentPath must be rooted");            

            if (!Path.IsPathRooted(outputPath))
                throw new ArgumentException("outputPath must be rooted");

            url = url.Trim();

            if (url.StartsWith("/"))
                return url;

            var currentPathParts = SplitPath(currentPath);
            var outputPathParts = SplitPath(outputPath);

            int sameCounter = 0;
            for (int i = 0; i < Math.Min(currentPathParts.Length, outputPathParts.Length); i++)
            {
                if (currentPathParts[i].Equals(outputPathParts[i], StringComparison.CurrentCultureIgnoreCase))
                    sameCounter++;
                else
                    break;
            }

            var output = new List<string>();

            for (int i = sameCounter; i < outputPathParts.Length; i++)
                output.Add("..");

            for (int i = sameCounter; i < currentPathParts.Length; i++)
                output.Add(currentPathParts[i]);

            foreach (string part in url.Split('/'))
            {
                if (part == ".." && output.Count > 0)
                    output.RemoveAt(output.Count - 1);
                else
                    output.Add(part);
            }

            return string.Join("/", output.ToArray());
        }
    }
}