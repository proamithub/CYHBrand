using CYHBrandModel.DTO;
using CYHBrandUI.Helpers;
using Newtonsoft.Json;
using System.Text;

namespace CYHBrandUI.Services
{
    public class AiBannerService : IAiBannerService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<AiBannerService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public AiBannerService(IConfiguration config, ILogger<AiBannerService> logger, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<AiBannerResponseDTO> GenerateBannerAsync(AiBannerRequestDTO request, CustomPrincipal? doctor)
        {
            try
            {
                // 1. Calculate dimensions based on orientation or user inputs
                int width = request.WidthPx > 0 ? request.WidthPx : 1080;
                int height = request.HeightPx > 0 ? request.HeightPx : 1080;

                switch (request.Orientation?.ToLowerInvariant())
                {
                    case "portrait":
                    case "story":
                        width = 1080;
                        height = 1920;
                        break;
                    case "landscape":
                    case "banner":
                        width = 1920;
                        height = 1080;
                        break;
                    case "facebook":
                        width = 1200;
                        height = 630;
                        break;
                    case "square":
                    default:
                        width = 1080;
                        height = 1080;
                        break;
                }

                // 2. Determine colors based on style
                string bgColor = "#0B1B3D";      // Deep Navy
                string cardColor = "#162A5A";    // Slate Navy
                string accentColor = "#0EA5A0";  // Teal
                string textColor = "#FFFFFF";

                switch (request.Style?.ToLowerInvariant())
                {
                    case "minimal":
                        bgColor = "#F8FAFC";
                        cardColor = "#FFFFFF";
                        accentColor = "#2563EB";
                        textColor = "#0F172A";
                        break;
                    case "colorful":
                        bgColor = "#064E3B";     // Emerald Forest
                        cardColor = "#065F46";
                        accentColor = "#F59E0B";  // Warm Gold
                        textColor = "#FFFFFF";
                        break;
                    case "professional":
                        bgColor = "#0F172A";     // Midnight
                        cardColor = "#1E293B";
                        accentColor = "#38BDF8";  // Cyan
                        textColor = "#FFFFFF";
                        break;
                    case "modern":
                    default:
                        bgColor = "#0B1B3D";
                        cardColor = "#152852";
                        accentColor = "#0EA5A0";
                        textColor = "#FFFFFF";
                        break;
                }

                // 3. Extract Topic & Clinical Content (Online AI with local fallback)
                var (badge, headline, tips) = await ResolveMedicalContentAsync(request.Topic, request.Language, request.CustomAdvice);

                // 4. Build Structured Design JSON Layers
                var elements = new List<object>();

                // (0) Ambient Decorative Glow Circles (adds visual depth)
                elements.Add(new
                {
                    type = "circle",
                    x = width - 160,
                    y = -100,
                    w = 340,
                    h = 340,
                    color = accentColor,
                    opacity = 0.12,
                    label = "Ambient Glow Top-Right"
                });

                elements.Add(new
                {
                    type = "circle",
                    x = -80,
                    y = (int)(height * 0.45),
                    w = 240,
                    h = 240,
                    color = accentColor,
                    opacity = 0.08,
                    label = "Ambient Glow Mid-Left"
                });

                // (A) Top Header Category Badge
                int badgeW = Math.Min(440, width - 100);
                int badgeH = 48;
                int badgeX = (width - badgeW) / 2;
                int badgeY = (int)(height * 0.06);

                elements.Add(new
                {
                    type = "rect",
                    x = badgeX,
                    y = badgeY,
                    w = badgeW,
                    h = badgeH,
                    color = accentColor,
                    opacity = 0.9,
                    label = "Header Badge Background"
                });

                elements.Add(new
                {
                    type = "text",
                    x = badgeX,
                    y = badgeY + 11,
                    w = badgeW,
                    h = badgeH,
                    text = badge.ToUpper(),
                    color = (request.Style == "Minimal" ? "#FFFFFF" : "#FFFFFF"),
                    size = 20,
                    font = "Inter",
                    bold = true,
                    align = "center",
                    label = "Header Badge Text"
                });

                // (B) Main Headline
                int headY = badgeY + badgeH + (int)(height * 0.04);
                int headW = width - 120;
                int headX = 60;
                int headSize = (width > 1200) ? 58 : 46;

                elements.Add(new
                {
                    type = "text",
                    x = headX,
                    y = headY,
                    w = headW,
                    h = 120,
                    text = headline,
                    color = textColor,
                    size = headSize,
                    font = "Inter",
                    bold = true,
                    align = "center",
                    label = "Main Headline"
                });

                // (C) Content Card Box (Container for Tips)
                int cardX = 60;
                int cardY = headY + (int)(height * 0.12);
                int cardW = width - 120;
                int cardH = (int)(height * 0.38);

                elements.Add(new
                {
                    type = "rect",
                    x = cardX,
                    y = cardY,
                    w = cardW,
                    h = cardH,
                    color = cardColor,
                    opacity = (request.Style == "Minimal" ? 0.95 : 0.85),
                    label = "Tips Container Card"
                });

                // (D) Health Tips / Advice Bullet Points
                int tipY = cardY + 36;
                int tipStep = Math.Max(70, (cardH - 60) / Math.Max(1, tips.Count));
                int tipFontSize = (width > 1200) ? 28 : 24;

                for (int i = 0; i < tips.Count; i++)
                {
                    // Bullet circle indicator
                    elements.Add(new
                    {
                        type = "circle",
                        x = cardX + 36,
                        y = tipY + (i * tipStep) + 6,
                        w = 20,
                        h = 20,
                        color = accentColor,
                        opacity = 1.0,
                        label = $"Bullet Point {i + 1}"
                    });

                    // Bullet Text
                    elements.Add(new
                    {
                        type = "text",
                        x = cardX + 74,
                        y = tipY + (i * tipStep),
                        w = cardW - 100,
                        h = 60,
                        text = tips[i],
                        color = textColor,
                        size = tipFontSize,
                        font = "Inter",
                        bold = false,
                        align = "left",
                        label = $"Advice Text {i + 1}"
                    });
                }

                // (E) Doctor Branding Footer
                if (request.IncludeBranding)
                {
                    int footerH = (int)(height * 0.22);
                    int footerY = height - footerH - 40;
                    int footerX = 60;
                    int footerW = width - 120;

                    // Footer Card Background
                    elements.Add(new
                    {
                        type = "rect",
                        x = footerX,
                        y = footerY,
                        w = footerW,
                        h = footerH,
                        color = (request.Style == "Minimal" ? "#FFFFFF" : "#0EA5A0"),
                        opacity = (request.Style == "Minimal" ? 0.95 : 0.9),
                        label = "Doctor Branding Footer"
                    });

                    string docName = !string.IsNullOrWhiteSpace(doctor?.FullName) ? doctor.FullName : (!string.IsNullOrWhiteSpace(doctor?.Name) ? doctor.Name : "Dr. Specialist");
                    if (!docName.StartsWith("Dr.", StringComparison.OrdinalIgnoreCase)) docName = "Dr. " + docName;

                    string specialty = !string.IsNullOrWhiteSpace(doctor?.Specialty) ? doctor.Specialty : "Medical Specialist";
                    string clinic = !string.IsNullOrWhiteSpace(doctor?.ClinicName) ? doctor.ClinicName : "City Health Care";
                    string contact = !string.IsNullOrWhiteSpace(doctor?.MobileNo) ? $"For Consultation: {doctor.MobileNo}" : "Consult for Personal Advice";

                    string footerTextColor = (request.Style == "Minimal" ? "#0F172A" : "#FFFFFF");
                    string footerSubColor = (request.Style == "Minimal" ? "#475569" : "#E0F2FE");

                    // Doctor Name
                    elements.Add(new
                    {
                        type = "text",
                        x = footerX + 40,
                        y = footerY + 24,
                        w = footerW - 80,
                        h = 44,
                        text = docName,
                        color = footerTextColor,
                        size = (width > 1200) ? 36 : 30,
                        font = "Inter",
                        bold = true,
                        align = "left",
                        label = "Doctor Name"
                    });

                    // Specialty and Clinic
                    elements.Add(new
                    {
                        type = "text",
                        x = footerX + 40,
                        y = footerY + 70,
                        w = footerW - 80,
                        h = 36,
                        text = $"{specialty} · {clinic}",
                        color = footerSubColor,
                        size = (width > 1200) ? 22 : 18,
                        font = "Inter",
                        bold = false,
                        align = "left",
                        label = "Doctor Specialty & Clinic"
                    });

                    // Contact / Consultation
                    elements.Add(new
                    {
                        type = "text",
                        x = footerX + 40,
                        y = footerY + 110,
                        w = footerW - 80,
                        h = 32,
                        text = contact,
                        color = footerTextColor,
                        size = (width > 1200) ? 20 : 16,
                        font = "Inter",
                        bold = true,
                        align = "left",
                        label = "Contact Info"
                    });
                }

                var designObject = new
                {
                    bg = bgColor,
                    elements
                };

                string designJson = JsonConvert.SerializeObject(designObject);
                string designName = (!string.IsNullOrWhiteSpace(headline) && !headline.StartsWith("Doctor", StringComparison.OrdinalIgnoreCase))
                    ? $"{headline} - AI Banner"
                    : (!string.IsNullOrWhiteSpace(request.Topic) ? $"{request.Topic.Trim()} - AI Banner" : "AI Health Banner");
                if (designName.Length > 60) designName = designName[..60];

                return new AiBannerResponseDTO
                {
                    IsSuccess = true,
                    Message = "AI Banner generated successfully!",
                    Headline = headline,
                    AdvicePoints = tips,
                    BadgeText = badge,
                    DesignJson = designJson,
                    DesignName = designName,
                    WidthPx = width,
                    HeightPx = height,
                    Orientation = request.Orientation ?? "Square",
                    BgColor = bgColor,
                    AccentColor = accentColor
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AiBannerService.GenerateBannerAsync failed for prompt: {Prompt}", request.Topic);
                return new AiBannerResponseDTO
                {
                    IsSuccess = false,
                    Message = "Generation failed: " + ex.Message
                };
            }
        }

        private async Task<(string Badge, string Headline, List<string> Tips)> ResolveMedicalContentAsync(string? topic, string? language, string? customAdvice)
        {
            // 1. If custom advice is provided by user, respect it
            if (!string.IsNullOrWhiteSpace(customAdvice))
            {
                var customTips = customAdvice.Split(new[] { '\n', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                             .Select(t => t.Trim().TrimStart('-', '*', '•'))
                                             .Where(t => !string.IsNullOrWhiteSpace(t))
                                             .Take(4)
                                             .ToList();

                if (customTips.Any())
                {
                    return ("Health Advisory", "Doctor's Health Recommendation", customTips);
                }
            }

            // 2. Dynamic Online AI Medical Content Generation
            try
            {
                var aiContent = await FetchOnlineAiMedicalContentAsync(topic, language);
                if (aiContent != null && !string.IsNullOrWhiteSpace(aiContent.Headline) && aiContent.Tips?.Any() == true)
                {
                    string badgeText = !string.IsNullOrWhiteSpace(aiContent.Badge) ? aiContent.Badge : "Health Awareness";
                    return (badgeText, aiContent.Headline, aiContent.Tips.Take(4).ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Online AI medical content generation failed. Falling back to local clinical knowledge bank for topic: {Topic}", topic);
            }

            // 3. Fallback to Local Medical Knowledge Bank
            return GetLocalFallbackMedicalContent(topic, language);
        }

        private async Task<AiMedicalContentDto?> FetchOnlineAiMedicalContentAsync(string? topic, string? language)
        {
            string targetTopic = (topic ?? "").Trim();
            string targetLang = string.IsNullOrWhiteSpace(language) ? "English" : language.Trim();

            bool isRandom = string.IsNullOrWhiteSpace(targetTopic)
                            || targetTopic.Equals("RANDOM", StringComparison.OrdinalIgnoreCase)
                            || targetTopic.Equals("AUTO_RANDOM", StringComparison.OrdinalIgnoreCase)
                            || targetTopic.Contains("random", StringComparison.OrdinalIgnoreCase)
                            || targetTopic.Contains("surprise", StringComparison.OrdinalIgnoreCase);

            string provider = _config["AIContentGeneration:Provider"] ?? "OpenAI";
            string apiKey = _config["AIContentGeneration:ApiKey"]
                            ?? _config["AIImageGeneration:OpenAI:ApiKey"]
                            ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                            ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY")
                            ?? "";

            var client = _httpClientFactory.CreateClient();
            // Fast timeout to guarantee zero UI freezing / hanging
            client.Timeout = TimeSpan.FromSeconds(3);

            // If an API key is provided, use the configured official provider (OpenAI or Gemini)
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                if (provider.Equals("Gemini", StringComparison.OrdinalIgnoreCase))
                {
                    return await CallGeminiAiAsync(client, apiKey, targetTopic, targetLang, isRandom);
                }
                else
                {
                    // Default: OpenAI or OpenAI-compatible (Groq, OpenRouter, DeepSeek, etc.)
                    return await CallOpenAiAsync(client, apiKey, targetTopic, targetLang, isRandom);
                }
            }

            // If no API key configured, use Free Online AI with fast 2-second timeout and instant fallback
            return await CallFreeOnlineAiAsync(client, targetTopic, targetLang, isRandom);
        }

        private async Task<AiMedicalContentDto?> CallFreeOnlineAiAsync(HttpClient client, string topic, string language, bool isRandom)
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2.5));
                string prompt = isRandom
                    ? $"Generate random doctor clinic awareness banner in {language}. Return JSON with badge (2-4 words), headline (under 8 words), and tips (array of 3 prevention tips, 8-14 words each)."
                    : $"Medical topic: {topic}. Language: {language}. Generate medical awareness banner for doctor clinic. Return JSON with badge (2-4 words), headline (under 8 words), and tips (array of 3 prevention tips, 8-14 words each).";

                string url = $"https://text.pollinations.ai/{Uri.EscapeDataString(prompt)}?json=true&model=openai";
                var response = await client.GetAsync(url, cts.Token);
                if (!response.IsSuccessStatusCode)
                {
                    // Rate limit or service busy - fallback immediately with zero delay
                    return null;
                }

                string respString = await response.Content.ReadAsStringAsync(cts.Token);
                return ParseAiContentJson(respString);
            }
            catch
            {
                // Instant fallback on timeout or connection lag
                return null;
            }
        }

        private async Task<AiMedicalContentDto?> CallOpenAiAsync(HttpClient client, string apiKey, string topic, string language, bool isRandom)
        {
            string baseUrl = _config["AIContentGeneration:BaseUrl"] ?? "https://api.openai.com/v1";
            if (baseUrl.EndsWith("/")) baseUrl = baseUrl.TrimEnd('/');
            string model = _config["AIContentGeneration:Model"] ?? "gpt-4o-mini";
            string seed = $"{DateTime.UtcNow:HHmmssfff}_{Guid.NewGuid():N}";

            string systemPrompt = isRandom
                ? "You are an expert creative clinical medical copywriter. Pick a completely RANDOM and UNIQUE medical awareness domain out of 40+ clinical fields. Return ONLY a raw valid JSON object with: 1) 'badge' (2-4 words), 2) 'headline' (under 8 words), 3) 'tips' (array of 3 tips, 8-14 words each). No markdown."
                : "You are an expert clinical medical copywriter for a doctor branding and health awareness platform. Return ONLY a raw valid JSON object with: 1) 'badge', 2) 'headline', 3) 'tips' (array of 3 tips). No markdown.";

            string userPrompt = isRandom
                ? $"Generate a completely fresh random medical awareness banner for a doctor in {language}. Random Seed: {seed}"
                : $"Medical Topic: {topic}\nLanguage: {language}\nSeed: {seed}";

            var requestBody = new
            {
                model = model,
                messages = new object[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                temperature = 0.8,
                max_tokens = 350
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/chat/completions");
            requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
            requestMessage.Content = jsonContent;

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3.5));
            var response = await client.SendAsync(requestMessage, cts.Token);
            if (!response.IsSuccessStatusCode)
            {
                string err = await response.Content.ReadAsStringAsync(cts.Token);
                _logger.LogWarning("OpenAI API returned status {Status}: {Error}", response.StatusCode, err);
                return null;
            }

            string respString = await response.Content.ReadAsStringAsync(cts.Token);
            dynamic? parsed = JsonConvert.DeserializeObject(respString);
            string? rawContent = parsed?.choices?[0]?.message?.content?.ToString();

            if (string.IsNullOrWhiteSpace(rawContent)) return null;

            return ParseAiContentJson(rawContent);
        }

        private async Task<AiMedicalContentDto?> CallGeminiAiAsync(HttpClient client, string apiKey, string topic, string language, bool isRandom)
        {
            string model = _config["AIContentGeneration:Model"] ?? "gemini-1.5-flash";
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";
            string seed = $"{DateTime.UtcNow:HHmmssfff}_{Guid.NewGuid():N}";

            string promptText = isRandom
                ? $"You are an expert clinical medical copywriter. Pick a RANDOM medical awareness domain out of 40+ clinical specialties. Generate an awareness banner in {language}. Seed: {seed}. Output ONLY raw valid JSON: {{\"badge\": \"...\", \"headline\": \"...\", \"tips\": [\"...\", \"...\", \"...\"]}}"
                : $"You are an expert clinical medical copywriter. Topic: '{topic}', Language: '{language}'. Seed: {seed}. Output ONLY raw valid JSON: {{\"badge\": \"...\", \"headline\": \"...\", \"tips\": [\"...\", \"...\", \"...\"]}}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = promptText }
                        }
                    }
                },
                generationConfig = new
                {
                    responseMimeType = "application/json"
                }
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3.5));
            var response = await client.PostAsync(url, jsonContent, cts.Token);
            if (!response.IsSuccessStatusCode)
            {
                string err = await response.Content.ReadAsStringAsync(cts.Token);
                _logger.LogWarning("Gemini API returned status {Status}: {Error}", response.StatusCode, err);
                return null;
            }

            string respString = await response.Content.ReadAsStringAsync(cts.Token);
            dynamic? parsed = JsonConvert.DeserializeObject(respString);
            string? rawContent = parsed?.candidates?[0]?.content?.parts?[0]?.text?.ToString();

            if (string.IsNullOrWhiteSpace(rawContent)) return null;

            return ParseAiContentJson(rawContent);
        }

        private static AiMedicalContentDto? ParseAiContentJson(string raw)
        {
            try
            {
                string clean = raw.Trim();
                if (clean.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
                {
                    clean = clean[7..].Trim();
                }
                else if (clean.StartsWith("```"))
                {
                    clean = clean[3..].Trim();
                }
                if (clean.EndsWith("```"))
                {
                    clean = clean[..^3].Trim();
                }

                var result = JsonConvert.DeserializeObject<AiMedicalContentDto>(clean);
                if (result?.Tips != null)
                {
                    result.Tips = result.Tips
                        .Select(t => System.Text.RegularExpressions.Regex.Replace(t, @"^(\d+[\.\)]\s*|[-*•]\s*)", "").Trim())
                        .Where(t => !string.IsNullOrWhiteSpace(t))
                        .ToList();
                }
                return result;
            }
            catch
            {
                return null;
            }
        }

        public class AiMedicalContentDto
        {
            public string? Badge { get; set; }
            public string? Headline { get; set; }
            public List<string>? Tips { get; set; }
        }

        private static (string Badge, string Headline, List<string> Tips) GetLocalFallbackMedicalContent(string? topic, string? language)
        {
            topic = (topic ?? "").ToLowerInvariant().Trim();
            language = (language ?? "English").ToLowerInvariant().Trim();

            bool isRandom = string.IsNullOrWhiteSpace(topic)
                            || topic.Equals("random", StringComparison.OrdinalIgnoreCase)
                            || topic.Equals("auto_random", StringComparison.OrdinalIgnoreCase)
                            || topic.Contains("random")
                            || topic.Contains("surprise");

            if (isRandom)
            {
                var randomKeys = new[] { "heart", "diabet", "yoga", "doctor", "diwali", "blood", "dengue", "child", "eye", "dent", "mental", "bone", "monsoon", "kidney", "skin", "digest", "hypertens" };
                topic = randomKeys[Random.Shared.Next(randomKeys.Length)];
            }

            // Hindi language bank
            if (language == "hindi")
            {
                if (topic.Contains("kidney") || topic.Contains("gurde"))
                {
                    return ("गुर्दा स्वास्थ्य जागरूकता", "स्वस्थ किडनी, स्वस्थ जीवन की कुंजी", new List<string>
                    {
                        "प्रतिदिन 8-10 गिलास पर्याप्त पानी पीकर हाइड्रेटेड रहें",
                        "दर्द निवारक (painkillers) दवाओं का अनावश्यक उपयोग न करें",
                        "रक्तचाप और शुगर को नियंत्रित रखकर किडनी सुरक्षित रखें"
                    });
                }
                if (topic.Contains("skin") || topic.Contains("twacha"))
                {
                    return ("त्वचा स्वास्थ्य सुरक्षा", "प्राकृतिक निखार और त्वचा की देखभाल", new List<string>
                    {
                        "धूप में निकलने से पहले सनस्क्रीन (SPF 30+) अवश्य लगाएं",
                        "त्वचा को नमी देने के लिए पर्याप्त पानी व नारियल तेल का उपयोग करें",
                        "किसी भी नए तिल या मस्से में बदलाव पर चर्म रोग विशेषज्ञ से मिलें"
                    });
                }
                if (topic.Contains("digest") || topic.Contains("pet") || topic.Contains("gas"))
                {
                    return ("पाचन स्वास्थ्य सलाह", "पेट रहेगा स्वस्थ, तो तन रहेगा तंदुरुस्त", new List<string>
                    {
                        "आहार में फाइबर युक्त फल, सलाद व हरी सब्जियां बढ़ाएं",
                        "भोजन के तुरंत बाद लेटने से बचें और 100 कदम टहलें",
                        "दही व छाछ जैसे प्रोबायोटिक्स का नियमित सेवन करें"
                    });
                }
                if (topic.Contains("heart") || topic.Contains("cardio") || topic.Contains("dil"))
                {
                    return ("हृदय स्वास्थ्य जागरूकता", "स्वस्थ हृदय, दीर्घायु जीवन", new List<string>
                    {
                        "प्रतिदिन 30 मिनट पैदल चलें या हल्का व्यायाम करें",
                        "आहार में नमक, चीनी और तली-भुनी चीजों का सेवन कम करें",
                        "नियमित रूप से रक्तचाप और कोलेस्ट्रॉल की जांच कराएं"
                    });
                }
                if (topic.Contains("diabet") || topic.Contains("sugar") || topic.Contains("madhumeh"))
                {
                    return ("मधुमेह नियंत्रण सलाह", "मधुमेह को नियंत्रित रखें, खुशहाल रहें", new List<string>
                    {
                        "ब्लड शुगर स्तर की समय पर नियमित जांच अवश्य करें",
                        "संतुलित आहार लें और फाइबर युक्त सब्जियों का सेवन बढ़ाएं",
                        "तनाव से दूर रहें और डॉक्टर की सलाह अनुसार दवाएं लें"
                    });
                }
                if (topic.Contains("monsoon") || topic.Contains("flu") || topic.Contains("rain") || topic.Contains("barish"))
                {
                    return ("मानसून स्वास्थ्य सुझाव", "मानसून में रखें सेहत का विशेष ध्यान", new List<string>
                    {
                        "हमेशा उबला या स्वच्छ पानी ही पिएं",
                        "मच्छरों से बचाव के लिए फुल स्लीव कपड़े पहनें",
                        "बाहर का खुला और बासी खाना खाने से बचें"
                    });
                }
                if (topic.Contains("child") || topic.Contains("pediatric") || topic.Contains("kid") || topic.Contains("bacha"))
                {
                    return ("बाल स्वास्थ्य देखभाल", "बच्चों की सेहत, परिवार की मुस्कान", new List<string>
                    {
                        "टीकाकरण चार्ट के अनुसार सभी टीके समय पर लगवाएं",
                        "पोषण युक्त संतुलित भोजन और दूध नियमित दें",
                        "स्क्रीन टाइम सीमित रखें और खेलकूद को बढ़ावा दें"
                    });
                }
                if (topic.Contains("eye") || topic.Contains("vision") || topic.Contains("aankh"))
                {
                    return ("नेत्र सुरक्षा सुझाव", "स्वस्थ आंखें, उज्ज्वल भविष्य", new List<string>
                    {
                        "डिजिटल स्क्रीन पर 20-20-20 नियम का पालन करें",
                        "पर्याप्त रोशनी में पढ़ें और आंखों को बार-बार न मलें",
                        "वर्ष में एक बार आंखों की जांच अवश्य कराएं"
                    });
                }
                if (topic.Contains("dent") || topic.Contains("oral") || topic.Contains("daant"))
                {
                    return ("दंत स्वास्थ्य सुरक्षा", "मजबूत दांत, सुंदर मुस्कान", new List<string>
                    {
                        "दिन में दो बार ब्रश करें और कुल्ला अवश्य करें",
                        "मीठी और चिपचिपी चीजों का सेवन सीमित करें",
                        "हर 6 महीने में दंत चिकित्सक से जांच कराएं"
                    });
                }
                if (topic.Contains("doctor") || topic.Contains("chikitsak"))
                {
                    return ("राष्ट्रीय डॉक्टर दिवस", "स्वास्थ्य रक्षक को नमन, नियमित जांच अपनाएं", new List<string>
                    {
                        "समय पर नियमित स्वास्थ्य परीक्षण अवश्य करवाएं",
                        "बिना डॉक्टर सलाह के कोई भी दवा स्वयं न लें",
                        "डॉक्टर के साथ अपने लक्षणों की पूरी जानकारी साझा करें"
                    });
                }
                if (topic.Contains("yoga") || topic.Contains("yog"))
                {
                    return ("अंतर्राष्ट्रीय योग दिवस", "योग से निरोग, स्वस्थ मन और तन", new List<string>
                    {
                        "प्रतिदिन 20 मिनट सूर्य नमस्कार व प्राणायाम करें",
                        "रीढ़ की हड्डी सीधी रखें और गहरी सांस का अभ्यास करें",
                        "तनाव मुक्ति और मानसिक शांति के लिए ध्यान लगाएं"
                    });
                }
                if (topic.Contains("diwali") || topic.Contains("deepavali"))
                {
                    return ("दीपावली स्वास्थ्य सुरक्षा", "रोशनी का पर्व, स्वास्थ्य और खुशहाली", new List<string>
                    {
                        "मधुमेह रोगी चीनी रहित और प्राकृतिक मिठाइयां चुनें",
                        "प्रदूषण व धुएं से बचने के लिए मास्क का उपयोग करें",
                        "जलने की स्थिति में तुरंत ठंडा पानी डालें और डॉक्टर से मिलें"
                    });
                }
                if (topic.Contains("blood") || topic.Contains("raktdan"))
                {
                    return ("रक्तदान महादान जागरूकता", "रक्तदान करें, किसी का जीवन बचाएं", new List<string>
                    {
                        "18 से 65 वर्ष के स्वस्थ व्यक्ति रक्तदान कर सकते हैं",
                        "रक्तदान से पहले और बाद में भरपूर पानी पिएं",
                        "नियमित रक्तदान शरीर में नए रक्त कोशिकाओं को बढ़ावा देता है"
                    });
                }
                if (topic.Contains("dengue") || topic.Contains("malaria"))
                {
                    return ("डेंगू एवं मलेरिया रोकथाम", "मच्छरों से रहें सावधान, स्वस्थ रहें परिवार", new List<string>
                    {
                        "कूलर, गमलों और छतों पर जमा पानी तुरंत हटाएं",
                        "पूरी बांह के कपड़े पहनें और मच्छरदानी का उपयोग करें",
                        "तेज बुखार व बदन दर्द पर तुरंत खून की जांच करवाएं"
                    });
                }
                if (topic.Contains("thyroid") || topic.Contains("gale"))
                {
                    return ("थायराइड स्वास्थ्य जागरूकता", "थायराइड को रखें संतुलित, पाएं भरपूर ऊर्जा", new List<string>
                    {
                        "हर 6 महीने में T3, T4 व TSH की जांच अवश्य कराएं",
                        "डॉक्टर द्वारा निर्धारित थायराइड दवा खाली पेट नियमित लें",
                        "आहार में आयोडीन युक्त नमक, हरी पत्तेदार सब्जियां और अखरोट शामिल करें"
                    });
                }
                if (topic.Contains("asthma") || topic.Contains("dama") || topic.Contains("sans") || topic.Contains("cough"))
                {
                    return ("दमा एवं श्वसन स्वास्थ्य", "स्वच्छ सांस, स्वस्थ फेफड़े और सुकून भरा जीवन", new List<string>
                    {
                        "धूल, धुएं और परागकणों से बचने के लिए मास्क पहनें",
                        "डॉक्टर द्वारा बताए गए इनहेलर को हमेशा अपने पास रखें",
                        "प्रतिदिन सुबह अनुलोम-विलोम व गहरी सांस का अभ्यास करें"
                    });
                }
                if (topic.Contains("bp") || topic.Contains("hyper") || topic.Contains("raktchap"))
                {
                    return ("रक्तचाप नियंत्रण जागरूकता", "रक्तचाप रखें सामान्य, हृदय रहेगा स्वस्थ", new List<string>
                    {
                        "दैनिक आहार में नमक की मात्रा सीमित करें (5 ग्राम से कम)",
                        "तनाव प्रबंधन करें और प्रतिदिन 30 मिनट सैर करें",
                        "घर पर नियमित बीपी जांचें और डॉक्टर की सलाह से दवाएं लें"
                    });
                }
                if (topic.Contains("knee") || topic.Contains("joint") || topic.Contains("arthrit") || topic.Contains("ghutna") || topic.Contains("jod"))
                {
                    return ("जोड़ों का दर्द एवं गठिया देखभाल", "जोड़ों को रखें मजबूत, गतिशीलता बनाए रखें", new List<string>
                    {
                        "वजन नियंत्रित रखें ताकि घुटनों पर अतिरिक्त दबाव न पड़े",
                        "विटामिन D3 व कैल्शियम युक्त आहार लें और सुबह धूप में बैठें",
                        "जोड़ों के लचीलेपन के लिए हल्का व्यायाम और स्ट्रेचिंग करें"
                    });
                }
                if (topic.Contains("migraine") || topic.Contains("headache") || topic.Contains("sar dard"))
                {
                    return ("माइग्रेन एवं सिरदर्द राहत", "तनाव मुक्त जीवनशैली, सिरदर्द से पाएं निजात", new List<string>
                    {
                        "प्रतिदिन 7-8 घंटे की पर्याप्त और नियमित नींद लें",
                        "तेज धूप, स्क्रीन चमक और लगातार खाली पेट रहने से बचें",
                        "सिरदर्द के ट्रिगर्स को पहचानें और समय पर चिकित्सक से परामर्श लें"
                    });
                }
                if (topic.Contains("pcos") || topic.Contains("pcod") || topic.Contains("women") || topic.Contains("mahila"))
                {
                    return ("महिला स्वास्थ्य एवं हार्मोनल संतुलन", "हार्मोनल संतुलन और स्वस्थ जीवनशैली की राह", new List<string>
                    {
                        "आहार में कम ग्लाइसेमिक इंडेक्स वाले साबुत अनाज व हरी सब्जियां चुनें",
                        "सक्रिय जीवनशैली अपनाएं और वजन को स्वस्थ सीमा में रखें",
                        "अनियमित माहवारी या हार्मोनल बदलाव पर स्त्री रोग विशेषज्ञ से मिलें"
                    });
                }

                // General Hindi fallback
                string hiTopicClean = !string.IsNullOrWhiteSpace(topic) ? topic : "स्वास्थ्य";
                return ($"{hiTopicClean} स्वास्थ्य सलाह", $"{hiTopicClean} से बचाव और स्वस्थ जीवन के नियम", new List<string>
                {
                    "विशेषज्ञ डॉक्टर की सलाह से समय पर सही जांच और उपचार अपनाएं",
                    "संतुलित आहार, नियमित दिनचर्या और पर्याप्त पानी का सेवन करें",
                    "लक्षणों को नजरअंदाज न करें और स्वस्थ जीवनशैली को प्राथमिकता दें"
                });
            }

            // Marathi language bank
            if (language == "marathi")
            {
                if (topic.Contains("heart") || topic.Contains("cardio"))
                {
                    return ("हृदय आरोग्य जागृती", "निरोगी हृदयासाठी योग्य काळजी", new List<string>
                    {
                        "दररोज ३० मिनिटे चालण्याचा किंवा हलका व्यायाम करा",
                        "आहारात मिठाचे आणि तेलकट पदार्थांचे प्रमाण मर्यादित ठेवा",
                        "रक्तदाब आणि कोलेस्टेरॉलची नियमित तपासणी करा"
                    });
                }
                return ("आरोग्य सल्ला", "सकस आहार आणि निरोगी जीवनशैली", new List<string>
                {
                    "दररोज पुरेसे पाणी पिऊन शरीर हायड्रेटेड ठेवा",
                    "आहारात फळे, पालेभाज्यांचा समावेश करा",
                    "कोणत्याही त्रासासाठी तज्ज्ञ डॉक्टरांचा वेळेवर सल्ला घ्या"
                });
            }

            // English language bank
            if (topic.Contains("heart") || topic.Contains("cardio"))
            {
                return ("Heart Health Awareness", "Keep Your Heart Strong & Healthy", new List<string>
                {
                    "Engage in 30 minutes of moderate aerobic activity daily",
                    "Maintain a low-sodium, heart-friendly Mediterranean diet",
                    "Schedule regular blood pressure & lipid profile screenings"
                });
            }

            if (topic.Contains("diabet") || topic.Contains("sugar") || topic.Contains("glucose"))
            {
                return ("Diabetes Care Advisory", "Control Your Sugar, Protect Your Future", new List<string>
                {
                    "Monitor fasting and post-prandial blood sugar consistently",
                    "Swap refined carbohydrates for high-fiber whole grains",
                    "Stay active and follow your physician's prescribed regimen"
                });
            }

            if (topic.Contains("monsoon") || topic.Contains("rain") || topic.Contains("flu") || topic.Contains("fever"))
            {
                return ("Seasonal Wellness Tips", "Stay Protected During Rainy Season", new List<string>
                {
                    "Drink only boiled or safely filtered water",
                    "Prevent water stagnation around home to stop mosquito breeding",
                    "Boost immunity with warm soups, vitamin C, and fresh fruits"
                });
            }

            if (topic.Contains("diet") || topic.Contains("nutrition") || topic.Contains("weight") || topic.Contains("food"))
            {
                return ("Nutrition & Wellness", "Fuel Your Body with Right Nutrition", new List<string>
                {
                    "Fill half your plate with colorful vegetables and salads",
                    "Stay adequately hydrated with 2.5 to 3 liters of water daily",
                    "Avoid processed sugary snacks and prioritize lean proteins"
                });
            }

            if (topic.Contains("child") || topic.Contains("pediatric") || topic.Contains("baby") || topic.Contains("kid"))
            {
                return ("Pediatric Wellness", "Nurturing Your Child's Healthy Growth", new List<string>
                {
                    "Complete all scheduled immunizations on time",
                    "Ensure adequate sleep and limit daily digital screen exposure",
                    "Encourage outdoor sports and age-appropriate calcium intake"
                });
            }

            if (topic.Contains("dent") || topic.Contains("teeth") || topic.Contains("oral"))
            {
                return ("Oral Hygiene Advisory", "Healthy Teeth, Confident Smile", new List<string>
                {
                    "Brush twice daily for two minutes using soft bristles",
                    "Floss daily to remove plaque trapped between teeth",
                    "Visit your dentist every 6 months for preventive cleanings"
                });
            }

            if (topic.Contains("mental") || topic.Contains("stress") || topic.Contains("mind") || topic.Contains("sleep"))
            {
                return ("Mental Wellness", "Prioritize Peace of Mind & Sound Sleep", new List<string>
                {
                    "Practice 10 minutes of deep breathing or mindfulness daily",
                    "Maintain a consistent 7-8 hour sleep schedule every night",
                    "Take regular breaks and consult a specialist when overwhelmed"
                });
            }

            if (topic.Contains("eye") || topic.Contains("vision") || topic.Contains("screen"))
            {
                return ("Eye Care & Vision Protection", "Protect Your Eyes in the Digital Age", new List<string>
                {
                    "Follow the 20-20-20 rule during screen usage (look 20 ft away)",
                    "Adjust screen brightness and keep monitor at arm's length",
                    "Undergo comprehensive annual dilated eye examinations"
                });
            }

            if (topic.Contains("bone") || topic.Contains("joint") || topic.Contains("ortho") || topic.Contains("calcium"))
            {
                return ("Bone & Joint Wellness", "Build Strong Bones, Stay Active Longer", new List<string>
                {
                    "Ensure adequate daily intake of Calcium and Vitamin D3",
                    "Incorporate weight-bearing exercises to boost bone density",
                    "Practice good posture during desk work and driving"
                });
            }

            if (topic.Contains("doctor") || topic.Contains("healer") || topic.Contains("physician"))
            {
                return ("National Doctor's Day", "Honoring Healers, Celebrating Life", new List<string>
                {
                    "Schedule an annual comprehensive clinical health checkup",
                    "Share complete medical history and symptoms openly with your physician",
                    "Adhere strictly to prescribed medicine dosages and preventive advice"
                });
            }

            if (topic.Contains("yoga") || topic.Contains("asana") || topic.Contains("pranayam"))
            {
                return ("International Yoga Day", "Yoga for Inner Harmony & Complete Vitality", new List<string>
                {
                    "Practice 20 minutes of daily Pranayama for lung and heart capacity",
                    "Maintain correct spinal posture during work hours to avoid chronic pain",
                    "Combine mindful meditation with gentle stretching for deep relaxation"
                });
            }

            if (topic.Contains("diwali") || topic.Contains("deepavali"))
            {
                return ("Green & Healthy Diwali", "Celebrate Festivals with Health & Vitality", new List<string>
                {
                    "Choose sugar-free sweets and nuts for balanced festive indulgence",
                    "Protect lungs and eyes from smoke and fine particulate matter",
                    "Keep basic first-aid handy and stay well-hydrated throughout the festival"
                });
            }

            if (topic.Contains("blood") || topic.Contains("donor") || topic.Contains("donation"))
            {
                return ("World Blood Donor Day", "Give Blood, Give Hope, Save Lives", new List<string>
                {
                    "Healthy adults can safely donate blood every three months",
                    "Stay well-hydrated and have a nutritious meal before donation",
                    "One single blood donation can save up to three lives in emergency care"
                });
            }

            if (topic.Contains("dengue") || topic.Contains("malaria"))
            {
                return ("Dengue & Vector Prevention", "Stop Mosquito Breeding, Protect Your Family", new List<string>
                {
                    "Eliminate standing water in coolers, plant pots, and open buckets",
                    "Wear full-coverage clothing and apply mosquito repellents outdoors",
                    "Seek immediate medical attention and blood platelet screening if fever spikes"
                });
            }

            if (topic.Contains("swasth") || topic.Contains("bharat") || topic.Contains("republic") || topic.Contains("independence"))
            {
                return ("Swasth Bharat Mission", "Healthy Citizens, Strong & Prosperous Nation", new List<string>
                {
                    "Adopt active daily mobility and complete 10,000 steps daily",
                    "Choose fresh local seasonal vegetables and minimize ultra-processed foods",
                    "Pledge to complete timely immunizations and annual preventive checks"
                });
            }

            if (topic.Contains("kidney") || topic.Contains("renal"))
            {
                return ("Kidney Health Advisory", "Protect Your Kidneys, Guard Your Life", new List<string>
                {
                    "Drink 8-10 glasses of clean water daily to flush metabolic toxins",
                    "Avoid unnecessary or unprescribed long-term painkiller medications",
                    "Keep blood pressure and blood sugar strictly within target ranges"
                });
            }

            if (topic.Contains("skin") || topic.Contains("derma"))
            {
                return ("Dermatology & Skin Barrier", "Nourish Your Skin, Shield from Sun Damage", new List<string>
                {
                    "Apply broad-spectrum SPF 30+ sunscreen 20 minutes before stepping out",
                    "Keep skin hydrated with daily moisturizers and adequate water intake",
                    "Consult a dermatologist for any suspicious changing moles or lesions"
                });
            }

            if (topic.Contains("digest") || topic.Contains("gut") || topic.Contains("gerd") || topic.Contains("acid"))
            {
                return ("Digestive Wellness", "Healthy Gut, Strong Immunity & Vitality", new List<string>
                {
                    "Increase dietary soluble fiber with whole grains, salads, and fruits",
                    "Avoid lying down immediately after meals; take a light 15-minute walk",
                    "Include natural probiotics like yogurt and fermented foods in your diet"
                });
            }

            if (topic.Contains("thyroid") || topic.Contains("tsh") || topic.Contains("goitre"))
            {
                return ("Thyroid Health Advisory", "Balancing Thyroid for Active Energy & Metabolism", new List<string>
                {
                    "Screen T3, T4 and TSH levels every 6 months to ensure dosage precision",
                    "Take thyroid medication consistently every morning on an empty stomach",
                    "Incorporate iodine, selenium, and zinc-rich foods into your daily meals"
                });
            }

            if (topic.Contains("asthma") || topic.Contains("breath") || topic.Contains("pulmon") || topic.Contains("inhaler") || topic.Contains("lung"))
            {
                return ("Respiratory & Asthma Care", "Breathe Freely: Essential Pulmonary Health", new List<string>
                {
                    "Identify and minimize exposure to smoke, dust mites, and cold drafts",
                    "Always keep your prescribed rescue inhaler accessible wherever you go",
                    "Perform daily deep breathing exercises to build vital lung capacity"
                });
            }

            if (topic.Contains("bp") || topic.Contains("hyperten") || topic.Contains("pressure"))
            {
                return ("Hypertension Management", "Keep Blood Pressure in Safe Target Zone", new List<string>
                {
                    "Limit dietary sodium intake to under 1 teaspoon (5 grams) per day",
                    "Commit to 30 minutes of brisk walking or cardio activity 5 days a week",
                    "Monitor blood pressure regularly at home and take medications as advised"
                });
            }

            if (topic.Contains("cholesterol") || topic.Contains("lipid") || topic.Contains("triglyceride"))
            {
                return ("Lipid & Cholesterol Control", "Lower Bad Cholesterol, Protect Arteries", new List<string>
                {
                    "Cut down on trans fats, processed palm oils, and fried snacks",
                    "Add oats, flaxseeds, walnuts, and soluble fiber to your daily breakfast",
                    "Check annual fasting lipid panels to guard against arterial plaque"
                });
            }

            if (topic.Contains("knee") || topic.Contains("joint") || topic.Contains("arthrit") || topic.Contains("spine") || topic.Contains("back"))
            {
                return ("Joint & Arthritis Care", "Protect Your Joints, Preserve Active Mobility", new List<string>
                {
                    "Maintain an optimal body weight to reduce stress on knees and hips",
                    "Ensure adequate daily Vitamin D3 and Calcium for cartilage integrity",
                    "Engage in low-impact exercises like swimming and gentle leg stretches"
                });
            }

            if (topic.Contains("migraine") || topic.Contains("headache"))
            {
                return ("Migraine & Headache Relief", "Recognize Triggers, Restore Calm & Focus", new List<string>
                {
                    "Maintain a consistent 7-8 hour sleep schedule without late screen exposure",
                    "Stay well-hydrated throughout the day and avoid skipping regular meals",
                    "Track specific food or stress triggers and seek clinical guidance early"
                });
            }

            if (topic.Contains("pcos") || topic.Contains("pcod") || topic.Contains("women") || topic.Contains("hormon"))
            {
                return ("Women's Hormonal Health", "Holistic Habits for PCOS & Hormone Harmony", new List<string>
                {
                    "Focus on a low glycemic index, anti-inflammatory whole-food diet",
                    "Engage in daily strength training and brisk walking for insulin sensitivity",
                    "Consult your gynecologist for periodic ultrasound and hormonal evaluations"
                });
            }

            if (topic.Contains("cancer") || topic.Contains("oncolog") || topic.Contains("tumor"))
            {
                return ("Cancer Awareness & Prevention", "Early Detection Saves Lives: Stay Informed", new List<string>
                {
                    "Undergo age-recommended annual clinical screenings and health checkups",
                    "Adopt a tobacco-free lifestyle and eliminate alcohol consumption",
                    "Maintain a nutrient-rich, plant-based diet high in natural antioxidants"
                });
            }

            // General English fallback
            string cleanTitle = !string.IsNullOrWhiteSpace(topic)
                ? System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(topic)
                : "Health & Wellness";

            return ($"{cleanTitle} Advisory", $"Essential Health Guide: {cleanTitle}", new List<string>
            {
                $"Follow clinical preventive guidelines and timely screenings for {cleanTitle.ToLower()}",
                "Maintain balanced nutrition, adequate hydration, and daily physical activity",
                "Consult your healthcare specialist early for personalized diagnosis and care"
            });
        }
    }
}
