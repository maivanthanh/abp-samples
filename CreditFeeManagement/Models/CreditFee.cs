using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditFeeManagement.Models
{
    public class CreditFee
    {
        public int Id { get; set; }
        
        [Required]
        public int StudentTypeId { get; set; }
        
        [Required]
        public int ModulesTypeId { get; set; }
        
        [Required]
        public StudyType StudyType { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,0)")]
        public decimal FeePerCredit { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        
        // Navigation properties
        public virtual StudentType StudentType { get; set; }
        public virtual ModulesType ModulesType { get; set; }
    }
}