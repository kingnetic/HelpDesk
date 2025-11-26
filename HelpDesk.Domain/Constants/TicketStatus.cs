namespace HelpDesk.Domain.Constants
{
    /// <summary>
    /// IDs de estados de tickets según el catálogo en la base de datos.
    /// Estos valores corresponden a los registros en la tabla Catalogs con ParentId = 1 (Status).
    /// </summary>
    public static class TicketStatus
    {
        public const int Open = 5;          // Abierto
        public const int Assigned = 6;      // Asignado
        public const int InProgress = 7;    // En Progreso
        public const int Resolved = 8;      // Resuelto
        public const int Closed = 9;        // Cerrado
    }
}
