using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewLifeHRT.API.Controllers.Controllers;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Controllers
{
    public class PharmacyControllerTests
    {
        private readonly Mock<IPharmacyService> _pharmacyServiceMock = new();
        private readonly Mock<ICurrencyService> _currencyServiceMock = new();
        private readonly Mock<IShippingMethodService> _shippingMethodServiceMock = new();
        private readonly Mock<IPharmacyShippingMethodService> _pharmacyShippingMethodServiceMock = new();

        [Fact]
        public async Task CreatePharmacy_Should_ReturnUnauthorized_When_UserMissing()
        {
            var controller = new PharmacyController(_pharmacyServiceMock.Object, _currencyServiceMock.Object, _shippingMethodServiceMock.Object, _pharmacyShippingMethodServiceMock.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            var result = await controller.CreatePharmacy(new PharmacyCreateRequestDto());

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task GetPharmacyById_Should_ReturnNotFound_When_ServiceReturnsNull()
        {
            var id = Guid.NewGuid();
            _pharmacyServiceMock.Setup(s => s.GetPharmacyByIdAsync(id)).ReturnsAsync((Application.Services.Models.Response.PharmacyGetResponseDto?)null);

            var controller = new PharmacyController(_pharmacyServiceMock.Object, _currencyServiceMock.Object, _shippingMethodServiceMock.Object, _pharmacyShippingMethodServiceMock.Object);

            var result = await controller.GetPharmacyById(id);

            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
