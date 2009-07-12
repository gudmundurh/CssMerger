using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CssMerger.Tests
{
    [TestClass]
    public class CssMergerTest
    {
        [TestMethod]
        public void TestSimplePassthrough()
        {
            // Arrange
            var mock = new Mock<IFileManager>();
            string inputCss = "body { background: red; }\n@media print { h1 { font-weight: bold; } }";
            mock.Setup(manager => manager.ReadFile("in.css")).Returns(inputCss);

            string outputCss = null;
            mock.Setup(manager => manager.WriteFile("out.css", It.IsAny<string>()))
                .Callback((string filename, string contents) => outputCss = contents);

            // Act
            CssMerger.MergeCss("in.css", "out.css", mock.Object);

            // Assert
            Assert.AreEqual(inputCss, outputCss);
        }


        [TestMethod]
        public void TestSingleImport()
        {
            // Arrange
            var mock = new Mock<IFileManager>();
            string mainFileCss =
                "/* A comment */\n"
                + "body { background: red; }\n"
                + "@import url(sub.css);\n";
            string subFileCss =
                "div.sub { color: cyan }\n";
            string expectedCss =
                "/* A comment */\n"
                + "body { background: red; }\n"
                + "div.sub { color: cyan }\n\n";

            mock.Setup(manager => manager.ReadFile("main.css")).Returns(mainFileCss);
            mock.Setup(manager => manager.ReadFile("sub.css")).Returns(subFileCss);

            string outputCss = null;
            mock.Setup(manager => manager.WriteFile("out.css", It.IsAny<string>()))
                .Callback((string filename, string contents) => outputCss = contents);

            // Act
            CssMerger.MergeCss("main.css", "out.css", mock.Object);

            // Assert
            Assert.AreEqual(expectedCss, outputCss);
        }


        [TestMethod]
        public void TestImportWithoutUrl()
        {
            // Arrange
            var mock = new Mock<IFileManager>();
            string mainFileCss = "@import 'sub.css';";
            string subFileCss = "div.sub { color: cyan }";
            string expectedCss = subFileCss;

            mock.Setup(manager => manager.ReadFile("main.css")).Returns(mainFileCss);
            mock.Setup(manager => manager.ReadFile("sub.css")).Returns(subFileCss);

            string outputCss = null;
            mock.Setup(manager => manager.WriteFile("out.css", It.IsAny<string>()))
                .Callback((string filename, string contents) => outputCss = contents);

            // Act
            CssMerger.MergeCss("main.css", "out.css", mock.Object);

            // Assert
            Assert.AreEqual(expectedCss, outputCss);
        }


        [TestMethod]
        public void TestImportWithUrlAndSingleQuotes()
        {
            // Arrange
            var mock = new Mock<IFileManager>();
            string mainFileCss = "@import url('sub.css'); h1 { color: red; }";
            string subFileCss = "div.sub { color: cyan }";
            string expectedCss = subFileCss + " h1 { color: red; }";

            mock.Setup(manager => manager.ReadFile("main.css")).Returns(mainFileCss);
            mock.Setup(manager => manager.ReadFile("sub.css")).Returns(subFileCss);

            string outputCss = null;
            mock.Setup(manager => manager.WriteFile("out.css", It.IsAny<string>()))
                .Callback((string filename, string contents) => outputCss = contents);

            // Act
            CssMerger.MergeCss("main.css", "out.css", mock.Object);

            // Assert
            Assert.AreEqual(expectedCss, outputCss);
        }

        [TestMethod]
        public void TestMultipleImports()
        {
            // Arrange
            var mock = new Mock<IFileManager>();
            string mainFileCss = "@import url(\"sub.css\");\n@import url(abc.css);";
            string subFileCss = "div.sub { color: cyan }";
            string abcFileCss = "a img { border 0; }";
            string expectedCss = subFileCss + "\n" + abcFileCss;

            mock.Setup(manager => manager.ReadFile("main.css")).Returns(mainFileCss);
            mock.Setup(manager => manager.ReadFile("sub.css")).Returns(subFileCss);
            mock.Setup(manager => manager.ReadFile("abc.css")).Returns(abcFileCss);

            string outputCss = null;
            mock.Setup(manager => manager.WriteFile("out.css", It.IsAny<string>()))
                .Callback((string filename, string contents) => outputCss = contents);

            // Act
            CssMerger.MergeCss("main.css", "out.css", mock.Object);

            // Assert
            Assert.AreEqual(expectedCss, outputCss);
        }

        [TestMethod]
        public void TestNestedImports()
        {
            // Arrange
            var mock = new Mock<IFileManager>();
            string mainFileCss = "@import url(\"sub.css\");\n@import url(\"abc.css\");";
            string subFileCss = "@import url(\"lib/typography.css\"); div.sub { color: cyan }";
            string typoFileCss = "body { font-family: Arial; }";
            string abcFileCss = "a img { border 0; }";
            string expectedCss = typoFileCss + " div.sub { color: cyan }\n" + abcFileCss;

            mock.Setup(manager => manager.ReadFile("c:\\css\\main.css")).Returns(mainFileCss);
            mock.Setup(manager => manager.ReadFile("c:\\css\\sub.css")).Returns(subFileCss);
            mock.Setup(manager => manager.ReadFile("c:\\css\\abc.css")).Returns(abcFileCss);
            mock.Setup(manager => manager.ReadFile("c:\\css\\lib\\typography.css")).Returns(typoFileCss);

            string outputCss = null;
            mock.Setup(manager => manager.WriteFile("out.css", It.IsAny<string>()))
                .Callback((string filename, string contents) => outputCss = contents);

            // Act
            CssMerger.MergeCss("c:\\css\\main.css", "out.css", mock.Object);

            // Assert
            Assert.AreEqual(expectedCss, outputCss);
        }


        [TestMethod]
        public void TestImportingNonExistingFile()
        {
            // Arrange
            var mock = new Mock<IFileManager>();
            string mainFileCss = "@import url(\"sub.css\"); h1 { color: red; }";

            mock.Setup(manager => manager.ReadFile("main.css")).Returns(mainFileCss);

            mock.Setup(manager => manager.WriteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new FileNotFoundException());

            var cssMerger = new CssMerger(mock.Object);
            cssMerger.RaiseErrors = true;

            // Act
            FileNotFoundException exception = null;
            try
            {
                cssMerger.MergeAndWrite("main.css", "out.css");
            }
            catch (FileNotFoundException e)
            {
                exception = e;
            }

            // Assert
            Assert.IsNotNull(exception, "Exception not raised when trying to read unexisting file");
            //Assert.AreEqual("sub.css", exception.FileName);
        }
    }
}