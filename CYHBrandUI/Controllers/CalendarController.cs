using CYHBrandRepo;
using CYHBrandUI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CYHBrandUI.Controllers
{
    public class IndianCalendarEvent
    {
        public int Month { get; set; }
        public int Day { get; set; }
        public string Title { get; set; } = "";
        public string Category { get; set; } = "HealthDay"; // "HealthDay" or "Festival"
        public string Icon { get; set; } = "bi-calendar-heart";
        public string BadgeColor { get; set; } = "#0EA5A0";
        public string PromptTopic { get; set; } = "";
        public string Description { get; set; } = "";
    }

    [Authorize]
    public class CalendarController : BaseController
    {
        public CalendarController(IHttpContextAccessor http, IConfiguration config, IUnitOfWork uof, IUrlEncryptor enc)
            : base(http, config, uof, enc) { }

        public IActionResult Index(int? month = null, int? year = null)
        {
            var today = DateTime.Today;
            int curMonth = month ?? today.Month;
            int curYear = year ?? today.Year;

            if (curMonth < 1) { curMonth = 12; curYear--; }
            if (curMonth > 12) { curMonth = 1; curYear++; }

            var allEvents = GetAllIndianCalendarEvents();
            var monthEvents = allEvents.Where(e => e.Month == curMonth).OrderBy(e => e.Day).ToList();

            ViewBag.CurMonth = curMonth;
            ViewBag.CurYear = curYear;
            ViewBag.MonthEvents = monthEvents;
            ViewBag.AllEvents = allEvents;

            ViewData["Title"] = "Indian Medical & Content Calendar";
            ViewData["PageSubtitle"] = "Explore upcoming Indian health awareness days, festivals, and generate 1-click AI banners.";
            return View();
        }

        public static List<IndianCalendarEvent> GetAllIndianCalendarEvents()
        {
            return new List<IndianCalendarEvent>
            {
                // January
                new IndianCalendarEvent { Month = 1, Day = 12, Title = "National Youth Day", Category = "HealthDay", Icon = "bi-person-arms-up", BadgeColor = "#0284C7", PromptTopic = "Youth Physical Fitness, Sports & Mental Strength", Description = "Swami Vivekananda Jayanti inspiring healthy lifestyle for youth." },
                new IndianCalendarEvent { Month = 1, Day = 24, Title = "National Girl Child Day", Category = "HealthDay", Icon = "bi-heart-fill", BadgeColor = "#EC4899", PromptTopic = "Girl Child Nutrition, Anemia Prevention & Healthcare", Description = "Empowering health and nutrition for every girl child in India." },
                new IndianCalendarEvent { Month = 1, Day = 26, Title = "Republic Day (Swasth Bharat)", Category = "Festival", Icon = "bi-flag-fill", BadgeColor = "#EA580C", PromptTopic = "Swasth Bharat: National Health, Hygiene & Wellness Pledge", Description = "Celebrate Republic Day with a pledge for family and national health." },
                new IndianCalendarEvent { Month = 1, Day = 30, Title = "World Leprosy Day", Category = "HealthDay", Icon = "bi-shield-check", BadgeColor = "#64748B", PromptTopic = "Leprosy Eradication, Early Diagnosis & Stigma Reduction", Description = "Promoting early skin diagnosis and complete cure for leprosy." },

                // February
                new IndianCalendarEvent { Month = 2, Day = 4, Title = "World Cancer Day", Category = "HealthDay", Icon = "bi-ribbon", BadgeColor = "#7C3AED", PromptTopic = "World Cancer Day: Early Detection & Healthy Lifestyle Screening", Description = "Close the care gap with regular cancer screenings and tobacco cessation." },
                new IndianCalendarEvent { Month = 2, Day = 10, Title = "National Deworming Day", Category = "HealthDay", Icon = "bi-capsule", BadgeColor = "#0D9488", PromptTopic = "Childhood Deworming, Clean Drinking Water & Hygiene Habits", Description = "National Deworming campaign for children across schools and anganwadis." },
                new IndianCalendarEvent { Month = 2, Day = 28, Title = "National Science & Rare Disease Day", Category = "HealthDay", Icon = "bi-virus", BadgeColor = "#4F46E5", PromptTopic = "Rare Disease Awareness & Genetic Health Counseling", Description = "Supporting families and patients dealing with rare medical conditions." },

                // March
                new IndianCalendarEvent { Month = 3, Day = 8, Title = "International Women's Day", Category = "HealthDay", Icon = "bi-gender-female", BadgeColor = "#E11D48", PromptTopic = "Women's Health, Anemia Screening, Bone Density & Self-Care", Description = "Prioritizing women's wellness, hormonal health, and preventive checkups." },
                new IndianCalendarEvent { Month = 3, Day = 16, Title = "National Immunization Day", Category = "HealthDay", Icon = "bi-eyedropper", BadgeColor = "#2563EB", PromptTopic = "National Immunization Day: Timely Child Vaccination Schedule", Description = "Protecting children through complete vaccination and polio prevention." },
                new IndianCalendarEvent { Month = 3, Day = 20, Title = "World Oral Health Day", Category = "HealthDay", Icon = "bi-emoji-smile", BadgeColor = "#059669", PromptTopic = "World Oral Health Day: Proper Brushing, Cavity & Gum Care", Description = "Promoting clean teeth, oral hygiene, and confident smiles." },
                new IndianCalendarEvent { Month = 3, Day = 24, Title = "World Tuberculosis (TB) Day", Category = "HealthDay", Icon = "bi-lungs-fill", BadgeColor = "#DC2626", PromptTopic = "Yes! We Can End TB: Early Cough Diagnosis & Complete Treatment", Description = "Raising awareness on stopping TB transmission with timely medication." },
                new IndianCalendarEvent { Month = 3, Day = 25, Title = "Holi (Skin & Eye Health)", Category = "Festival", Icon = "bi-palette-fill", BadgeColor = "#D97706", PromptTopic = "Safe & Healthy Holi: Herbal Colors, Skin Barrier & Eye Care", Description = "Celebrate Holi safely with eco-friendly organic colors and protective eye care." },

                // April
                new IndianCalendarEvent { Month = 4, Day = 7, Title = "World Health Day", Category = "HealthDay", Icon = "bi-heart-pulse-fill", BadgeColor = "#059669", PromptTopic = "World Health Day: My Health, My Right - Universal Healthcare", Description = "WHO global health day emphasizing equitable healthcare access for all." },
                new IndianCalendarEvent { Month = 4, Day = 11, Title = "National Safe Motherhood Day", Category = "HealthDay", Icon = "bi-person-heart", BadgeColor = "#DB2777", PromptTopic = "Safe Motherhood, Antenatal Care & Institutional Deliveries", Description = "Promoting maternal health and optimal infant care across India." },
                new IndianCalendarEvent { Month = 4, Day = 25, Title = "World Malaria Day", Category = "HealthDay", Icon = "bi-bug-fill", BadgeColor = "#D97706", PromptTopic = "World Malaria Day: Zero Malaria Starts with Clean Water", Description = "Preventing mosquito breeding and adopting fever management." },

                // May
                new IndianCalendarEvent { Month = 5, Day = 6, Title = "World Asthma Day", Category = "HealthDay", Icon = "bi-wind", BadgeColor = "#0284C7", PromptTopic = "World Asthma Day: Inhaler Myths, Triggers & Breathing Easy", Description = "Empowering patients to control asthma through proper medication." },
                new IndianCalendarEvent { Month = 5, Day = 8, Title = "World Red Cross & Thalassemia Day", Category = "HealthDay", Icon = "bi-droplet-half", BadgeColor = "#DC2626", PromptTopic = "Thalassemia Screening & Pre-Marital Blood Test Awareness", Description = "Educating families on hemoglobinopathies and voluntary blood donation." },
                new IndianCalendarEvent { Month = 5, Day = 16, Title = "National Dengue Day", Category = "HealthDay", Icon = "bi-shield-shaded", BadgeColor = "#B45309", PromptTopic = "National Dengue Day: Platelet Monitoring & Mosquito Prevention", Description = "Ministry of Health initiative to eradicate dengue breeding spots." },
                new IndianCalendarEvent { Month = 5, Day = 17, Title = "World Hypertension Day", Category = "HealthDay", Icon = "bi-activity", BadgeColor = "#E11D48", PromptTopic = "Measure Your Blood Pressure Accurately, Control It, Live Longer", Description = "Highlighting silent hypertension risks and salt reduction." },
                new IndianCalendarEvent { Month = 5, Day = 31, Title = "World No Tobacco Day", Category = "HealthDay", Icon = "bi-slash-circle-fill", BadgeColor = "#475569", PromptTopic = "World No Tobacco Day: Quitting Smoking & Gutkha for Clean Lungs", Description = "Protecting youth from tobacco and gutkha addiction." },

                // June
                new IndianCalendarEvent { Month = 6, Day = 5, Title = "World Environment Day", Category = "HealthDay", Icon = "bi-tree-fill", BadgeColor = "#15803D", PromptTopic = "Clean Air, Green Living & Respiratory Health Protection", Description = "Connecting environmental conservation with healthy human lungs." },
                new IndianCalendarEvent { Month = 6, Day = 14, Title = "World Blood Donor Day", Category = "HealthDay", Icon = "bi-droplet-fill", BadgeColor = "#DC2626", PromptTopic = "World Blood Donor Day: Give Blood, Give Plasma, Save Lives", Description = "Thanking voluntary blood donors and promoting life-saving donation." },
                new IndianCalendarEvent { Month = 6, Day = 21, Title = "International Yoga Day", Category = "HealthDay", Icon = "bi-yin-yang", BadgeColor = "#0D9488", PromptTopic = "International Yoga Day: Daily Asanas, Pranayama & Mental Peace", Description = "Celebrating ancient Indian yoga for physical vitality and calm minds." },

                // July
                new IndianCalendarEvent { Month = 7, Day = 1, Title = "National Doctor's Day", Category = "HealthDay", Icon = "bi-hospital-fill", BadgeColor = "#0B1B3D", PromptTopic = "National Doctor's Day: Honoring Healers & Preventive Health Checkups", Description = "Honoring Dr. B.C. Roy and celebrating the bond between patients and doctors." },
                new IndianCalendarEvent { Month = 7, Day = 11, Title = "World Population Day", Category = "HealthDay", Icon = "bi-people-fill", BadgeColor = "#0284C7", PromptTopic = "Family Planning, Reproductive Health & Sustainable Families", Description = "Promoting informed family health and gender equality." },
                new IndianCalendarEvent { Month = 7, Day = 28, Title = "World Hepatitis Day", Category = "HealthDay", Icon = "bi-shield-check", BadgeColor = "#D97706", PromptTopic = "World Hepatitis Day: Hepatitis B Vaccination & Liver Care", Description = "Eliminating viral hepatitis through safe injection practices and screening." },

                // August
                new IndianCalendarEvent { Month = 8, Day = 1, Title = "World Breastfeeding Week", Category = "HealthDay", Icon = "bi-person-heart", BadgeColor = "#DB2777", PromptTopic = "Exclusive Breastfeeding for 6 Months: Immunity & Infant Growth", Description = "Empowering mothers with lactation guidance and baby immunity." },
                new IndianCalendarEvent { Month = 8, Day = 15, Title = "Independence Day (Swasthya)", Category = "Festival", Icon = "bi-flag-fill", BadgeColor = "#EA580C", PromptTopic = "Swastha Bharat, Samriddh Bharat: Freedom from Lifestyle Diseases", Description = "Celebrate 15th August with a national health independence commitment." },
                new IndianCalendarEvent { Month = 8, Day = 25, Title = "National Eye Donation Fortnight", Category = "HealthDay", Icon = "bi-eye-fill", BadgeColor = "#0284C7", PromptTopic = "National Eye Donation Fortnight: Pledge Your Eyes, Gift Vision", Description = "Dispelling corneal donation myths and giving the gift of sight." },

                // September
                new IndianCalendarEvent { Month = 9, Day = 1, Title = "National Nutrition Week", Category = "HealthDay", Icon = "bi-apple", BadgeColor = "#16A34A", PromptTopic = "National Nutrition Week: Balanced Thali, Micronutrients & Pure Food", Description = "Promoting balanced dietary intake and reducing junk food in Indian households." },
                new IndianCalendarEvent { Month = 9, Day = 21, Title = "World Alzheimer's Day", Category = "HealthDay", Icon = "bi-brain", BadgeColor = "#7C3AED", PromptTopic = "World Alzheimer's Day: Cognitive Health & Elderly Memory Care", Description = "Supporting senior citizens and understanding early dementia symptoms." },
                new IndianCalendarEvent { Month = 9, Day = 29, Title = "World Heart Day", Category = "HealthDay", Icon = "bi-heart-pulse-fill", BadgeColor = "#DC2626", PromptTopic = "World Heart Day: 3 Proven Rules to Protect Your Heart", Description = "Global campaign encouraging daily cardio, clean diet, and lipid checks." },

                // October
                new IndianCalendarEvent { Month = 10, Day = 1, Title = "International Day for Elderly", Category = "HealthDay", Icon = "bi-person-walking", BadgeColor = "#475569", PromptTopic = "Geriatric Wellness, Joint Mobility & Senior Health Screenings", Description = "Caring for our elders with regular blood pressure, vision, and bone care." },
                new IndianCalendarEvent { Month = 10, Day = 10, Title = "World Mental Health Day", Category = "HealthDay", Icon = "bi-emoji-smile-fill", BadgeColor = "#4F46E5", PromptTopic = "World Mental Health Day: Break the Silence, Prioritize Peace of Mind", Description = "Promoting psychological wellness, reducing screen fatigue, and therapy." },
                new IndianCalendarEvent { Month = 10, Day = 12, Title = "World Arthritis Day", Category = "HealthDay", Icon = "bi-person-arms-up", BadgeColor = "#D97706", PromptTopic = "World Arthritis Day: Joint Lubrication, Calcium & Daily Mobility", Description = "Managing osteoarthritis and rheumatoid pain through early therapy." },
                new IndianCalendarEvent { Month = 10, Day = 20, Title = "World Osteoporosis Day", Category = "HealthDay", Icon = "bi-body-text", BadgeColor = "#2563EB", PromptTopic = "World Osteoporosis Day: Strong Bones with Calcium & Vitamin D3", Description = "Preventing silent fractures with bone mineral density tests." },
                new IndianCalendarEvent { Month = 10, Day = 24, Title = "World Polio Day", Category = "HealthDay", Icon = "bi-droplet-half", BadgeColor = "#059669", PromptTopic = "World Polio Day: Two Drops of Life, Keeping India Polio-Free", Description = "Sustaining India's polio eradication milestone with booster drops." },
                new IndianCalendarEvent { Month = 10, Day = 29, Title = "World Stroke Day", Category = "HealthDay", Icon = "bi-lightning-charge-fill", BadgeColor = "#DC2626", PromptTopic = "World Stroke Day: Act F.A.S.T. to Save Brain Cells", Description = "Recognizing Face, Arms, Speech and Time signs of brain stroke." },
                new IndianCalendarEvent { Month = 10, Day = 31, Title = "Diwali (Green & Safe Celebrations)", Category = "Festival", Icon = "bi-lamp-fill", BadgeColor = "#EAB308", PromptTopic = "Healthy Diwali: Diabetes-Safe Sweets, Air Quality & Burn Safety", Description = "Wishes for a prosperous Diwali with respiratory health and sugar control." },

                // November
                new IndianCalendarEvent { Month = 11, Day = 7, Title = "National Cancer Awareness Day", Category = "HealthDay", Icon = "bi-ribbon", BadgeColor = "#7C3AED", PromptTopic = "National Cancer Awareness: Madame Curie Legacy & Early Screenings", Description = "Empowering Indian citizens with early warning cancer symptoms." },
                new IndianCalendarEvent { Month = 11, Day = 12, Title = "World Pneumonia Day", Category = "HealthDay", Icon = "bi-lungs-fill", BadgeColor = "#0284C7", PromptTopic = "World Pneumonia Day: Every Breath Counts, Protect Children's Lungs", Description = "Stopping respiratory infections through hygiene and pneumococcal vaccines." },
                new IndianCalendarEvent { Month = 11, Day = 14, Title = "World Diabetes Day", Category = "HealthDay", Icon = "bi-circle", BadgeColor = "#2563EB", PromptTopic = "World Diabetes Day: Know Your Risk, Control Fasting Blood Sugar", Description = "Educating families on low-GI diet, HbA1c tests, and daily brisk walking." },
                new IndianCalendarEvent { Month = 11, Day = 18, Title = "Antimicrobial Resistance (AMR) Week", Category = "HealthDay", Icon = "bi-capsule-pill", BadgeColor = "#DC2626", PromptTopic = "Stop Overusing Antibiotics: Prevent Superbugs & Drug Resistance", Description = "Do not take antibiotics without doctor prescription for viral cough/flu." },

                // December
                new IndianCalendarEvent { Month = 12, Day = 1, Title = "World AIDS Day", Category = "HealthDay", Icon = "bi-shield-fill-exclamation", BadgeColor = "#E11D48", PromptTopic = "World AIDS Day: Let Communities Lead, Safe Practices & Zero Stigma", Description = "Global solidarity in ending HIV transmission and standing with patients." },
                new IndianCalendarEvent { Month = 12, Day = 12, Title = "Universal Health Coverage Day", Category = "HealthDay", Icon = "bi-globe", BadgeColor = "#059669", PromptTopic = "Health For All: Affordable, Accessible Quality Healthcare", Description = "Promoting preventive care so no family is driven into medical poverty." }
            };
        }
    }
}