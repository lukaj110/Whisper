namespace WhisperCrypto
{
    static class Helper
    {
        public static string BytesToString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }
    }
}