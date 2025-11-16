using System;

namespace HelpDesk.Domain.Entities.Common
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
