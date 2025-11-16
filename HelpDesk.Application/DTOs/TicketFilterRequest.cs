namespace HelpDesk.Application.DTOs
{
    public class TicketFilterRequest
    {
        // filtros opcionales
        public int? StatusId { get; set; }
        public int? CategoryId { get; set; }
        public int? TypeId { get; set; }
        public int? PriorityId { get; set; }
        public int? AssignedToEmployeeId { get; set; }

        // rango de fechas (inclusive)
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }

        // paginación
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
