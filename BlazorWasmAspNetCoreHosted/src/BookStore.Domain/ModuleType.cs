using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace BookStore.ModuleTypes
{
    public class ModuleType : AuditedAggregateRoot<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        
        protected ModuleType()
        {
        }

        public ModuleType(Guid id, string name, string description = null) : base(id)
        {
            Name = name;
            Description = description;
        }
    }
}