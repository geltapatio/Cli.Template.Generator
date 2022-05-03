using Cli.Template.Generator.ConfigFiles;
using Cli.Template.Generator.Handler;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Cli.Template.Generator.Commands
{
    public class CreateSolutionCommand: GeneratorBase, ICommand
    {
        private readonly CreateSolutionCommandConfig _config;
        public CreateSolutionCommand(IConfigurationRoot? configuration)
        {
            Configuration = configuration;
            Setup();
            _config = new CreateSolutionCommandConfig(configuration);
        }

        public string Name => "Create a .net core solution based on template";

        public bool Activate { get; set; } = true;
        public IConfigurationRoot? Configuration { get; set; }

        public void Setup()
        {
            if (Configuration == null)
            {
                return;
            }

            GeneralGeneratorConfig = new GeneralGeneratorConfig();
            Configuration.GetSection("GeneralGeneratorConfig").Bind(GeneralGeneratorConfig);
            DirectoryReplacerConfig = new DirectoryReplacerConfig();
            Configuration.GetSection("DirectoryReplacerConfig").Bind(DirectoryReplacerConfig);
            DirectoryReplacerConfig.ExcludedDirectories = Configuration.GetSection("DirectoryReplacerConfig:ExcludedDirectories").Get<List<string>>();
            DirectoryReplacer = new DirectoryReplacer(GeneralGeneratorConfig, DirectoryReplacerConfig);
            FileReplacer = new FileReplacer(GeneralGeneratorConfig.TargetFolderPath);
        }

        public Dictionary<string, string> PlaceHolders { get; set; }  = new Dictionary<string, string>();

        public void Execute()
        {
            if (DirectoryReplacerConfig.ClearTarget)
            {
                DirectoryReplacer?.ClearTarget();
            }
            DirectoryReplacer?.CopySoureToTarget();
            DirectoryReplacer?.RenameDirectoryTree(_config.RenamingFunctionsRules);
            FileReplacer?.FindAndReplaceInFile(_config.PlaceHolders);
        }
    }
}
