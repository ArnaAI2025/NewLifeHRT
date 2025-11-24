using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewLifeHRT.Api.Controllers;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Controllers
{
    public class MedicalRecommendationControllerTests
    {
        private static MedicalRecommendationController CreateController(Mock<IMedicalRecommendationService> recommendationService, Mock<IMedicationTypeService> medicationTypeService, Mock<IFollowUpLabTestService> followUpLabTestService, int? userId = null)
        {
            var controller = new MedicalRecommendationController(recommendationService.Object, medicationTypeService.Object, followUpLabTestService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = userId.HasValue
                            ? new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()) }))
                            : new ClaimsPrincipal(new ClaimsIdentity())
                    }
                }
            };

            return controller;
        }

        [Fact]
        public async Task GetById_Should_ReturnUnauthorized_When_UserMissing()
        {
            var controller = CreateController(new Mock<IMedicalRecommendationService>(), new Mock<IMedicationTypeService>(), new Mock<IFollowUpLabTestService>());

            var result = await controller.GetById(Guid.NewGuid());

            result.Result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task Update_Should_ReturnNotFound_When_ServiceReturnsEmptyMessage()
        {
            var recommendationService = new Mock<IMedicalRecommendationService>();
            recommendationService.Setup(s => s.UpdateAsync(It.IsAny<MedicalRecommendationRequestDto>(), It.IsAny<int>()))
                .ReturnsAsync(new CommonOperationResponseDto<Guid>());

            var controller = CreateController(recommendationService, new Mock<IMedicationTypeService>(), new Mock<IFollowUpLabTestService>(), 1);

            var result = await controller.Update(new MedicalRecommendationRequestDto());

            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}