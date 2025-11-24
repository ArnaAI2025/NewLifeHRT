using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Infrastructure.Generators.Interfaces;
using NewLifeHRT.Infrastructure.Settings;
using NewLifeHRT.Tests.Common.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class OrderServiceBuilder : ServiceBuilder<OrderService>
    {
        public Mock<IOrderRepository> OrderRepositoryMock { get; private set; } = new();
        public Mock<IOrderDetailService> OrderDetailServiceMock { get; private set; } = new();
        public Mock<ICommissionsPayableService> CommissionsPayableServiceMock { get; private set; } = new();
        public Mock<IPoolService> PoolServiceMock { get; private set; } = new();
        public Mock<ITemplateContentGenerator> TemplateContentGeneratorMock { get; private set; } = new();
        public Mock<IPdfConverter> PdfConverterMock { get; private set; } = new();
        public Mock<IPatientCreditCardRepository> PatientCreditCardRepositoryMock { get; private set; } = new();
        public Mock<ICourierServiceRepository> CourierServiceRepositoryMock { get; private set; } = new();
        public IOptions<ClinicInformationSettings> ClinicInformationOptions { get; private set; } = Options.Create(new ClinicInformationSettings());

        public override OrderService Build()
        {
            return new OrderService(
                OrderRepositoryMock.Object,
                OrderDetailServiceMock.Object,
                ShippingAddressServiceMock.Object,
                PharmacyShippingMethodServiceMock.Object,
                CommissionsPayableServiceMock.Object,
                PoolServiceMock.Object,
                PatientCreditCardRepositoryMock.Object,
                ClinicDbContextMock.Object,
                TemplateContentGeneratorMock.Object,
                PdfConverterMock.Object,
                AzureBlobStorageOptions,
                ClinicInformationOptions,
                CourierServiceRepositoryMock.Object);
        }

        public OrderServiceBuilder SetParameter(Mock<IOrderRepository> repository)
        {
            OrderRepositoryMock = repository;
            return this;
        }

        public OrderServiceBuilder SetParameter(Mock<IOrderDetailService> orderDetailService)
        {
            OrderDetailServiceMock = orderDetailService;
            return this;
        }

        public OrderServiceBuilder SetParameter(Mock<IPharmacyShippingMethodService> pharmacyShippingMethodService)
        {
            PharmacyShippingMethodServiceMock = pharmacyShippingMethodService;
            return this;
        }

        public OrderServiceBuilder SetParameter(Mock<ICommissionsPayableService> commissionsPayableService)
        {
            CommissionsPayableServiceMock = commissionsPayableService;
            return this;
        }

        public OrderServiceBuilder SetParameter(Mock<IPoolService> poolService)
        {
            PoolServiceMock = poolService;
            return this;
        }

        public OrderServiceBuilder SetParameter(Mock<IPatientCreditCardRepository> patientCreditCardRepository)
        {
            PatientCreditCardRepositoryMock = patientCreditCardRepository;
            return this;
        }

        public OrderServiceBuilder SetParameter(Mock<ITemplateContentGenerator> templateContentGenerator)
        {
            TemplateContentGeneratorMock = templateContentGenerator;
            return this;
        }

        public OrderServiceBuilder SetParameter(Mock<IPdfConverter> pdfConverter)
        {
            PdfConverterMock = pdfConverter;
            return this;
        }

        public OrderServiceBuilder SetParameter(IOptions<ClinicInformationSettings> clinicInformationOptions)
        {
            ClinicInformationOptions = clinicInformationOptions;
            return this;
        }

        public OrderServiceBuilder SetParameter(Mock<ICourierServiceRepository> courierServiceRepository)
        {
            CourierServiceRepositoryMock = courierServiceRepository;
            return this;
        }
    }

    public class OrderServiceTests
    {
        [Fact]
        public async Task CreateOrderAsync_Should_SetDeliveryChargeOverride_When_DeliveryDiffersFromOriginalPrice()
        {
            // Arrange
            var orderRepositoryMock = new Mock<IOrderRepository>();
            Order? capturedOrder = null;
            var generatedOrderId = Guid.NewGuid();
            orderRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Order>()))
                .ReturnsAsync((Order o) =>
                {
                    capturedOrder = o;
                    o.Id = generatedOrderId;
                    return o;
                });
            orderRepositoryMock.Setup(r => r.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>()))
                .ReturnsAsync(false);

            var pharmacyShippingMethodServiceMock = new Mock<IPharmacyShippingMethodService>();
            pharmacyShippingMethodServiceMock.Setup(s => s.GetShippingMethodPriceAsync(It.IsAny<Guid>()))
                .ReturnsAsync(10m);

            var shippingAddressServiceMock = new Mock<IShippingAddressService>();
            shippingAddressServiceMock.Setup(s => s.SetDefaultAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<int>()))
                .ReturnsAsync(new CommonOperationResponseDto<Guid> { Id = Guid.NewGuid() });

            var request = new OrderRequestDto
            {
                Name = "Order",
                PatientId = Guid.NewGuid(),
                PharmacyId = Guid.NewGuid(),
                CounselorId = 1,
                DeliveryCharge = 20m,
                PharmacyShippingMethodId = Guid.NewGuid(),
                ShippingAddressId = Guid.NewGuid(),
                Status = 1
            };

            var orderService = new OrderServiceBuilder()
                .SetParameter(orderRepositoryMock)
                .SetParameter(pharmacyShippingMethodServiceMock)
                .SetParameter(shippingAddressServiceMock)
                .Build();

            // Act
            var response = await orderService.CreateOrderAsync(request, 5);

            // Assert
            response.Id.Should().Be(generatedOrderId);
            response.Message.Should().Be("Order successfully created");
            capturedOrder.Should().NotBeNull();
            capturedOrder!.IsDeliveryChargeOverRidden.Should().BeTrue();
            pharmacyShippingMethodServiceMock.Verify(s => s.GetShippingMethodPriceAsync(request.PharmacyShippingMethodId.Value), Times.Once);
            shippingAddressServiceMock.Verify(s => s.SetDefaultAsync(request.PatientId, request.ShippingAddressId.Value, 5), Times.Once);
        }

        [Fact]
        public async Task GetOrderByIdAsync_Should_ReturnNull_When_OrderNotFound()
        {
            // Arrange
            var orderRepositoryMock = new Mock<IOrderRepository>();
            orderRepositoryMock.Setup(r => r.GetWithIncludeAsync(It.IsAny<Guid>(), It.IsAny<string[]>()))
                .ReturnsAsync((Order?)null);

            var orderService = new OrderServiceBuilder()
                .SetParameter(orderRepositoryMock)
                .Build();

            // Act
            var result = await orderService.GetOrderByIdAsync(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }
    }
}