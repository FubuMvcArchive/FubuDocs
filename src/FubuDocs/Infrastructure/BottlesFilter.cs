using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FubuDocs.Infrastructure
{
	public class BottlesFilter
	{
		private static readonly IList<string> Projects = new List<string>();

		public static void Include(string project)
		{
			Projects.Add(project);
		}

		public static bool ShouldLoad(string directory)
		{
			var info = new DirectoryInfo(directory);
			return shouldLoadProject(info.Name.Replace(".Docs", ""));
		}

		public static bool ShouldLoad(Assembly assembly)
		{
			var name = assembly.GetName().Name.Replace(".Docs", "");
			return shouldLoadProject(name);
		}

		private static bool shouldLoadProject(string project)
		{
			if (!Projects.Any()) return true;

			return Projects.Any(x => x.Equals(project, StringComparison.OrdinalIgnoreCase));
		}

		public static void Reset()
		{
			Projects.Clear();
		}
	}
}