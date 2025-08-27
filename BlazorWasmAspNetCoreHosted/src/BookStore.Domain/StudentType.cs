using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace BookStore.StudentTypes
{
    public class StudentType : AuditedAggregateRoot<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        
        protected StudentType()
        {
        }

        public StudentType(Guid id, string name, string description = null) : base(id)
        {
            Name = name;
            Description = description;
        }
    }
}