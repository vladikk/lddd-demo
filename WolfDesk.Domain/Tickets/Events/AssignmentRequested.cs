using WolfDesk.Domain.Tickets.Values;

namespace WolfDesk.Domain.Tickets.Events;

public record AssignmentRequested : IDomainEvent
{
    public EventId EventId { get; }
    public TicketId TicketId { get; }
    public DateTime Timestamp { get; }
        
    public AssignmentRequested(EventId eventId, TicketId ticketId, DateTime timestamp) {
        this.EventId = eventId;
        this.TicketId = ticketId;
        this.Timestamp = timestamp;
    }
}