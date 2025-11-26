using HelpDesk.Domain.Entities.Common;
using HelpDesk.Domain.Exceptions;

namespace HelpDesk.Domain.Entities.Catalog
{
    /// <summary>
    /// Entidad de catálogo recursiva que permite crear jerarquías de catálogos y valores.
    /// Ejemplo: Catalog "Status" (padre) -> CatalogValue "Open", "Closed", etc. (hijos)
    /// </summary>
    public class Catalog : BaseEntity
    {
        /// <summary>
        /// Nombre del catálogo o valor
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Valor opcional (usado principalmente en nodos hijos)
        /// </summary>
        public string? Value { get; private set; }

        /// <summary>
        /// ID del catálogo padre (null si es un catálogo raíz)
        /// </summary>
        public int? ParentId { get; private set; }

        /// <summary>
        /// Referencia al catálogo padre
        /// </summary>
        public Catalog? Parent { get; private set; }

        /// <summary>
        /// Colección de catálogos hijos (valores del catálogo)
        /// </summary>
        public ICollection<Catalog> Children { get; private set; } = new List<Catalog>();

        /// <summary>
        /// Indica si este catálogo está activo
        /// </summary>
        public bool IsActive { get; private set; } = true;

        /// <summary>
        /// Orden de visualización
        /// </summary>
        public int DisplayOrder { get; private set; }

        /// <summary>
        /// Descripción opcional del catálogo
        /// </summary>
        public string? Description { get; private set; }

        // Constructor privado para EF Core
        private Catalog() { }

        /// <summary>
        /// Crea un catálogo raíz (padre)
        /// </summary>
        public Catalog(string name, string? description = null, int displayOrder = 0)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Catalog name is required.");

            Name = name;
            Description = description;
            DisplayOrder = displayOrder;
            ParentId = null;
        }

        /// <summary>
        /// Crea un valor de catálogo (hijo)
        /// </summary>
        public Catalog(string name, string value, int parentId, string? description = null, int displayOrder = 0)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Catalog name is required.");

            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Catalog value is required.");

            Name = name;
            Value = value;
            ParentId = parentId;
            Description = description;
            DisplayOrder = displayOrder;
        }

        /// <summary>
        /// Añade un valor hijo a este catálogo
        /// </summary>
        public Catalog AddChild(string name, string value, string? description = null, int displayOrder = 0)
        {
            if (ParentId.HasValue)
                throw new DomainException("Cannot add children to a catalog value. Only root catalogs can have children.");

            var child = new Catalog(name, value, Id, description, displayOrder);
            Children.Add(child);
            return child;
        }

        /// <summary>
        /// Actualiza el nombre del catálogo
        /// </summary>
        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Catalog name is required.");

            Name = name;
        }

        /// <summary>
        /// Actualiza el valor del catálogo
        /// </summary>
        public void UpdateValue(string? value)
        {
            Value = value;
        }

        /// <summary>
        /// Actualiza la descripción
        /// </summary>
        public void UpdateDescription(string? description)
        {
            Description = description;
        }

        /// <summary>
        /// Actualiza el orden de visualización
        /// </summary>
        public void UpdateDisplayOrder(int displayOrder)
        {
            DisplayOrder = displayOrder;
        }

        /// <summary>
        /// Activa el catálogo
        /// </summary>
        public void Activate()
        {
            IsActive = true;
        }

        /// <summary>
        /// Desactiva el catálogo
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
        }

        /// <summary>
        /// Verifica si este catálogo es raíz (padre)
        /// </summary>
        public bool IsRoot() => !ParentId.HasValue;

        /// <summary>
        /// Verifica si este catálogo es un valor (hijo)
        /// </summary>
        public bool IsValue() => ParentId.HasValue;
    }
}
