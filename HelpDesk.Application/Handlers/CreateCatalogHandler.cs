using System.Threading;
using System.Threading.Tasks;
using MediatR;
using HelpDesk.Application.Interfaces;
using HelpDesk.Application.Commands;
using HelpDesk.Domain.Entities.Catalog;

namespace HelpDesk.Application.Handlers
{
    public class CreateCatalogHandler : IRequestHandler<CreateCatalogCommand, int>
    {
        private readonly IUnitOfWork _uow;

        public CreateCatalogHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> Handle(CreateCatalogCommand request, CancellationToken cancellationToken)
        {
            Catalog catalog;
            if (request.ParentId.HasValue)
            {
                // It's a child value
                catalog = new Catalog(request.Name, request.Value!, request.ParentId.Value, request.Description, request.DisplayOrder);
            }
            else
            {
                // It's a root catalog
                catalog = new Catalog(request.Name, request.Description, request.DisplayOrder);
            }

            await _uow.Repository<Catalog>().AddAsync(catalog);
            await _uow.SaveChangesAsync(cancellationToken);

            return catalog.Id;
        }
    }
}
