using CreditFeeManagement.Models;
using CreditFeeManagement.ViewModels;

namespace CreditFeeManagement.Services
{
    public interface ICreditFeeService
    {
        Task<List<StudentType>> GetStudentTypesAsync();
        Task<List<ModulesType>> GetModulesTypesAsync();
        Task<List<CreditFee>> GetCreditFeesAsync();
        Task<CreditFeeMatrixViewModel> GetMatrixViewModelAsync();
        Task<bool> SaveCreditFeesAsync(List<CreditFee> creditFees);
        Task<bool> BulkUpdateCreditFeesAsync(List<CreditFee> creditFees);
        Task<byte[]> ExportToExcelAsync(CreditFeeMatrixViewModel viewModel);
        Task<CreditFeeMatrixViewModel> ImportFromExcelAsync(byte[] fileContent);
        Task<ValidationResult> ValidateMatrixDataAsync(CreditFeeMatrixViewModel viewModel);
    }
    
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }
}