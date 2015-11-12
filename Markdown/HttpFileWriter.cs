using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown
{
    public class HttpFileWriter : IWriter
    {
        public void Write(string text, string name)
        {
            try
            {
                text = "<meta http-equiv = \"Content-Type\" content = \"text/html; charset=utf-8\">" + text;
                File.WriteAllText(name, text);
            }
            catch (Exception error)
            {
                throw error;
            }
        }
    }
}
