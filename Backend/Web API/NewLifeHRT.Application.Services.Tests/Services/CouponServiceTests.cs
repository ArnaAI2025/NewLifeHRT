using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class CouponServiceBuilder : ServiceBuilder<CouponService>
    {
        public override CouponService Build()
        {
            return new CouponService(CouponRepositoryMock.Object);
        }
    }

    public class CouponServiceTests
    {
        [Fact]
        public async Task GetAllAsync_Should_ReturnCoupons()
        {
            var coupons = new List<Coupon> { new(), new() };
            var repositoryMock = new Mock<ICouponRepository>();
            repositoryMock.Setup(r => r.AllWithIncludeAsync(It.Is<string[]>(i => i.Single() == nameof(Coupon.User))))
                .ReturnsAsync(coupons);

            var service = new CouponServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var result = await service.GetAllAsync();

            result.Should().HaveCount(2);
            repositoryMock.Verify(r => r.AllWithIncludeAsync(It.Is<string[]>(i => i.Single() == nameof(Coupon.User))), Times.Once);
        }

        [Fact]
        public async Task GetActiveCoupons_Should_ReturnEmpty_When_NoneFound()
        {
            var repositoryMock = new Mock<ICouponRepository>();

            repositoryMock.Setup(r => r.FindAsync(
                    It.IsAny<Expression<Func<Coupon, bool>>>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(new List<Coupon>());

            var service = new CouponServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var result = await service.GetActiveCoupons();

            result.Should().BeEmpty();

            repositoryMock.Verify(r => r.FindAsync(
                    It.IsAny<Expression<Func<Coupon, bool>>>(),
                    It.IsAny<bool>()),
                Times.Once);
        }

        [Fact]
        public async Task GetCoupons_Should_ReturnEmpty_When_NoneExist()
        {
            var repositoryMock = new Mock<ICouponRepository>();
            repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Coupon>());

            var service = new CouponServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var result = await service.GetCoupons();

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetCouponById_Should_ReturnNull_When_NotFound()
        {
            var repositoryMock = new Mock<ICouponRepository>();
            repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Coupon?)null);

            var service = new CouponServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var result = await service.GetCouponById(Guid.NewGuid());

            result.Should().BeNull();
        }

        [Fact]
        public async Task Create_Should_ReturnMessage_When_NameExists()
        {
            var repositoryMock = new Mock<ICouponRepository>();
            repositoryMock.Setup(r => r.ExistAsync("Duplicate", null)).ReturnsAsync(true);

            var service = new CouponServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var response = await service.Create(new CouponRequestDto { CouponName = "Duplicate" }, 1);

            response.Message.Should().Be("Coupon name already exists. Please choose a different name.");
            repositoryMock.Verify(r => r.ExistAsync("Duplicate", null), Times.Once);
        }

        [Fact]
        public async Task Create_Should_AddCoupon_When_NameUnique()
        {
            var repositoryMock = new Mock<ICouponRepository>();
            repositoryMock.Setup(r => r.ExistAsync(It.IsAny<string>(), null)).ReturnsAsync(false);

            var service = new CouponServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var response = await service.Create(new CouponRequestDto
            {
                CouponName = "NewCoupon",
                ExpiryDate = DateTime.UtcNow.AddDays(1),
                Amount = 10,
                Percentage = 5,
                Buget = 100
            }, 2);

            response.Id.Should().NotBe(Guid.Empty);
            response.Message.Should().Be("Coupon created successfully.");
            repositoryMock.Verify(r => r.AddAsync(It.IsAny<Coupon>()), Times.Once);
            repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Update_Should_ReturnMessage_When_CouponNotFound()
        {
            var repositoryMock = new Mock<ICouponRepository>();
            repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Coupon?)null);

            var service = new CouponServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var response = await service.Update(Guid.NewGuid(), new CouponRequestDto(), 1);

            response.Message.Should().Be("Coupon not found.");
        }

        [Fact]
        public async Task Update_Should_ReturnMessage_When_NameAlreadyExists()
        {
            var coupon = new Coupon { Id = Guid.NewGuid(), CouponName = "Old Name" };
            var repositoryMock = new Mock<ICouponRepository>();
            repositoryMock.Setup(r => r.GetByIdAsync(coupon.Id)).ReturnsAsync(coupon);
            repositoryMock.Setup(r => r.ExistAsync("New Name", coupon.Id)).ReturnsAsync(true);

            var service = new CouponServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var response = await service.Update(coupon.Id, new CouponRequestDto { CouponName = "New Name" }, 5);

            response.Message.Should().Be("Coupon name already exists. Please choose a different name.");
            repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Coupon>()), Times.Never);
        }

        [Fact]
        public async Task Update_Should_UpdateCoupon_When_Valid()
        {
            var coupon = new Coupon { Id = Guid.NewGuid(), CouponName = "Current", Amount = 5, Percentage = 1, Buget = 10 };
            var repositoryMock = new Mock<ICouponRepository>();
            repositoryMock.Setup(r => r.GetByIdAsync(coupon.Id)).ReturnsAsync(coupon);
            repositoryMock.Setup(r => r.ExistAsync(It.IsAny<string>(), coupon.Id)).ReturnsAsync(false);

            var service = new CouponServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var response = await service.Update(coupon.Id, new CouponRequestDto
            {
                CouponName = "Current",
                ExpiryDate = DateTime.UtcNow,
                Amount = 15,
                Percentage = 2,
                Buget = 20
            }, 9);

            response.Message.Should().Be("Coupon updated successfully.");
            coupon.Amount.Should().Be(15);
            repositoryMock.Verify(r => r.UpdateAsync(coupon), Times.Once);
            repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task BulkToggleActiveStatusAsync_Should_ReturnMessage_When_NoIds()
        {
            var service = new CouponServiceBuilder().Build();

            var response = await service.BulkToggleActiveStatusAsync(new List<Guid>(), 1, true);

            response.Message.Should().Be("No valid coupon IDs provided.");
        }

        [Fact]
        public async Task BulkToggleActiveStatusAsync_Should_ReturnMessage_When_NoCouponsFound()
        {
            var repositoryMock = new Mock<ICouponRepository>();

            repositoryMock.Setup(r => r.FindAsync(
                    It.IsAny<Expression<Func<Coupon, bool>>>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(new List<Coupon>());

            var service = new CouponServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var response = await service.BulkToggleActiveStatusAsync(
                new List<Guid> { Guid.NewGuid() }, 1, false);

            response.Message.Should().Be("No coupons found for the provided IDs.");

            repositoryMock.Verify(r => r.BulkUpdateAsync(
                It.IsAny<List<Coupon>>()),
                Times.Never);
        }


        [Fact]
        public async Task BulkToggleActiveStatusAsync_Should_UpdateCoupons()
        {
            var ids = new List<Guid> { Guid.NewGuid() };
            var coupons = new List<Coupon> { new() { Id = ids[0] } };
            var repositoryMock = new Mock<ICouponRepository>();
            repositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Coupon, bool>>>(), false))
                .ReturnsAsync(coupons);

            var service = new CouponServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var response = await service.BulkToggleActiveStatusAsync(ids, 2, true);

            response.SuccessCount.Should().Be(1);
            coupons[0].IsActive.Should().BeTrue();
            repositoryMock.Verify(r => r.BulkUpdateAsync(coupons), Times.Once);
        }

        [Fact]
        public async Task BulkDeleteProposalAsync_Should_ReturnMessage_When_NoIds()
        {
            var service = new CouponServiceBuilder().Build();

            var response = await service.BulkDeleteProposalAsync(new List<Guid>());

            response.Message.Should().Be("No valid coupon IDs provided.");
        }

        [Fact]
        public async Task BulkDeleteProposalAsync_Should_RemoveCoupons()
        {
            var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var coupons = new List<Coupon> { new() { Id = ids[0] }, new() { Id = ids[1] } };
            var repositoryMock = new Mock<ICouponRepository>();
            repositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Coupon, bool>>>(), false))
                .ReturnsAsync(coupons);

            var service = new CouponServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var response = await service.BulkDeleteProposalAsync(ids);

            response.SuccessCount.Should().Be(2);
            repositoryMock.Verify(r => r.RemoveRangeAsync(coupons), Times.Once);
            repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}