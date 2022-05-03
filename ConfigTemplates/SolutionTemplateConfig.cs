using Cli.Template.Generator.ConfigTemplates;

namespace Cli.Template.Generator.ConfigTemplates
{
    public class SolutionTemplateConfig
    {
        public JsonEntry? FullNamespaceProjectName { get; set; }

        public JsonEntry? DContextName { get; set; }

        public JsonEntry? AngularClientName { get; set; }

        public JsonEntry? SwaggerProjectName { get; set; }
    }
}
