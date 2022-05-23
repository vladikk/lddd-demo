using System.Data;
using WolfDesk.Domain.Tickets;
using WolfDesk.Domain.Tickets.Values;

namespace WolfDesk.Application;

public class Tickets
{
    private readonly ITicketsRepository _repository;

    public Tickets(ITicketsRepository repository) {
        _repository = repository;
    }

    public void Escalate(TicketId id) {
        try {
            var ticket = _repository.Load(id);
            ticket.Escalate();
            _repository.Save(ticket);
        }
        catch (DBConcurrencyException ex) {
            // Notify the user
        }
    }
}