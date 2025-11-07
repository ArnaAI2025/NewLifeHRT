using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Tests.Common.Builders
{
    public abstract class BaseServiceBuilder<T> : IDisposable
        where T : class
    {
        protected Mock<ILogger<T>> LoggerMock { get; set; } = new();


        public abstract T Build();

        public virtual Mock<T> BuildMock()
        {
            return null;
        }

        #region IDisposable Support
        private bool disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
            {
                return;
            }

            if (disposing)
            {
               // Clear unnecessary resources while disposing
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            disposedValue = true;
        }

     
        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
