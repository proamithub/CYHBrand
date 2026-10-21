namespace CYHBrandUI.Helpers
{
    public class CustomPrincipal
    {
        public int      UserId            { get; set; }
        public string?  Name              { get; set; }
        public string?  Email             { get; set; }
        public string?  MobileNo          { get; set; }
        public string[] Roles             { get; set; } = Array.Empty<string>();
        public string?  ProfilePic        { get; set; }
        public string?  FullName          { get; set; }
        public string?  Specialty         { get; set; }
        public string?  ClinicName        { get; set; }
        public bool     IsProfileComplete { get; set; }
    }
}
