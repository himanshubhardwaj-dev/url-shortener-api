namespace URLShortner.API.Exceptions
{
    public class ReservedShortCodeException : Exception
    {
        public ReservedShortCodeException(string shortCode) : base($"Short code '{shortCode}' is reserved.")
        {
            
        }
    }
}
