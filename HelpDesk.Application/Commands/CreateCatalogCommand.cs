using MediatR;

namespace HelpDesk.Application.Commands
{
    public class CreateCatalogCommand : IRequest<int>
    {
        public string Name { get; set; }
        public string? Value { get; set; }
        public int? ParentId { get; set; }
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }

        public CreateCatalogCommand(string name, string? value, int? parentId, string? description, int displayOrder)
        {
            Name = name;
            Value = value;
            ParentId = parentId;
            Description = description;
            DisplayOrder = displayOrder;
        }
    }
}
