using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BookStore.StudentTypes
{
    public interface IStudentTypeAppService : 
        ICrudAppService<
            StudentTypeDto,
            Guid,
            GetStudentTypeListDto,
            CreateStudentTypeDto,
            UpdateStudentTypeDto>
    {
    }
}