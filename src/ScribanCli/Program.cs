using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using CommandLine;
using Scriban;
using Scriban.Runtime;
using Scriban.Syntax;

namespace ScribanCli
{
    partial class Program
    {
        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<TransformOptions>(args)
                .WithParsed<TransformOptions>(RunTransform)
                .WithNotParsed(errs => errs.Output());
        }

        private static void RunTransform(TransformOptions options)
        {
            var source = File.ReadAllText(options.TemplateFile);
            var template = Template.Parse(source, options.TemplateFile);
            var fg = Console.ForegroundColor;
            if (template.Messages.Count > 0)
            {
                foreach (var item in template.Messages)
                {
                    Console.ForegroundColor = item.Type == Scriban.Parsing.ParserMessageType.Error ? ConsoleColor.Red : ConsoleColor.Yellow;
                    Console.Error.WriteLine($"{item.Type}: {item.Message}.");
                    Console.Error.WriteLine($"At {item.Span.FileName}: Line {item.Span.Start.Line + 1}, Column {item.Span.Start.Column + 1}");
                    var start = source.LastIndexOf('\n', item.Span.Start.Offset);
                    start = start == -1 ? 0 : start;
                    var end = source.IndexOf('\n', item.Span.Start.Offset);
                    end = end == -1 ? source.Length : end;
                    Console.Error.WriteLine(source[start..end]);
                    Console.Error.WriteLine("^".PadLeft(item.Span.Start.Column + 1));
                }
            }

            Console.ForegroundColor = fg;
            if (template.HasErrors)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Error parsing template.");
                Console.ForegroundColor = fg;
                return;
            }

            var context = new TemplateContext();
            context.LoopLimit = options.LoopLimit;
            context.TemplateLoader = new TemplateLoader();
            var filetools = new ScriptObject();
            filetools.Import(typeof(FileTools));
            var consoletools = new ScriptObject();
            consoletools.Import(typeof(ConsoleTools));
            var tools = new ScriptObject();
            tools.Add("file", filetools);
            tools.Add("console", consoletools);
            context.PushGlobal(tools);
            var model = JsonSerializer.Deserialize<JsonElement>(options.JsonInput.ReadToEnd());
            context.PushGlobal(BuildModel(model));

            try
            {
                var result = template.Render(context);
                options.OutputWriter.Write(result);
                options.OutputWriter.Close();
            }
            catch (ScriptRuntimeException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(ex.OriginalMessage);
                Console.Error.WriteLine($"At {ex.Span.FileName}: Line {ex.Span.Start.Line + 1}, Column {ex.Span.Start.Column + 1}");
                var start = source.LastIndexOf('\n', ex.Span.Start.Offset);
                start = start == -1 ? 0 : start;
                var end = source.IndexOf('\n', ex.Span.Start.Offset);
                end = end == -1 ? source.Length : end;
                Console.Error.WriteLine(source[start..end]);
                Console.Error.WriteLine("^".PadLeft(ex.Span.Start.Column + 1));
            }

            Console.ForegroundColor = fg;
        }

        private static IScriptObject BuildModel(JsonElement model)
        {
            return GetModel(model) as IScriptObject;
        }

        private static object GetModel(JsonElement model)
        {
            switch (model.ValueKind)
            {
                case JsonValueKind.Array:
                    return GetArrayModel(model);
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.Number:
                    if (model.TryGetInt64(out var longValue))
                        return longValue;
                    else
                        return model.GetDecimal();
                case JsonValueKind.Object:
                    var scriptObject = new ScriptObject();
                    foreach (var item in model.EnumerateObject())
                    {
                        scriptObject.Add(StandardMemberRenamer.Rename(item.Name), GetModel(item.Value));
                    }

                    return scriptObject;
                case JsonValueKind.String:
                    return model.GetString();
                case JsonValueKind.True:
                    return true;
                default:
                    return null;
            }
        }

        private static object GetArrayModel(JsonElement value)
        {
            var list = new List<object>();
            foreach (var item in value.EnumerateArray())
            {
                list.Add(GetModel(item));
            }

            return list;
        }
    }
}