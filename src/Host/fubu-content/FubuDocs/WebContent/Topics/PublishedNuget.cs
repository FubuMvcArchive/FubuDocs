using System;
using FubuCore;
using System.Linq;

namespace FubuDocs.Topics
{
    public class PublishedNuget
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public static PublishedNuget ReadFrom(string file)
        {
            var text = new FileSystem().ReadStringFromFile(file);

            var nuget = new PublishedNuget();

            findName(text, nuget);
            findDescription(text, nuget);

            return nuget;
        }

        private static void findDescription(string text, PublishedNuget nuget)
        {
            string openingTag = "<description>";
            string closingTag = "</description>";

            var start = text.IndexOf(openingTag);

            var end = text.IndexOf(closingTag);

            if (start > -1 && end > -1)
            {
                nuget.Description = text.Substring(start + openingTag.Length, end - start - openingTag.Length);
            }
        }

        private static void findName(string text, PublishedNuget nuget)
        {
            string openingTag = "<id>";
            string closingTag = "</id>";

            var start = text.IndexOf(openingTag);
            
            var end = text.IndexOf(closingTag);

            if (start > -1 && end > -1)
            {
                nuget.Name = text.Substring(start + openingTag.Length, end - start - openingTag.Length);
            }
        }



        public static void GatherNugetsIntoProject(ProjectRoot project, string directory)
        {
            var files = new FileSystem().FindFiles(directory, FileSet.Deep("*.nuspec"));
            var nugets = files.Select(x => ReadFrom(x));

            if (project.NugetWhitelist.IsEmpty())
            {
                project.Nugets = nugets.Where(x => !x.Name.EndsWith(".Docs")).ToArray();
            }
            else
            {
                var acceptable = project.NugetWhitelist.ToDelimitedArray();

                project.Nugets = nugets.Where(x => acceptable.Contains(x.Name)).ToArray();
            }
        }

        protected bool Equals(PublishedNuget other)
        {
            return string.Equals(Name, other.Name) && string.Equals(Description, other.Description);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PublishedNuget) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0)*397) ^ (Description != null ? Description.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, Description: {1}", Name, Description);
        }
    }
}