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
            var excepted = new [] {"This is test", "some string"};
            IEnumerable<string> actual = null;
            try
            {
                IReader reader = new MarkdownReader();
                actual = reader.ReadLines(sourceName);
            }
            catch (Exception error)
            {
                Assert.Fail(error.Message);
            }
            CollectionAssert.AreEqual(excepted,actual);
        }
        [Test]
        public void Reader_ReadFromUnexistableFile_ErrorThrown()
        {
            var sourceName = "..//..//Tests//ThisFileSHouldNotBeExisted.txt";
            IEnumerable<string> actual = null;
            try
            {
                IReader reader = new MarkdownReader();
                actual = reader.ReadLines(sourceName);
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
            var excepted = new[] { "This is test", "          ", "some string" };
            IEnumerable<string> actual = null;
            try
            {
                IReader reader = new MarkdownReader();
                actual = reader.ReadLines(sourceName);
            }
            catch (Exception error)
            {
                Assert.Fail(error.Message);
            }
            CollectionAssert.AreEqual(excepted, actual);
        }
    }
}
