using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDesk.Domain.Entities.Auth
{
    /// <summary>
    /// Relación entre roles de Identity y permisos personalizados.
    /// Usa RoleId como clave foránea para evitar duplicación de tablas.
    /// </summary>
    public class RolePermission
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Id del rol (debe coincidir con AspNetRoles.Id)
        /// </summary>
        [Required]
        public int RoleId { get; set; }

        /// <summary>
        /// Nombre del permiso (ej. "AssignTicket", "ViewAuditLogs")
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Permission { get; set; } = string.Empty;
    }
}
