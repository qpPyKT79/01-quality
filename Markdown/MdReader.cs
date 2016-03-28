using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Markdown
{
    public class MdReader: IReader
    {
        public IEnumerable<string> ReadLines(string sourceName)
            => File.ReadAllLines(sourceName).Select(HttpUtility.HtmlEncode);
    }
}
