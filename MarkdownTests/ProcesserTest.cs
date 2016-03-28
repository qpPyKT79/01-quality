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
        public void WrapIntoTags_CodeTag()
        {
            var text = "This _is test_ __string__".Split(' ');
            var excepted = new[] { "<code>", "This", "_is", "test_", "__string__", "</code>" };
            var actual = Processor.WrapIntoTag(text, "`");
            CollectionAssert.AreEqual(excepted, actual);
        }
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
            var actual = Processor.FindOpenTag(text,0,out actualTag, Processor.AllTags.Keys);
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
            var actual = Processor.FindOpenTag(text, 0, out actualTag, Processor.AllTags.Keys);
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
            var actual = Processor.FindOpenTag(text, 0, out actualTag, Processor.AllTags.Keys);
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
            var actual = Processor.FindOpenTag(text, 4, out actualTag, Processor.AllTags.Keys);
            Assert.AreEqual(exceptedIndex, actual);
            Assert.AreEqual(exceptedTag, actualTag);
        }
        [Test]
        public void FindTagEnd_FindEndTagIndex()
        {
            var text = "This is test _string_".Split(' ');
            var exceptedIndex = 3;
            var actualTag = "_";
            var actual = Processor.FindCloseTag(text, 0, actualTag);
            Assert.AreEqual(exceptedIndex, actual);
        }
        [Test]
        public void FindTagEnd_Collisions()
        {
            var text = "_This __is test__ string_".Split(' ');
            var exceptedIndex = 3;
            var actualTag = "_";
            var actual = Processor.FindCloseTag(text, 0, actualTag);
            Assert.AreEqual(exceptedIndex, actual);
        }
        [Test]
        public void FindTagEnd_UnexistableTag()
        {
            var text = "This is test string".Split(' ');
            var exceptedIndex = -1;
            var actualTag = string.Empty;
            var actual = Processor.FindCloseTag(text, 1, actualTag);
            Assert.AreEqual(exceptedIndex, actual);
        }
        [Test]
        public void FindTagEnd_StartIndexOutOfRange()
        {
            var text = "This is test string".Split(' ');
            var exceptedIndex = -1;
            var actualTag = string.Empty;
            var actual = Processor.FindCloseTag(text, 4, actualTag);
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
        public void Wraper_subTags()
        {

            var text = "This _is __test__ string_".Split(' ');
            var excepted = new[] { "This", "<em>", "is", "<strong>", "test", "</strong>", "string", "</em>" };
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
        
        /*
        + ___a a___ 
        + _a __a a__ a_
        + _a a_ __a a__
        + _a a _a a_    => <em> a a _a a <\em>
        + _a __a a_ a__ => <em> a __a a <\em> a__
        + _a `b _a a_ c_ __d d__` => _a <code> b _a a_ c_ __d d__ <\code>
        + \_a a_ => _a a_
        + _a a\_ => _a a_
        */
        [Test]
        public void Wrapper_DoubleOpenTag()
        {
            var text = "_This _is test_ string".Split(' ');
            var excepted = new[] { "<em>", "This", "_is", "test", "</em>", "string"};
            var actual = Processor.Wrapper(text);
            CollectionAssert.AreEqual(excepted, actual);
        }
        [Test]
        public void Wrapper_CodeTag_NoWrapping()
        {
            var text = "_This `is _test_ __string__`".Split(' ');
            var excepted = new[] { "_This","<code>", "is", "_test_", "__string__", "</code>" };
            var actual = Processor.Wrapper(text);
            CollectionAssert.AreEqual(excepted, actual);
        }
        [Test]
        public void Wrapper_EscaeStartTag_NoWrapping()
        {
            var text = "\\_This is test_ string`".Split(' ');
            var excepted = new[] { "_This", "is", "test_", "string`" };
            var actual = Processor.Wrapper(text);
            CollectionAssert.AreEqual(excepted, actual);
        }
        [Test]
        public void Wrapper_EscapeCloseTagTag_NoWrapping()
        {
            var text = "_This is test\\_ string".Split(' ');
            var excepted = new[] { "_This","is", "test_", "string"};
            var actual = Processor.Wrapper(text);
            CollectionAssert.AreEqual(excepted, actual);
        }

        [Test]
        public void Markdown_HardTest()
        {
            IReader reader = new MdReader();

            var text = reader.ReadLines("..//..//Tests//TestText3.txt");
            var header = "<p> ";
            var body = "This is test <em> string <br> </p><p> this must be wrapped anyway </em> <br> This <strong> is test </strong> <em> this is test too </em> <br> next symbol __ must not be wrapped <br> and how about <code> that? <br> </p><p> (define (factorial n) <br>    (if (= n 0) <br>        1 <br>        (* (factorial (- n 1)) n))) </code> <br> </p><p> it was factorial <strong> (scheme) </strong> and nothing must not be wrapped there <br> </p><p> this is factorial on c# <br> <code> public int fact(int num) { <br>      return (num == 0) ? 1 : num * fact(num - 1); <br> } </code> <br> lets try to wrap into tag some words here <br> <code> public int _fact_ (int __num__ ) { <br>      return ( __num__ == 0) ? 1 : __num__ * _fact_ ( __num__ - 1); <br> } </code> <br> nothing has happened! <br> </p><p> <code> _a \\`a_ a\\` a_ </code> =&gt; <em> a `a_` a </em> <br> </p><p> lets repeat this 150 times <br> (~ 5000 lines) <br> </p><p> ========================= <br> ";
            var excepted = header;
            for (var i = 0; i < 150; i++)
                excepted += body;
            excepted = excepted.Substring(0, excepted.Length - 5) + "</p> <br>";
            var actual = Processor.Parse(text);
            Assert.AreEqual(excepted, actual);
        }




    }
}
