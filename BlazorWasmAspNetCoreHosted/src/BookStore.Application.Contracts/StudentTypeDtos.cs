using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace BookStore.StudentTypes
{
    public class StudentTypeDto : AuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateStudentTypeDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
    }

    public class UpdateStudentTypeDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
        
        public bool IsActive { get; set; }
    }

    public class GetStudentTypeListDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public bool? IsActive { get; set; }
    }
}