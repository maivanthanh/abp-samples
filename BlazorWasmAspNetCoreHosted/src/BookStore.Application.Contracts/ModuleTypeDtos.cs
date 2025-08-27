using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace BookStore.ModuleTypes
{
    public class ModuleTypeDto : AuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateModuleTypeDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
    }

    public class UpdateModuleTypeDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
        
        public bool IsActive { get; set; }
    }

    public class GetModuleTypeListDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public bool? IsActive { get; set; }
    }
}