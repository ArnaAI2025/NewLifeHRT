using AutoFixture;
using AutoFixture.Kernel;

namespace NewLifeHRT.Tests.Common.Fixtures
{
    public class DatabaseFixture
    {
        public Fixture Fixture { get; }

        public DatabaseFixture()
        {
            Fixture = new Fixture();

            // Remove default recursion behavior that throws on circular references
            Fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => Fixture.Behaviors.Remove(b));

            // Add behavior that omits circular references instead of throwing
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }
    }
}
