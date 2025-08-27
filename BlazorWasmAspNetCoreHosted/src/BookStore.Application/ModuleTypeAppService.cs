using System;
using System.Linq;
using System.Threading.Tasks;
using BookStore.ModuleTypes;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace BookStore.ModuleTypes
{
    [Authorize]
    public class ModuleTypeAppService : 
        CrudAppService<
            ModuleType,
            ModuleTypeDto,
            Guid,
            GetModuleTypeListDto,
            CreateModuleTypeDto,
            UpdateModuleTypeDto>,
        IModuleTypeAppService
    {
        public ModuleTypeAppService(IRepository<ModuleType, Guid> repository) 
            : base(repository)
        {
        }

        public override async Task<PagedResultDto<ModuleTypeDto>> GetListAsync(GetModuleTypeListDto input)
        {
            var query = Repository
                .WhereIf(!input.Filter.IsNullOrWhiteSpace(), x => 
                    x.Name.Contains(input.Filter) || 
                    x.Description.Contains(input.Filter))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(input.Sorting ?? nameof(ModuleType.Name))
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();

            var dtos = items.Select(MapToGetListOutputDto).ToList();

            return new PagedResultDto<ModuleTypeDto>
            {
                TotalCount = totalCount,
                Items = dtos
            };
        }
    }
}