using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Markdown
{
    public class MarkdownReader 
    {
        public IEnumerable<string> ReadLines(string sourceName)
        {
            try
            {
                return File.ReadAllLines(sourceName).Select(HttpUtility.HtmlEncode);
            }
            catch (FileNotFoundException)
            {
                throw;
            }
        }
    }
}
