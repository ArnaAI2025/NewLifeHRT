using NewLifeHRT.Infrastructure.Models.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.Generators.Interfaces
{
    public interface ITemplateContentGenerator
    {
        public string GetTemplateContent(TemplateBaseModel model);
    }
}
