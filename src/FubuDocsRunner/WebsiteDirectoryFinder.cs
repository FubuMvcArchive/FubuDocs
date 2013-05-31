using System;
using System.IO;
using FubuCore;

namespace FubuDocsRunner
{
    public static class WebsiteDirectoryFinder
    {
         public static string FindApplicationFolder()
         {
             var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
             var inBaseDirectory = baseDirectory.AppendPath("FubuDocs");

             if (Directory.Exists(inBaseDirectory))
             {
                 return inBaseDirectory;
             }

             return baseDirectory.ParentDirectory().ParentDirectory().AppendPath("FubuDocs");
         }
    }
}