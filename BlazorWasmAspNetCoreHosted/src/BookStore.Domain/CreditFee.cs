using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace BookStore.CreditFees
{
    public class CreditFee : AuditedAggregateRoot<Guid>
    {
        public Guid StudentTypeId { get; set; }
        public Guid ModuleTypeId { get; set; }
        public StudyAttemptType StudyAttemptType { get; set; }
        public decimal FeeAmount { get; set; }
        public string AcademicYear { get; set; }
        public string Semester { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual StudentType StudentType { get; set; }
        public virtual ModuleType ModuleType { get; set; }
        
        protected CreditFee()
        {
        }

        public CreditFee(
            Guid id, 
            Guid studentTypeId, 
            Guid moduleTypeId, 
            StudyAttemptType studyAttemptType, 
            decimal feeAmount,
            string academicYear,
            string semester) : base(id)
        {
            StudentTypeId = studentTypeId;
            ModuleTypeId = moduleTypeId;
            StudyAttemptType = studyAttemptType;
            FeeAmount = feeAmount;
            AcademicYear = academicYear;
            Semester = semester;
        }
    }

    public enum StudyAttemptType
    {
        FirstAttempt = 1,      // Học lần 1
        SecondAttempt = 2,     // Học lần 2
        Improvement = 3,       // Học cải thiện
        Retake = 4,            // Học lại
        Special = 5            // Học đặc biệt
    }
}