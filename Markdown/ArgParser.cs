using System;
using Fclp;

namespace Markdown
{
    public class ArgParser
    {
        public string InputFileName { get; set; }
        public string OutputFileName { get; set; }

        public static bool TryGetArguments(string[] args, out ArgParser parsedArguments)
        {
            var argumentsParser = new FluentCommandLineParser<ArgParser>();
            argumentsParser.Setup(a => a.InputFileName)
                .As('i', "input")
                .WithDescription("Input file name")
                .Required();

            argumentsParser.Setup(a => a.OutputFileName)
                .As('o', "output")
                .WithDescription("Output file name")
                .Required();

            argumentsParser.SetupHelp("?", "h", "help")
                .Callback(text => Console.WriteLine(text));

            var parsingResult = argumentsParser.Parse(args);

            if (parsingResult.HasErrors)
            {
                argumentsParser.HelpOption.ShowHelp(argumentsParser.Options);
                parsedArguments = null;
                return false;
            }

            parsedArguments = argumentsParser.Object;
            return !parsingResult.HasErrors;
        }

    }
}
