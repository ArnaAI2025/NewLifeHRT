using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interfaces
{
    public interface IPharmacyService
    {   
        Task<List<PharmacyGetAllResponseDto>> GetAllPharmaciesAsync();
        Task<PharmacyGetResponseDto?> GetPharmacyByIdAsync(Guid id);
        Task<CreatePharmacyResponseDto> CreatePharmacyAsync(PharmacyCreateRequestDto request, int userId);
        Task<CreatePharmacyResponseDto> UpdatePharmacyAsync(Guid id, PharmacyCreateRequestDto request, int userId);
        Task DeletePharmaciesAsync(List<Guid> pharmacyIds, int userId);
        Task ActivatePharmaciesAsync(List<Guid> pharmacyIds, int userId);
        Task DeactivatePharmaciesAsync(List<Guid> pharmacyIds, int userId);
        Task<List<PharmaciesDropdownResponseDto>> GetAllPharmaciesForDropdownAsync();
        Task<List<PharmaciesDropdownResponseDto>> GetAllActivePharmaciesForDropdownAsync();
    }
}
