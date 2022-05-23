namespace WolfDesk.Domain;

public interface IDomainEvent
{
    EventId EventId { get; }
    DateTime Timestamp { get; }
}