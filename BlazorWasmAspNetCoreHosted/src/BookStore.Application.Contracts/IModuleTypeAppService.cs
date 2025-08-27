using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BookStore.ModuleTypes
{
    public interface IModuleTypeAppService : 
        ICrudAppService<
            ModuleTypeDto,
            Guid,
            GetModuleTypeListDto,
            CreateModuleTypeDto,
            UpdateModuleTypeDto>
    {
    }
}