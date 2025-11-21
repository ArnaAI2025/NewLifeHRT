using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewLifeHRT.API.Controllers.Controllers;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Response;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Controllers
{
    public class TimezoneControllerTests
    {
        [Fact]
        public async Task GetAll_Should_ReturnOk_WithTimezones()
        {
            var timezones = new List<TimezoneResponseDto> { new(), new() };
            var timezoneServiceMock = new Mock<ITimezoneService>();
            timezoneServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(timezones);

            var controller = new TimezoneController(timezoneServiceMock.Object);

            var result = await controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            okResult.Value.Should().BeEquivalentTo(timezones);
        }
    }
}
