using System;
using System.Collections.Generic;
using HelpDesk.Domain.Entities.Common;
using HelpDesk.Domain.Events;
using HelpDesk.Domain.Exceptions;

namespace HelpDesk.Domain.Entities.HelpDesk
{
    public class Ticket : BaseEntity
    {
        private readonly List<TicketComment> _comments = new();
        private readonly List<TicketStatusHistory> _statusHistory = new();

        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;

        public int StatusId { get; private set; }
        public int PriorityId { get; private set; }
        public int CategoryId { get; private set; }
        public int? TypeId { get; private set; }

        public int CreatedById { get; private set; }
        public int? AssignedToEmployeeId { get; private set; }

        public DateTime TicketCreatedAt { get; private set; } = DateTime.UtcNow;

        public IReadOnlyCollection<TicketComment> Comments => _comments.AsReadOnly();
        public IReadOnlyCollection<TicketStatusHistory> StatusHistory => _statusHistory.AsReadOnly();

        public Ticket() { }

        public Ticket(
            string title,
            string description,
            int createdById,
            int categoryId,
            int priorityId,
            int initialStatusId,
            int? typeId = null)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new DomainException("Title is required.");
            Title = title;
            Description = description ?? string.Empty;
            CreatedById = createdById;
            CategoryId = categoryId;
            PriorityId = priorityId;
            StatusId = initialStatusId;
            TypeId = typeId;

            AddStatusRecord(initialStatusId);
        }

        public void AssignTo(int employeeId, int assignedStatusId)
        {
            if (StatusId == assignedStatusId) throw new DomainException("Ticket is already in assigned status.");
            AssignedToEmployeeId = employeeId;
            AddDomainEvent(new TicketAssignedEvent(Id, employeeId));
            ChangeStatus(assignedStatusId);
        }

        public void Resolve(int employeeId, int resolvedStatusId)
        {
            if (AssignedToEmployeeId != employeeId) throw new DomainException("Only assigned technician can resolve this ticket.");
            ChangeStatus(resolvedStatusId);
        }

        public void ChangeStatus(int newStatusId)
        {
            if (StatusId == newStatusId) return;
            StatusId = newStatusId;
            AddStatusRecord(newStatusId);
            Update();
        }

        public void ChangePriority(int priorityId)
        {
            PriorityId = priorityId;
            Update();
        }

        public void ChangeCategory(int categoryId)
        {
            CategoryId = categoryId;
            Update();
        }

        public void ChangeType(int? typeId)
        {
            TypeId = typeId;
            Update();
        }

        public void AddComment(int userId, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment)) throw new DomainException("Comment cannot be empty.");
            var commentEntity = new TicketComment(Id, userId, comment);
            _comments.Add(commentEntity);
            AddDomainEvent(new TicketRepliedEvent(Id, userId, comment));
            Update();
        }

        public void Reply(int userId, string message) => AddComment(userId, message);

        public void Close(int closedByUserId, int closedStatusId)
        {
            if (StatusId == closedStatusId) throw new DomainException("Ticket already closed.");
            ChangeStatus(closedStatusId);
            AddDomainEvent(new TicketClosedEvent(Id, closedByUserId));
        }

        private void AddStatusRecord(int statusId)
        {
            var record = new TicketStatusHistory(Id, statusId);
            _statusHistory.Add(record);
        }
    }
}
