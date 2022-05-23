using WolfDesk.Domain.Tickets.Values;

namespace WolfDesk.Domain.Tickets;

public interface ITicketsRepository
{
     Ticket Load(TicketId id);
     void Save(Ticket ticket);
     
     // Queries required by the UI
}