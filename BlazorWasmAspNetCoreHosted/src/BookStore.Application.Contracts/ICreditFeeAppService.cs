using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BookStore.CreditFees
{
    public interface ICreditFeeAppService : 
        ICrudAppService<
            CreditFeeDto,
            Guid,
            GetCreditFeeListDto,
            CreateCreditFeeDto,
            UpdateCreditFeeDto>
    {
        Task<List<CreditFeeTableDto>> GetTableDataAsync(string academicYear, string semester);
        Task BulkCreateAsync(BulkCreditFeeInputDto input);
    }
}