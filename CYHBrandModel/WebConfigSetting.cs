using Microsoft.Extensions.Configuration;

namespace CYHBrandModel
{
    /// <summary>
    /// Static config accessor — call WebConfigSetting.Initialize(config) once in Program.cs
    /// </summary>
    public static class WebConfigSetting
    {
        private static IConfiguration _config = null!;

        public static void Initialize(IConfiguration configuration)
            => _config = configuration;

        public static string BaseURL          => _config["AppSettings:BaseURL"] ?? "";
        public static string ImgUploadPath    => _config["AppSettings:ImgUploadPath"] ?? "uploads";
        public static string DefaultConnection => _config.GetConnectionString("DefaultConnection") ?? "";
    }
}
