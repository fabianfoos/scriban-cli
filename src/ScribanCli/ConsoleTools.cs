using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;
using Scriban.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScribanCli
{
    static class ConsoleTools
    {
        public static void Error(string message)
        {
            Console.Error.WriteLine(message);
        }
    }
}
