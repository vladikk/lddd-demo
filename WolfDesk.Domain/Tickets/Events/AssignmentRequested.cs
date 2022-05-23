using WolfDesk.Domain.Tickets.Values;

namespace WolfDesk.Domain.Tickets.Events;

public record AssignmentRequested : IDomainEvent
{
    public EventId EventId { get; }
    public TicketId TicketId { get; }
    public DateTime Timestamp { get; }
    public TicketPriority Priority { get; }
    public TicketCategory Category { get; }
    public ProductId Product { get; }
        
    public AssignmentRequested(TicketId ticketId, EventId eventId, TicketCategory category, TicketPriority priority,
        ProductId product, DateTime timestamp) {
        this.EventId = eventId;
        this.TicketId = ticketId;
        this.Timestamp = timestamp;
        Priority = priority;
        Category = category;
        Product = product;
    }
}