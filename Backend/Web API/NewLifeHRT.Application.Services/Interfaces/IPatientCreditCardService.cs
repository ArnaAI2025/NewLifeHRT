using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Domain.Entities;

namespace NewLifeHRT.Application.Services.Interfaces
{
    public interface IPatientCreditCardService
    {        
        Task CreateCardsAsync(IEnumerable<PatientCreditCardDto> cards, Guid patientId, int? userId);
        Task UpdateCardsAsync(Guid patientId, IEnumerable<PatientCreditCardDto> incomingCards, IEnumerable<PatientCreditCard> existingCards, int? userId);
        Task<IEnumerable<PatientCreditCard>> GetByPatientIdAsync(Guid id);
    }
}
