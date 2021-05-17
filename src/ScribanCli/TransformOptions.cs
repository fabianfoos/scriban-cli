using System;
using System.IO;
using CommandLine;

namespace ScribanCli
{
    [Verb("transform")]
    internal class TransformOptions
    {
        [Option('t', "template", HelpText = "Template file.", Required = true)]
        public string TemplateFile { get; set; }

        [Option('j', "json", HelpText = "Json model file or standard input if not specified.", Required = false)]
        public string JsonFile { get; set; }

        [Option('o', "output", HelpText = "Output file or standard output if not specified.", Required = false, Default = null)]
        public string Output { get; set; }

        public TextWriter OutputWriter
        {
            get
            {
                if (Output == null)
                    return Console.Out;
                else
                    return new StreamWriter(Output);
            }
        }

        public TextReader JsonInput
        {
            get
            {
                if (JsonFile == null)
                    return Console.In;
                else
                    return new StreamReader(JsonFile);
            }
        }

        [Option('l', "loop-limit", HelpText = "Loop limit.", Required = false, Default = 1000)]
        public int LoopLimit { get; set; }
    }
}