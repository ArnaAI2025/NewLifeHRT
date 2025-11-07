using NewLifeHRT.External.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NewLifeHRT.External.Models
{
    public class CommonResponseModel<T> 
    {
        public ResponseTypeEnum Type { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
