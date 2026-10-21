using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CYHBrandData.Entities
{
    [Table("TemplateCategoryMaster")]
    public class TemplateCategoryMaster
    {
        [Key]
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
