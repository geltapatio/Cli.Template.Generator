using System.Collections.Generic;

namespace Cli.Template.Generator.ConfigFiles
{
    public class GenerateSqlStatement
    {
        public string SourceFullFilePath { get; set; } = null!;
        public string TargetFullFilePath { get; set; } = null!;
        public string TableName { get; set; } = null!;

        public Dictionary<string, string> PlaceHolders { get; set; } = new Dictionary<string, string>();
    }
}
