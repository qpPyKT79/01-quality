using System;
using System.Collections.Generic;
using System.IO;
using Markdown;
using NUnit.Framework;

namespace MarkdownTests
{
    [TestFixture]
    public class FileWriterTest
    {
        //[Test]
        //public void Writer_WriteAllLinesToFile()
        //{
        //    var sourceName = "..//..//Tests//TestText1.html";
        //    var excepted = new string[2];
        //    excepted[0] = "This is test";
        //    excepted[1] = "some string";
        //    var actual = new string[0];
        //    try
        //    {
        //        IReader reader = new FileReader();
        //        actual = reader.Read(sourceName);
        //    }
        //    catch (FileNotFoundException error)
        //    {
        //        Assert.Fail(error.Message);
        //    }
        //    CollectionAssert.AreEqual(excepted, actual);
        //}
        //[Test]
        //public void Reader_ReadFromUnexistableFile_ErrorThrown()
        //{
        //    var sourceName = "..//..//Tests//ThisFileSHouldNotBeExisted.txt";
        //    var actual = new string[0];
        //    try
        //    {
        //        IReader reader = new FileReader();
        //        actual = reader.Read(sourceName);
        //    }
        //    catch (Exception error)
        //    {
        //        Assert.Pass(error.Message);
        //    }
        //    Assert.Fail("File must not be founded");
        //}
    }
}
