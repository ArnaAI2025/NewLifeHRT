using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Enums;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class PatientCreditCardService : IPatientCreditCardService
    {
        private readonly IPatientCreditCardRepository _patientCreditCardRepository;
        public PatientCreditCardService(IPatientCreditCardRepository patientCreditCardRepository)
        {
            _patientCreditCardRepository = patientCreditCardRepository;
        }
        public async Task CreateCardsAsync(IEnumerable<PatientCreditCardDto> cards, Guid patientId, int? userId)
        {
            var existingDefault = await _patientCreditCardRepository
                .GetSingleAsync(x => x.PatientId == patientId && x.IsDefaultCreditCard == true && x.IsActive);

            bool hasDefaultAlready = existingDefault != null;

            bool isFirstCard = !hasDefaultAlready;
            foreach (var dto in cards)
            {
                var entity = new PatientCreditCard
                {
                    Id = Guid.NewGuid(),
                    PatientId = patientId,
                    CardNumber = dto.CardNumber ?? string.Empty,
                    CardType = (CreditCardTypeEnum)dto.CardType,
                    Month = (MonthEnum)dto.Month,
                    Year = dto.Year,
                    IsActive = true,
                    IsDefaultCreditCard = isFirstCard,
                    CreatedBy = userId?.ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                await _patientCreditCardRepository.AddAsync(entity);
                isFirstCard = false;
            }
        }
        public async Task UpdateCardsAsync(Guid patientId, IEnumerable<PatientCreditCardDto> incomingCards, IEnumerable<PatientCreditCard> existingCards, int? userId)
        {
            var utcNow = DateTime.UtcNow;

            var incomingCardIds = incomingCards
                .Where(c => c.Id.HasValue && c.Id != Guid.Empty)
                .Select(c => c.Id.Value)
                .ToHashSet();

            var updatedEntities = new List<PatientCreditCard>();
            bool defaultCardRemoved = false;

            foreach (var dbCard in existingCards)
            {
                if (incomingCardIds.Contains(dbCard.Id))
                {
                    var updatedCard = incomingCards.First(x => x.Id == dbCard.Id);
                    dbCard.CardNumber = updatedCard.CardNumber ?? string.Empty;
                    dbCard.CardType = (CreditCardTypeEnum)updatedCard.CardType;
                    dbCard.Month = (MonthEnum)updatedCard.Month;
                    dbCard.Year = updatedCard.Year;
                    dbCard.IsActive = true;
                    dbCard.UpdatedBy = userId?.ToString();
                    dbCard.UpdatedAt = utcNow;
                }
                else
                {
                    if (dbCard.IsDefaultCreditCard == true)
                        defaultCardRemoved = true;

                    dbCard.IsActive = false;
                    dbCard.UpdatedBy = userId?.ToString();
                    dbCard.UpdatedAt = utcNow;
                    dbCard.IsDefaultCreditCard = false;
                }

                updatedEntities.Add(dbCard);
            }

            if (updatedEntities.Any())
            {
                await _patientCreditCardRepository.BulkUpdateAsync(updatedEntities);
            }

            bool hasDefaultAlready = existingCards.Any(x => x.IsActive && x.IsDefaultCreditCard == true);

            bool isFirstNewCard = !hasDefaultAlready;
            var newCards = new List<PatientCreditCard>();

            foreach (var card in incomingCards.Where(x => !x.Id.HasValue || x.Id == Guid.Empty))
            {
                var entity = new PatientCreditCard
                {
                    Id = Guid.NewGuid(),
                    PatientId = patientId,
                    CardNumber = card.CardNumber ?? string.Empty,
                    CardType = (CreditCardTypeEnum)card.CardType,
                    Month = (MonthEnum)card.Month,
                    Year = card.Year,
                    IsActive = true,
                    IsDefaultCreditCard = isFirstNewCard,
                    CreatedBy = userId?.ToString(),
                    CreatedAt = utcNow
                };

                newCards.Add(entity);
                isFirstNewCard = false;
            }

            if (newCards.Any())
                await _patientCreditCardRepository.AddRangeAsync(newCards);

            var allActiveCards = existingCards.Concat(newCards).Where(x => x.IsActive).ToList();
            if ((defaultCardRemoved || !hasDefaultAlready) && !allActiveCards.Any(x => x.IsDefaultCreditCard == true) && allActiveCards.Any())
            {
                var latestCard = allActiveCards.OrderByDescending(x => x.CreatedAt).First();
                latestCard.IsDefaultCreditCard = true;

                if (existingCards.Contains(latestCard))
                    await _patientCreditCardRepository.BulkUpdateAsync(new List<PatientCreditCard> { latestCard });
                else
                    await _patientCreditCardRepository.SaveChangesAsync();
            }

            if (newCards.Any() || updatedEntities.Any())
                await _patientCreditCardRepository.SaveChangesAsync();
        }
        public async Task<IEnumerable<PatientCreditCard>> GetByPatientIdAsync(Guid id)
        {
            var creditCards = await _patientCreditCardRepository.FindAsync(p => p.PatientId == id && p.IsActive);
            return creditCards;

        }

    }
}
