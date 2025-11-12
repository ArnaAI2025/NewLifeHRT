using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class CurrencyMappings
    {
        public static CurrencyResponseDto ToCurrencyResponseDto(this Currency currency)
        {
            return new CurrencyResponseDto
            {
                Id = currency.Id,
                CurrencyName = currency.CurrencyName
            };
        }

        public static List<CurrencyResponseDto> ToCurrencyResponseDtoList(this IEnumerable<Currency> currency)
        {
            return currency.Select(currency => currency.ToCurrencyResponseDto()).ToList();
        }
    }
}
