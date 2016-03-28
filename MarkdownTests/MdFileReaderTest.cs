using System;
using System.Collections.Generic;
using Markdown;
using NUnit.Framework;

namespace MarkdownTests
{
    [TestFixture]
    public class MdFileReaderTest
    {
        IReader sut;
        [TestFixtureSetUp]
        public void TestSetup()
        {
            sut = new MdReader();
        }

        [Test]
        public void Reader_ReadAllLinesFromFile_Readed()
        {
            var sourceName = "..//..//Tests//TestText1.txt";
            var excepted = new [] {"This is test", "some string"};
            var actual = sut.ReadLines(sourceName);
            CollectionAssert.AreEqual(excepted,actual);
        }

        [Test]
        public void Reader_ReadFromUnexistableFile_ErrorThrown()
        {
            var sourceName = "..//..//Tests//ThisFileSHouldNotBeExisted.txt";
            try
            {
                sut.ReadLines(sourceName);
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
            var actual = sut.ReadLines(sourceName);
            CollectionAssert.AreEqual(excepted, actual);
        }
    }
}
