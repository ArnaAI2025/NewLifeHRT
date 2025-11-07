using Microsoft.Extensions.Options;
using NewLifeHRT.Domain.Models;
using NewLifeHRT.Infrastructure.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.Llm
{
    public class PromptStore
    {
        private readonly string _promptDir;

        public PromptStore(IOptions<AISettings> options)
        {
            _promptDir = Path.Combine(AppContext.BaseDirectory, options.Value.PromptDirectory);
        }

        public string Render(string promptName, RefillInput input)
        {
            var path = Path.Combine(_promptDir, $"{promptName}.txt");
            var template = File.ReadAllText(path);

            return template
                .Replace("{{product_name}}", input.ProductName)
                .Replace("{{protocol}}", input.Protocol)
                .Replace("{{quantity}}", input.Quantity.ToString())
                .Replace("{{start_date}}", input.StartDate.ToString("yyyy-MM-dd"))
                .Replace("{{input_json}}", JsonSerializer.Serialize(input));
        }
    }
}
