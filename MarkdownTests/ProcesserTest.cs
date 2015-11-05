﻿using System;
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
            IProcesser processer = new Processor();
            var actual = processer.WrapIntoTag(text, "`");
            CollectionAssert.AreEqual(excepted, actual);
        }
        [Test]
        public void WrapIntoTag_WrapIntoUnexistableTags_Pass()
        {
            var text = "This is test string".Split(' ');
            var excepted = new[] { "This", "is", "test", "string" };
            IProcesser processer = new Processor();
            var actual = processer.WrapIntoTag(text, "This tag does not exist");
            CollectionAssert.AreEqual(excepted, actual);
        }
        [Test]
        public void FindTagStart_FindStartIndexOfTag()
        {
            var text = "This is test `string`";
            var exceptedIndex = 3;
            var exceptedTag = "`";
            IProcesser processer = new Processor();
            var actualTag = string.Empty;
            var actual = processer.FindTagStart(text.Split(' '),0,out actualTag);
            Assert.AreEqual(exceptedIndex, actual);
            Assert.AreEqual(exceptedTag, actualTag);
        }
        [Test]
        public void FindTagStart_FIndTagIndexWhenItsNotExist()
        {
            var text = "This is test string";
            var exceptedIndex = -1;
            var exceptedTag = string.Empty;
            IProcesser processer = new Processor();
            var actualTag = string.Empty;
            var actual = processer.FindTagStart(text.Split(' '), 0, out actualTag);
            Assert.AreEqual(exceptedIndex, actual);
            Assert.AreEqual(exceptedTag, actualTag);
        }
        [Test]
        public void FindTagStart_StartIndexOutOfRange()
        {
            var text = "This is test string";
            var exceptedIndex = -1;
            var exceptedTag = string.Empty;
            IProcesser processer = new Processor();
            var actualTag = string.Empty;
            var actual = processer.FindTagStart(text.Split(' '), 4, out actualTag);
            Assert.AreEqual(exceptedIndex, actual);
            Assert.AreEqual(exceptedTag, actualTag);
        }
        [Test]
        public void FindTagEnd_FindEndTagIndex()
        {
            var text = "This is test `string`";
            var exceptedIndex = 3;
            IProcesser processer = new Processor();
            var actualTag = "`";
            var actual = processer.FindTagEnd(text.Split(' '), 0, actualTag);
            Assert.AreEqual(exceptedIndex, actual);
        }
        [Test]
        public void FindTagEnd_FIndTagIndexWhenTagIsNotExist()
        {
            var text = "This is test string";
            var exceptedIndex = -1;
            IProcesser processer = new Processor();
            var actualTag = string.Empty;
            var actual = processer.FindTagEnd(text.Split(' '), 0, actualTag);
            Assert.AreEqual(exceptedIndex, actual);
        }
        [Test]
        public void FindTagEnd_StartIndexOutOfRange()
        {
            var text = "This is test string";
            var exceptedIndex = -1;
            IProcesser processer = new Processor();
            var actualTag = string.Empty;
            var actual = processer.FindTagEnd(text.Split(' '), 4, actualTag);
            Assert.AreEqual(exceptedIndex, actual);
        }
        [Test]
        public void Do_some()
        {
            var text = "___aaa___".Split(' ');
            var excepted = new[] { "<strong>","<em>", "aaa", "</em>", "</strong>" };
            IProcesser processer = new Processor();
            var actual = processer.Do(text);
            CollectionAssert.AreEqual(excepted, actual);
        }
        [Test]
        public void Do_some1()
        {
            var text = "_aaa_".Split(' ');
            var excepted = new[] { "<em>", "aaa", "</em>" };
            IProcesser processer = new Processor();
            var actualTag = string.Empty;
            var actual = processer.Do(text);
            CollectionAssert.AreEqual(excepted, actual);
        }
        [Test]
        public void Do_some2()
        {
            var text = "_a __aa__ a_".Split(' ');
            var excepted = new[] { "<em>", "a", "<strong>", "aa", "</strong>", "a", "</em>" };
            IProcesser processer = new Processor();
            var actualTag = string.Empty;
            var actual = processer.Do(text);
            CollectionAssert.AreEqual(excepted, actual);
        }
        [Test]
        public void Do_some3()
        {
            var text = "_a __aa___".Split(' ');
            var excepted = new[] { "<em>", "a", "<strong>", "aa", "</strong>", "</em>" };
            IProcesser processer = new Processor();
            var actualTag = string.Empty;
            var actual = processer.Do(text);
            CollectionAssert.AreEqual(excepted, actual);
        }
        [Test]
        public void Do_some4()
        {
            var text = "___a aa__ a_".Split(' ');
            var excepted = new[] { "<strong>", "<em>", "a", "aa", "</strong>", "a", "</em>" };
            IProcesser processer = new Processor();
            var actualTag = string.Empty;
            var actual = processer.Do(text);
            CollectionAssert.AreEqual(excepted, actual);
        }




    }
}