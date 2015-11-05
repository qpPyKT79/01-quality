﻿using System;
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

        public static IEnumerable<string> WrapIntoTag(IEnumerable<string> text, string tagType)
        {
            if (TagName.ContainsKey(tagType))
            {
                return
                    new[] {TagName[tagType].Item1}
                        .Concat(text)
                        .Concat(new[] {TagName[tagType].Item2});
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

            string currentTag;
            var startTagInd = FindTagStart(words, 0, out currentTag);
            if (startTagInd == -1)
                return words; 
            var endTagInd = FindTagEnd(words, startTagInd, currentTag);
            var startTag = GetSuffix(words.ElementAt(startTagInd), currentTag.Length);// SplitStartWordByTag(words.ElementAt(startTagInd), currentTag);
            if (endTagInd == -1)
                return words.Take(startTagInd).Concat(Wrapper(words.Skip(startTagInd+1))); 
            if (startTagInd == endTagInd)
            {
                var splited = GetMiddle(words.ElementAt(startTagInd), currentTag.Length);
                return words.Take(startTagInd).Concat(WrapIntoTag(new[] {splited},currentTag)).Concat(Wrapper(words.Skip(startTagInd+1)));
            }
            var endTag = GetPrefix(words.ElementAt(endTagInd), words.ElementAt(endTagInd).Length - currentTag.Length); //SplitEndWordByTag(words.ElementAt(endTagInd), currentTag);
            var wrappedText = WrapIntoTag(
                new[] {startTag}
                .Concat(words.Skip(startTagInd+1).Take(endTagInd-startTagInd-1))
                .Concat(new [] {endTag})
                , currentTag);
            return words.Take(startTagInd).Concat(wrappedText).Concat(Wrapper(words.Skip(endTagInd+1)));
        }
        
        public static int FindTagStart(IEnumerable<string> text, int startIndex, out string tag)
        {
            tag = string.Empty;
            if (text != null && startIndex < text.Count() && startIndex >= 0)
                for (var count = TagName.Keys.Max(x => x.Length); count > 0; count--)
                    for (var wordCount = startIndex; wordCount < text.Count(); wordCount++)
                    {
                        var prefix = GetPrefix(text.ElementAt(wordCount), count);
                        if (text.ElementAt(wordCount).Length > count && TagName.Keys.Contains(prefix))
                        {
                            tag = prefix;
                            return wordCount;
                        }
                    }
            return -1;
        }

        public static int FindTagEnd(IEnumerable<string> text, int startIndex, string tag)
        {
            if (text != null && startIndex < text.Count() && startIndex >= 0)
                for (var wordCount = startIndex; wordCount < text.Count(); wordCount++)
                {
                    if (text.ElementAt(wordCount).Length > tag.Length &&
                        TagName.Keys.Contains(GetSuffix(text.ElementAt(wordCount), tag.Length)) &&
                        GetSuffix(text.ElementAt(wordCount), tag.Length + 1)[0] != '\\')
                        return wordCount;
                }
            return -1;
        }


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
