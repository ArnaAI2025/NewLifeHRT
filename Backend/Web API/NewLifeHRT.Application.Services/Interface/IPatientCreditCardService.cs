using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interface
{
    public interface IPatientCreditCardService
    {        
        Task CreateCardsAsync(IEnumerable<PatientCreditCardDto> cards, Guid patientId, int? userId);
        Task UpdateCardsAsync(Guid patientId, IEnumerable<PatientCreditCardDto> incomingCards, IEnumerable<PatientCreditCard> existingCards, int? userId);
        Task<IEnumerable<PatientCreditCard>> GetByPatientIdAsync(Guid id);
    }
}
