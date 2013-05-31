using FubuCore.CommandLine;

namespace FubuDocs.CLI
{
    public interface ICommandDocumentationSource
    {
        CommandLineApplicationReport ReportFor(string applicationName);
    }
}