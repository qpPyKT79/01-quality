using System.Linq;
using Markdown.Processors;

namespace Markdown
{
    static class Program
    {
        static void Main(string[] args)
        {
            ArgParser parsedArguments;
            if (!ArgParser.TryGetArguments(args, out parsedArguments))
                return;
        
            IReader reader = new MdReader();
            var text = reader.ReadLines(parsedArguments.InputFileName);
            IWriter writer = new HttpFileWriter();
            IProcessor processor = new RecursiveProcessor();
            writer.Write(processor.Parse(text.ToArray()), parsedArguments.OutputFileName);
        }
    }
}
