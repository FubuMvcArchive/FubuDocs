using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuCore;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;

namespace FubuDocs.Topics
{
    // Only testing w/ integration tests.
    public class TopicLoader
    {
        private readonly ISparkTemplateRegistry _sparkTemplates;

        public TopicLoader(ISparkTemplateRegistry sparkTemplates)
        {
            _sparkTemplates = sparkTemplates;
        }

        public TopicLoader(BehaviorGraph graph)
        {
            // Just need to force the views to be executed and found
            var views = graph.Settings.Get<ViewEngines>().Views;

            // Not super wild about this
            _sparkTemplates = (ISparkTemplateRegistry) graph.Services.DefaultServiceFor<ISparkTemplateRegistry>().Value;
        }

        public static bool IsTopic(ITemplate descriptor)
        {
            string path = descriptor.ViewPath.Replace("\\", "/");

            if (path.Contains("/Samples/") || path.Contains("/Examples/")) return false;
            if (path.Contains("/samples/") || path.Contains("/examples/")) return false;

            if (descriptor.RelativePath().StartsWith("snippets")) return false;

            return true;
        }

        public IEnumerable<ITopicFile> FindFilesFromBottle(string bottleName)
        {
            return _sparkTemplates.Where(x => x.Origin == bottleName)
                                  .OfType<Template>()
                                  .Where(IsTopic)
                                  .Select(x => new SparkTopicFile(new ViewDescriptor<Template>(x)));
        }

        public ProjectRoot LoadProject(string bottleName, string folder)
        {
            IEnumerable<ITopicFile> files = FindFilesFromBottle(bottleName);
            ProjectRoot project = ProjectRoot.LoadFrom(folder.AppendPath(ProjectRoot.File));
            
            return CorrelateProject(project, files);
        }

        public static ProjectRoot CorrelateProject(ProjectRoot project, IEnumerable<ITopicFile> files)
        {
            var folders = new Cache<string, TopicFolder>(raw => new TopicFolder(raw, project));
            files.GroupBy(x => (x.Folder ?? String.Empty)).Each(@group => {
                TopicFolder topicFolder = folders[@group.Key];
                var folderTopics = @group.Select(file => new Topic(topicFolder, file)).ToArray();

                topicFolder.AddFiles(folderTopics);
                folderTopics.Each(TopicBuilder.BuildOut);

                var parentUrl = @group.Key.ParentUrl();
                while (parentUrl.IsNotEmpty())
                {
                    folders.FillDefault(parentUrl);
                    parentUrl = parentUrl.ParentUrl();
                }
            });

            folders.Each(x => {
                if (x.Raw == String.Empty) return;

                string rawParent = x.Raw.ParentUrl();


                folders.WithValue(rawParent, parent => parent.Add(x));
            });

            TopicFolder masterFolder = folders[String.Empty];
            IEnumerable<Topic> topLevelSubjects = masterFolder.TopLevelTopics().ToArray();
            project.Index = topLevelSubjects.FirstOrDefault(x => x.IsIndex);
            project.Splash = project.Index.ChildNodes.FirstOrDefault(x => x.Key.EndsWith("/splash"));

            if (project.Splash != null)
            {
                project.Splash.Remove();

                project.Splash.Url = project.Url;
                project.Index.Url = project.Url.AppendUrl("index");
            }

            return project;
        }

        public static ProjectRoot LoadFromFolder(string folder)
        {
            var project = ProjectRoot.LoadFrom(folder.AppendPath(ProjectRoot.File));

            var files = new FileSystem()
                .FindFiles(folder, FileSet.Deep("*.spark"))
                .Select(file => new Template {FilePath = file, RootPath = folder, ViewPath = file})
                .Where(IsTopic)
                .Select(x => new SparkTopicFile(new ViewDescriptor<Template>(x))).ToArray();

            CorrelateProject(project, files);

            return project;
        }

        public static string FindProjectRootFolder(string folder)
        {
            if (folder.IsDocProjectRoot()) return folder;

            if (folder.Contains(".Docs"))
            {
                var parent = folder.ParentDirectory();
                while (!parent.IsDocProjectRoot())
                {
                    parent = parent.ParentDirectory();
                }

                return parent;
            }

            return null;
        }

        public static IEnumerable<string> FindDocumentDirectories(string directory)
        {
            if (!Directory.Exists(directory.AppendPath("src"))) return new string[0];

            return Directory.GetDirectories(directory.AppendPath("src"), "*.Docs", SearchOption.TopDirectoryOnly);
        }
    }

    public static class TopicFilePathExtensions
    {
        public static bool IsDocProjectRoot(this string folder)
        {
            return folder.EndsWith(".Docs") && File.Exists(folder.AppendPath(ProjectRoot.File));
        }

        public static bool IsSolutionRoot(this string folder)
        {
            return File.Exists(folder.AppendPath("ripple.config"));
        }

        public static int OrderPrefix(this string name)
        {
            var parts = name.Split('.');
            var order = -1;

            return int.TryParse(parts.First(), out order) ? order : -1;
        }
    }
}