using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Tests.Common.Tests
{
    public class BaseUnitTest
    {
        protected static Mock<ILogger<T>> GetLoggerTMock<T>()
        {
            var loggerMock = new Mock<ILogger<T>>();

            loggerMock.Setup(m => m.IsEnabled(It.IsAny<LogLevel>()))
                .Returns(true);

            return loggerMock;
        }

    }
}
