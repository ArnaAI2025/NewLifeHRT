using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class ReminderServiceBuilder : ServiceBuilder<ReminderService>
    {
        public override ReminderService Build()
        {
            return new ReminderService(
                ReminderRepositoryMock.Object,
                ReminderTypeRepositoryMock.Object,
                RecurrenceRuleRepositoryMock.Object,
                LeadReminderRepositoryMock.Object,
                PatientReminderRepositoryMock.Object,
                PatientRepositoryMock.Object,
                LeadRepositoryMock.Object,
                UserRepositoryMock.Object);
        }
    }

    public class ReminderServiceTests
    {
        [Fact]
        public async Task CreateReminderAsync_Should_ReturnError_When_TimezoneMissing()
        {
            var patientRepoMock = new Mock<IPatientRepository>();
            patientRepoMock.Setup(r => r.Query()).Returns(BuildAsyncQueryable(new List<Patient>()));

            var leadRepoMock = new Mock<ILeadRepository>();
            leadRepoMock.Setup(r => r.Query()).Returns(BuildAsyncQueryable(new List<Lead>()));

            var service = new ReminderServiceBuilder()
                .SetParameter(patientRepoMock)
                .SetParameter(leadRepoMock)
                .Build();

            var response = await service.CreateReminderAsync(new CreateReminderRequestDto { PatientId = Guid.NewGuid(), ReminderDateTime = DateTime.UtcNow, ReminderTypeId = 1 }, 5);

            response.Id.Should().Be(Guid.Empty);
            response.Message.Should().Contain("Could not determine counselor/owner timezone.");
        }

        [Fact]
        public async Task CreateReminderAsync_Should_CreateReminder_ForPatient()
        {
            var patientId = Guid.NewGuid();
            var reminderTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

            var patientRepoMock = new Mock<IPatientRepository>();
            patientRepoMock.Setup(r => r.Query()).Returns(BuildAsyncQueryable(new List<Patient>
            {
                new()
                {
                    Id = patientId,
                    Counselor = new ApplicationUser
                    {
                        Timezone = new Timezone { StandardName = "UTC" }
                    }
                }
            }));

            Reminder? addedReminder = null;
            var reminderRepoMock = new Mock<IReminderRepository>();
            reminderRepoMock.Setup(r => r.AddAsync(It.IsAny<Reminder>()))
                .Callback<Reminder>(r => addedReminder = r)
                .ReturnsAsync((Reminder r) => r);

            var patientReminderRepoMock = new Mock<IPatientReminderRepository>();
            patientReminderRepoMock.Setup(r => r.AddAsync(It.IsAny<PatientReminder>()))
                .ReturnsAsync(new PatientReminder(Guid.NewGuid(), Guid.NewGuid(), patientId, "1", DateTime.UtcNow));

            var service = new ReminderServiceBuilder()
                .SetParameter(patientRepoMock)
                .SetParameter(reminderRepoMock)
                .SetParameter(patientReminderRepoMock)
                .Build();

            var response = await service.CreateReminderAsync(new CreateReminderRequestDto
            {
                PatientId = patientId,
                ReminderDateTime = reminderTime,
                ReminderTypeId = 1,
                IsRecurring = false
            }, 9);

            response.Id.Should().Be(addedReminder!.Id);
            addedReminder.ReminderDateTime.Should().Be(reminderTime);
            patientReminderRepoMock.Verify(r => r.AddAsync(It.IsAny<PatientReminder>()), Times.Once);
        }

        private static IQueryable<T> BuildAsyncQueryable<T>(IEnumerable<T> source)
        {
            return new TestAsyncEnumerable<T>(source).AsQueryable();
        }

        private class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable)
            {
            }

            public TestAsyncEnumerable(Expression expression) : base(expression)
            {
            }

            public IAsyncEnumerator<T> GetAsyncEnumerator(System.Threading.CancellationToken cancellationToken = default)
            {
                return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            }

            IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
        }

        private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;

            public TestAsyncEnumerator(IEnumerator<T> inner)
            {
                _inner = inner;
            }

            public T Current => _inner.Current;

            public ValueTask DisposeAsync()
            {
                _inner.Dispose();
                return ValueTask.CompletedTask;
            }

            public ValueTask<bool> MoveNextAsync()
            {
                return new ValueTask<bool>(_inner.MoveNext());
            }
        }

        private class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;

            internal TestAsyncQueryProvider(IQueryProvider inner)
            {
                _inner = inner;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new TestAsyncEnumerable<TEntity>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new TestAsyncEnumerable<TElement>(expression);
            }

            public object Execute(Expression expression)
            {
                return _inner.Execute(expression)!;
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return _inner.Execute<TResult>(expression);
            }

            public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
            {
                return new TestAsyncEnumerable<TResult>(expression);
            }

            public TResult ExecuteAsync<TResult>(Expression expression, System.Threading.CancellationToken cancellationToken)
            {
                var expectedResultType = typeof(TResult).GetGenericArguments()[0];
                var executionResult = typeof(IQueryProvider)
                    .GetMethod(nameof(IQueryProvider.Execute), 1, new[] { typeof(Expression) })!
                    .MakeGenericMethod(expectedResultType)
                    .Invoke(_inner, new[] { expression });

                var taskResult = typeof(Task)
                    .GetMethod(nameof(Task.FromResult))!
                    .MakeGenericMethod(expectedResultType)
                    .Invoke(null, new[] { executionResult });

                return (TResult)taskResult!;
            }
        }

        [Fact]
        public async Task GetTodayRemindersForAllLeadsAsync_Should_Throw_When_TimezoneMissing()
        {
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetUserTimezoneAsync(It.IsAny<int>())).ReturnsAsync(string.Empty);

            var service = new ReminderServiceBuilder()
                .SetParameter(userRepoMock)
                .Build();

            await Assert.ThrowsAsync<Exception>(() => service.GetTodayRemindersForAllLeadsAsync(3));
        }
    }
}