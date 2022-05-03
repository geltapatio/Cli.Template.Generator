
using System.Collections.Generic;

namespace Cli.Template.Generator.ConfigFiles
{
    public class DirectoryReplacerConfig
    {
        public IList<string> ExcludedDirectories { get; set; } = new List<string>();

        public string? VsSolutionName { get; set; }

        public bool IsSolutionInRoot { get; set; }

        public bool ClearTarget { get; set; }
    }
}
