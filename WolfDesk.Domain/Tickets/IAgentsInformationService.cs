using WolfDesk.Domain.Tickets.Values;
using WolfDesk.Domain.Users;

namespace WolfDesk.Domain.Tickets;

public interface IAgentsInformationService
{
    TimeSpan GetResponseDeadlineForAgent(UserId agentId, TicketPriority priority, TicketCategory category);
    TimeSpan GetResponseDeadlineForCustomer(UserId agentId);
    bool IsOnShift(UserId agentId, DateTime time);
}