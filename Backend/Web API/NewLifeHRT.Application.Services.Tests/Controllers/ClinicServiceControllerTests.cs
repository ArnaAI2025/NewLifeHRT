using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewLifeHRT.API.Controllers.Controllers;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Response;
using System.Collections.Generic;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Controllers
{
    public class ClinicServiceControllerTests
    {
        [Fact]
        public async Task GetAllServiceByType_Should_ReturnOk_WithServices()
        {
            var services = new List<ClinicServiceResponseDto> { new(), new() };
            var clinicServiceMock = new Mock<IClinicServiceService>();
            clinicServiceMock.Setup(s => s.GetAllServiceByTypeAsync("Telehealth"))
                .ReturnsAsync(services);

            var controller = new ClinicServiceController(clinicServiceMock.Object);

            var result = await controller.GetAllServiceByType("Telehealth");

            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(services);
        }

        [Fact]
        public async Task GetAllAppointmentServices_Should_ReturnOk_WithAppointmentServices()
        {
            var appointmentServices = new List<AppointmentServiceResponseDto> { new(), new() };
            var clinicServiceMock = new Mock<IClinicServiceService>();
            clinicServiceMock.Setup(s => s.GetAllAppointmentServicesAsync())
                .ReturnsAsync(appointmentServices);

            var controller = new ClinicServiceController(clinicServiceMock.Object);

            var result = await controller.GetAllAppointmentServices();

            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(appointmentServices);
        }
    }
}