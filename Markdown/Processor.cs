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

        public string WrapIntoTag(string text, string tagType)
        {
            if (tagName.ContainsKey(tagType))
                return tagName[tagType].Item1 + text + tagName[tagType].Item2;
            return text;
        }

        public string Do(string text)
        {

            var lines = text.Split('\n');




            return null;

        }

        public int FindTagStart(string[] text, int startIndex, out string tag)
        {
            tag = string.Empty;
            if (startIndex<text.Length && startIndex >= 0)
                for (var count = tags.Max(x => x.Length); count > 0; count--)
                    for (var wordCount = startIndex; wordCount < text.Length; wordCount++)
                    {
                        var prefix = GetPrefix(text[wordCount], count);
                        if (text[wordCount].Length > count && tags.Contains(prefix))
                        {
                            tag = prefix;
                            return wordCount;
                        }
                    }
            return -1;
        }

        public int FindTagEnd(string[] text, int startIndex, string tag)
        {
            if (startIndex<text.Length && startIndex>=0)
                for (var wordCount = startIndex; wordCount < text.Length; wordCount++)
                {
                    var suffix = GetSuffix(text[wordCount], tag.Length);
                    if (text[wordCount].Length > tag.Length && tags.Contains(suffix))
                        return wordCount;
                }
            return -1;
        }


        public string GetPrefix(string word, int count) => word.Substring(0, count);

        public string GetSuffix(string word, int count) => word.Substring(word.Length - count);
    }
}
