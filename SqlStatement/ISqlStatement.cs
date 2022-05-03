using Cli.Template.Generator.Handler;
using System.Collections.Generic;

namespace Cli.Template.Generator.SqlStatement
{
    public interface ISqlStatement
    {
        string? SourceFullFilePath { get; set; }
        string? DatabaseName { get; set; }
        string? TableName { get; set; }
        void Create<T>(IList<T>? items);

        void SetPlaceHolders(IDictionary<string, string> placeHolders);

        void SetFileHandler(IFileHandler fileHandler);
    }
}
