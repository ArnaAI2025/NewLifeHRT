using AutoFixture;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Tests.Common.Fixtures;

namespace NewLifeHRT.Tests.Common.Builders.Data
{
    public class UserBuilder : BaseEntityBuilder
    {
        private ApplicationUser _user;

        public UserBuilder(Fixture fixture) : base(fixture)
        {
            // Avoid circular references and simplify object creation
            _user = _fixture.Build<ApplicationUser>()
                .Without(u => u.Address)
                .Without(u => u.UserServices)
                .Without(u => u.RefreshTokens)
                .Without(u => u.Otps)
                .Without(u => u.Role)
                .Create();
        }

        public UserBuilder WithId(int id)
        {
            _user.Id = id;
            return this;
        }

        public UserBuilder WithUserName(string userName)
        {
            _user.UserName = userName;
            return this;
        }

        public UserBuilder WithEmail(string email)
        {
            _user.Email = email;
            return this;
        }

        public ApplicationUser Build()
        {
            return _user;
        }
    }
}
