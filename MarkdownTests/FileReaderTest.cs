using System;
using System.Collections.Generic;
using Markdown;
using NUnit.Framework;

namespace MarkdownTests
{
    [TestFixture]
    public class FileReaderTest
    {
        [Test]
        public void Reader_ReadAllLinesFromFile()
        {
            var sourceName = "..//..//Tests//TestText1.txt";
            var excepted = "This is test\nsome string";
            var actual = string.Empty;
            try
            {
                IReader reader = new FileReader();
                actual = reader.Read(sourceName);
            }
            catch (Exception error)
            {
                Assert.Fail(error.Message);
            }
            Assert.AreEqual(excepted,actual);
        }
        [Test]
        public void Reader_ReadFromUnexistableFile_ErrorThrown()
        {
            var sourceName = "..//..//Tests//ThisFileSHouldNotBeExisted.txt";
            var actual = string.Empty;
            try
            {
                IReader reader = new FileReader();
                actual = reader.Read(sourceName);
            }
            catch (Exception error)
            {
                Assert.Pass(error.Message);
            }
            Assert.Fail("File must not be founded");
        }
        [Test]
        public void Reader_DeleteSpaces()
        {
            var sourceName = "..//..//Tests//TestText2.txt";
            var excepted = "This is test\n\nsome string";
            var actual = string.Empty;
            try
            {
                IReader reader = new FileReader();
                actual = reader.Read(sourceName);
            }
            catch (Exception error)
            {
                Assert.Fail(error.Message);
            }
            Assert.AreEqual(excepted, actual);
        }
    }
}
