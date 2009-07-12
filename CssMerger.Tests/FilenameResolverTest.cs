using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CssMerger.Tests
{
    [TestClass]
    public class FilenameResolverTest
    {
        [TestMethod]
        public void ResolveFilenameTest()
        {
            TestResolveFilename("main.css", "", "main.css");
            TestResolveFilename("../main.css", @"c:\css\abc.css", @"c:\main.css");
            TestResolveFilename("lib/main.css", @"c:\css\abc.css", @"c:\css\lib\main.css");
        }

        [TestMethod]
        public void ResolveUrlTest()
        {
            TestResolveUrl("main.css", "", "main.css");
            TestResolveUrl("../main.css", "file.css", "../main.css");
            TestResolveUrl("a.css", "../x.css", "../a.css");
            TestResolveUrl("images/logo.gif", "header/header.css", "header/images/logo.gif");
        }

        private void TestResolveUrl(string url, string relativeToUrl, string expected)
        {
            Assert.AreEqual(expected, FilenameResolver.ResolveUrl(url, relativeToUrl));
        }

        public void TestResolveFilename(string filename, string relativeToFilename, string expected)
        {
            Assert.AreEqual(expected, FilenameResolver.ResolveFilename(filename, relativeToFilename));
        }
    }
}