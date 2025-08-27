using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.CreditFees;
using BookStore.StudentTypes;
using BookStore.ModuleTypes;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace BookStore.CreditFees
{
    [Authorize]
    public class CreditFeeAppService : 
        CrudAppService<
            CreditFee,
            CreditFeeDto,
            Guid,
            GetCreditFeeListDto,
            CreateCreditFeeDto,
            UpdateCreditFeeDto>,
        ICreditFeeAppService
    {
        private readonly IRepository<StudentType, Guid> _studentTypeRepository;
        private readonly IRepository<ModuleType, Guid> _moduleTypeRepository;

        public CreditFeeAppService(
            IRepository<CreditFee, Guid> repository,
            IRepository<StudentType, Guid> studentTypeRepository,
            IRepository<ModuleType, Guid> moduleTypeRepository) 
            : base(repository)
        {
            _studentTypeRepository = studentTypeRepository;
            _moduleTypeRepository = moduleTypeRepository;
        }

        public async Task<PagedResultDto<CreditFeeDto>> GetListAsync(GetCreditFeeListDto input)
        {
            var query = Repository
                .WhereIf(!input.Filter.IsNullOrWhiteSpace(), x => 
                    x.StudentType.Name.Contains(input.Filter) || 
                    x.ModuleType.Name.Contains(input.Filter))
                .WhereIf(input.StudentTypeId.HasValue, x => x.StudentTypeId == input.StudentTypeId)
                .WhereIf(input.ModuleTypeId.HasValue, x => x.ModuleTypeId == input.ModuleTypeId)
                .WhereIf(input.StudyAttemptType.HasValue, x => x.StudyAttemptType == input.StudyAttemptType)
                .WhereIf(!input.AcademicYear.IsNullOrWhiteSpace(), x => x.AcademicYear == input.AcademicYear)
                .WhereIf(!input.Semester.IsNullOrWhiteSpace(), x => x.Semester == input.Semester);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(input.Sorting ?? nameof(CreditFee.CreationTime))
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();

            var dtos = items.Select(MapToGetListOutputDto).ToList();

            return new PagedResultDto<CreditFeeDto>
            {
                TotalCount = totalCount,
                Items = dtos
            };
        }

        public async Task<List<CreditFeeTableDto>> GetTableDataAsync(string academicYear, string semester)
        {
            var studentTypes = await _studentTypeRepository.GetListAsync(x => x.IsActive);
            var moduleTypes = await _moduleTypeRepository.GetListAsync(x => x.IsActive);
            var creditFees = await Repository.GetListAsync(x => 
                x.AcademicYear == academicYear && 
                x.Semester == semester && 
                x.IsActive);

            var result = new List<CreditFeeTableDto>();

            foreach (var studentType in studentTypes)
            {
                foreach (var moduleType in moduleTypes)
                {
                    var tableDto = new CreditFeeTableDto
                    {
                        StudentTypeId = studentType.Id,
                        StudentTypeName = studentType.Name,
                        ModuleTypeId = moduleType.Id,
                        ModuleTypeName = moduleType.Name
                    };

                    // Lấy mức phí cho từng loại học
                    var fees = creditFees.Where(x => 
                        x.StudentTypeId == studentType.Id && 
                        x.ModuleTypeId == moduleType.Id).ToList();

                    tableDto.FirstAttemptFee = fees.FirstOrDefault(x => x.StudyAttemptType == StudyAttemptType.FirstAttempt)?.FeeAmount ?? 0;
                    tableDto.SecondAttemptFee = fees.FirstOrDefault(x => x.StudyAttemptType == StudyAttemptType.SecondAttempt)?.FeeAmount ?? 0;
                    tableDto.ImprovementFee = fees.FirstOrDefault(x => x.StudyAttemptType == StudyAttemptType.Improvement)?.FeeAmount ?? 0;
                    tableDto.RetakeFee = fees.FirstOrDefault(x => x.StudyAttemptType == StudyAttemptType.Retake)?.FeeAmount ?? 0;
                    tableDto.SpecialFee = fees.FirstOrDefault(x => x.StudyAttemptType == StudyAttemptType.Special)?.FeeAmount ?? 0;

                    result.Add(tableDto);
                }
            }

            return result;
        }

        public async Task BulkCreateAsync(BulkCreditFeeInputDto input)
        {
            // Xóa dữ liệu cũ nếu có
            var existingFees = await Repository.GetListAsync(x => 
                x.AcademicYear == input.AcademicYear && 
                x.Semester == input.Semester);
            
            await Repository.DeleteManyAsync(existingFees);

            // Tạo dữ liệu mới
            var newFees = new List<CreditFee>();

            foreach (var matrix in input.FeeMatrix)
            {
                foreach (var fee in matrix.Fees)
                {
                    var creditFee = new CreditFee(
                        GuidGenerator.Create(),
                        matrix.StudentTypeId,
                        matrix.ModuleTypeId,
                        fee.Key,
                        fee.Value,
                        input.AcademicYear,
                        input.Semester
                    );
                    newFees.Add(creditFee);
                }
            }

            await Repository.InsertManyAsync(newFees);
        }

        protected override CreditFeeDto MapToGetListOutputDto(CreditFee entity)
        {
            var dto = base.MapToGetListOutputDto(entity);
            dto.StudentTypeName = entity.StudentType?.Name;
            dto.ModuleTypeName = entity.ModuleType?.Name;
            dto.StudyAttemptTypeName = GetStudyAttemptTypeName(entity.StudyAttemptType);
            return dto;
        }

        private string GetStudyAttemptTypeName(StudyAttemptType type)
        {
            return type switch
            {
                StudyAttemptType.FirstAttempt => "Học lần 1",
                StudyAttemptType.SecondAttempt => "Học lần 2",
                StudyAttemptType.Improvement => "Học cải thiện",
                StudyAttemptType.Retake => "Học lại",
                StudyAttemptType.Special => "Học đặc biệt",
                _ => type.ToString()
            };
        }
    }
}