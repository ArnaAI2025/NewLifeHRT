using NewLifeHRT.External.Interfaces;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace NewLifeHRT.External.Services
{
    public class AudioConverter : IAudioConverter
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task<byte[]> ConvertAmrToMp3Async(byte[] amrBytes)
        {
            if (amrBytes == null || amrBytes.Length == 0)
                throw new ArgumentException("amrBytes must not be null or empty", nameof(amrBytes));

            if (string.IsNullOrWhiteSpace(FFmpeg.ExecutablesPath))
                throw new InvalidOperationException("FFmpeg executables path is not configured.");

            await _semaphore.WaitAsync();
            var tempAmr = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.amr");
            var tempMp3 = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.mp3");

            try
            {
                await File.WriteAllBytesAsync(tempAmr, amrBytes);

                var conversion = FFmpeg.Conversions.New()
                    .AddParameter($"-i \"{tempAmr}\" -acodec libmp3lame \"{tempMp3}\"")
                    .SetOverwriteOutput(true);

                await conversion.Start();

                return await File.ReadAllBytesAsync(tempMp3);
            }
            finally
            {
                try { if (File.Exists(tempAmr)) File.Delete(tempAmr); } catch { }
                try { if (File.Exists(tempMp3)) File.Delete(tempMp3); } catch { }
                _semaphore.Release();
            }
        }
    }
}
