namespace CreditFeeManager.Models
{
    public class CreditFee
    {
        public int StudentTypeId { get; set; }
        public int ModuleTypeId { get; set; }
        public string StudyType { get; set; } = string.Empty; // Học lần 1, Học lần 2, Học cải thiện
        public decimal Fee { get; set; }
        
        public string StudentTypeName { get; set; } = string.Empty;
        public string ModuleTypeName { get; set; } = string.Empty;
    }
}