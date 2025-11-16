using HelpDesk.Domain.Exceptions;

namespace HelpDesk.Domain.ValueObjects
{
    public class CatalogItem
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }

        private CatalogItem() { }

        public CatalogItem(string name, string type)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Catalog name is required.");

            if (string.IsNullOrWhiteSpace(type))
                throw new DomainException("Catalog type is required.");

            Name = name;
            Type = type;
        }
    }
}
