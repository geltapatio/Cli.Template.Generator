using Cli.Template.Generator.CSV;
using Cli.Template.Generator.SqlStatement;

namespace Cli.Template.Generator.Commands
{
    public class CreateSqlStatementsCommand<T>: ICommand
    {
        public string Name => "Create a SQL INSERT Statement";
        public bool Activate { get; set; }
        

        private ISqlStatement _sqlStatement;
        private IFileReader reader;
        public CreateSqlStatementsCommand(ISqlStatement sqlStatement)
        {
            reader = new CsvFileReader();
            _sqlStatement = sqlStatement;
        }

        public void Execute()
        {
            var rows = reader.GetLinesAs<T>(_sqlStatement.SourceFullFilePath);
            _sqlStatement.Create<T>(rows);
        }
    }
}
