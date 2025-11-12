using Microsoft.Extensions.Options;
using NewLifeHRT.Infrastructure.Generators.Interfaces;
using NewLifeHRT.Infrastructure.Models.Templates;
using NewLifeHRT.Infrastructure.Settings;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.Generators
{
    public class TemplateContentGenerator : ITemplateContentGenerator
    {
        private readonly TemplateSettings _templateSettings;
        public TemplateContentGenerator(IOptions<TemplateSettings> templateSettingsOptions)
        {
            _templateSettings = templateSettingsOptions.Value;
        }

        public string GetTemplateContent(TemplateBaseModel model)
        {
            var exeDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var fullPath = Path.Combine(exeDir, _templateSettings.Path, model.TemplatePath);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Template not found at: {fullPath}");

            string t = File.ReadAllText(fullPath);

            string templateName = _templateSettings.ShouldCache ? model.TemplatePath : Guid.NewGuid().ToString();

            try
            {
                if (Engine.Razor.IsTemplateCached(templateName, model.GetType()))
                    return Engine.Razor.Run(templateName, model.GetType(), model);
                else
                    return Engine.Razor.RunCompile(t, templateName, model.GetType(), model);
            }
            catch
            {
                return Engine.Razor.RunCompile(t, templateName, model.GetType(), model);
            }
        }

    }
}
