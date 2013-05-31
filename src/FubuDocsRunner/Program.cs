using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FubuCore.CommandLine;

namespace FubuDocsRunner
{

    public class FubuDocsExecutor : CommandExecutor
    {
        public FubuDocsExecutor()
        {
            Factory.RegisterCommands(GetType().Assembly);
        }
    }

    class Program
    {
        public static int Main(string[] args)
        {
            return CommandExecutor.ExecuteInConsole<FubuDocsExecutor>(args);
        }
    }
}
