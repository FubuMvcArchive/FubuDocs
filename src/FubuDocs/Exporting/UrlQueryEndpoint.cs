using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.PathBased;

namespace FubuDocs.Exporting
{
    public class UrlQueryEndpoint
    {
        private static readonly string[] IgnoredPatterns;

        static UrlQueryEndpoint()
        {
            IgnoredPatterns = new[]
            {
                "/_fubu", "/_diagnostics", "/_content", "/_about"
            };
        }

        private readonly BehaviorGraph _graph;

        public UrlQueryEndpoint(BehaviorGraph graph)
        {
            _graph = graph;
        }

        public UrlList get_urls()
        {
            return new UrlList
            {
                Urls = findUrls().ToArray()
            };
        }

        private IEnumerable<string> findUrls()
        {
            foreach (var chain in _graph.Behaviors)
            {
                if (chain.IsPartialOnly) continue;
                if (chain.Route == null) continue;
                if (!chain.Route.RespondsToMethod("GET")) continue;

                
                if (chain.Route.Rank != 0) continue;
                if (chain.InputType() != null && chain.InputType().CanBeCastTo<ResourcePath>()) continue;

                if (chain.Calls.Any(x => x.HandlerType.Assembly == Assembly.GetExecutingAssembly()))
                {
                    if (!chain.Calls.Any(x => x.HasAttribute<ExportAttribute>())) continue;
                }

                var pattern = GetPattern(chain);
                
                if (ShouldIgnore(pattern)) continue;

                yield return pattern;
            }
        }

        public static string GetPattern(BehaviorChain chain)
        {
            var pattern = chain.GetRoutePattern();
            return GetPattern(pattern);
        }

        public static string GetPattern(string pattern)
        {
            if (pattern.StartsWith("http://"))
            {
                var index = pattern.IndexOf('/', 7);
                pattern = index == -1 ? "" : pattern.Substring(index);
            }

            return pattern;
        }

        public static bool ShouldIgnore(string pattern)
        {
            return IgnoredPatterns.Any(pattern.StartsWith);
        }
    }
}