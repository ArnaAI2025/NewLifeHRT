using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.API.Controllers.Controllers;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Controllers
{
    public class PatientControllerTests
    {
        private static PatientController CreateController(
            Mock<IPatientService> patientService,
            Mock<IVisitTypeService> visitTypeService,
            Mock<IAgendaService> agendaService,
            Mock<IDocumentCategoryService> documentCategoryService,
            Mock<IPatientCreditCardService> patientCreditCardService,
            int? userId = null)
        {
            var controller = new PatientController(
                patientService.Object,
                visitTypeService.Object,
                agendaService.Object,
                documentCategoryService.Object,
                patientCreditCardService.Object)
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
        public async Task Create_Should_ReturnUnauthorized_When_UserMissing()
        {
            // Arrange
            var controller = CreateController(new Mock<IPatientService>(), new Mock<IVisitTypeService>(), new Mock<IAgendaService>(), new Mock<IDocumentCategoryService>(), new Mock<IPatientCreditCardService>());

            // Act
            var result = await controller.Create(new CreatePatientRequestDto());

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task DeletePatients_Should_ReturnBadRequest_When_IdsMissing()
        {
            // Arrange
            var controller = CreateController(new Mock<IPatientService>(), new Mock<IVisitTypeService>(), new Mock<IAgendaService>(), new Mock<IDocumentCategoryService>(), new Mock<IPatientCreditCardService>(), 1);

            // Act
            var result = await controller.DeletePatients(new BulkOperationRequestDto<Guid>());

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Activate_Should_ReturnUnauthorized_When_UserMissing()
        {
            // Arrange
            var controller = CreateController(new Mock<IPatientService>(), new Mock<IVisitTypeService>(), new Mock<IAgendaService>(), new Mock<IDocumentCategoryService>(), new Mock<IPatientCreditCardService>());

            // Act
            var result = await controller.Activate(Guid.NewGuid());

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }
    }
}