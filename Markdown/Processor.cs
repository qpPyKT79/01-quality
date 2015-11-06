using System;
using System.Collections.Generic;
using System.Linq;

namespace Markdown
{
    public static class Processor
    {
        private static readonly Dictionary<string, Tuple<string, string>> TagName = new Dictionary<string, Tuple<string, string>>()
        {
            {"`", new Tuple<string, string>("<code>", "</code>")},
            {"_", new Tuple<string, string>("<em>", "</em>")},
            {"__", new Tuple<string, string>("<strong>", "</strong>")}
        };

        // todo: не лучшее решение вызывать враппер отсюда для внутрненних текстов
        public static IEnumerable<string> WrapIntoTag(IEnumerable<string> words, string tagType)
        {
            if (TagName.ContainsKey(tagType))
            {
                var wrappedText = new[] {TagName[tagType].Item1}.Concat(words).Concat(new[] {TagName[tagType].Item2});
                return tagType == "`" ? wrappedText : Wrapper(wrappedText);
            }
            return words;
        }

        public static IEnumerable<string> Wrapper(IEnumerable<string> words)
        {
            string currentTag;
            var openTagInd = FindOpenTag(words, 0, out currentTag);
            if (openTagInd == -1)
                return words;

            var closeTagInd = FindCloseTag(words, openTagInd, currentTag);
            if (closeTagInd == -1)
                return words.Take(openTagInd).Concat(Wrapper(words.Skip(openTagInd+1)));

            if (openTagInd == closeTagInd)
                return words.Take(openTagInd).Concat(WrapIntoTag(new[] { GetMiddle(words.ElementAt(openTagInd), currentTag.Length) },currentTag)).Concat(Wrapper(words.Skip(openTagInd+1)));
            
            return words.Take(openTagInd).Concat(WrapIntoTag(
                new[] { GetSuffix(words.ElementAt(openTagInd), words.ElementAt(openTagInd).Length - currentTag.Length) }
                .Concat(words.Skip(openTagInd + 1).Take(closeTagInd - openTagInd - 1))
                .Concat(new[] { GetPrefix(words.ElementAt(closeTagInd), words.ElementAt(closeTagInd).Length - currentTag.Length) })
                , currentTag)).Concat(Wrapper(words.Skip(closeTagInd+1)));
        }
        
        public static int FindOpenTag(IEnumerable<string> words, int startIndex, out string tag)
        {
            tag = string.Empty;
            var minLength = TagName.Keys.Min(t => t.Length);
            var maxLength = TagName.Keys.Max(t => t.Length);
            if (words == null || startIndex >= words.Count() || startIndex < 0) return -1;
            for (var wordCount = startIndex; wordCount < words.Count(); wordCount++)
                for ( var prefixLength = maxLength; prefixLength >=minLength; prefixLength--)
                {
                    if (prefixLength >= words.ElementAt(wordCount).Length) continue;
                    var prefix = GetPrefix(words.ElementAt(wordCount), prefixLength);
                    if (!TagName.Keys.Contains(prefix)) continue;
                    tag = prefix;
                    return wordCount;
                }
            return -1;
        }
        public static int FindCloseTag(IEnumerable<string> words, int startIndex, string tag)
        {
            if (words != null && startIndex < words.Count() && startIndex >= 0)
                for (var wordCount = startIndex; wordCount < words.Count(); wordCount++)
                {
                    var suffix = GetSuffix(words.ElementAt(wordCount), tag.Length);
                    var presuffix = GetSuffix(words.ElementAt(wordCount), tag.Length + 1);
                    if (words.ElementAt(wordCount).Length > tag.Length &&
                        TagName.Keys.Contains(suffix) && presuffix[0] != '\\' && !TagName.Keys.Contains(presuffix))
                        return wordCount;
                }
            return -1;
        }

        // эти три метода чисто чтобы не было ада в аргументах сабстринга,
        // имхо "GetPrefix" в данной задаче гораздо больше смысла имеет чем просто сабстринг с непонятными аргументами
        public static string GetPrefix(string word, int count) => word.Substring(0, count);

        public static string GetSuffix(string word, int count) => word.Substring(word.Length - count);

        public static string GetMiddle(string word, int count) => word.Substring(count, word.Length - 2 * count);


        public static string[] Parse(string[] lines)
        {
            var words = WrapParagraphs(lines.ToArray()).Split(new[] { ' ' });
            Wrapper(words);
            return lines;
        }

        public static string WrapParagraphs(string[] lines)
        {
            lines[0] = "<p> " + lines[0];
            lines[lines.Length - 1] += " </p>";
            return string.Join(" ", lines.Select(x => string.IsNullOrWhiteSpace(x) ? x + "</p><p>" : x + " <br>"));
        }
    }
}
