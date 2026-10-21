using System.Text;

namespace CYHBrandRepo.Utilities
{
    /// <summary>Simple Base64 encode/decode for password storage (matching existing SP logic).</summary>
    public static class AESEncryption
    {
        public static string Encode(string plainText)
            => Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

        public static string Decode(string encodedText)
            => Encoding.UTF8.GetString(Convert.FromBase64String(encodedText));
    }
}
