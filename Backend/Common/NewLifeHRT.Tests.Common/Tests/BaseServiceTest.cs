using AutoFixture;
using NewLifeHRT.Tests.Common.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    

namespace NewLifeHRT.Tests.Common.Tests
{
    public class BaseServiceTest : BaseUnitTest
    {
        protected readonly Fixture _fixture;
        public BaseServiceTest(DatabaseFixture databaseFixture) 
        { 
            _fixture = databaseFixture.Fixture;
        }
        public BaseServiceTest() { }

    }
}
