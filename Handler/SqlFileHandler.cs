namespace Cli.Template.Generator.Handler
{
    public class SqlFileHandler: FileHandler
    {
        public SqlFileHandler(string fullFilePath) : base(fullFilePath) { }

        public override void WriteNewline(string value)
        {
            streamWriter?.WriteLine(value);
        }
    }
}
