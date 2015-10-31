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
        string Do(string text);
        string WrapIntoTag(string text, string tagName);
        int FindTagStart(string[] text, int startIndex, out string tag);
        int FindTagEnd(string[] text, int startIndex, string tag);
        string GetPrefix(string word, int count);
        string GetSuffix(string word, int count);
    }
}