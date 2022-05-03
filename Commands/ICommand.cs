using Microsoft.Extensions.Configuration;

namespace Cli.Template.Generator.Commands
{
    public interface ICommand
    {
        string Name { get; }
        bool Activate { get; set; }
        void Execute();
    }
}
