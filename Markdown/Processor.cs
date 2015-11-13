using System;
using System.Collections.Generic;
using System.Linq;
namespace Markdown
{
    public static class Processor
    {
        public static readonly Dictionary<string, Tuple<string, string>> AllTags = new Dictionary<string, Tuple<string, string>>()
        {
            {"`", new Tuple<string, string>("<code>", "</code>")},
            {"_", new Tuple<string, string>("<em>", "</em>")},
            {"__", new Tuple<string, string>("<strong>", "</strong>")}
        };

        public static readonly HashSet<string> UnWrappedInsideTags = new HashSet<string>()
        {
            {"`"}
        };

        public static IEnumerable<string> WrapIntoTag(IEnumerable<string> words, string tagType)
        {
            if (!AllTags.ContainsKey(tagType)) return words;
            var wrappedText = new[] {AllTags[tagType].Item1}.Concat(words).Concat(new[] {AllTags[tagType].Item2});
            return UnWrappedInsideTags.Contains(tagType) ? wrappedText : Wrapper(wrappedText);
        }

        public static IEnumerable<string> Wrapper(IEnumerable<string> words)
        {
            string currentTag;
            var wordsSequence = words as IList<string> ?? words.ToList();
            var openTagPosition = FindOpenTag(wordsSequence, 0, out currentTag, AllTags.Keys);
            if (openTagPosition == -1)
                return RemoveEscapes(wordsSequence);

            var splitedStart = GetSuffix(wordsSequence.ElementAt(openTagPosition),
                wordsSequence.ElementAt(openTagPosition).Length - currentTag.Length);

            var closeTagPosition = FindCloseTag(wordsSequence, openTagPosition, currentTag);
            if (closeTagPosition == -1)
                return GetHead(wordsSequence, openTagPosition+1).Concat(Wrapper(GetTail(wordsSequence, openTagPosition+1)));

            var splitedEnd = GetPrefix(wordsSequence.ElementAt(closeTagPosition),
                wordsSequence.ElementAt(closeTagPosition).Length - currentTag.Length);

            string specialTag;
            var openSpecialTag = FindOpenTag(
                    GetHead(wordsSequence, openTagPosition)
                        .Concat(ToArray(splitedStart)
                        .Concat(GetTail(wordsSequence, openTagPosition + 1))), openTagPosition, out specialTag, UnWrappedInsideTags);
            if (openSpecialTag != -1)
            {
                var closeSpecialTag = FindCloseTag(
                    GetHead(wordsSequence, closeTagPosition)
                        .Concat(ToArray(splitedEnd)
                        .Concat(GetTail(wordsSequence, closeTagPosition + 1))), closeTagPosition, specialTag);
                if (openSpecialTag <= closeTagPosition && closeTagPosition < closeSpecialTag)
                {
                    splitedStart = GetSuffix(wordsSequence.ElementAt(openSpecialTag),
                        wordsSequence.ElementAt(openSpecialTag).Length - specialTag.Length);
                    splitedEnd = GetPrefix(wordsSequence.ElementAt(closeSpecialTag),
                        wordsSequence.ElementAt(closeSpecialTag).Length - specialTag.Length);

                    return GetHead(wordsSequence, openSpecialTag)
                        .Concat(WrapIntoTag(
                            GetBody(splitedStart, GetBody(wordsSequence, openSpecialTag + 1, closeSpecialTag - openSpecialTag - 1), splitedEnd), specialTag))
                            .Concat(Wrapper(GetTail(wordsSequence, closeSpecialTag + 1)));
                }   
            }

            if (openTagPosition == closeTagPosition)
                return GetHead(wordsSequence, openTagPosition)
                    .Concat(WrapIntoTag(ToArray(GetMiddle(wordsSequence.ElementAt(openTagPosition), currentTag.Length)), currentTag))
                    .Concat(Wrapper(GetTail(wordsSequence, openTagPosition + 1)));

            return GetHead(wordsSequence, openTagPosition)
                .Concat(WrapIntoTag(
                    GetBody(
                        splitedStart,
                        GetBody(wordsSequence, openTagPosition + 1, closeTagPosition - openTagPosition - 1),
                        splitedEnd)
                        , currentTag)
                       )
                .Concat(Wrapper(GetTail(wordsSequence, closeTagPosition + 1)));
        }

        public static IEnumerable<string> GetHead(IEnumerable<string> words, int count) => words.Take(count);

        public static IEnumerable<string> GetTail(IEnumerable<string> words, int startInd) => words.Skip(startInd);

        public static IEnumerable<string> GetBody(IEnumerable<string> words, int startInd, int count) => words.Skip(startInd).Take(count);

        public static IEnumerable<string> GetBody(string start, IEnumerable<string> words, string finish) => ToArray(start).Concat(words).Concat(ToArray(finish));

        public static IEnumerable<string> ToArray(string word) => new [] {word};

        public static int FindOpenTag(IEnumerable<string> words, int startIndex, out string tag, IEnumerable<string> TagGroup)
        {
            tag = string.Empty;
            var minTagLength = TagGroup.Min(t => t.Length);
            var maxTagLength = TagGroup.Max(t => t.Length);
            var wordsSequence = words as IList<string> ?? words.ToList();
            if (words != null && startIndex < wordsSequence.Count() && startIndex >= 0)
                for (var wordCount = startIndex; wordCount < wordsSequence.Count(); wordCount++)
                    if (!TagGroup.Contains(wordsSequence.ElementAt(wordCount)))
                        for (var prefixLength = maxTagLength; prefixLength >= minTagLength; prefixLength--)
                        {
                            if (prefixLength >= wordsSequence.ElementAt(wordCount).Length) continue;
                            var prefix = GetPrefix(wordsSequence.ElementAt(wordCount), prefixLength);
                            if (!TagGroup.Contains(prefix)) continue;
                            tag = prefix;
                            return wordCount;
                        }
            return -1;
        }

        public static int FindCloseTag(IEnumerable<string> words, int startIndex, string tag)
        {
            var wordsSequence = words as IList<string> ?? words.ToList();
            if (words != null && startIndex < wordsSequence.Count() && startIndex >= 0)
                for (var wordCount = startIndex; wordCount < wordsSequence.Count(); wordCount++)
                    if (wordsSequence.ElementAt(wordCount).Length > tag.Length && wordsSequence.ElementAt(wordCount) != tag)
                    {
                        var suffix = GetSuffix(wordsSequence.ElementAt(wordCount), tag.Length);
                        var presuffix = GetSuffix(wordsSequence.ElementAt(wordCount), tag.Length + 1);
                        if (AllTags.Keys.Contains(suffix) && presuffix[0] != '\\' && !AllTags.Keys.Contains(presuffix) && suffix == tag)
                            return wordCount;
                    }
            return -1;
        }

        public static string GetPrefix(string word, int count) => word.Substring(0, count);

        public static string GetSuffix(string word, int count) => word.Substring(word.Length - count);

        public static string GetMiddle(string word, int count) => word.Substring(count, word.Length - 2 * count);

        public static string Parse(IEnumerable<string> lines) => string.Join(" ", Wrapper(WrapParagraphs(lines.ToArray()).Split(new[] { ' ' })));

        public static string WrapParagraphs(string[] lines)
        {
            lines[0] = "<p> " + lines[0];
            lines[lines.Length - 1] += " </p>";
            return string.Join(" ", lines.Select(x => string.IsNullOrWhiteSpace(x) ? x + "</p><p>" : x + " <br>"));
        }

        public static IEnumerable<string> RemoveEscapes(IEnumerable<string> words)=> words.Select(word => word.Replace("\\", ""));
    }
}