using WolfDesk.Domain.Tickets.Values;

namespace WolfDesk.Domain.Users;

public interface IAgentsInformationService
{
    TimeSpan GetResponseDeadlineForAgent(UserId agentId, TicketPriority priority, TicketCategory category);
    TimeSpan GetResponseDeadlineForCustomer(UserId agentId);
    bool IsOnShift(UserId agentId, DateTime time);
}