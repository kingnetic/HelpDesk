using HelpDesk.Application.DTOs;
using MediatR;

namespace HelpDesk.Application.Queries
{
    public class GetCatalogsQuery : IRequest<CatalogsResultDto>
    {
    }
}
