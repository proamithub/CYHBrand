namespace CYHBrandModel.DTO
{
    public class AiBannerRequestDTO
    {
        public string   Topic           { get; set; } = string.Empty;
        public string   Style           { get; set; } = "Modern";      // Modern | Minimal | Colorful | Professional
        public string   Language        { get; set; } = "English";     // English | Hindi | Marathi | Tamil | Gujarati
        public string   Orientation     { get; set; } = "Square";      // Square | Portrait | Landscape
        public int      WidthPx         { get; set; } = 1080;
        public int      HeightPx        { get; set; } = 1080;
        public bool     IncludeBranding { get; set; } = true;
        public string   EngineMode      { get; set; } = "Free";        // Free | Paid
        public string?  CustomAdvice    { get; set; }
    }

    public class AiBannerResponseDTO
    {
        public bool     IsSuccess       { get; set; }
        public string   Message         { get; set; } = string.Empty;
        public string?  Headline        { get; set; }
        public List<string> AdvicePoints { get; set; } = new();
        public string?  BadgeText       { get; set; }
        public string?  DesignJson      { get; set; }
        public string?  DesignName      { get; set; }
        public int      WidthPx         { get; set; } = 1080;
        public int      HeightPx        { get; set; } = 1080;
        public string   Orientation     { get; set; } = "Square";
        public string?  BgColor         { get; set; } = "#0F172A";
        public string?  AccentColor     { get; set; } = "#0EA5A0";
        public int      RemainingCredits { get; set; } = 0;
        public string   EngineUsed      { get; set; } = "Free";
    }
}
