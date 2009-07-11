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
            TestResolveFilename("../main.css", "c:\\css", "c:\\main.css");
        }

        public void TestResolveFilename(string filename, string path, string expected)
        {
            Assert.AreEqual(expected, FilenameResolver.ResolveFilename(filename, path));
        }
    }
}