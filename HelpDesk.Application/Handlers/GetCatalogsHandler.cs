using HelpDesk.Application.DTOs;
using HelpDesk.Application.Queries;
using HelpDesk.Domain.Entities.Catalog;
using HelpDesk.Application.Interfaces;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HelpDesk.Application.Handlers
{
    public class GetCatalogsHandler : IRequestHandler<GetCatalogsQuery, CatalogsResultDto>
    {
        private readonly IGenericRepository<Catalog> _repository;

        public GetCatalogsHandler(IGenericRepository<Catalog> repository)
        {
            _repository = repository;
        }

        public async Task<CatalogsResultDto> Handle(GetCatalogsQuery request, CancellationToken cancellationToken)
        {
            // Obtener todos los catálogos
            var catalogs = await _repository.GetAllAsync(cancellationToken);
            
            // Obtener solo los catálogos padre (sin ParentId)
            var parentCatalogs = catalogs.Where(c => c.ParentId == null).ToList();
            
            // Función helper para obtener los valores de un catálogo padre
            var getChildValues = (string parentName) =>
            {
                var parent = parentCatalogs.FirstOrDefault(p => p.Name == parentName);
                if (parent == null) return new List<CatalogDto>();
                
                return catalogs
                    .Where(c => c.ParentId == parent.Id && c.IsActive)
                    .OrderBy(c => c.DisplayOrder)
                    .Select(c => new CatalogDto
                    {
                        Id = c.Id,
                        Name = c.Value ?? c.Name,
                        Type = parentName
                    })
                    .ToList();
            };

            return new CatalogsResultDto
            {
                Priorities = getChildValues("Priority"),
                Categories = getChildValues("Category"),
                Statuses = getChildValues("Status"),
                Types = getChildValues("Type")
            };
        }
    }
}
