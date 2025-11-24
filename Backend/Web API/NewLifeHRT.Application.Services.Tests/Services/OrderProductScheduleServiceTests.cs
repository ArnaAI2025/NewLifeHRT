using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class OrderProductScheduleServiceBuilder : ServiceBuilder<OrderProductScheduleService>
    {
        public Mock<IOrderProductScheduleRepository> OrderProductScheduleRepositoryMock { get; private set; } = new();
        public Mock<IOrderRepository> OrderRepositoryMock { get; private set; } = new();
        public Mock<IOrderProductScheduleSummaryRepository> OrderProductScheduleSummaryRepositoryMock { get; private set; } = new();
        public Mock<IScheduleSummaryProcessingRepository> ScheduleSummaryProcessingRepositoryMock { get; private set; } = new();
        public Mock<IPatientSelfReminderRepository> PatientSelfReminderRepositoryMock { get; private set; } = new();

        public override OrderProductScheduleService Build()
        {
            return new OrderProductScheduleService(
                OrderProductScheduleRepositoryMock.Object,
                OrderRepositoryMock.Object,
                OrderProductScheduleSummaryRepositoryMock.Object,
                ScheduleSummaryProcessingRepositoryMock.Object,
                PatientSelfReminderRepositoryMock.Object);
        }

        public OrderProductScheduleServiceBuilder SetParameter(Mock<IOrderProductScheduleSummaryRepository> summaryRepository)
        {
            OrderProductScheduleSummaryRepositoryMock = summaryRepository;
            return this;
        }

        public OrderProductScheduleServiceBuilder SetParameter(Mock<IScheduleSummaryProcessingRepository> processingRepository)
        {
            ScheduleSummaryProcessingRepositoryMock = processingRepository;
            return this;
        }

        public OrderProductScheduleServiceBuilder SetParameter(Mock<IPatientSelfReminderRepository> reminderRepository)
        {
            PatientSelfReminderRepositoryMock = reminderRepository;
            return this;
        }
    }

    public class OrderProductScheduleServiceTests
    {
        [Fact]
        public async Task UpdateScheduleSummaryAsync_Should_ReturnFalse_When_SummaryMissing()
        {
            var summaryRepository = new Mock<IOrderProductScheduleSummaryRepository>();
            summaryRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((OrderProductScheduleSummary?)null);

            var service = new OrderProductScheduleServiceBuilder()
                .SetParameter(summaryRepository)
                .Build();

            var result = await service.UpdateScheduleSummaryAsync(Guid.NewGuid(), new UpdateOrderProductScheduleSummaryRequestDto());

            result.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateScheduleSummaryAsync_Should_UpdateSummary_And_CreateProcessingEntry()
        {
            // Arrange
            var summaryId = Guid.NewGuid();
            var summaryRepository = new Mock<IOrderProductScheduleSummaryRepository>();
            var summary = new OrderProductScheduleSummary { Id = summaryId };
            summaryRepository.Setup(r => r.GetByIdAsync(summaryId)).ReturnsAsync(summary);

            var processingRepository = new Mock<IScheduleSummaryProcessingRepository>();
            processingRepository.Setup(r => r.AddAsync(It.IsAny<ScheduleSummaryProcessing>())).ReturnsAsync((ScheduleSummaryProcessing p) => p);

            var request = new UpdateOrderProductScheduleSummaryRequestDto
            {
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            };

            request.SelectedDays.AddRange(new[] { "Mon", "Tue" });
            request.Times.AddRange(new[] { "08:00 AM", "09:00 AM" });


            var service = new OrderProductScheduleServiceBuilder()
                .SetParameter(summaryRepository)
                .SetParameter(processingRepository)
                .Build();

            // Act
            var result = await service.UpdateScheduleSummaryAsync(summaryId, request);

            // Assert
            result.Should().BeTrue();
            summary.Status.Should().Be("InProgress");
            summaryRepository.Verify(r => r.UpdateAsync(summary), Times.Once);
            processingRepository.Verify(r => r.AddAsync(It.Is<ScheduleSummaryProcessing>(p => p.ScheduleSummaryId == summaryId)), Times.Once);
        }
    }
}