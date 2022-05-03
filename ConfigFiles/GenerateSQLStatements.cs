
using System.Collections.Generic;

namespace Cli.Template.Generator.ConfigFiles
{
    public class GenerateSqlStatements 
    {
        public string? DatabaseName { get; set; } 
        public IList<GenerateSqlStatement> SqlStatements = new List<GenerateSqlStatement>();
    }
}
