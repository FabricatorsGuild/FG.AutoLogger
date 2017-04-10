using System;

namespace CodeEffect.Diagnostics.EventSourceGenerator
{
    public static class PathExtensions
    {

        public static string GetAbsolutePath(string relativePath)
        {
            var currentDirectory = System.IO.Directory.GetCurrentDirectory();
            return GetAbsolutePath(currentDirectory, relativePath);
        }

        public static string GetAbsolutePath(string basePath, string relativePath)
        {
            var combinedPath = relativePath;
            if (!System.IO.Path.IsPathRooted(relativePath))
            {
                combinedPath = System.IO.Path.Combine(basePath, relativePath);
            }
            
            return System.IO.Path.GetFullPath((new Uri(combinedPath)).LocalPath);
        }

        public static string GetEvaluatedPath(string path)
        {
            if (!System.IO.Path.IsPathRooted(path))
            {
                return GetAbsolutePath(path);
            }

            return System.IO.Path.GetFullPath((new Uri(path)).LocalPath);
        }
    }
}