namespace URLShortner.API.Interfaces
{
    public interface IQRCodeService
    {
        public Task<byte[]> GetorCreateQRCodeAsync(string shortCode,string shortUrl);
    }
}
