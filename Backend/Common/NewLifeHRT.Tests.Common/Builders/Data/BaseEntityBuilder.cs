using AutoFixture;
using NewLifeHRT.Tests.Common.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Tests.Common.Builders.Data
{
    public abstract class BaseEntityBuilder
    {
        protected readonly Fixture _fixture;

        protected BaseEntityBuilder(Fixture fixture)
        {
            _fixture = fixture;
        }
        protected BaseEntityBuilder() { }

    }
}
