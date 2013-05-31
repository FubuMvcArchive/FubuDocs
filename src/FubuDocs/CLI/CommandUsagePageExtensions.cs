using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.CommandLine;
using FubuMVC.Core.View;
using HtmlTags;

namespace FubuDocs.CLI
{
    public static class CommandUsagePageExtensions
    {
        public static CommandSectionTag SectionForCommand(this IFubuPage page, string applicationName, string commandName)
        {
            var command = page.CommandReportFor(applicationName, commandName);

            var tag = new CommandSectionTag(applicationName, command);
            tag.NoClosingTag();

            return tag;
        }

        public static CommandReport CommandReportFor(this IFubuPage page, string applicationName, string commandName)
        {
            var application = page.Get<ICommandDocumentationSource>().ReportFor(applicationName);
            var command = application.Commands.FirstOrDefault(x => x.Name == commandName);
            if (command == null)
                throw new ArgumentOutOfRangeException("commandName", "Could not find the named command in this application");

            return command;
        }


        public static TagList BodyForCommand(this IFubuPage page, string applicationName, string commandName)
        {
            var command = page.CommandReportFor(applicationName, commandName);


            return new TagList(CommandBodyTags(applicationName, command));
        }

        public static IEnumerable<HtmlTag> CommandBodyTags(string applicationName, CommandReport report)
        {
            if (report.Usages.Count() == 1)
            {
                yield return new SingleUsageTag(report.Usages.Single());
            }
            else
            {
                yield return new UsageTableTag(report);
            }

            if (report.Arguments.Any())
            {
                yield return new ArgumentsTag(report);
            }

            if (report.Flags.Any())
            {
                yield return new FlagsTag(report);
            }
        } 
    }
}