using Cli.Template.Generator.ConfigFiles;
using System;
using System.Collections.Generic;
using System.IO;

namespace Cli.Template.Generator
{
    /// <summary>
    /// DirectoryRenamer.RenameDirectoryTree(name => name.Replace( "MatchA", "AMatch" ) );
    /// </summary>
    public class DirectoryReplacer
    {
        private readonly DirectoryReplacerConfig? _directoryConfig= null;
        private readonly GeneralGeneratorConfig? _generalConfig = null;

        public DirectoryReplacer(GeneralGeneratorConfig? generalConfig, DirectoryReplacerConfig? directoryConfig)
        {
            if (generalConfig == null || directoryConfig == null)
            {
                return;
            }

            _directoryConfig = directoryConfig;
            _generalConfig = generalConfig;
            // Directory.Delete(targetFolderPath, true);
        }

        public void ClearTarget()
        {
            if (Directory.Exists(_generalConfig?.TargetFolderPath))
            {
                Directory.Delete(_generalConfig?.TargetFolderPath, true);
            }
            Directory.CreateDirectory(_generalConfig?.TargetFolderPath);
        }

        public void CopySoureToTarget()
        {
            if (_generalConfig == null)
            {
                return;
            }

            if (_directoryConfig!.IsSolutionInRoot && !string.IsNullOrEmpty(_directoryConfig!.VsSolutionName))
            {
                _generalConfig.TargetFolderPath += $"\\{_directoryConfig!.VsSolutionName}";
                EnsureDirectory(_generalConfig.TargetFolderPath);
            }

            DirectoryCopy(_generalConfig.SourceFolderPath, _generalConfig.TargetFolderPath);
        }

        private void EnsureDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                return;
            }

            Directory.CreateDirectory(path);
            Console.WriteLine($"The directory was created successfully at {path}.");
        }

        public void RenameDirectoryTree(List<Func<string, string>> renamingRules)
        {
            var di = new DirectoryInfo(_generalConfig?.TargetFolderPath);
            foreach (var renamingRule in renamingRules)
            {
                RenameDirectoryTree(di, renamingRule);
            }
        }

        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs=true)
        {
            var dir = GetDirectoriesFromSource(sourceDirName, out var dirs);
            // If the destination directory doesn't exist, create it.
            Directory.CreateDirectory(destDirName);
            CopySourceFilesToTarget(destDirName, dir);
            CopySubDirectories(destDirName, copySubDirs, dirs);
        }

        /// <summary>
        /// Get the subdirectories for the specified directory.
        /// </summary>
        private DirectoryInfo GetDirectoriesFromSource(string sourceDirName, out DirectoryInfo[] dirs)
        {
            var dir = GetDirectoryInfo(sourceDirName);

            dirs = dir.GetDirectories();
            return dir;
        }

        private DirectoryInfo GetDirectoryInfo(string sourceDirName)
        {
            var dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"Directory does not exist or could not be found: {sourceDirName}");
            }

            return dir;
        }

        /// <summary>
        /// Get the files in the directory and copy them to the new location.
        /// </summary>
        private void CopySourceFilesToTarget(string destDirName, DirectoryInfo dir)
        {
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }
        }

        /// <summary>
        /// If copying subdirectories, copy them and their contents to new location.
        /// </summary>
        private void CopySubDirectories(string destDirName, bool copySubDirs, DirectoryInfo[] dirs)
        {
            if (!copySubDirs)
            {
                return;
            }

            foreach (var subdirectory in dirs)
            {
                if (_directoryConfig != null && _directoryConfig.ExcludedDirectories.Contains(subdirectory.Name))
                {
                    continue;
                }

                var tempPath = Path.Combine(destDirName, subdirectory.Name);
                DirectoryCopy(subdirectory.FullName, tempPath);
            }
        }

        private void RenameDirectoryTree(DirectoryInfo directory, Func<string, string> renamingRule)
        {
            InternalRenameDirectoryTree(directory, renamingRule);

            var currentName = directory.Name;
            var newName = renamingRule(currentName);
            if (currentName == newName)
            {
                return;
            }

            if (directory.Parent == null)
            {
                return;
            }

            var newDirname = Path.Combine(directory.Parent.FullName, newName);
            directory.MoveTo(newDirname);
        }

        private void InternalRenameDirectoryTree(DirectoryInfo di, Func<string, string> renamingRule)
        {
            foreach (var item in di.GetFileSystemInfos())
            {
                switch (item)
                {
                    case DirectoryInfo subdir:
                    {
                        InternalRenameDirectoryTree(subdir, renamingRule);

                        var currentName = subdir.Name;
                        var newName = renamingRule(currentName);
                        if (currentName != newName)
                        {
                            var newDirname = Path.Combine(subdir.Parent.FullName, newName);
                            subdir.MoveTo(newDirname);
                        }

                        break;
                    }
                    case FileInfo file:
                    {
                        var currentName = Path.GetFileNameWithoutExtension(file.Name);
                        var newName = renamingRule(currentName);
                        if (currentName != newName)
                        {
                            var newFilename = Path.Combine(file.DirectoryName!, newName + file.Extension);
                            file.MoveTo(newFilename);
                        }

                        break;
                    }
                }
            }
        }
    }
}