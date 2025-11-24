using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewLifeHRT.API.Controllers.Controllers;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Controllers
{
    public class PatientAttachmentControllerTests
    {
        private static PatientAttachmentController CreateController(Mock<IPatientAttachmentService> serviceMock, int? userId = null)
        {
            var controller = new PatientAttachmentController(serviceMock.Object)
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
        public async Task HandleUploadAsync_Should_ReturnUnauthorized_When_UserMissing()
        {
            // Arrange
            var controller = CreateController(new Mock<IPatientAttachmentService>());

            // Act
            var result = await controller.HandleUploadAsync(new UploadFilesRequestDto());

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task HandleUploadAsync_Should_ReturnBadRequest_When_IdMissing()
        {
            // Arrange
            var controller = CreateController(new Mock<IPatientAttachmentService>(), 1);

            // Act
            var result = await controller.HandleUploadAsync(new UploadFilesRequestDto());

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeleteDocuments_Should_ReturnUnauthorized_When_UserMissing()
        {
            // Arrange
            var controller = CreateController(new Mock<IPatientAttachmentService>());

            // Act
            var result = await controller.DeleteDocuments(new BulkOperationRequestDto<Guid>());

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }
    }
}