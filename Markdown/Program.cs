using System.Linq;

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
            writer.Write(Processor.Parse(text.ToArray()), parsedArguments.OutputFileName);
        }
    }
}
