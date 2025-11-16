using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Entities.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; protected set; }

        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(IDomainEvent ev)
        {
            _domainEvents.Add(ev);
            DomainEventsStore.Add(ev);
        }

        protected void Update() => UpdatedAt = DateTime.UtcNow;
    }
}
