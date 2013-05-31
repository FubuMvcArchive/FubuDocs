using System;
using System.Diagnostics;
using FubuCore;

namespace FubuDocs.Infrastructure
{
    public static class EditorLauncher
    {
        public static string Editor = "FubuDocsEditor";

        public static void LaunchFile(string file)
        {
            var editor = GetEditor();
        
            if (editor.IsNotEmpty())
            {
                Process.Start(editor, file);
            }
            else
            {
                new FileSystem().LaunchEditor(file);
            }
        }

        public static string GetEditor()
        {
            return Environment.GetEnvironmentVariable(Editor, EnvironmentVariableTarget.Machine) ?? "";
        }

        public static void SetEditor(string editor)
        {
            Environment.SetEnvironmentVariable(Editor, editor, EnvironmentVariableTarget.Machine);
        }
    }
}