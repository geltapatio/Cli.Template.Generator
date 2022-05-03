using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Cli.Template.Generator.CSV
{
    /// <remarks>
    /// https://joshclose.github.io/CsvHelper/getting-started/
    /// </remarks>
    public class CsvFileReader: IFileReader
    {
        public IList<T> GetLinesAs<T>(string? fullFilePath) 
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header,
                Delimiter = ";"
            };
            using var reader = new StreamReader(fullFilePath!);
            using var csv = new CsvReader(reader, config);
            return csv.GetRecords<T>().ToList();
        }
    }
}
