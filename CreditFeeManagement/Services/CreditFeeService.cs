using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using CreditFeeManagement.Models;
using CreditFeeManagement.ViewModels;
using System.Data;

namespace CreditFeeManagement.Services
{
    public class CreditFeeService : ICreditFeeService
    {
        private readonly IDbContextFactory<CreditFeeDbContext> _contextFactory;
        private readonly ILogger<CreditFeeService> _logger;

        public CreditFeeService(
            IDbContextFactory<CreditFeeDbContext> contextFactory,
            ILogger<CreditFeeService> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<List<StudentType>> GetStudentTypesAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.StudentTypes
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<ModulesType>> GetModulesTypesAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.ModulesTypes
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<CreditFee>> GetCreditFeesAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.CreditFees
                .Include(x => x.StudentType)
                .Include(x => x.ModulesType)
                .ToListAsync();
        }

        public async Task<CreditFeeMatrixViewModel> GetMatrixViewModelAsync()
        {
            var viewModel = new CreditFeeMatrixViewModel();
            
            // Load master data
            viewModel.StudentTypes = await GetStudentTypesAsync();
            viewModel.ModulesTypes = await GetModulesTypesAsync();
            
            // Initialize matrix
            viewModel.InitializeMatrix();
            
            // Load existing credit fees
            var existingFees = await GetCreditFeesAsync();
            foreach (var fee in existingFees)
            {
                viewModel.SetFee(fee.StudentTypeId, fee.ModulesTypeId, fee.StudyType, fee.FeePerCredit);
            }
            
            // Set default selected student type
            if (viewModel.StudentTypes.Any())
            {
                viewModel.SelectedStudentTypeId = viewModel.StudentTypes.First().Id;
            }
            
            return viewModel;
        }

        public async Task<bool> SaveCreditFeesAsync(List<CreditFee> creditFees)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                using var transaction = await context.Database.BeginTransactionAsync();

                // Remove existing fees for the same combinations
                var keysToRemove = creditFees.Select(cf => new { cf.StudentTypeId, cf.ModulesTypeId, cf.StudyType }).ToList();
                
                foreach (var key in keysToRemove)
                {
                    var existing = await context.CreditFees
                        .FirstOrDefaultAsync(cf => 
                            cf.StudentTypeId == key.StudentTypeId &&
                            cf.ModulesTypeId == key.ModulesTypeId &&
                            cf.StudyType == key.StudyType);
                    
                    if (existing != null)
                    {
                        context.CreditFees.Remove(existing);
                    }
                }

                // Add new fees
                foreach (var fee in creditFees.Where(f => f.FeePerCredit > 0))
                {
                    fee.CreatedDate = DateTime.Now;
                    context.CreditFees.Add(fee);
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                _logger.LogInformation($"Successfully saved {creditFees.Count} credit fee records");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving credit fees");
                return false;
            }
        }

        public async Task<bool> BulkUpdateCreditFeesAsync(List<CreditFee> creditFees)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                
                // Use bulk operations for better performance
                var chunks = creditFees.Chunk(1000); // Process in chunks of 1000
                
                foreach (var chunk in chunks)
                {
                    await SaveCreditFeesAsync(chunk.ToList());
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk update credit fees");
                return false;
            }
        }

        public async Task<byte[]> ExportToExcelAsync(CreditFeeMatrixViewModel viewModel)
        {
            try
            {
                using var workbook = new XLWorkbook();
                
                foreach (var studentType in viewModel.StudentTypes)
                {
                    var worksheet = workbook.Worksheets.Add(studentType.Name);
                    
                    // Headers
                    worksheet.Cell(1, 1).Value = "Module Type";
                    var col = 2;
                    foreach (StudyType studyType in viewModel.StudyTypes)
                    {
                        worksheet.Cell(1, col).Value = studyType.GetDisplayName();
                        col++;
                    }
                    
                    // Data
                    var row = 2;
                    foreach (var moduleType in viewModel.ModulesTypes)
                    {
                        worksheet.Cell(row, 1).Value = moduleType.Name;
                        col = 2;
                        foreach (StudyType studyType in viewModel.StudyTypes)
                        {
                            var fee = viewModel.GetFee(studentType.Id, moduleType.Id, studyType);
                            worksheet.Cell(row, col).Value = fee;
                            worksheet.Cell(row, col).Style.NumberFormat.Format = "#,##0";
                            col++;
                        }
                        row++;
                    }
                    
                    // Formatting
                    var range = worksheet.Range(1, 1, row - 1, col - 1);
                    range.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                    range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    
                    var headerRange = worksheet.Range(1, 1, 1, col - 1);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    
                    worksheet.ColumnsUsed().AdjustToContents();
                }
                
                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting to Excel");
                throw;
            }
        }

        public async Task<CreditFeeMatrixViewModel> ImportFromExcelAsync(byte[] fileContent)
        {
            try
            {
                using var stream = new MemoryStream(fileContent);
                using var workbook = new XLWorkbook(stream);
                
                var viewModel = await GetMatrixViewModelAsync();
                
                foreach (var worksheet in workbook.Worksheets)
                {
                    var studentType = viewModel.StudentTypes.FirstOrDefault(st => st.Name == worksheet.Name);
                    if (studentType == null) continue;
                    
                    // Read headers to map study types
                    var studyTypeMapping = new Dictionary<int, StudyType>();
                    for (int col = 2; col <= worksheet.LastColumnUsed().ColumnNumber(); col++)
                    {
                        var headerValue = worksheet.Cell(1, col).GetString();
                        var studyType = viewModel.StudyTypes.FirstOrDefault(st => st.GetDisplayName() == headerValue);
                        if (studyType != default(StudyType))
                        {
                            studyTypeMapping[col] = studyType;
                        }
                    }
                    
                    // Read data
                    for (int row = 2; row <= worksheet.LastRowUsed().RowNumber(); row++)
                    {
                        var moduleTypeName = worksheet.Cell(row, 1).GetString();
                        var moduleType = viewModel.ModulesTypes.FirstOrDefault(mt => mt.Name == moduleTypeName);
                        if (moduleType == null) continue;
                        
                        foreach (var mapping in studyTypeMapping)
                        {
                            var cellValue = worksheet.Cell(row, mapping.Key).GetValue();
                            if (decimal.TryParse(cellValue.ToString(), out decimal fee))
                            {
                                viewModel.SetFee(studentType.Id, moduleType.Id, mapping.Value, fee);
                            }
                        }
                    }
                }
                
                return viewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing from Excel");
                throw;
            }
        }

        public async Task<ValidationResult> ValidateMatrixDataAsync(CreditFeeMatrixViewModel viewModel)
        {
            var result = new ValidationResult { IsValid = true };
            
            try
            {
                // Validate business rules
                foreach (var studentTypeKvp in viewModel.MatrixData)
                {
                    var studentType = viewModel.StudentTypes.FirstOrDefault(st => st.Id == studentTypeKvp.Key);
                    if (studentType == null)
                    {
                        result.Errors.Add($"Student type with ID {studentTypeKvp.Key} not found");
                        continue;
                    }
                    
                    foreach (var moduleTypeKvp in studentTypeKvp.Value)
                    {
                        var moduleType = viewModel.ModulesTypes.FirstOrDefault(mt => mt.Id == moduleTypeKvp.Key);
                        if (moduleType == null)
                        {
                            result.Errors.Add($"Module type with ID {moduleTypeKvp.Key} not found");
                            continue;
                        }
                        
                        foreach (var studyTypeKvp in moduleTypeKvp.Value)
                        {
                            var fee = studyTypeKvp.Value;
                            
                            // Validation rules
                            if (fee < 0)
                            {
                                result.Errors.Add($"Phí âm không hợp lệ cho {studentType.Name} - {moduleType.Name} - {studyTypeKvp.Key.GetDisplayName()}");
                            }
                            
                            if (fee > 10000000) // 10 triệu
                            {
                                result.Warnings.Add($"Phí cao bất thường ({fee:N0}) cho {studentType.Name} - {moduleType.Name} - {studyTypeKvp.Key.GetDisplayName()}");
                            }
                            
                            // Business rule: Học lần 2 should be >= Học lần 1
                            if (studyTypeKvp.Key == StudyType.HocLan2)
                            {
                                var lan1Fee = moduleTypeKvp.Value.GetValueOrDefault(StudyType.HocLan1, 0);
                                if (fee > 0 && lan1Fee > 0 && fee < lan1Fee)
                                {
                                    result.Warnings.Add($"Phí học lần 2 thấp hơn học lần 1 cho {studentType.Name} - {moduleType.Name}");
                                }
                            }
                            
                            // Business rule: Học cải thiện should be >= Học lần 1
                            if (studyTypeKvp.Key == StudyType.HocCaiThien)
                            {
                                var lan1Fee = moduleTypeKvp.Value.GetValueOrDefault(StudyType.HocLan1, 0);
                                if (fee > 0 && lan1Fee > 0 && fee < lan1Fee)
                                {
                                    result.Warnings.Add($"Phí học cải thiện thấp hơn học lần 1 cho {studentType.Name} - {moduleType.Name}");
                                }
                            }
                        }
                    }
                }
                
                if (result.Errors.Any())
                {
                    result.IsValid = false;
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating matrix data");
                result.IsValid = false;
                result.Errors.Add($"Validation error: {ex.Message}");
                return result;
            }
        }
    }
}