using System;
using System.Collections.Generic;
using System.IO;

namespace Cli.Template.Generator.Handler
{
    public class FileReplacer
    {
        private string _targetFolderPath;
        private int _totalFiles;
        public string Extension { get; set; } = "*";

        public FileReplacer(string targetFolderPath)
        {
            _targetFolderPath = targetFolderPath;
        }

        private IList<string> _filePaths { get; set; } = new List<string>();

        public void FindAndReplaceInFile(IDictionary<string, string> placeHolders)
        {
            _filePaths = Directory.GetFiles(_targetFolderPath, $"*.{Extension}", SearchOption.AllDirectories);
            _totalFiles = _filePaths.Count;
            Console.WriteLine($"#####################  There are '{_filePaths.Count}' files to process   ##################### {Environment.NewLine}");
            int counter = 0;
            foreach (var placeHolder in placeHolders)
            {
                counter++;
                Console.WriteLine($"{Environment.NewLine}************************************************************************************* ");
                Console.WriteLine($"({counter}/{placeHolders.Count}) Replace '{placeHolder.Key}' for '{placeHolder.Value}'");
                Console.WriteLine($"*************************************************************************************{Environment.NewLine} ");
                FindAndReplaceInFile(placeHolder.Key, placeHolder.Value);
            }
        }

        public void FindAndReplaceInFile(string oldValue, string newValue)
        {
            int counter = 0;

            foreach (var filePath in _filePaths)
            {
                try
                {
                    counter++;
                    Console.WriteLine($"({ counter}/{_totalFiles}) processing file '{filePath}'");
                    string contents = File.ReadAllText(filePath);
                    // If file is ReadOnly then remove that attribute.
                    var attributes = File.GetAttributes(filePath);
                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        File.SetAttributes(filePath, FileAttributes.Normal);
                    }

                    contents = contents.Replace(oldValue, newValue);
                    File.WriteAllText(filePath, contents);
                }
                catch {
                }
            }
        }
    }
}