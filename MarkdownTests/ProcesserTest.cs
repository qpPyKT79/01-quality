using System;
using System.Collections.Generic;
using Markdown;
using NUnit.Framework;

namespace MarkdownTests
{
    [TestFixture]
    public class ProcesserTest
    {
        [Test]
        public void WrapIntoTag_WrapIntoTags_Succesful()
        {
            var text = "This is test string".Split(' ');
            var excepted = new [] {"<code>", "This", "is",  "test", "string", "</code>"};
            var actual = Processor.WrapIntoTag(text, "`");
            CollectionAssert.AreEqual(excepted, actual);
        }
        [Test]
        public void WrapIntoTag_WrapIntoUnexistableTags_Pass()
        {
            var text = "This is test string".Split(' ');
            var excepted = new[] { "This", "is", "test", "string" };
            var actual = Processor.WrapIntoTag(text, "This tag does not exist");
            CollectionAssert.AreEqual(excepted, actual);
        }
        [Test]
        public void FindTagStart_FindStartIndexOfTag()
        {
            var text = "This is test `string`".Split(' ');
            var exceptedIndex = 3;
            var exceptedTag = "`";
            string actualTag;
            var actual = Processor.FindTagStart(text,0,out actualTag);
            Assert.AreEqual(exceptedIndex, actual);
            Assert.AreEqual(exceptedTag, actualTag);
        }
        [Test]
        public void FindTagStart_FindStartIndexOfTwoTag()
        {
            var text = "This _is test_ __string__".Split(' ');
            var exceptedIndex = 1;
            var exceptedTag = "_";
            string actualTag;
            var actual = Processor.FindTagStart(text, 0, out actualTag);
            Assert.AreEqual(exceptedIndex, actual);
            Assert.AreEqual(exceptedTag, actualTag);
        }
        [Test]
        public void FindTagStart_UnexistableTag()
        {
            var text = "This is test string".Split(' ');
            var exceptedIndex = -1;
            var exceptedTag = string.Empty;
            string actualTag;
            var actual = Processor.FindTagStart(text, 0, out actualTag);
            Assert.AreEqual(exceptedIndex, actual);
            Assert.AreEqual(exceptedTag, actualTag);
        }
        [Test]
        public void FindTagStart_StartIndexOutOfRange()
        {
            var text = "This is test string".Split(' ');
            var exceptedIndex = -1;
            var exceptedTag = string.Empty;
            string actualTag;
            var actual = Processor.FindTagStart(text, 4, out actualTag);
            Assert.AreEqual(exceptedIndex, actual);
            Assert.AreEqual(exceptedTag, actualTag);
        }
        [Test]
        public void FindTagEnd_FindEndTagIndex()
        {
            var text = "This is test _string_".Split(' ');
            var exceptedIndex = 3;
            var actualTag = "_";
            var actual = Processor.FindTagEnd(text, 0, actualTag);
            Assert.AreEqual(exceptedIndex, actual);
        }
        [Test]
        public void FindTagEnd_UnexistableTag()
        {
            var text = "This is test string".Split(' ');
            var exceptedIndex = -1;
            var actualTag = string.Empty;
            var actual = Processor.FindTagEnd(text, 1, actualTag);
            Assert.AreEqual(exceptedIndex, actual);
        }
        [Test]
        public void FindTagEnd_StartIndexOutOfRange()
        {
            var text = "This is test string".Split(' ');
            var exceptedIndex = -1;
            var actualTag = string.Empty;
            var actual = Processor.FindTagEnd(text, 4, actualTag);
            Assert.AreEqual(exceptedIndex, actual);
        }
        [Test]
        public void GetPrefixTest()
        {
            var text = "string";
            var actual = Processor.GetPrefix(text, 3);
            var excepted = "str";
            Assert.AreEqual(excepted, actual);
        }
        [Test]
        public void GetPrefix_IndexOutOfRange()
        {
            var text = "string";
            try
            {
                Processor.GetPrefix(text, 7);
            }
            catch (ArgumentOutOfRangeException)
            {
                Assert.Pass();
            }
            Assert.Fail();
        }
        [Test]
        public void GetSuffixTest()
        {
            var text = "string";
            var actual = Processor.GetSuffix(text, 3);
            var excepted = "ing";
            Assert.AreEqual(excepted, actual);
        }
        [Test]
        public void GetSuffix_IndexOutOfRange()
        {
            var text = "string";
            try
            {
                Processor.GetPrefix(text, 8);
            }
            catch (ArgumentOutOfRangeException)
            {
                Assert.Pass();
            }
            Assert.Fail();
        }
        [Test]
        public void GetMiddleTest()
        {
            var text = "string";
            var actual = Processor.GetMiddle(text, 2);
            var excepted = "ri";
            Assert.AreEqual(excepted, actual);
        }
        [Test]
        public void GetMiddle_IndexOutOfRange()
        {
            var text = "string";
            try
            {
                Processor.GetPrefix(text, 8);
            }
            catch (ArgumentOutOfRangeException)
            {
                Assert.Pass();
            }
            Assert.Fail();
        }
        [Test]
        public void WrapParagraphsTest()
        {
            var text = new [] {"This is test string", "", "new paragraph", "just new line", "", "new paragraph"};
            var excepted = "<p> This is test string <br> </p><p> new paragraph <br> just new line <br> </p><p> new paragraph </p> <br>";
            var actual = Processor.WrapParagraphs(text);
            Assert.AreEqual(excepted, actual);
        }

        [Test]
        public void WraperSimpleTest()
        {

            var text = "This is _test_ string".Split(' ');
            var excepted = new[] {"This", "is", "<em>", "test", "</em>", "string" };
            var actual = Processor.Wrapper(text);
            CollectionAssert.AreEqual(excepted, actual);
        }
        [Test]
        public void Wraper_DoubleTags()
        {

            var text = "This is ___test___ string".Split(' ');
            var excepted = new[] { "This", "is", "<strong>", "<em>", "test", "</em>", "</strong>", "string" };
            var actual = Processor.Wrapper(text);
            CollectionAssert.AreEqual(excepted, actual);
        }
        [Test]
        public void Wraper_TwoTags()
        {

            var text = "This _is test_ __string__".Split(' ');
            var excepted = new[] { "This", "<em>", "is", "test", "</em>", "<strong>", "string", "</strong>" };
            var actual = Processor.Wrapper(text);
            CollectionAssert.AreEqual(excepted, actual);
        }





    }
}
