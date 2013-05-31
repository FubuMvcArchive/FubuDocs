using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuCore;

namespace FubuDocs.Topics
{
    [ApplicationLevel]
    public class TopicGraph
    {
        public static readonly TopicGraph AllTopics = new TopicGraph();
        private readonly Cache<string, ProjectRoot> _projects = new Cache<string, ProjectRoot>(); 
        private readonly Cache<string, Topic> _topicCache = new Cache<string, Topic>(); 

        public TopicGraph()
        {
            _topicCache.OnMissing = key => {
                var projectName = key.Split('/').First();

                if (!_projects.Has(projectName))
                {
                    return null;
                }

                return _projects[projectName].FindByKey(key);
            };
        }

        public IEnumerable<Topic> AllPossibleTopics()
        {
            return _projects.SelectMany(x => x.AllTopics());
        }

        public void AddProject(ProjectRoot project)
        {
            _projects[project.Name.ToLowerInvariant()] = project;
        }

        public IEnumerable<ProjectRoot> Projects
        {
            get { return _projects; }
        }

        public ProjectRoot ProjectFor(string name)
        {
            return _projects[name.ToLower()];
        }

        /// <summary>
        /// Returns the Topic for a specified key.  
        /// </summary>
        /// <returns></returns>
        public Topic Find(string key)
        {
            return _topicCache[key];
        }

        public void ConfigureRelationships()
        {
            var imports = AllPossibleTopics().Where(x => x.Import.IsNotEmpty()).ToArray();
            imports.Each(x => {
                if (!_projects.Has(x.Import))
                {
                    x.Remove();
                    return; // CANNOT BLOW UP
                }

                var project = _projects[x.Import];
                project.Parent = x.Project;
                var index = project.Index;

                var rootUrl = x.Url;

                x.ReplaceWith(index);
                index.Url = rootUrl;

                index.Project = x.Project;
                index.Descendents().Each(t => {
                    t.Project = x.Project;
                    t.Url = rootUrl.AppendUrl(t.Url.ChildUrl());
                });

                _projects.Remove(x.Import);
            });

            _projects.Where(x => x.PluginTo.IsNotEmpty()).Each(x => {
                if (!_projects.Has(x.PluginTo.ToLower()))
                {
                    return;
                }

                var parent = _projects[x.PluginTo.ToLower()];
                parent.Plugins.Add(x);
            });
        }

        public ProjectRoot TryFindProject(string name)
        {
            var project = _projects.FirstOrDefault(x => x.Name.EqualsIgnoreCase(name));
            return project;
        }
    }
}