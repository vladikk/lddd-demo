using WolfDesk.Domain.Tickets.Values;

namespace WolfDesk.Domain.Tickets.Events;

public record AgentResponded : IDomainEvent
{
    public AgentResponded(TicketId ticketId, EventId eventId, MessageBody messageBody, TicketStatus ticketStatus, DateTime? responseDeadline) {
        EventId = eventId;
        TicketId = ticketId;
        MessageBody = messageBody;
        TicketStatus = ticketStatus;
        ResponseDeadline = responseDeadline;
    }

    public TicketId TicketId { get; }
    public EventId EventId { get; }
    public MessageBody MessageBody { get; }
    public TicketStatus TicketStatus { get; }
    public DateTime? responseDeadline { get; }
}
