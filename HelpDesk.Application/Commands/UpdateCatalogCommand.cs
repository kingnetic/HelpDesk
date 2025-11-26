using MediatR;

namespace HelpDesk.Application.Commands
{
    public class UpdateCatalogCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Value { get; set; }
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }

        public UpdateCatalogCommand(int id, string name, string? value, string? description, int displayOrder)
        {
            Id = id;
            Name = name;
            Value = value;
            Description = description;
            DisplayOrder = displayOrder;
        }
    }
}
