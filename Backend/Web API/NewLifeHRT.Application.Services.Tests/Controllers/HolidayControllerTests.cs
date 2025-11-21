using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewLifeHRT.API.Controllers.Controllers;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Controllers
{
    public class HolidayControllerTests
    {
        private readonly Mock<IHolidayService> _holidayServiceMock = new();

        [Fact]
        public async Task CreateHoliday_Should_ReturnBadRequest_When_RequestNull()
        {
            var controller = new HolidayController(_holidayServiceMock.Object);

            var result = await controller.CreateHoliday(null!);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CreateHoliday_Should_ReturnOk_When_UserAuthenticated()
        {
            var request = new CreateHolidayRequestDto();
            var expected = new CommonOperationResponseDto<Guid> { Id = Guid.NewGuid(), Message = "created" };
            _holidayServiceMock.Setup(s => s.CreateHolidayAsync(request, 2)).ReturnsAsync(expected);

            var controller = new HolidayController(_holidayServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "2") }))
                    }
                }
            };

            var result = await controller.CreateHoliday(request);

            var ok = Assert.IsType<OkObjectResult>(result);
            ok.Value.Should().Be(expected);
        }

        [Fact]
        public async Task GetAllHolidays_Should_ReturnBadRequest_When_RequestNull()
        {
            var controller = new HolidayController(_holidayServiceMock.Object);

            var result = await controller.GetAllHolidays(null!);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
