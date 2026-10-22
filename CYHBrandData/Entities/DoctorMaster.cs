using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CYHBrandData.Entities
{
    [Table("DoctorMaster")]
    public class DoctorMaster
    {
        [Key]
        public int DoctorId { get; set; }
        public string? FullName { get; set; }
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? MobileNo { get; set; }
        public string? Specialty { get; set; }
        public string? Qualifications { get; set; }
        public string? ClinicName { get; set; }
        public string? ClinicAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PinCode { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePic { get; set; }
        public string? ClinicLogo { get; set; }
        public string? PrimaryColorHex { get; set; }
        public string? SecondaryColorHex { get; set; }
        public string? PreferredFont { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsVerified { get; set; } = false;
        public bool IsProfileComplete { get; set; } = false;
        public int RoleId { get; set; } = 1;
        public int AiCredits { get; set; } = 10;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
    }
}
