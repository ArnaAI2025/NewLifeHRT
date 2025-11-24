using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewLifeHRT.API.Controllers;
using NewLifeHRT.API.Controllers.Controllers;
using NewLifeHRT.Application.DTOs.Leads;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System.Security.Claims;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Controllers
{
    public class LeadControllerTests
    {
        private static LeadController CreateController(Mock<ILeadService> leadService, Mock<IPatientService> patientService, int? userId = null)
        {
            var controller = new LeadController(leadService.Object, patientService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = userId.HasValue
                            ? new ClaimsPrincipal(new ClaimsIdentity(new[]
                            {
                                new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString())
                            }))
                            : new ClaimsPrincipal(new ClaimsIdentity())
                    }
                }
            };

            return controller;
        }

        [Fact]
        public async Task GetById_Should_ReturnNotFound_When_ServiceReturnsNull()
        {
            var leadService = new Mock<ILeadService>();
            leadService.Setup(s => s.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((LeadResponseDto?)null);
            var controller = CreateController(leadService, new Mock<IPatientService>(), 1);

            var result = await controller.GetById(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_Should_ReturnUnauthorized_When_UserMissing()
        {
            var controller = CreateController(new Mock<ILeadService>(), new Mock<IPatientService>());

            var result = await controller.Create(new LeadRequestDto());

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task BulkAssignee_Should_ReturnBadRequest_When_IdsMissing()
        {
            var controller = CreateController(new Mock<ILeadService>(), new Mock<IPatientService>(), 1);

            var result = await controller.BulkAssignee(new BulkOperationAssigneeRequestDto<Guid, int> { Id = 1 });

            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}