using Cli.Template.Generator.ConfigFiles;
using Cli.Template.Generator.Handler;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Cli.Template.Generator.Commands
{
    public class GeneratorBase
    {
        protected DirectoryReplacer? DirectoryReplacer { get; set; }
        protected FileReplacer? FileReplacer { get; set; }
        protected GeneralGeneratorConfig GeneralGeneratorConfig { get; set; } = default!;
        protected DirectoryReplacerConfig DirectoryReplacerConfig  { get; set; } = default!;
    }
}
