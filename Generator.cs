
using Cli.Template.Generator.Commands;
using System;
using System.Collections.Generic;

namespace Cli.Template.Generator
{
    public class Generator
    {
        protected IList<ICommand> Commands { get; set; } = new List<ICommand>();
        
        public void SetCommand(ICommand command)
        {
            //check if the actions has already been added
            if (!Commands.Contains(command))
            {
                Commands.Add(command);
            }
        }

        public void ExecuteCommands()
        {
            foreach (var command in Commands)
            {
                try
                {
                    if (command.Activate)
                    {
                        Console.WriteLine($"Executing Command :: ({command.Name}) ");
                        command.Execute();
                        Console.WriteLine($"----------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine($"Command '{command.Name}' is deactivated.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Command '{command.Name}' could not be executed. Error: {ex}");
                }
            }
        }
    }
}
