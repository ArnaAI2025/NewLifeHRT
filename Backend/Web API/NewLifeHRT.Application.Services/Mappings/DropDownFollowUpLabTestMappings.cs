using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class DropDownFollowUpLabTestMappings
    {
        public static DropDownIntResponseDto ToDropDownIntResponseDto(this FollowUpLabTest followUpLabTest)
        {
            return new DropDownIntResponseDto
            {
                Id = followUpLabTest.Id,
                Value = followUpLabTest.Duration,
            };
        }
        public static List<DropDownIntResponseDto> ToDropDownIntResponseDtoList(this IEnumerable<FollowUpLabTest> followUpLabTests)
        {
            return followUpLabTests.Select(followUpLabTests => followUpLabTests.ToDropDownIntResponseDto()).ToList();
        }
    }
}
