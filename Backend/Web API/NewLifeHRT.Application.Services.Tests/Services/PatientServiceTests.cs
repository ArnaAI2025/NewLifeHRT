using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Settings;
using NewLifeHRT.Tests.Common.Builders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class PatientServiceBuilder : ServiceBuilder<PatientService>
    {
        public override PatientService Build()
        {
            return new PatientService(
                PatientRepositoryMock.Object,
                AddressRepositoryMock.Object,
                ClinicDbContextMock.Object,
                PatientAgendaServiceMock.Object,
                PatientCreditCardServiceMock.Object,
                AddressServiceMock.Object,
                PatientAttachmentServiceMock.Object,
                AttachmentServiceMock.Object,
                ShippingAddressServiceMock.Object,
                AzureBlobStorageOptions,
                UserServiceMock.Object);
        }
    }

    public class PatientServiceTests
    {
        [Fact]
        public async Task CreateAsync_Should_ReturnMessage_When_UserMissing()
        {
            // Arrange
            var patientService = new PatientServiceBuilder().Build();

            // Act
            var response = await patientService.CreateAsync(new CreatePatientRequestDto(), null);

            // Assert
            response.Id.Should().BeNull();
            response.Message.Should().Be("User information is required to create a patient.");
        }

        [Fact]
        public async Task CreateAsync_Should_ReturnDuplicateMessage_When_PatientExists()
        {
            // Arrange
            var request = new CreatePatientRequestDto
            {
                PhoneNumber = "123",
                Email = "test@example.com",
                Address = new AddressDto(),
                CounselorId = 1
            };

            var patientRepositoryMock = new Mock<IPatientRepository>();
            patientRepositoryMock.Setup(r => r.ExistAsync(request.PhoneNumber, request.Email)).ReturnsAsync(true);

            var service = new PatientServiceBuilder()
                .SetParameter(patientRepositoryMock)
                .Build();

            // Act
            var response = await service.CreateAsync(request, 5);

            // Assert
            response.Id.Should().BeNull();
            response.Message.Should().Be("Phone number or email already exists.");
            patientRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Patient>()), Times.Never);
        }

        [Fact]
        public async Task BulkAssignPatientsAsync_Should_ReturnMessage_When_NoIds()
        {
            // Arrange
            var service = new PatientServiceBuilder().Build();

            // Act
            var response = await service.BulkAssignPatientsAsync(null, 2, 1);

            // Assert
            response.Id.Should().Be(0);
            response.Message.Should().Be("No valid Patient IDs provided.");
        }
    }
}