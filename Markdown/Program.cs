using System.Linq;
using System.Reflection;
using Markdown.Processors;
using Ninject;

namespace Markdown
{
    class Program
    {
        private IReader Reader { get; }
        private IProcessor Processor { get; }
        private IWriter Writer { get; }

        private Program(IReader reader, IProcessor processor, IWriter writer)
        {
            Processor = processor;
            Reader = reader;
            Writer = writer;
        }

        private void Run(string[] args)
        {
            ArgParser parsedArguments;
            if (!ArgParser.TryGetArguments(args, out parsedArguments))
                return;
            var text = Reader.ReadLines(parsedArguments.InputFileName).ToArray();
            Writer.Write(Processor.Parse(text), parsedArguments.OutputFileName);
        }

        static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            kernel.Bind<IReader>().To<MdReader>();
            kernel.Bind<IWriter>().To<HttpFileWriter>();
            kernel.Bind<IProcessor>().To<RecursiveProcessor>();
            new Program(kernel.Get<IReader>(), kernel.Get<IProcessor>(), kernel.Get<IWriter>()).Run(args);
        }

    }
}
