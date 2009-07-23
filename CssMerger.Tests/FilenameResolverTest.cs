using NUnit.Framework;

namespace CssMerger.Tests
{
    [TestFixture]
    public class FilenameResolverTest
    {
        private static void TestResolveUrl(string url, string currentPath, string outputPath, string expectedUrl)
        {
            Assert.AreEqual(expectedUrl, FilenameResolver.GetUrlRelativeToOutputPath(url, currentPath, outputPath));
        }

        public static void TestResolveFilename(string filename, string currentPath, string expected)
        {
            Assert.AreEqual(expected, FilenameResolver.GetFilenameFromUrl(filename, currentPath));
        }

        [Test]
        [Category("Exploratory")]
        public void GetFilenameFromUrl()
        {
            TestResolveFilename("abc.css", @"c:\", @"c:\abc.css");
            TestResolveFilename("../main.css", @"c:\css", @"c:\main.css");
            TestResolveFilename("lib/main.css", @"c:\css", @"c:\css\lib\main.css");

            // TODO: Add test for absolute URL
        }

        [Test]
        [Category("Exploratory")]
        public void GetUrlRelativeToOutputPath()
        {
            const string root = @"c:\css";

            TestResolveUrl("images/k.gif", @"c:\css", root, "images/k.gif");
            TestResolveUrl("images/k.gif", @"c:\css\nested", root, "nested/images/k.gif");
            TestResolveUrl("../a.png", @"c:\css\nested", root, "a.png");
            TestResolveUrl("../images/a.png", @"c:\css\nested", root, "images/a.png");

            TestResolveUrl("main.css", @"C:\CSS", root, "main.css");

            TestResolveUrl("main.css", @"c:\css", root, "main.css");
            TestResolveUrl("../main.css", @"c:\css", root, "../main.css");
            TestResolveUrl("a.css", @"c:\othercss", root, "../othercss/a.css");
            TestResolveUrl("images/logo.gif", @"c:\css\header", root, "header/images/logo.gif");

            TestResolveUrl("/logo.gif", @"c:\css\header", root, "/logo.gif");
        }
    }
}