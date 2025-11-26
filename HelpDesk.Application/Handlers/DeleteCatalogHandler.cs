using System.Threading;
using System.Threading.Tasks;
using MediatR;
using HelpDesk.Application.Interfaces;
using HelpDesk.Application.Commands;
using HelpDesk.Domain.Entities.Catalog;

namespace HelpDesk.Application.Handlers
{
    public class DeleteCatalogHandler : IRequestHandler<DeleteCatalogCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public DeleteCatalogHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(DeleteCatalogCommand request, CancellationToken cancellationToken)
        {
            var catalog = await _uow.Repository<Catalog>().GetByIdAsync(request.Id);
            if (catalog == null) return false;

            // Check if it has children? The entity doesn't enforce it but database might.
            // For now, we assume simple delete.
            
            _uow.Repository<Catalog>().Remove(catalog);
            await _uow.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
