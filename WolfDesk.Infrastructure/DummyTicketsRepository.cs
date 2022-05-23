using System.Collections.Immutable;
using System.Data;
using WolfDesk.Domain.Tickets;
using WolfDesk.Domain.Tickets.Values;

namespace WolfDesk.Infrastructure;

public class DummyTicketsRepository : ITicketsRepository
{
    private readonly Dictionary<TicketId, Ticket> _db = new();

    public Ticket Load(TicketId id) {
        // ...
    }

    public void Save(Ticket ticket) {
        // SqlTransaction transaction = null;
        // SqlConnection connection = ...;

        // transaction = connection.BeginTransaction();

        try {
            var expectedVersion = ticket.Version;
            var newDomainEvents = ticket.UnpublishedDomainEvents;

            foreach (var @event in newDomainEvents) {
                // INSERT INTO EVENTS_OUTBOX
            }

            // UPDATE TICKETS WHERE ID = ticket.Id and Version = expectedVersion

            // transaction.Commit();

        }
        catch (Exception ex) {
            // transaction.Rollback();
        }
        
        ticket.MarkAsCommitted();
    }
}