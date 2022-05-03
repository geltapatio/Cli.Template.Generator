using Cli.Template.Generator.Handler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Cli.Template.Generator.SqlStatement
{
    public class SqlInsertStatement : ISqlStatement
    {
        private IDictionary<string, string>? _placeHolders;
        private IFileHandler? _fileHandler;
        private IList<string> _sqlInsertStatements;
        private List<string>_columnNames { get; }
        private List<string> _sqlVariables { get; }
        private string? _columnsAsStringCommaSeparator { get; set; }
        public string? SourceFullFilePath { get; set; }
        public string? DatabaseName { get; set; }
        public string? TableName { get; set; }

        public SqlInsertStatement()
        {
            _columnNames = new List<string>();
            _sqlVariables = new List<string>();
            _sqlInsertStatements = new List<string>();
        }

        public void Create<T>(IList<T>? items)
        {            
            if (items == null || !items.Any()) 
            {
                return;
            }
            SetColumns(items);
            CreateInsertStatement(items);
            SaveSqlStatement();
        }
        
        private void CreateInsertStatement<T>(IList<T>? items)
        {
            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                AddToSqlStatements(GetInsertStatement(GetValues(item)));
            }
        }

        private void AddToSqlStatements(string sqlInsertStatement)
        {
            if (!_sqlInsertStatements.Contains(sqlInsertStatement))
            {
                _sqlInsertStatements.Add(sqlInsertStatement);
            }
        }
        
        public void SetPlaceHolders(IDictionary<string, string> placeHoldersSettings)
        {
            _placeHolders = placeHoldersSettings;
        }

        public void SetFileHandler(IFileHandler fileHandler)
        {
            _fileHandler = fileHandler;
        }

        private void SaveSqlStatement()
        {
            try
            {
                WriteSqlVariables();
                WriteSqlUseDatabase();
                WriteSqlStatements();
            }
            finally
            {
                _fileHandler?.Close();
            }
        }

        private void WriteSqlUseDatabase()
        {
            if (_fileHandler == null)
            {
                return;
            }
            _fileHandler.WriteNewline(" ");
            _fileHandler.WriteNewline($"USE {DatabaseName}");
            _fileHandler.WriteNewline($"SET @DatabaseName = '{DatabaseName}' -- this variable is only used as information");
            _fileHandler.WriteNewline("PRINT N'Datenbank : ' + RTRIM(@DatabaseName)");
        }

        private void WriteSqlStatements()
        {
            if (_fileHandler == null)
            {
                return;
            }
            foreach (var sqlInsertStatement in _sqlInsertStatements)
            {
                _fileHandler.WriteNewline(sqlInsertStatement);
            }
        }

        private void WriteSqlVariables()
        {
            if (!_sqlVariables.Any())
            {
                return;
            }

            var counter = 0;
            var separator = ",";
            _fileHandler!.WriteNewline($"DECLARE @DatabaseName VARCHAR(50){separator}");
            foreach (var sqlVariable in _sqlVariables)
            {
                counter += 1;
                if (_sqlVariables.Count() == counter)
                {
                    separator = ";";
                }
                _fileHandler!.WriteNewline($"        @{sqlVariable} INT{separator}");
            }
            _fileHandler!.WriteNewline(" ");
        }

        private void SetColumns<T>(IList<T>? items)
        {
            if (items == null)
            {
                return;
            }
            var firstItem = items.First();
            if (firstItem == null)
            {
                return;
            }

            var properties = firstItem.GetType().GetProperties();
            foreach (var property in properties)
            {
                _columnNames.Add(property.Name);
            }
            _columnsAsStringCommaSeparator = string.Join(",", _columnNames.Select(n => n.ToString()).ToArray());
        }

        private string GetInsertStatement(string values) 
        {            
            return $"INSERT INTO {TableName} ({_columnsAsStringCommaSeparator}) VALUES({values})";
        }

        private string GetSetVariable(string variableName,string sqlStatement)
        {
            return $"SET @{variableName} = {sqlStatement}";
        }

        private string GetValues<T>(T item)
        {
            return item == null
                ? string.Empty
                : string.Join(",", item.GetType().GetProperties().Select(selector: prop => GetPropertyValue(item, prop)));
        }

        private void AddComment(string value)
        {
            _sqlInsertStatements.Add(" ");
            _sqlInsertStatements.Add($"-- {value}");
        }

        private void AddSqlPrint(string value)
        {
            _sqlInsertStatements.Add($"PRINT N'{value}'");
        }
        private string GetPropertyValue<T>(T item, PropertyInfo propertyInfo)
        {
            var propertyName = propertyInfo.Name;
            var hasValue = _placeHolders!.TryGetValue(propertyName, out var placeHolderValue);
            var propertyValue = $" N'{propertyInfo.GetValue(item)?.ToString()?.Replace("'", "`")}'";
            if (!hasValue || placeHolderValue == null)
            {
                return propertyValue;
            }

            if (!placeHolderValue.Contains("AsSqlVariable"))
            {
                return propertyValue;
            }
            
            var sqlVariable = $"{propertyInfo.GetValue(item)!}";
            var sanitizeVariable = SanitizeVariableName(sqlVariable);
            if (_sqlVariables.Contains(sanitizeVariable))
            {
                return $" @{sanitizeVariable}";
            }

            _sqlVariables.Add(sanitizeVariable);
            hasValue = _placeHolders!.TryGetValue($"Sql{propertyName}", out var sqlValue);
            if (!hasValue || sqlValue == null)
            {
                return propertyValue;
            }

            var value = $"{propertyInfo.GetValue(item)!}";
            var sqlStatement = string.Format(sqlValue, $"'{value}'");
            AddComment(value);
            AddSqlPrint($"INSERT Statement(s) for ({value})");
            _sqlInsertStatements.Add(GetSetVariable(sanitizeVariable, sqlStatement));

            return $" @{sanitizeVariable}";
        }

        private string SanitizeVariableName(string value)
        {
            return Regex.Replace(value, @"[^a-zA-Z]", string.Empty);
        }
    }
}
