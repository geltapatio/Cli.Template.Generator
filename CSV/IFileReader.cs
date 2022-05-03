using System.Collections.Generic;

namespace Cli.Template.Generator.CSV
{
    public interface IFileReader
    {
        IList<T> GetLinesAs<T>(string? fullFilePath);
    }
}
