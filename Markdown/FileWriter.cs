using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown
{
    public class FileWriter : IWriter
    {
        public void Write(IEnumerable<string> text, string name)
        {
            try
            {

            }
            catch (Exception error)
            {
                throw error;
            }
            System.IO.StreamWriter file = new System.IO.StreamWriter(name);
            file.WriteLine(text);
        }
    }
}
