using System.Text;

namespace NewLifeHRT.API.Controllers.Middlewares
{
    public class ResponseInterceptorStream : Stream
    {
        private readonly Stream _originalStream;
        private readonly MemoryStream _memoryStream = new();

        public ResponseInterceptorStream(Stream original)
        {
            _originalStream = original;
        }

        public string GetContent()
        {
            _memoryStream.Position = 0;
            return Encoding.UTF8.GetString(_memoryStream.ToArray());
        }

        public async Task WriteToOriginalAsync(byte[] data)
        {
            await _originalStream.WriteAsync(data);
            await _originalStream.FlushAsync();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _memoryStream.Write(buffer, offset, count);
        }

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => _memoryStream.Length;
        public override long Position { get => _memoryStream.Position; set => _memoryStream.Position = value; }

        public override void Flush() => _memoryStream.Flush();
        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
    }
}
