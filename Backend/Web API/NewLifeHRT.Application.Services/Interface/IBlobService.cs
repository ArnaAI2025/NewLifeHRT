using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interface
{
    public interface IBlobService
    {
        Task<string> UploadFileAsync(IFormFile file, string blobPath);
        Task<string> UploadMediaAsync(byte[] mediaBytes, string blobPath, string contentType);

    }
}
