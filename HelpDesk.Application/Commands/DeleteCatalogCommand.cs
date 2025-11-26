using MediatR;

namespace HelpDesk.Application.Commands
{
    public class DeleteCatalogCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public DeleteCatalogCommand(int id)
        {
            Id = id;
        }
    }
}
