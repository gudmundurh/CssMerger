using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CssMerger.Tests
{
    [TestClass]
    public class FilenameResolverTest
    {
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion

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