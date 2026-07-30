using QRCoder;
using URLShortner.API.Interfaces;

namespace URLShortner.API.Services
{
    public class QRCodeService : IQRCodeService
    {
        private readonly IWebHostEnvironment _environment;
        public QRCodeService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public async Task<byte[]> GetorCreateQRCodeAsync(string shortCode, string shortUrl)
        {
            var qrcodeFolder = Path.Combine(_environment.WebRootPath, "qrcodes");
            Directory.CreateDirectory(qrcodeFolder);
            var filePath = Path.Combine(qrcodeFolder, $"{shortCode}.png");
            if (File.Exists(filePath))
            {
                return await File.ReadAllBytesAsync(filePath);
            }
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(shortUrl, QRCodeGenerator.ECCLevel.Q);
            var pngQrcode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeBytes = pngQrcode.GetGraphic(20);
            await File.WriteAllBytesAsync(filePath, qrCodeBytes);
            return qrCodeBytes;
        }
    }
}
