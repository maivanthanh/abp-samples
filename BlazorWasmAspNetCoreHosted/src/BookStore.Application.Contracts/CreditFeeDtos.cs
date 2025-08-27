using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace BookStore.CreditFees
{
    public class CreditFeeDto : AuditedEntityDto<Guid>
    {
        public Guid StudentTypeId { get; set; }
        public string StudentTypeName { get; set; }
        public Guid ModuleTypeId { get; set; }
        public string ModuleTypeName { get; set; }
        public StudyAttemptType StudyAttemptType { get; set; }
        public string StudyAttemptTypeName { get; set; }
        public decimal FeeAmount { get; set; }
        public string AcademicYear { get; set; }
        public string Semester { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCreditFeeDto
    {
        [Required]
        public Guid StudentTypeId { get; set; }
        
        [Required]
        public Guid ModuleTypeId { get; set; }
        
        [Required]
        public StudyAttemptType StudyAttemptType { get; set; }
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Mức phí phải lớn hơn 0")]
        public decimal FeeAmount { get; set; }
        
        [Required]
        public string AcademicYear { get; set; }
        
        [Required]
        public string Semester { get; set; }
    }

    public class UpdateCreditFeeDto
    {
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Mức phí phải lớn hơn 0")]
        public decimal FeeAmount { get; set; }
        
        [Required]
        public string AcademicYear { get; set; }
        
        [Required]
        public string Semester { get; set; }
        
        public bool IsActive { get; set; }
    }

    // DTO cho việc nhập dữ liệu hàng loạt
    public class BulkCreditFeeInputDto
    {
        [Required]
        public string AcademicYear { get; set; }
        
        [Required]
        public string Semester { get; set; }
        
        [Required]
        public List<CreditFeeMatrixDto> FeeMatrix { get; set; } = new List<CreditFeeMatrixDto>();
    }

    public class CreditFeeMatrixDto
    {
        public Guid StudentTypeId { get; set; }
        public string StudentTypeName { get; set; }
        public Guid ModuleTypeId { get; set; }
        public string ModuleTypeName { get; set; }
        public Dictionary<StudyAttemptType, decimal> Fees { get; set; } = new Dictionary<StudyAttemptType, decimal>();
    }

    // DTO cho việc hiển thị dữ liệu dạng bảng
    public class CreditFeeTableDto
    {
        public Guid StudentTypeId { get; set; }
        public string StudentTypeName { get; set; }
        public Guid ModuleTypeId { get; set; }
        public string ModuleTypeName { get; set; }
        public decimal FirstAttemptFee { get; set; }
        public decimal SecondAttemptFee { get; set; }
        public decimal ImprovementFee { get; set; }
        public decimal RetakeFee { get; set; }
        public decimal SpecialFee { get; set; }
    }

    public class GetCreditFeeListDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public Guid? StudentTypeId { get; set; }
        public Guid? ModuleTypeId { get; set; }
        public StudyAttemptType? StudyAttemptType { get; set; }
        public string AcademicYear { get; set; }
        public string Semester { get; set; }
    }
}