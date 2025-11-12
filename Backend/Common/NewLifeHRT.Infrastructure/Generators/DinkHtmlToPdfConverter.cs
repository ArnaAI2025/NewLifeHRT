using DinkToPdf.Contracts;
using DinkToPdf;
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
using System.Collections;

namespace NewLifeHRT.Infrastructure.Generators
{
    public class DinkHtmlToPdfConverter : IPdfConverter
    {
        private readonly IConverter _converter;

        public DinkHtmlToPdfConverter(IConverter converter)
        {
            _converter = converter;
        }

        public string ConvertToPdf(string html)
        {

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10, Bottom = 10 }
            },
                Objects = {
                new ObjectSettings {
                    HtmlContent = html,
                    WebSettings = { DefaultEncoding = "utf-8", LoadImages = true, EnableIntelligentShrinking = true },
                    HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]" }
                }
                }
            };
            try
            {
                var byteArray = _converter.Convert(doc);
            return Convert.ToBase64String(byteArray);
            }
            catch (Exception ex) { 
                var message = ex.Message;   
                return message;
            }

        }

    }
}
