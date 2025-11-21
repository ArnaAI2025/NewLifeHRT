using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class AgendaServiceBuilder : ServiceBuilder<AgendaService>
    {
        public override AgendaService Build()
        {
            return new AgendaService(AgendaRepositoryMock.Object);
        }
    }

    public class AgendaServiceTests
    {
        [Fact]
        public async Task GetAllAsync_Should_ReturnActiveAgendas()
        {
            var agendas = new List<Agenda> { new(), new() };
            var agendaRepo = new Mock<IAgendaRepository>();
            agendaRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Agenda, bool>>>(), false))
                .ReturnsAsync(agendas);

            var service = new AgendaServiceBuilder()
                .SetParameter(agendaRepo)
                .Build();

            var result = await service.GetAllAsync();

            result.Should().HaveCount(2);
        }
    }
}
