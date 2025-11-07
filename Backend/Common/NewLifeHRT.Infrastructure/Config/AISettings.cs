using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.Config
{
    public class AISettings
    {
        public string ApiKey { get; set; }
        public string BaseUrl { get; set; }
        public string PromptDirectory { get; set; } = "Llm/Prompts";
    }
}
