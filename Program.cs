using Cli.Template.Generator.Commands;
using Cli.Template.Generator.ConfigFiles;
using Cli.Template.Generator.Handler;
using Cli.Template.Generator.Model;
using Cli.Template.Generator.SqlStatement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Cli.Template.Generator
{
    public static class Program
    {
        private static IConfigurationRoot? _configuration;
        private static GenerateSqlStatements? SqlStatementsConfig { get; } = new GenerateSqlStatements();
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .CreateLogger();
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ConfigurateSqlStatements();

           var generator = new Generator();
           var sqlStatement= CreateSqlStatement(0);

           var createCommand1 = new CreateSqlStatementsCommand<KompetenzUntergruppe>(sqlStatement)
            {
                Activate = true
            };
           sqlStatement = CreateSqlStatement(1);
            var createCommand2 = new CreateSqlStatementsCommand<Kompetenz>(sqlStatement)
           {
               Activate = true
           };
            generator.SetCommand(createCommand1);
            generator.SetCommand(createCommand2);
            generator.ExecuteCommands();
        }

        private static SqlInsertStatement CreateSqlStatement(int sqlStatementConfigIndex)
        {
            var sqlStatementConfig = SqlStatementsConfig!.SqlStatements[sqlStatementConfigIndex];
            var sqlStatement = new SqlInsertStatement
            {
                TableName = sqlStatementConfig.TableName,
                SourceFullFilePath = sqlStatementConfig.SourceFullFilePath,
                DatabaseName = SqlStatementsConfig.DatabaseName
            };
            sqlStatement.SetPlaceHolders(sqlStatementConfig.PlaceHolders);
            sqlStatement.SetFileHandler(new SqlFileHandler(sqlStatementConfig.TargetFullFilePath));
            return sqlStatement;
        }

        private static void ConfigurateSqlStatements()
        {
            if (_configuration == null)
            {
                return;
            }
            _configuration.GetSection("GenerateSQLStatements").Bind(SqlStatementsConfig);
            SqlStatementsConfig!.SqlStatements = _configuration.GetSection("GenerateSQLStatements:SqlStatements").Get<List<GenerateSqlStatement>>();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Add logging
            services.AddSingleton(LoggerFactory.Create(builder =>
            {
                builder
                    .AddSerilog(dispose: true);
            }));


            // configure logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            // Build configuration
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location))
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            // Add access to generic IConfigurationRoot
            services.AddSingleton(_configuration);
        }
    }
}
