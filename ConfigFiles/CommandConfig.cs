using System.Collections.Generic;

namespace Cli.Template.Generator.ConfigFiles
{
    public abstract class CommandConfig
    {
        public Dictionary<string, string> PlaceHolders { get; set; } = new Dictionary<string, string>();
    }
}
