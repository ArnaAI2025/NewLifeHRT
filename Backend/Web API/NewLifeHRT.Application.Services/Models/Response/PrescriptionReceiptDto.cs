using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class PrescriptionReceiptDto
    {
        public string? RenderedHtml { get; set; }
        public string? PdfBase64 { get; set; }

        public PrescriptionReceiptDto()
        {
        }

        public PrescriptionReceiptDto(string? renderedHtml, string? pdfBase64)
        {
            RenderedHtml = renderedHtml;
            PdfBase64 = pdfBase64;
        }
    }

}
