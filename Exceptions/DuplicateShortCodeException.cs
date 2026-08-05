namespace URLShortner.API.Exceptions
{
    public class DuplicateShortCodeException : Exception
    {
        public DuplicateShortCodeException(string shortCode) : base($"Short code '{shortCode}' already exists.")
        {

        }

    }
}
