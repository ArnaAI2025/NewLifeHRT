using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewLifeHRT.Api.Requests;
using NewLifeHRT.API.Controllers.Controllers;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using System.Security.Claims;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Controllers
{
    public class ProposalControllerTests
    {
        private static ProposalController CreateController(Mock<IProposalService> proposalService, Mock<IOrderService> orderService, Mock<IProposalDetailService> proposalDetailService, int? userId = null)
        {
            var controller = new ProposalController(proposalService.Object, orderService.Object, proposalDetailService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = userId.HasValue
                            ? new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()) }))
                            : new ClaimsPrincipal(new ClaimsIdentity())
                    }
                }
            };

            return controller;
        }

        [Fact]
        public async Task Create_Should_ReturnUnauthorized_When_UserMissing()
        {
            var controller = CreateController(new Mock<IProposalService>(), new Mock<IOrderService>(), new Mock<IProposalDetailService>());

            var result = await controller.Create(new ProposalRequestDto());

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task BulkAssignee_Should_ReturnBadRequest_When_IdsMissing()
        {
            var controller = CreateController(new Mock<IProposalService>(), new Mock<IOrderService>(), new Mock<IProposalDetailService>(), 1);

            var result = await controller.BulkAssignee(new BulkOperationAssigneeRequestDto<Guid, int> { Id = 1 });

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task RejectProposal_Should_ReturnUnauthorized_When_UserMissing()
        {
            var controller = CreateController(new Mock<IProposalService>(), new Mock<IOrderService>(), new Mock<IProposalDetailService>());

            var result = await controller.RejectProposal(Guid.NewGuid(), string.Empty);

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }
    }
}