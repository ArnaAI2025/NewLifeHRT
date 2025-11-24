using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using System.Linq.Expressions;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class PoolDetailServiceBuilder : ServiceBuilder<PoolDetailService>
    {
        public override PoolDetailService Build()
        {
            return new PoolDetailService(PoolDetailRepositoryMock.Object);
        }
    }

    public class PoolDetailServiceTests
    {
        [Fact]
        public async Task GetCounselorsByDateRangeAsync_Should_ReturnEmpty_When_DatesMissing()
        {
            var repo = new Mock<IPoolDetailRepository>();
            var service = new PoolDetailServiceBuilder()
                .SetParameter(repo)
                .Build();

            var result = await service.GetCounselorsByDateRangeAsync(null, null);

            result.Should().BeEmpty();
            repo.Verify(r => r.Query(), Times.Never);
        }

        [Fact]
        public async Task GetCounselorsByDateRangeAsync_Should_ReturnMappedResults()
        {
            var fromDate = new DateTime(2024, 1, 1);
            var toDate = new DateTime(2024, 1, 7);
            var poolDetails = new List<PoolDetail>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    PoolId = Guid.NewGuid(),
                    CounselorId = 2,
                    Counselor = new ApplicationUser { FirstName = "Test", LastName = "Counselor" },
                    Pool = new Pool { FromDate = fromDate, ToDate = toDate, Week = 2 }
                }
            };

            var repo = new Mock<IPoolDetailRepository>();
            repo.Setup(r => r.Query()).Returns(new TestAsyncEnumerable<PoolDetail>(poolDetails));

            var service = new PoolDetailServiceBuilder()
                .SetParameter(repo)
                .Build();

            var result = await service.GetCounselorsByDateRangeAsync(fromDate, toDate);

            result.Should().HaveCount(1);
            result[0].CounselorName.Should().Be("Test Counselor");
            result[0].Week.Should().Be(2);
        }
    }

    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
            => new TestAsyncEnumerable<TEntity>(expression);

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => new TestAsyncEnumerable<TElement>(expression);

        public object Execute(Expression expression)
            => _inner.Execute(expression);

        public TResult Execute<TResult>(Expression expression)
            => _inner.Execute<TResult>(expression);

        // *** This is the important part ***
        TResult IAsyncQueryProvider.ExecuteAsync<TResult>(
            Expression expression,
            CancellationToken cancellationToken)
        {
            // TResult is usually Task<T>
            var resultType = typeof(TResult).GetGenericArguments()[0];

            var execResult = typeof(IQueryProvider)
                .GetMethod(
                    name: nameof(IQueryProvider.Execute),
                    genericParameterCount: 1,
                    types: new[] { typeof(Expression) })!
                .MakeGenericMethod(resultType)
                .Invoke(_inner, new object[] { expression });

            var task = typeof(Task)
                .GetMethod(nameof(Task.FromResult))!
                .MakeGenericMethod(resultType)
                .Invoke(null, new[] { execResult });

            return (TResult)task!;
        }
    }

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }

        public TestAsyncEnumerable(Expression expression) : base(expression) { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }

    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;

        public T Current => _inner.Current;

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync()
            => new ValueTask<bool>(_inner.MoveNext());
    }
}