using System.IO;
using Moq;
using NUnit.Framework;

namespace CssMerger.Tests
{
    [TestFixture]
    public class CssMergerTest
    {
        [Test]
        public void ImportingNonExistingFile()
        {
            // Arrange
            var mock = new Mock<IFileManager>();
            string mainFileCss = "@import url(\"sub.css\"); h1 { color: red; }";

            mock.Setup(manager => manager.ReadFile(@"c:\main.css")).Returns(mainFileCss);

            mock.Setup(manager => manager.WriteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new FileNotFoundException());

            var cssMerger = new CssMerger(mock.Object);
            cssMerger.RaiseErrors = true;

            // Act
            FileNotFoundException exception = null;
            try
            {
                cssMerger.MergeAndWrite(@"c:\main.css", @"c:\out.css");
            }
            catch (FileNotFoundException e)
            {
                exception = e;
            }

            // Assert
            Assert.IsNotNull(exception, "Exception not raised when trying to read unexisting file");
            //Assert.AreEqual("sub.css", exception.FileName);
        }

        [Test]
        public void ImportWithoutUrl()
        {
            // Arrange
            var mock = new Mock<IFileManager>();
            string mainFileCss = "@import 'sub.css';";
            string subFileCss = "div.sub { color: cyan }";
            string expectedCss = subFileCss;

            mock.Setup(manager => manager.ReadFile(@"c:\main.css")).Returns(mainFileCss);
            mock.Setup(manager => manager.ReadFile(@"c:\sub.css")).Returns(subFileCss);

            string outputCss = null;
            mock.Setup(manager => manager.WriteFile(@"c:\out.css", It.IsAny<string>()))
                .Callback((string filename, string contents) => outputCss = contents);

            // Act
            CssMerger.MergeCss(@"c:\main.css", @"c:\out.css", mock.Object);

            // Assert
            Assert.AreEqual(expectedCss, outputCss);
        }


        [Test]
        public void ImportWithUrlAndSingleQuotes()
        {
            // Arrange
            var mock = new Mock<IFileManager>();
            string mainFileCss = "@import url('sub.css'); h1 { color: red; }";
            string subFileCss = "div.sub { color: cyan }";
            string expectedCss = subFileCss + " h1 { color: red; }";

            mock.Setup(manager => manager.ReadFile(@"c:\main.css")).Returns(mainFileCss);
            mock.Setup(manager => manager.ReadFile(@"c:\sub.css")).Returns(subFileCss);

            string outputCss = null;
            mock.Setup(manager => manager.WriteFile(@"c:\out.css", It.IsAny<string>()))
                .Callback((string filename, string contents) => outputCss = contents);

            // Act
            CssMerger.MergeCss(@"c:\main.css", @"c:\out.css", mock.Object);

            // Assert
            Assert.AreEqual(expectedCss, outputCss);
        }

        [Test]
        public void MultipleImports()
        {
            // Arrange
            var mock = new Mock<IFileManager>();
            string mainFileCss = "@import url(\"sub.css\");\n@import url(abc.css);";
            string subFileCss = "div.sub { color: cyan }";
            string abcFileCss = "a img { border 0; }";
            string expectedCss = subFileCss + "\n" + abcFileCss;

            mock.Setup(manager => manager.ReadFile(@"c:\main.css")).Returns(mainFileCss);
            mock.Setup(manager => manager.ReadFile(@"c:\sub.css")).Returns(subFileCss);
            mock.Setup(manager => manager.ReadFile(@"c:\abc.css")).Returns(abcFileCss);

            string outputCss = null;
            mock.Setup(manager => manager.WriteFile(@"c:\out.css", It.IsAny<string>()))
                .Callback((string filename, string contents) => outputCss = contents);

            // Act
            CssMerger.MergeCss(@"c:\main.css", @"c:\out.css", mock.Object);

            // Assert
            Assert.AreEqual(expectedCss, outputCss);
        }

        [Test]
        public void NestedImports()
        {
            // Arrange
            var mock = new Mock<IFileManager>();
            string mainFileCss = "@import url(\"sub.css\");\n@import url(\"abc.css\");";
            string subFileCss = "@import url(\"lib/typography.css\"); div.sub { color: cyan }";
            string typoFileCss = "body { font-family: Arial; }";
            string abcFileCss = "a img { border 0; }";
            string expectedCss = typoFileCss + " div.sub { color: cyan }\n" + abcFileCss;

            mock.Setup(manager => manager.ReadFile(@"c:\css\main.css")).Returns(mainFileCss);
            mock.Setup(manager => manager.ReadFile(@"c:\css\sub.css")).Returns(subFileCss);
            mock.Setup(manager => manager.ReadFile(@"c:\css\abc.css")).Returns(abcFileCss);
            mock.Setup(manager => manager.ReadFile(@"c:\css\lib\typography.css")).Returns(typoFileCss);

            string outputCss = null;
            mock.Setup(manager => manager.WriteFile(@"c:\merged\out.css", It.IsAny<string>()))
                .Callback((string filename, string contents) => outputCss = contents);

            // Act
            CssMerger.MergeCss(@"c:\css\main.css", @"c:\merged\out.css", mock.Object);

            // Assert
            Assert.AreEqual(expectedCss, outputCss);
        }

        [Test]
        public void ResolvingPathsToImages()
        {
            // Arrange
            var mock = new Mock<IFileManager>();
            const string inputCss = "body { background: url(images/background.gif) }";
            const string expectedCss = "body { background: url(css/images/background.gif) }";

            mock.Setup(manager => manager.ReadFile(@"c:\css\main.css")).Returns(inputCss);

            string actualCss = null;
            mock.Setup(manager => manager.WriteFile(@"c:\out.css", It.IsAny<string>()))
                .Callback((string filename, string contents) => actualCss = contents);

            // Act
            CssMerger.MergeCss(@"c:\css\main.css", @"c:\out.css", mock.Object);

            // Assert
            Assert.AreEqual(expectedCss, actualCss);
        }

        [Test]
        public void SimplePassthrough()
        {
            // Arrange
            var mock = new Mock<IFileManager>();
            string inputCss = "body { background: red; }\n@media print { h1 { font-weight: bold; } }";
            mock.Setup(manager => manager.ReadFile(@"c:\in.css")).Returns(inputCss);

            string outputCss = null;
            mock.Setup(manager => manager.WriteFile(@"c:\out.css", It.IsAny<string>()))
                .Callback((string filename, string contents) => outputCss = contents);

            // Act
            CssMerger.MergeCss(@"c:\in.css", @"c:\out.css", mock.Object);

            // Assert
            Assert.AreEqual(inputCss, outputCss);
        }


        [Test]
        public void SingleImport()
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

            mock.Setup(manager => manager.ReadFile(@"c:\main.css")).Returns(mainFileCss);
            mock.Setup(manager => manager.ReadFile(@"c:\sub.css")).Returns(subFileCss);

            string outputCss = null;
            mock.Setup(manager => manager.WriteFile(@"c:\out.css", It.IsAny<string>()))
                .Callback((string filename, string contents) => outputCss = contents);

            // Act
            CssMerger.MergeCss(@"c:\main.css", @"c:\out.css", mock.Object);

            // Assert
            Assert.AreEqual(expectedCss, outputCss);
        }

        [Test]
        public void ThatImportAndAssetUrlsAreNotMixed()
        {
            // Arrange
            var mock = new Mock<IFileManager>();
            const string mainCss = "@import url(sub.css); body { background: url(images/background.gif) }";
            const string subCss = "h1 { font-weight: bold; }";
            const string expectedCss = "h1 { font-weight: bold; } body { background: url(css/images/background.gif) }";

            mock.Setup(manager => manager.ReadFile(@"c:\css\main.css")).Returns(mainCss);
            mock.Setup(manager => manager.ReadFile(@"c:\css\sub.css")).Returns(subCss);

            string actualCss = null;
            mock.Setup(manager => manager.WriteFile(@"c:\out.css", It.IsAny<string>()))
                .Callback((string filename, string contents) => actualCss = contents);

            // Act
            CssMerger.MergeCss(@"c:\css\main.css", @"c:\out.css", mock.Object);

            // Assert
            Assert.AreEqual(expectedCss, actualCss);
        }
    }
}