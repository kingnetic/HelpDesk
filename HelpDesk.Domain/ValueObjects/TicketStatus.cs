using HelpDesk.Domain.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace HelpDesk.Domain.ValueObjects
{
    /// <summary>
    /// Smart Enum para estados de tickets con comportamiento rico.
    /// Implementa las reglas de transición de estados en el propio enum.
    /// </summary>
    public class TicketStatus
    {
        // Instancias estáticas (como un enum tradicional)
        public static readonly TicketStatus Open = new(5, "Open", "Abierto");
        public static readonly TicketStatus Assigned = new(6, "Assigned", "Asignado");
        public static readonly TicketStatus InProgress = new(7, "InProgress", "En Progreso");
        public static readonly TicketStatus Resolved = new(8, "Resolved", "Resuelto");
        public static readonly TicketStatus Closed = new(9, "Closed", "Cerrado");

        // Propiedades
        public int Id { get; }
        public string Name { get; }
        public string DisplayName { get; }

        // Constructor privado
        private TicketStatus(int id, string name, string displayName)
        {
            Id = id;
            Name = name;
            DisplayName = displayName;
        }

        // Obtener todos los estados
        public static IEnumerable<TicketStatus> GetAll()
        {
            yield return Open;
            yield return Assigned;
            yield return InProgress;
            yield return Resolved;
            yield return Closed;
        }

        // Obtener por ID
        public static TicketStatus FromId(int id)
        {
            return GetAll().FirstOrDefault(s => s.Id == id)
                ?? throw new DomainException($"Estado de ticket inválido: {id}");
        }

        // Obtener por nombre
        public static TicketStatus FromName(string name)
        {
            return GetAll().FirstOrDefault(s => s.Name == name)
                ?? throw new DomainException($"Estado de ticket inválido: {name}");
        }

        /// <summary>
        /// Define las transiciones permitidas desde este estado.
        /// Esta es la clave del Smart Enum - cada estado conoce sus transiciones válidas.
        /// </summary>
        public IEnumerable<TicketStatus> AllowedTransitions()
        {
            // Desde Abierto
            if (this == Open)
            {
                yield return Assigned;
                yield return Closed;
            }
            // Desde Asignado
            else if (this == Assigned)
            {
                yield return InProgress;
                yield return Closed;
            }
            // Desde En Progreso
            else if (this == InProgress)
            {
                yield return Resolved;
                yield return Closed;
            }
            // Desde Resuelto
            else if (this == Resolved)
            {
                yield return Closed;
                yield return InProgress; // Reapertura
            }
            // Desde Cerrado - no hay transiciones permitidas
            // (yield return nada)
        }

        /// <summary>
        /// Valida si se puede transicionar a otro estado.
        /// </summary>
        public bool CanTransitionTo(TicketStatus newStatus)
        {
            return AllowedTransitions().Contains(newStatus);
        }

        /// <summary>
        /// Valida la transición y lanza excepción si no es válida.
        /// </summary>
        public void ValidateTransitionTo(TicketStatus newStatus)
        {
            if (!CanTransitionTo(newStatus))
            {
                throw new DomainException(GetTransitionErrorMessage(newStatus));
            }
        }

        /// <summary>
        /// Obtiene el mensaje de error específico para una transición inválida.
        /// </summary>
        private string GetTransitionErrorMessage(TicketStatus newStatus)
        {
            if (this == Open)
                return "Un ticket abierto solo puede ser asignado o cerrado.";

            if (this == Assigned)
                return "Un ticket asignado solo puede pasar a 'En Progreso' o ser cerrado.";

            if (this == InProgress)
                return "Un ticket en progreso solo puede ser resuelto o cerrado.";

            if (this == Resolved)
                return "Un ticket resuelto solo puede ser cerrado o reabierto a 'En Progreso'.";

            if (this == Closed)
                return "Un ticket cerrado no puede cambiar de estado. Debe ser reabierto primero.";

            return $"Transición inválida de {DisplayName} a {newStatus.DisplayName}.";
        }

        // Sobrescribir Equals y GetHashCode para comparación
        public override bool Equals(object? obj)
        {
            if (obj is not TicketStatus other)
                return false;

            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(TicketStatus? left, TicketStatus? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(TicketStatus? left, TicketStatus? right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
