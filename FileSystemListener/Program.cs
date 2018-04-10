using FileSystemListener.EventArgs;
using FileSystemListener.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemListener
{
    class Program
    {
        static ILogger logger;
        static IWatcher<FileInfo> watcher;
        static RulesService servise;

        static void Main(string[] args)
        {
            CultureInfo cI = new CultureInfo("en-US");

            CultureInfo.DefaultThreadCurrentCulture = cI;
            CultureInfo.DefaultThreadCurrentUICulture = cI;
            //CultureInfo.CurrentUICulture = cI;
            //CultureInfo.CurrentCulture = cI;

            String[] directories = { "F:\\BCL", "F:\\BCL\\New folder" };
            Rule[] rules = { new Rule { FilePattern = "\\.doc*", IsOrderAppended = false, IsDateAppended =false, DestinationFolder = "F:\\BCL\\DOCS" },
                             new Rule { FilePattern = "\\.txt*", IsOrderAppended = true, IsDateAppended =true, DestinationFolder = "F:\\BCL\\TXTS"  }};

            logger = new Logger();
            watcher = new FilesWatcher(directories, logger);
            servise = new RulesService( rules, "F:\\BCL\\Default", logger);

            watcher.Created += OnCreated;

            Console.WriteLine("To exit enter\"Exit\" or Ctrl+C or Ctrl+Break ");
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
    }
}
