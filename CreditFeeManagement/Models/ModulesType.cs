using System.ComponentModel.DataAnnotations;

namespace CreditFeeManagement.Models
{
    public class ModulesType
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(50)]
        public string Code { get; set; }
        
        public string Description { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}