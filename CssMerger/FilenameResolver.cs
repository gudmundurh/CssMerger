using System;
using System.IO;

namespace CssMerger
{
    internal static class FilenameResolver
    {
        public static string ResolveFilename(string filename, string relativeToPath)
        {
            if (String.IsNullOrEmpty(relativeToPath))
                return Path.GetFullPath(filename);

            if (!Path.IsPathRooted(relativeToPath))
                throw new ArgumentException("'relativeToPath' cannot be relative");

            filename = filename.Replace('/', Path.DirectorySeparatorChar);

            

            Path.GetPathRoot()

            while (filename.StartsWith("../"))
            
            return filename;
        } 
    }
}