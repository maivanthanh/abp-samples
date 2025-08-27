using System;
using System.Linq;
using System.Threading.Tasks;
using BookStore.StudentTypes;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace BookStore.StudentTypes
{
    [Authorize]
    public class StudentTypeAppService : 
        CrudAppService<
            StudentType,
            StudentTypeDto,
            Guid,
            GetStudentTypeListDto,
            CreateStudentTypeDto,
            UpdateStudentTypeDto>,
        IStudentTypeAppService
    {
        public StudentTypeAppService(IRepository<StudentType, Guid> repository) 
            : base(repository)
        {
        }

        public override async Task<PagedResultDto<StudentTypeDto>> GetListAsync(GetStudentTypeListDto input)
        {
            var query = Repository
                .WhereIf(!input.Filter.IsNullOrWhiteSpace(), x => 
                    x.Name.Contains(input.Filter) || 
                    x.Description.Contains(input.Filter))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(input.Sorting ?? nameof(StudentType.Name))
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();

            var dtos = items.Select(MapToGetListOutputDto).ToList();

            return new PagedResultDto<StudentTypeDto>
            {
                TotalCount = totalCount,
                Items = dtos
            };
        }
    }
}