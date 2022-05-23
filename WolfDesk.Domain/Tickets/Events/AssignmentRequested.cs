using WolfDesk.Domain.Tickets.Values;

namespace WolfDesk.Domain.Tickets.Events;

public record AssignmentRequested : IDomainEvent
{
    public AssignmentRequested(TicketId ticketId, EventId eventId, TicketCategory category, TicketPriority priority,
        ProductId product, DateTime timestamp) {
        EventId = eventId;
        TicketId = ticketId;
        Timestamp = timestamp;
        Priority = priority;
        Category = category;
        Product = product;
    }

    public TicketId TicketId { get; }
    public TicketPriority Priority { get; }
    public TicketCategory Category { get; }
    public ProductId Product { get; }
    public EventId EventId { get; }
    public DateTime Timestamp { get; }
}