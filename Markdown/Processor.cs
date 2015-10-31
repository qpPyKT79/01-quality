using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using Markdown;

namespace Markdown
{
    public class Processor : IProcesser
    {
        private HashSet<string> tags = new HashSet<string>() {"`", "__", "_"};

        private Dictionary<string, Tuple<string, string>> tagName = new Dictionary<string, Tuple<string, string>>()
        {
            {"\n\n", new Tuple<string, string>("<p>", "</p>")},
            {"`", new Tuple<string, string>("<code>", "</code>")},
            {"_", new Tuple<string, string>("<em>", "</em>")},
            {"__", new Tuple<string, string>("<strong>", "</strong>")}
        };

        public IEnumerable<string> WrapIntoTag(IEnumerable<string> text, string tagType)
        {
            if (tagName.ContainsKey(tagType))
            {
                return
                    new[] {tagName[tagType].Item1}
                    .Concat(text.Skip(1).Take((text.Count() - 2)))
                    .Concat(new[] {tagName[tagType].Item2});
            }
            return text;
        }

        public IEnumerable<string> Do(IEnumerable<string> words)
        {
            var currentTag = string.Empty;
            //var words = words.Split(new [] {'\n',' '});
            var startTagInd = FindTagStart(words, 0, out currentTag);
            if (startTagInd == -1)
                return words; //todo into words
            var endTagInd = FindTagEnd(words, startTagInd, currentTag);
            IEnumerable<string> startTag;
            if (endTagInd == -1)
            {
                startTag = SplitWordByTag(words.ElementAt(startTagInd), currentTag);
                return words.Take(startTagInd-1).Concat(Do(startTag.Skip(startTagInd+1)));
            }
            startTag = SplitWordByTag(words.ElementAt(startTagInd), currentTag);
            var endTag = SplitWordByTag(words.ElementAt(endTagInd), currentTag);
            var wrappedText = WrapIntoTag(startTag
                .Concat(Do(words.Skip(startTagInd).Take(endTagInd - startTagInd - 1)))
                .Concat(endTag), currentTag);
            return words.Take(startTagInd - 1).Concat(wrappedText).Concat(Do(words.Skip(endTagInd)));
        }

        public IEnumerable<string> SplitWordByTag(string word, string tag)=>  
            new[] { word.Substring(0, tag.Length), word.Substring(tag.Length) };

        public int FindTagStart(IEnumerable<string> text, int startIndex, out string tag)
        {
            tag = string.Empty;
            if (startIndex<text.Count() && startIndex >= 0)
                for (var count = tags.Max(x => x.Length); count > 0; count--)
                    for (var wordCount = startIndex; wordCount < text.Count(); wordCount++)
                    {
                        var prefix = GetPrefix(text.ElementAt(wordCount), count);
                        if (text.ElementAt(wordCount).Length > count && tags.Contains(prefix))
                        {
                            tag = prefix;
                            return wordCount;
                        }
                    }
            return -1;
        }

        public int FindTagEnd(IEnumerable<string> text, int startIndex, string tag)
        {
            if (startIndex<text.Count() && startIndex>=0)
                for (var wordCount = startIndex; wordCount < text.Count(); wordCount++)
                {
                    var suffix = GetSuffix(text.ElementAt(wordCount), tag.Length);
                    if (text.ElementAt(wordCount).Length > tag.Length && tags.Contains(suffix))
                        return wordCount;
                }
            return -1;
        }


        public string GetPrefix(string word, int count) => word.Substring(0, count);

        public string GetSuffix(string word, int count) => word.Substring(word.Length - count);
    }
}
