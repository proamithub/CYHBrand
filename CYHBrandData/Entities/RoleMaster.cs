using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CYHBrandData.Entities
{
    [Table("RoleMaster")]
    public class RoleMaster
    {
        [Key]
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
