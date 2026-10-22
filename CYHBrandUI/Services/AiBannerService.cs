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

        public AiBannerService(IConfiguration config, ILogger<AiBannerService> logger)
        {
            _config = config;
            _logger = logger;
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

                // 3. Extract Topic & Knowledge Base Content
                var (badge, headline, tips) = ResolveMedicalContent(request.Topic, request.Language, request.CustomAdvice);

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
                string designName = $"{request.Topic.Trim()} - AI Banner";
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

        private static (string Badge, string Headline, List<string> Tips) ResolveMedicalContent(string? topic, string? language, string? customAdvice)
        {
            topic = (topic ?? "").ToLowerInvariant().Trim();
            language = (language ?? "English").ToLowerInvariant().Trim();

            // Custom advice takes precedence if provided
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
