using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown
{
    public class FileReader :IReader
    {
        public string Read(string sourceName)
        {
            try
            {
                return string.Join(string.Empty,File.ReadAllLines(sourceName).Select(line => line.TrimEnd(new [] {' '})+"\n")).TrimEnd(new[] {'\n'});
            }
            catch (FileNotFoundException)
            {
                throw;
            }
        }
    }
}
