using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Settings;
using NewLifeHRT.Tests.Common.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class PatientAttachmentServiceBuilder : ServiceBuilder<PatientAttachmentService>
    {
        public override PatientAttachmentService Build()
        {
            return new PatientAttachmentService(
                PatientAttachmentRepositoryMock.Object,
                AttachmentServiceMock.Object,
                AzureBlobStorageOptions,
                MessageContentServiceMock.Object);
        }
    }

    public class PatientAttachmentServiceTests
    {
        [Fact]
        public async Task ToggleIsActiveAsync_Should_ReturnMessage_When_AttachmentMissing()
        {
            // Arrange
            var patientAttachmentRepositoryMock = new Mock<IPatientAttachmentRepository>();
            patientAttachmentRepositoryMock
                .Setup(r => r.GetSingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<PatientAttachment, bool>>>(), null, false))
                .ReturnsAsync((PatientAttachment)null);

            var service = new PatientAttachmentServiceBuilder()
                .SetParameter(patientAttachmentRepositoryMock)
                .Build();

            // Act
            var response = await service.ToggleIsActiveAsync(Guid.NewGuid(), true, 1);

            // Assert
            response.Message.Should().Be("Attachment not found.");
            response.Id.Should().Be(Guid.Empty);
        }

        [Fact]
        public async Task BulkUploadAttachmentAsync_Should_AttachUploadedIds_When_AttachmentsUploaded()
        {
            // Arrange
            var bulkUploadResponse = new BulkOperationResponseDto
            {
                SuccessIds = new List<string> { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() }
            };

            var attachmentServiceMock = new Mock<IAttachmentService>();
            attachmentServiceMock
                .Setup(s => s.BulkUploadAttachmentAsync(It.IsAny<UploadFilesRequestDto>(), It.IsAny<int>()))
                .ReturnsAsync(bulkUploadResponse);

            var patientAttachmentRepositoryMock = new Mock<IPatientAttachmentRepository>();

            var service = new PatientAttachmentServiceBuilder()
                .SetParameter(attachmentServiceMock)
                .SetParameter(patientAttachmentRepositoryMock)
                .Build();

            // Act
            var response = await service.BulkUploadAttachmentAsync(new UploadFilesRequestDto(), Guid.NewGuid(), 5);

            // Assert
            response.Should().NotBeNull();
            response.SuccessIds.Should().HaveCount(2);
            patientAttachmentRepositoryMock.Verify(r => r.AddRangeAsync(It.IsAny<List<PatientAttachment>>()), Times.Once);
        }

        [Fact]
        public async Task GetPatientAttachmentAsync_Should_ReturnNull_When_AttachmentMissing()
        {
            // Arrange
            var patientAttachmentRepositoryMock = new Mock<IPatientAttachmentRepository>();
            patientAttachmentRepositoryMock
                .Setup(r => r.GetSingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<PatientAttachment, bool>>>(), It.IsAny<Func<IQueryable<PatientAttachment>, IQueryable<PatientAttachment>>>(), false))
                .ReturnsAsync((PatientAttachment)null);

            var service = new PatientAttachmentServiceBuilder()
                .SetParameter(patientAttachmentRepositoryMock)
                .SetParameter(Options.Create(new AzureBlobStorageSettings()))
                .Build();

            // Act
            var response = await service.GetPatientAttachmentAsync(Guid.NewGuid());

            // Assert
            response.Should().BeNull();
        }
    }
}