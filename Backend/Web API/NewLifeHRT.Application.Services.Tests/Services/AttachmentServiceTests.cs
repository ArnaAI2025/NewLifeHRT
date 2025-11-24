using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class AttachmentServiceBuilder : ServiceBuilder<AttachmentService>
    {
        public override AttachmentService Build()
        {
            return new AttachmentService(AttachmentRepositoryMock.Object, BlobServiceMock.Object);
        }
    }

    public class AttachmentServiceTests
    {
        [Fact]
        public async Task UploadAsync_Should_Throw_When_FileMissing()
        {
            // Arrange
            var attachmentService = new AttachmentServiceBuilder().Build();

            // Act
            var action = async () => await attachmentService.UploadAsync(null, 1, Guid.NewGuid(), 1);

            // Assert
            await action.Should().ThrowAsync<ArgumentException>().WithMessage("File is empty.");
        }

        [Fact]
        public async Task BulkUploadAttachmentAsync_Should_ReturnZero_When_NoFiles()
        {
            // Arrange
            var attachmentService = new AttachmentServiceBuilder().Build();

            // Act
            var response = await attachmentService.BulkUploadAttachmentAsync(null, 1);

            // Assert
            response.SuccessCount.Should().Be(0);
            response.FailedCount.Should().Be(0);
        }

        [Fact]
        public async Task BulkToggleDocumentStatusAsync_Should_ReturnMessage_When_NoIds()
        {
            // Arrange
            var attachmentService = new AttachmentServiceBuilder().Build();

            // Act
            var response = await attachmentService.BulkToggleDocumentStatusAsync(null, 1, true);

            // Assert
            response.SuccessCount.Should().Be(0);
            response.Message.Should().Be("No attachment IDs provided.");
        }
    }
}