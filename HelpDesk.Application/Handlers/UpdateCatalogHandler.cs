using System.Threading;
using System.Threading.Tasks;
using MediatR;
using HelpDesk.Application.Interfaces;
using HelpDesk.Application.Commands;
using HelpDesk.Domain.Entities.Catalog;

namespace HelpDesk.Application.Handlers
{
    public class UpdateCatalogHandler : IRequestHandler<UpdateCatalogCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public UpdateCatalogHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(UpdateCatalogCommand request, CancellationToken cancellationToken)
        {
            var catalog = await _uow.Repository<Catalog>().GetByIdAsync(request.Id);
            if (catalog == null) return false;

            catalog.UpdateName(request.Name);
            catalog.UpdateValue(request.Value);
            catalog.UpdateDescription(request.Description);
            catalog.UpdateDisplayOrder(request.DisplayOrder);

            await _uow.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
