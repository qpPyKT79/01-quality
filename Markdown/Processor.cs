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
            if (AllTags.ContainsKey(tagType))
            {
                var wrappedText = new[] {AllTags[tagType].Item1}.Concat(words).Concat(new[] {AllTags[tagType].Item2});
                return UnWrappedInsideTags.Contains(tagType) ? wrappedText : Wrapper(wrappedText);
            }
            return words;
        }
        public static IEnumerable<string> Wrapper(IEnumerable<string> words)
        {
            string currentTag;
            var openTagPosition = FindOpenTag(words, 0, out currentTag, AllTags.Keys);
            if (openTagPosition == -1)
                return RemoveEscapes(words);
            var splitedStart = GetSuffix(words.ElementAt(openTagPosition),
                words.ElementAt(openTagPosition).Length - currentTag.Length);

            var closeTagPosition = FindCloseTag(words, openTagPosition, currentTag);
            if (closeTagPosition == -1)
                return GetHead(words, openTagPosition+1).Concat(Wrapper(GetTail(words, openTagPosition+1)));

            
            var splitedEnd = GetPrefix(words.ElementAt(closeTagPosition),
                words.ElementAt(closeTagPosition).Length - currentTag.Length);
            
            string specialTag;
            var openSpecialTag = FindOpenTag(
                    GetHead(words, openTagPosition)
                        .Concat(ToArray(splitedStart)
                        .Concat(GetTail(words, openTagPosition + 1))), openTagPosition, out specialTag, UnWrappedInsideTags);
            if (openSpecialTag != -1)
            {
                var closeSpecialTag = FindCloseTag(
                    GetHead(words, closeTagPosition)
                        .Concat(ToArray(splitedEnd)
                        .Concat(GetTail(words, closeTagPosition + 1))), closeTagPosition, specialTag);
                if (openSpecialTag <= closeTagPosition && closeTagPosition < closeSpecialTag)
                {
                    splitedStart = GetSuffix(words.ElementAt(openSpecialTag),
                        words.ElementAt(openSpecialTag).Length - specialTag.Length);
                    splitedEnd = GetPrefix(words.ElementAt(closeSpecialTag),
                        words.ElementAt(closeSpecialTag).Length - specialTag.Length);
                    return GetHead(words, openSpecialTag)
                        .Concat(WrapIntoTag(
                            GetBody(splitedStart, GetBody(words, openSpecialTag + 1, closeSpecialTag - openSpecialTag - 1), splitedEnd), specialTag))
                            .Concat(Wrapper(GetTail(words, closeSpecialTag + 1)));
                }   
            }

            if (openTagPosition == closeTagPosition)
                return GetHead(words, openTagPosition)
                    .Concat(WrapIntoTag(ToArray(GetMiddle(words.ElementAt(openTagPosition), currentTag.Length)), currentTag))
                    .Concat(GetTail(words, openTagPosition + 1));

            return GetHead(words, openTagPosition)
                .Concat(WrapIntoTag(
                    GetBody(
                        splitedStart,
                        GetBody(words, openTagPosition + 1, closeTagPosition - openTagPosition - 1),
                        splitedEnd)
                        , currentTag)
                       )
                .Concat(Wrapper(GetTail(words, closeTagPosition + 1)));
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
            if (words != null && startIndex < words.Count() && startIndex >= 0)
                for (var wordCount = startIndex; wordCount < words.Count(); wordCount++)
                    for (var prefixLength = maxTagLength; prefixLength >= minTagLength; prefixLength--)
                    {
                        if (prefixLength >= words.ElementAt(wordCount).Length) continue;
                        var prefix = GetPrefix(words.ElementAt(wordCount), prefixLength);
                        if (!TagGroup.Contains(prefix)) continue;
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
                        AllTags.Keys.Contains(suffix) && presuffix[0] != '\\' && !AllTags.Keys.Contains(presuffix))
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
//string currentTag;
//var openTagInd = FindOpenTag(words, 0, out currentTag, AllTags.Keys);
//if (openTagInd == -1)
//    return words;
//var wordSplitedFromStartTag = GetSuffix(words.ElementAt(openTagInd),
//    words.ElementAt(openTagInd).Length - currentTag.Length);
//
//var closeTagInd = FindCloseTag(words, openTagInd, currentTag);
//if (closeTagInd == -1)
//    return GetHead(words, closeTagInd)
//        .Concat(Wrapper(GetTail(words, closeTagInd + 1)));
//var wordSplitedFromCloseTag = GetPrefix(words.ElementAt(closeTagInd),
//    words.ElementAt(closeTagInd).Length - currentTag.Length);
//
//string specialTag;
//var startSpecialTagInd = FindOpenTag(
//    GetHead(words, openTagInd)
//        .Concat(ToArray(wordSplitedFromStartTag))
//        .Concat(GetTail(words, openTagInd + 1))
//    , 0, out specialTag, UnWrappedInsideTags);
//var closeSpecialTagInd = FindCloseTag(
//    GetHead(words, closeTagInd)
//        .Concat(ToArray(wordSplitedFromCloseTag))
//        .Concat(GetTail(words, closeTagInd + 1))
//    , startSpecialTagInd, specialTag);
//if (openTagInd >= startSpecialTagInd && startSpecialTagInd > closeTagInd &&
//    closeTagInd >= closeSpecialTagInd)
//    return GetHead(words, startSpecialTagInd)
//        .Concat(WrapIntoTag(
//            GetBody(
//                GetSuffix(words.ElementAt(startSpecialTagInd), words.ElementAt(startSpecialTagInd).Length - specialTag.Length),
//                GetBody(words, startSpecialTagInd + 1, closeSpecialTagInd - startSpecialTagInd - 1),
//                GetPrefix(words.ElementAt(closeSpecialTagInd), words.ElementAt(closeSpecialTagInd).Length - specialTag.Length))
//            , specialTag)
//        )
//        .Concat(GetTail(words, closeSpecialTagInd + 1));
///*GetBody(
//    ToArray(GetSuffix(words.ElementAt(startSpecialTagInd), words.ElementAt(startSpecialTagInd).Length - specialTag.Length)),
//    GetBody(words, startSpecialTagInd + 1, closeSpecialTagInd - openTagInd - 1),
//    ToArray(GetPrefix(words.ElementAt(closeSpecialTagInd), words.ElementAt(closeSpecialTagInd).Length - specialTag.Length)))*/
//
////return words.Take(openTagInd).Concat(Wrapper(words.Skip(openTagInd + 1)));
//
//if (openTagInd == closeTagInd)
//    return GetHead(words,openTagInd)
//        .Concat(WrapIntoTag(ToArray(GetMiddle(words.ElementAt(openTagInd), currentTag.Length)), currentTag))
//        .Concat(GetTail(words, openTagInd + 1));
////return words.Take(openTagInd).Concat(WrapIntoTag(new[] { GetMiddle(words.ElementAt(openTagInd), currentTag.Length) }, currentTag)).Concat(Wrapper(words.Skip(openTagInd + 1)));
//// todo: есть гипотеза: собственны конкатенаор будет работать быстрее, если нет то concat и так достаточно быстр
//return GetHead(words, openTagInd)
//    .Concat(WrapIntoTag(
//        GetBody(
//            wordSplitedFromStartTag,
//            GetBody(words, openTagInd + 1, closeTagInd - openTagInd - 1),
//            wordSplitedFromCloseTag)
//            , currentTag)
//           )
//    .Concat(GetTail(words, closeTagInd + 1));
///*return words.Take(openTagInd)
//    .Concat(WrapIntoTag(new[] { GetSuffix(words.ElementAt(openTagInd), words.ElementAt(openTagInd).Length - currentTag.Length) }
//        .Concat(words.Skip(openTagInd + 1).Take(closeTagInd - openTagInd - 1))
//    .Concat(new[] { GetPrefix(words.ElementAt(closeTagInd), words.ElementAt(closeTagInd).Length - currentTag.Length) })
//    , currentTag))
//    .Concat(Wrapper(words.Skip(closeTagInd + 1)));*/