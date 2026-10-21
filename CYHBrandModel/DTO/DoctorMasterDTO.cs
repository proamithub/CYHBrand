using System.ComponentModel.DataAnnotations;

namespace CYHBrandModel.DTO
{
    public class DoctorMasterDTO
    {
        public int DoctorId { get; set; }

        [Required, MaxLength(150)]
        public string? FullName { get; set; }
        public string? DisplayName { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }

        [Phone]
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
        public bool IsVerified { get; set; }
        public bool IsProfileComplete { get; set; }
        public int RoleId { get; set; } = 1;
        public string[]? Roles { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
