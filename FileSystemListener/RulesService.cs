using FileSystemListener.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileSystemListener
{
    public class RulesService
    {
        private readonly ILogger logger;
        private readonly List<Rule> rules;
        private readonly string defaultFolder;

        public RulesService(IEnumerable<Rule> rules, string defaultFolder, ILogger logger)
        {
            this.rules = rules.ToList();
            this.logger = logger;
            this.defaultFolder = defaultFolder;
        }

        public void MoveItem(FileInfo file)
        {
            string oldPath = file.FullName;

            foreach (Rule rule in rules)
            {
                var match = Regex.Match(file.Name, rule.FilePattern);

                if (match.Success && match.Length == file.Name.Length)
                {
                    rule.MatchesCount++;
                    string newPath = FormDestinationPath(file, rule);
                    logger.Log(/*Strings.RuleMatch*/"Правило совпало");
                    MoveFile(oldPath, newPath);
                    logger.Log(/*string.Format(Strings.FileMovedTemplate, file.FullName, to)*/"Файл перемещён");
                    return;
                }
            }

            string defaultPath = Path.Combine(defaultFolder, file.Name);
            logger.Log(/*Strings.RuleNoMatch*/"Правила не совпали");
            MoveFile(oldPath, defaultPath);
            logger.Log(/*string.Format(Strings.FileMovedTemplate, file.FullName, to)*/"Файл перемещён");
        }

        private void MoveFile(string from, string to)
        {
            string dir = Path.GetDirectoryName(to);
            Directory.CreateDirectory(dir);

            //bool cannotAccessFile = true;
            //do
            //{
                try
                {
                    if (File.Exists(to))
                    {
                        File.Delete(to);
                    }
                    File.Move(from, to);
                   // cannotAccessFile = false;
                }
                catch (FileNotFoundException)
                {
                    logger.Log(/*Strings.CannotFindFile*/"Файл не найден");
                    //break;
                }
                catch (IOException ex)
                {
                    logger.Log(/*Strings.CannotFindFile*/"Возникли проблемы");
                //await Task.Delay(FileCheckTimoutMiliseconds);
            }
            //} while (cannotAccessFile);
        }

        private string FormDestinationPath(FileInfo file, Rule rule)
        {
            string extension = Path.GetExtension(file.Name);
            string filename = Path.GetFileNameWithoutExtension(file.Name);
            StringBuilder destination = new StringBuilder();
            destination.Append(Path.Combine(rule.DestinationFolder, filename));

            if (rule.IsDateAppended)
            {
                var dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
                dateTimeFormat.DateSeparator = ".";
                destination.Append($"{destination}_{DateTime.Now.ToLocalTime().ToString(dateTimeFormat.ShortDatePattern)}");
            }

            if (rule.IsOrderAppended)
            {
                destination.Append($"_{rule.MatchesCount}");
            }

            destination.Append(extension);
            return destination.ToString();
        }
    }
}
