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
            int timeoutSec = _config.GetValue<int>("AIContentGeneration:TimeoutSeconds", 15);
            client.Timeout = TimeSpan.FromSeconds(timeoutSec > 0 ? timeoutSec : 15);

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

            // If no API key configured, use 100% Free Online AI Engine (Zero Cost, No Key Needed)
            _logger.LogInformation("Using 100% Free Online AI Engine (isRandom: {IsRandom}) for topic: {Topic}", isRandom, targetTopic);
            return await CallFreeOnlineAiAsync(client, targetTopic, targetLang, isRandom);
        }

        private async Task<AiMedicalContentDto?> CallFreeOnlineAiAsync(HttpClient client, string topic, string language, bool isRandom)
        {
            try
            {
                string systemPrompt;
                string userPrompt;
                string seed = $"{DateTime.UtcNow:HHmmssfff}_{Guid.NewGuid():N}";

                if (isRandom)
                {
                    systemPrompt = "You are an expert creative clinical medical copywriter for doctors and healthcare clinics in India. " +
                                  "Pick a completely RANDOM, FRESH and UNIQUE medical awareness or healthcare domain out of 40+ clinical fields " +
                                  "(such as Nephrology, Endocrinology, Dermatology, Ophthalmology, Sleep Science, Geriatrics, Cardiology, Orthopedics, " +
                                  "Pediatrics, Gastroenterology, Pulmonology, Mental Health, Seasonal Infections, Ayurveda & Yoga, Dental Hygiene, Ergonomics, etc.). " +
                                  "Every single response MUST choose a distinct health topic. " +
                                  "Return ONLY a raw valid JSON object with EXACTLY three fields:\n" +
                                  "1) \"badge\": Short category name or campaign tag (2-4 words).\n" +
                                  "2) \"headline\": Catchy, inspiring, patient-friendly medical headline (under 8 words).\n" +
                                  "3) \"tips\": An array of exactly 3 concise, practical, medically sound prevention tips (each 8-14 words).\n" +
                                  "Respond in the specified language. Do NOT use markdown code blocks or any explanation outside JSON.";

                    userPrompt = $"Generate a fresh, completely random, unexpected clinical medical awareness banner for a doctor in {language}. " +
                                 $"Random Seed / Unique ID: {seed}";
                }
                else
                {
                    systemPrompt = "You are an expert clinical medical copywriter for a doctor branding platform. " +
                                  "Generate high-impact, medically accurate health education content. " +
                                  "Return ONLY a raw valid JSON object with EXACTLY three fields:\n" +
                                  "1) \"badge\": Short category name or campaign tag (2-4 words, e.g. 'HEART HEALTH CARE', 'मधुमेह नियंत्रण').\n" +
                                  "2) \"headline\": Inspiring, patient-friendly medical headline (under 8 words).\n" +
                                  "3) \"tips\": An array of exactly 3 concise, practical, medically sound prevention tips (each 8-14 words).\n" +
                                  "Respond in the specified language. Do NOT use markdown code blocks or any explanation outside JSON.";

                    userPrompt = $"Medical Topic: {topic}\nLanguage: {language}\nUnique Seed: {seed}";
                }

                var requestBody = new
                {
                    messages = new object[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = userPrompt }
                    },
                    jsonMode = true
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://text.pollinations.ai/", jsonContent);
                if (!response.IsSuccessStatusCode)
                {
                    string err = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Free Online AI returned status {Status}: {Error}", response.StatusCode, err);
                    return null;
                }

                string respString = await response.Content.ReadAsStringAsync();
                return ParseAiContentJson(respString);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "CallFreeOnlineAiAsync failed for topic: {Topic}", topic);
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

            var response = await client.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode)
            {
                string err = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("OpenAI API returned status {Status}: {Error}", response.StatusCode, err);
                return null;
            }

            string respString = await response.Content.ReadAsStringAsync();
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
            var response = await client.PostAsync(url, jsonContent);
            if (!response.IsSuccessStatusCode)
            {
                string err = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Gemini API returned status {Status}: {Error}", response.StatusCode, err);
                return null;
            }

            string respString = await response.Content.ReadAsStringAsync();
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
                var randomKeys = new[] { "heart", "diabet", "yoga", "doctor", "diwali", "blood", "dengue", "child", "eye", "dent", "mental", "bone", "monsoon" };
                topic = randomKeys[Random.Shared.Next(randomKeys.Length)];
            }

            // Hindi language bank
            if (language == "hindi")
            {
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

                // General Hindi fallback
                return ("स्वास्थ्य सलाह", "स्वस्थ जीवनशैली अपनाएं, निरोगी रहें", new List<string>
                {
                    "प्रतिदिन 8-10 गिलास स्वच्छ पानी पिएं",
                    "ताजे फल, हरी सब्जियां और पौष्टिक आहार लें",
                    "किसी भी समस्या के लिए समय पर विशेषज्ञ डॉक्टर से परामर्श लें"
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

            // General English fallback
            string fallbackHeadline = string.IsNullOrWhiteSpace(topic) ? "Prioritize Your Health Every Day" : $"Health Guide: {char.ToUpper(topic[0]) + topic[1..]}";
            return ("Doctor's Health Advisory", fallbackHeadline, new List<string>
            {
                "Stay well hydrated and maintain daily physical mobility",
                "Consume balanced, fresh, and nutrient-dense home meals",
                "Consult your healthcare provider for periodic preventive health checks"
            });
        }
    }
}
