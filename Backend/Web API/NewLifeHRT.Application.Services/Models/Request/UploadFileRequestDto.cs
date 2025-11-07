using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class UploadFilesRequestDto
    {
        public string Id { get; set; } = string.Empty;
        public List<UploadFileRequestDto> UploadFileItemDto { get; set; } = new();
    }

    public class UploadFileRequestDto
    {
        public IFormFile? File { get; set; }
        public string? DocumentCategory { get; set; }
        public int? DocumentCategoryId { get; set; }
    }

}
