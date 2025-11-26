using HelpDesk.Application.DTOs;
using MediatR;

namespace HelpDesk.Application.Queries
{
    public class GetTicketByIdQuery : IRequest<TicketDto?>
    {
        public int Id { get; set; }

        public GetTicketByIdQuery(int id)
        {
            Id = id;
        }
    }
}
