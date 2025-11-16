using System.Collections.Generic;

namespace HelpDesk.Domain.Entities.Common
{
    internal static class DomainEventsStore
    {
        private static readonly List<IDomainEvent> _events = new();
        public static void Add(IDomainEvent ev) => _events.Add(ev);
        public static IReadOnlyCollection<IDomainEvent> ReadAll() => _events.AsReadOnly();
        public static void Clear() => _events.Clear();
    }
}
