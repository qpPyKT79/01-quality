using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown.Processors
{
    public interface IProcessor
    {
        string Parse(IEnumerable<string> lines);
    }
}
