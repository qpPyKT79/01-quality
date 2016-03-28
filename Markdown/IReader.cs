using System.Collections.Generic;

namespace Markdown
{
    public interface IReader
    {
        IEnumerable<string> ReadLines(string sourceName);
    }
}
