namespace Cli.Template.Generator.Handler
{
    public interface IFileHandler
    {
        void WriteNewline(string value);
        void Close();
    }
}
