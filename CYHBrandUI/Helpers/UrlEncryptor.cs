using Microsoft.AspNetCore.DataProtection;

namespace CYHBrandUI.Helpers
{
    /// <summary>
    /// Encrypts / decrypts integer IDs for safe URL exposure.
    /// Usage: inject IUrlEncryptor → encrypt ID before putting in URL, decrypt on receipt.
    /// </summary>
    public interface IUrlEncryptor
    {
        string  Encrypt(int id);
        int?    Decrypt(string? token);
    }

    public class UrlEncryptor : IUrlEncryptor
    {
        private readonly IDataProtector _protector;

        public UrlEncryptor(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("CYHBrand.UrlIds.v1");
        }

        public string Encrypt(int id)
        {
            var raw = _protector.Protect(id.ToString());
            // Make URL-safe: replace +/= with - _ ~
            return raw.Replace('+', '-').Replace('/', '_').Replace('=', '~');
        }

        public int? Decrypt(string? token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;

            // If token is already a plain numeric ID (e.g. "18"), parse directly
            if (int.TryParse(token.Trim(), out var directId) && directId > 0)
                return directId;

            try
            {
                var raw = token.Replace('-', '+').Replace('_', '/').Replace('~', '=');
                var plain = _protector.Unprotect(raw);
                return int.TryParse(plain, out var id) ? id : null;
            }
            catch { return null; }
        }
    }
}
