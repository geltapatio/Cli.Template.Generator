using System.IO;

namespace Cli.Template.Generator.Handler
{
    public abstract class FileHandler: IFileHandler
    {
        private string? _fullFilePath;
        protected StreamWriter? streamWriter;

        protected FileHandler(string fullFilePath)
        {
            _fullFilePath = fullFilePath;
            if (File.Exists(fullFilePath))
            {
                File.Delete(_fullFilePath);
            }

            streamWriter = File.CreateText(_fullFilePath);
        }

        public void Close()
        {
            streamWriter?.Flush();
            streamWriter?.Close();
        }

        public abstract void WriteNewline(string value);
    }
}
