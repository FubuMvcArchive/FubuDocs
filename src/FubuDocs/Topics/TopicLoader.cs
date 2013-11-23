using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Bottles;
using FubuCore;
using FubuCore.Util;
using FubuDocs.Skinning;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;

namespace FubuDocs.Topics
{
    // Only testing w/ integration tests.
    public class TopicLoader
    {
        /// <summary>
        /// Have to track this to help with topic loading in the case of
        /// the FubuMVC "application" being the one and only FubuDocs
        /// project because the files will be loaded against "Application"
        /// </summary>
        public static string ApplicationBottle;

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
            var path = descriptor.ViewPath.Replace("\\", "/");

            if (path.Contains("/Samples/") || path.Contains("/Examples/")) return false;
            if (path.Contains("/samples/") || path.Contains("/examples/")) return false;

            if (descriptor.RelativePath().StartsWith("snippets")) return false;

            var filename = Path.GetFileName(descriptor.FilePath);
            if (OverrideChrome.ReservedNames.Contains(filename)) return false;

            return true;
        }

        public IEnumerable<ITopicFile> FindFilesFromBottle(string bottleName)
        {
            var searchBottle = bottleName == ApplicationBottle ? "Application" : bottleName;

            return _sparkTemplates.Where(x => x.Origin == searchBottle)
                                  .OfType<Template>()
                                  .Where(IsTopic)
                                  .Select(x => new SparkTopicFile(new ViewDescriptor<Template>(x)));
        }

        public ProjectRoot LoadProject(string bottleName, string folder)
        {
            var files = FindFilesFromBottle(bottleName);
            var project = ProjectRoot.LoadFrom(folder.AppendPath(ProjectRoot.File));

            findProjectVersion(bottleName, folder, project);

            return CorrelateProject(project, files);
        }

        private static void findProjectVersion(string bottleName, string folder, ProjectRoot project)
        {
            if (bottleName == "Application") return; // hokey, but it stops an error.

            var assemblyFileName = bottleName + ".dll";

            var file = new FileSystem().FindFiles(folder, FileSet.Deep(assemblyFileName)).FirstOrDefault();
            if (file != null)
            {



                try
                {
                    var assembly = Assembly.ReflectionOnlyLoadFrom(file);
                    if (assembly != null)
                    {
                        project.Version = assembly.GetName().Version.ToString().Split('.').Take(3).Join(".");
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Unable to find a version for assembly at " + file);
                }
            }

            if (project.Version.IsNotEmpty()) return;

            try
            {
                var assembly = Assembly.Load(bottleName);
                if (assembly != null)
                {
                    project.Version = assembly.GetName().Version.ToString();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Could not load the assembly for " + bottleName);
            }
        }

        public static ProjectRoot CorrelateProject(ProjectRoot project, IEnumerable<ITopicFile> files)
        {
            var folders = new Cache<string, TopicFolder>(raw => new TopicFolder(raw, project));
            files.GroupBy(x => (x.Folder ?? String.Empty)).Each(@group => {
                var topicFolder = folders[@group.Key];
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
                if (x.OrderString == String.Empty) return;

                var rawParent = x.OrderString.ParentUrl();


                folders.WithValue(rawParent, parent => parent.Add(x));
            });

            var masterFolder = folders[String.Empty];
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
            return Directory.GetDirectories(directory, "*.Docs", SearchOption.AllDirectories).Where(x => !x.Contains("fubu-content") && !x.Contains("snippets") && !x.Contains("packages"));
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