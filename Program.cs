using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;

namespace photo_metadata
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunWithOptions)
                .WithNotParsed(HandleNotParsed);

            Console.WriteLine("Press 'q' to quit.");
            while (Console.Read() != 'q');
        }

        private static void HandleNotParsed(IEnumerable<Error> obj)
        {
            foreach (var error in obj)
            {
                Console.WriteLine($"{error.Tag}: {error}");
            }
        }

        private static void RunWithOptions(Options obj)
        {
            var watcher = new FileSystemWatcher(obj.Path);
            watcher.NotifyFilter = NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.FileName
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.Attributes
                                 | NotifyFilters.Size;
            watcher.Filter = "*.*";

            watcher.IncludeSubdirectories = true;
            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            watcher.Error += OnError;

            watcher.EnableRaisingEvents = true;
        }

        private static void OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine($"{e.GetException()}");
        }

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Renamed: {e.OldName} -> {e.Name}");
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Deleted: {e.Name}");
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Created: {e.Name}");
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Changed: {e.ChangeType} / {e.Name} ({e.FullPath})");
        }
    }

    public class Options
    {
        [Option('p', "path", Required = true, HelpText = "The path to watch for file changes")]
        public string Path { get; set; }
    }
}
