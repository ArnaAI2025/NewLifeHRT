using DinkToPdf.Contracts;
using DinkToPdf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewLifeHRT.Infrastructure.Constants;
using NewLifeHRT.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewLifeHRT.Infrastructure.Generators.Interfaces;
using NewLifeHRT.Infrastructure.Generators;

namespace NewLifeHRT.Infrastructure.StartupSection
{
    public static class TemplateInitializers
    {
        public static IServiceCollection RegisterTemplateGeneration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITemplateContentGenerator, TemplateContentGenerator>();
            services.Configure<TemplateSettings>(configuration.GetSection(AppSettingKeys.TemplateSettings));

            var assembly = typeof(TemplateInitializers).Assembly;
            var path = Path.GetDirectoryName(assembly.Location);


            var possiblePaths = new[]
            {
                Path.Combine(path, "libs", "libwkhtmltox.dll"),
            };

            string? libPath = possiblePaths.FirstOrDefault(File.Exists);

            if (libPath == null)
            {
                throw new FileNotFoundException("Unable to locate 'libwkhtmltox.dll'. Please ensure it exists under the 'libs' directory or build output path.");
            }

            var context = new CustomAssemblyLoadContext();
            context.LoadUnmanagedLibrary(libPath);           
            var converter = new SynchronizedConverter(new PdfTools());
            services.AddSingleton<IConverter>(converter);
            services.AddSingleton<IPdfConverter,DinkHtmlToPdfConverter>();
            return services;
        }
    }
}
