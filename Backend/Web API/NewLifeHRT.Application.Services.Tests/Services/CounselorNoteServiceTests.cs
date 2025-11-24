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
    public class CounselorNoteServiceBuilder : ServiceBuilder<CounselorNoteService>
    {
        public Mock<ICounselorNoteRepository> CounselorNoteRepositoryMock { get; private set; } = new();

        public CounselorNoteServiceBuilder SetParameter(Mock<ICounselorNoteRepository> repository)
        {
            CounselorNoteRepositoryMock = repository;
            return this;
        }

        public override CounselorNoteService Build()
        {
            return new CounselorNoteService(CounselorNoteRepositoryMock.Object);
        }
    }

    public class CounselorNoteServiceTests
    {
        [Fact]
        public async Task CreateAsync_Should_ReturnFailureMessage_When_ExceptionThrown()
        {
            var repository = new Mock<ICounselorNoteRepository>();
            repository.Setup(r => r.AddAsync(It.IsAny<CounselorNote>())).ThrowsAsync(new Exception("fail"));

            var service = new CounselorNoteServiceBuilder().SetParameter(repository).Build();

            var response = await service.CreateAsync(new CreateCounselorRequestDto { PatientId = Guid.NewGuid() }, 1);

            response.Message.Should().Be("failed to create counselor note.");
            response.Id.Should().Be(Guid.Empty);
        }

        [Fact]
        public async Task DeleteNoteAsync_Should_ReturnNotFoundMessage_When_NoteMissing()
        {
            var repository = new Mock<ICounselorNoteRepository>();
            repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((CounselorNote?)null);

            var service = new CounselorNoteServiceBuilder().SetParameter(repository).Build();

            var response = await service.DeleteNoteAsync(Guid.NewGuid(), 1);

            response.Message.Should().Be("Counselor note not found or already deleted.");
        }
    }
}