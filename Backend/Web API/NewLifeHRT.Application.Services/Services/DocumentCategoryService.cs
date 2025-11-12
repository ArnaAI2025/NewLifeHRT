using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class DocumentCategoryService : IDocumentCategoryService
    {
        private readonly IDocumentCategoryRepository _documentCategoryRepository;
        public DocumentCategoryService(IDocumentCategoryRepository documentCategoryRepository)
        {
            _documentCategoryRepository = documentCategoryRepository;
        }
        public async Task<List<CommonDropDownResponseDto<int>>> GetAllAsync()
        {
            var documentCategories = await _documentCategoryRepository.FindAsync(dc => dc.IsActive);
            return documentCategories.ToCommonDropDownResponsList();
        }
    }
}
