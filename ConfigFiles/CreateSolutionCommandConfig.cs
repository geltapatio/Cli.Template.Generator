using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Cli.Template.Generator.ConfigFiles
{
    public class CreateSolutionCommandConfig: CommandConfig
    {
        public CreateSolutionCommandConfig(IConfigurationRoot? configuration)
        {
            configuration?.GetSection("CreateSolutionCommand").Bind(this);
            InitFunctionsRules();
        }

        public Dictionary<string, string> RenamingDirectoryRules { get; set; }  = new Dictionary<string, string>();

        public bool IsSolutionInRoot { get; set; } = true;

        public List<Func<string, string>> RenamingFunctionsRules = new List<Func<string, string>>();

        private void InitFunctionsRules()
        {
            foreach (var rule in RenamingDirectoryRules)
            {
                RenamingFunctionsRules.Add(name => name.Replace(rule.Key, rule.Value));
            }
        }
    }
}
