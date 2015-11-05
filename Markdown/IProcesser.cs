using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Markdown
{
    public interface IProcesser
    {
        IEnumerable<string> WrapIntoTag(IEnumerable<string> text, string tagName);
        IEnumerable<string> Do(IEnumerable<string> words);
        IEnumerable<string> SplitStartWordByTag(string word, string tag);
        IEnumerable<string> SplitEndWordByTag(string word, string tag);
        int FindTagStart(IEnumerable<string> text, int startIndex, out string tag);
        int FindTagEnd(IEnumerable<string> text, int startIndex, string tag);
        string GetPrefix(string word, int count);
        string GetSuffix(string word, int count);
    }
}