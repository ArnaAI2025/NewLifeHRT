using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class MedicalRecommendationServiceBuilder : ServiceBuilder<MedicalRecommendationService>
    {
        public Mock<IMedicalRecommendationRepository> MedicalRecommendationRepositoryMock { get; private set; } = new();

        public MedicalRecommendationServiceBuilder SetParameter(Mock<IMedicalRecommendationRepository> repository)
        {
            MedicalRecommendationRepositoryMock = repository;
            return this;
        }

        public override MedicalRecommendationService Build()
        {
            return new MedicalRecommendationService(PatientRepositoryMock.Object, MedicalRecommendationRepositoryMock.Object);
        }
    }

    public class MedicalRecommendationServiceTests
    {
        [Fact]
        public async Task CreateAsync_Should_Throw_When_PatientNotFound()
        {
            var patientRepository = new Mock<IPatientRepository>();
            patientRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Patient?)null);

            var service = new MedicalRecommendationServiceBuilder().SetParameter(patientRepository).Build();

            await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateAsync(new MedicalRecommendationRequestDto { PatientId = Guid.NewGuid() }, 1));
        }

        [Fact]
        public async Task SoftDeleteAsync_Should_MarkRecommendationInactive()
        {
            var medicalRecommendation = new MedicalRecommendation { Id = Guid.NewGuid(), IsActive = true };

            var recommendationRepository = new Mock<IMedicalRecommendationRepository>();
            recommendationRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(medicalRecommendation);

            MedicalRecommendation? updatedEntity = null;

            recommendationRepository
                .Setup(r => r.UpdateAsync(It.IsAny<MedicalRecommendation>()))
                .Callback<MedicalRecommendation>(m => updatedEntity = m)
                .ReturnsAsync((MedicalRecommendation m) => m);
            recommendationRepository
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var service = new MedicalRecommendationServiceBuilder()
                .SetParameter(recommendationRepository)
                .Build();

            var response = await service.SoftDeleteAsync(medicalRecommendation.Id);

            response.Message.Should().Be("success");
            updatedEntity.Should().NotBeNull();
            updatedEntity!.IsActive.Should().BeFalse();
        }
    }
}