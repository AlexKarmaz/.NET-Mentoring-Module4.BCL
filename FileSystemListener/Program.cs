using FileSystemListener.Configuration;
using FileSystemListener.EventArgs;
using FileSystemListener.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using Resources = FileSystemListener.Resources;

namespace FileSystemListener
{
    class Program
    {
        private static RulesService servise;
        private static List<string> directories;
        private static List<Rule> rules;

        static void Main(string[] args)
        {
            ILogger logger;
            IWatcher<FileInfo> watcher;
            FileSystemListenerConfigSection config = ConfigurationManager.GetSection("fileSystemSection") as FileSystemListenerConfigSection;

            if (config != null)
            {
                GetConfigurationSection(config);
            }
            else
            {
                Console.Write(Resources.Resources.ConfigNotFounded);
                return;
            }

            Console.WriteLine(config.Culture.DisplayName);

            logger = new Logger();
            watcher = new FilesWatcher(directories, logger);
            servise = new RulesService( rules, config.Rules.DefaultDirectory, logger);

            watcher.Created += OnCreated;

            Console.WriteLine(Resources.Resources.Exit);
            string enteredString = "";

            while (true)
            {
                enteredString = Console.ReadLine();

                if (enteredString == "Exit") break;
            }
        }

        private static void OnCreated(object sender, CreatedEventArgs<FileInfo> args)
        {
            servise.MoveItem(args.CreatedItem);
        }

        private static void GetConfigurationSection(FileSystemListenerConfigSection config)
        {
            directories = new List<string>(config.Directories.Count);
            rules = new List<Rule>();

            foreach (DirectoryElement directory in config.Directories)
            {
                directories.Add(directory.Path);
            }

            foreach (RuleElement rule in config.Rules)
            {
                rules.Add(new Rule
                {
                    FilePattern = rule.FilePattern,
                    DestinationFolder = rule.DestinationFolder,
                    IsDateAppended = rule.IsDateAppended,
                    IsOrderAppended = rule.IsOrderAppended
                });
            }

            CultureInfo.DefaultThreadCurrentCulture = config.Culture;
            CultureInfo.DefaultThreadCurrentUICulture = config.Culture;
        }
    }
}
