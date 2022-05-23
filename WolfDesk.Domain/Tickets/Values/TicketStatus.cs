namespace WolfDesk.Domain.Tickets.Values;

public enum TicketStatus
{
    UNASSIGNED,
    PENDING_CUSTOMER_RESPONSE,
    PENDING_AGENT_RESPONSE,
    CLOSED,
    SOLVED
}