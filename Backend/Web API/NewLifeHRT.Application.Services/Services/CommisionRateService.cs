using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class CommisionRateService : ICommisionRateService
    {
        private readonly ICommisionRateRepository _commisionRateRepository;

        public CommisionRateService(ICommisionRateRepository commisionRateRepository)
        {
            _commisionRateRepository = commisionRateRepository;
        }

        public async Task ActivateCommisionRateAsync(List<Guid> ids, int userId)
        {
            var commisionRates = (await _commisionRateRepository.FindAsync(p => ids.Contains(p.Id))).ToList();

            if (commisionRates == null || !commisionRates.Any())
                throw new Exception("No matching commision rates found for the provided IDs.");

            foreach (var commisionRate in commisionRates)
            {
                commisionRate.IsActive = true;
                commisionRate.UpdatedBy = userId.ToString();
                commisionRate.UpdatedAt = DateTime.UtcNow;
            }

            await _commisionRateRepository.BulkUpdateAsync(commisionRates);
        }

        public async Task<CommonOperationResponseDto<Guid>> CreateCommisionRateAsync(CommisionRateRequestDto dto, int userId)
        {
            var commisionRate = new CommisionRate(dto.ProductId, dto.FromAmount, dto.ToAmount, dto.RatePercentage, userId.ToString(), DateTime.UtcNow);
            await _commisionRateRepository.AddAsync(commisionRate);
            return new CommonOperationResponseDto<Guid> { Id = commisionRate.Id, Message = "Commision Rate Created Successfully" };
        }

        public async Task DeactivateCommisionRateAsync(List<Guid> ids, int userId)
        {
            var commisionRates = (await _commisionRateRepository.FindAsync(p => ids.Contains(p.Id))).ToList();

            if (commisionRates == null || !commisionRates.Any())
                throw new Exception("No matching commision rates found for the provided IDs.");

            foreach (var commisionRate in commisionRates)
            {
                commisionRate.IsActive = false;
                commisionRate.UpdatedBy = userId.ToString();
                commisionRate.UpdatedAt = DateTime.UtcNow;
            }

            await _commisionRateRepository.BulkUpdateAsync(commisionRates);
        }

        public async Task DeleteCommisionRateAsync(List<Guid> ids, int userId)
        {
            var commisionRates = (await _commisionRateRepository.FindAsync(p => ids.Contains(p.Id))).ToList();

            if (commisionRates == null || !commisionRates.Any())
                throw new Exception("No matching commision rates found for the provided IDs.");

            await _commisionRateRepository.RemoveRangeAsync(commisionRates);
        }

        public async Task<List<CommisionRateGetAllResponseDto>> GetAllCommisionRatesAsync()
        {
            var includes = new[] { "Product" };
            var commisionRates = await _commisionRateRepository.AllWithIncludeAsync(includes);
            return commisionRates.ToCommisionRateGetAllResponseDtoList();
        }

        public async Task<List<CommisionRateByProductIdResponseDto>> GetCommisionRateByProductIdAsync(Guid productId)
        {
            var commisionRates = await _commisionRateRepository.FindAsync(c => c.ProductId == productId);
            return commisionRates.ToCommisionRateByProductIdResponseDtoList();
        }

        public async Task<CommisionRateGetByIdResponseDto> GetCommisionRateByIdAsync(Guid id)
        {
            var includes = new[] { "Product" };
            var commisionRate = await _commisionRateRepository.GetWithIncludeAsync(id, includes);
            return commisionRate.ToCommisionRateGetByIdResponseDto();
        }

        public async Task<CommonOperationResponseDto<Guid>> UpdateCommisionRateAsync(Guid id, CommisionRateRequestDto dto, int userId)
        {
            var existingCommisionRate = await _commisionRateRepository.GetByIdAsync(id);
            if (existingCommisionRate == null)
            {
                throw new Exception("Commision rate not found");
            }
            existingCommisionRate.ProductId = dto.ProductId;
            existingCommisionRate.FromAmount = dto.FromAmount;
            existingCommisionRate.ToAmount = dto.ToAmount;
            existingCommisionRate.RatePercentage = dto.RatePercentage;
            existingCommisionRate.UpdatedBy = userId.ToString();
            existingCommisionRate.UpdatedAt = DateTime.UtcNow;

            await _commisionRateRepository.UpdateAsync(existingCommisionRate);
            return new CommonOperationResponseDto<Guid> { Id = existingCommisionRate.Id, Message = "Commision Rate Updated Successfully" };
        }
    }
}