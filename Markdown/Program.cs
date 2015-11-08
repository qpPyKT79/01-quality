using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown
{
    class Program
    {
        static void Main(string[] args)
        {
            IReader reader = new MarkdownReader();
            var text = reader.ReadLines(args[0]);
            IWriter writer = new HttpFileWriter();
            writer.Write(Processor.Parse(text.ToArray()), args[1]);
        }
    }
}
