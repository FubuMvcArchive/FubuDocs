using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Files;
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
                "/_fubu", "/_diagnostics", "/_content", "/_about", "urls"
            };
        }

        private readonly BehaviorGraph _graph;
        private readonly IAssetFileGraph _assetFiles;

        public UrlQueryEndpoint(BehaviorGraph graph, IAssetFileGraph assetFiles)
        {
            _graph = graph;
            _assetFiles = assetFiles;
        }

        public UrlList get_urls()
        {
            var urls = findUrls().ToArray();
            if (!urls.Contains(string.Empty))
            {
                var list = new List<string>(urls);
                list.Insert(0, string.Empty);
                urls = list.ToArray();
            }

            return new UrlList
            {
                Urls = urls
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

                foreach ( var route in chain.AdditionalRoutes)
                {
                    yield return route.Pattern;
                }
            }

            foreach (var assetFile in _assetFiles.AllFiles())
            {
                if (assetFile.Name.Contains("diagnostics")) continue;
                if (assetFile.Name.Contains("slickgrid")) continue;

                yield return AssetUrls.DetermineRelativeAssetUrl(assetFile.Folder, assetFile.Name);
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
            var match = "/" + pattern.TrimStart('/');

            return IgnoredPatterns.Any(match.StartsWith);
        }
    }
}