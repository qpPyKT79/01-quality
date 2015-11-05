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
    public class Processor
    {
        private static HashSet<string> tags = new HashSet<string>() {"`", "__", "_"};

        private static Dictionary<string, Tuple<string, string>> tagName = new Dictionary<string, Tuple<string, string>>()
        {
            {"`", new Tuple<string, string>("<code>", "</code>")},
            {"_", new Tuple<string, string>("<em>", "</em>")},
            {"__", new Tuple<string, string>("<strong>", "</strong>")}
        };

        public static IEnumerable<string> WrapIntoTag(IEnumerable<string> text, string tagType)
        {
            if (tagName.ContainsKey(tagType))
            {
                return
                    new[] {tagName[tagType].Item1}
                        .Concat(text)
                        .Concat(new[] {tagName[tagType].Item2});
            }
            return text;
        }
        
        /// <summary>
        /// этот ад рекурсивно разбивает текст на блоки которые оберачиваются в теги
        /// todo: надо как то упрощать этот ад, читать невозможно уже через полчаса
        /// </summary>
        /// <param name="words">слова текста</param>
        /// <returns>текст обернутый в теги</returns>
        public static IEnumerable<string> Wrapper(IEnumerable<string> words)
        {

            var currentTag = string.Empty;
            var startTagInd = FindTagStart(words, 0, out currentTag);
            if (startTagInd == -1)
                return words; 
            var endTagInd = FindTagEnd(words, startTagInd, currentTag);
            var startTag = SplitStartWordByTag(words.ElementAt(startTagInd), currentTag);
            if (endTagInd == -1)
                return words.Take(startTagInd).Concat(Wrapper(startTag.Skip(startTagInd+1))); 
            if (startTagInd == endTagInd)
            {
                var splited = SplitAllWordByTag(words.ElementAt(startTagInd), currentTag);
                return words.Take(startTagInd).Concat(WrapIntoTag(new[] {splited.ElementAt(1)},currentTag)).Concat(Wrapper(words.Skip(startTagInd+1)));
            }
            var endTag = SplitEndWordByTag(words.ElementAt(endTagInd), currentTag);
            var wrappedText = WrapIntoTag(
                new[] {startTag.ElementAt(1)}
                .Concat(words.Skip(startTagInd+1).Take(endTagInd-startTagInd-1))
                .Concat(new [] {endTag.ElementAt(0)})
                , currentTag);
            return words.Take(startTagInd).Concat(wrappedText).Concat(Wrapper(words.Skip(endTagInd+1)));
        }
        // todo: зачем мне тут сам тег?
        public static IEnumerable<string> SplitStartWordByTag(string word, string tag) =>
            new[] {tag, word.Substring(tag.Length)};

        public static IEnumerable<string> SplitEndWordByTag(string word, string tag) =>
            new[] {word.Substring(0, word.Length - tag.Length), tag};

        public static IEnumerable<string> SplitAllWordByTag(string word, string tag) =>
            new[] {tag, word.Substring(tag.Length, word.Length - 2*tag.Length), tag};

        public static int FindTagStart(IEnumerable<string> text, int startIndex, out string tag)
        {
            tag = string.Empty;
            if (startIndex < text.Count() && startIndex >= 0)
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

        public static int FindTagEnd(IEnumerable<string> text, int startIndex, string tag) //тут проверить на экранирование
        {
            if (startIndex < text.Count() && startIndex >= 0)
                for (var wordCount = startIndex; wordCount < text.Count(); wordCount++)
                {
                    var suffix = GetSuffix(text.ElementAt(wordCount), tag.Length);
                    if (text.ElementAt(wordCount).Length > tag.Length && tags.Contains(suffix))
                        return wordCount;
                }
            return -1;
        }


        public static string GetPrefix(string word, int count) => word.Substring(0, count);

        public static string GetSuffix(string word, int count) => word.Substring(word.Length - count);


        public static string[] Parse(string[] lines)
        {
            var words = ParseParagraphs(lines.ToArray()).Split(new[] { ' ' });
            Wrapper(words);
            return lines;
        }

        public static string ParseParagraphs(string[] lines)
        {
            lines[0] = "<p> " + lines[0];//возможен баг
            lines[lines.Length - 1] += " </p>";
            return string.Join(" ",lines.Select(x => String.IsNullOrWhiteSpace(x) ? x + " </p><p>" : x + " <br>"));
        }
        











    }

}
