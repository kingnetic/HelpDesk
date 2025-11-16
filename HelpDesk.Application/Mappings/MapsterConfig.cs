using Mapster;
using HelpDesk.Application.DTOs;
using HelpDesk.Domain.Entities.HelpDesk;

namespace HelpDesk.Application.Mappings
{
    public static class MapsterConfig
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<Ticket, TicketDto>.NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.StatusId, src => src.StatusId)
                .Map(dest => dest.PriorityId, src => src.PriorityId)
                .Map(dest => dest.CategoryId, src => src.CategoryId)
                .Map(dest => dest.TypeId, src => src.TypeId)
                .Map(dest => dest.CreatedById, src => src.CreatedById)
                .Map(dest => dest.AssignedToEmployeeId, src => src.AssignedToEmployeeId)
                .Map(dest => dest.TicketCreatedAt, src => src.TicketCreatedAt)
                .Map(dest => dest.Comments, src => src.Comments);
        }
    }
}
