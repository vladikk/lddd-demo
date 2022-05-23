namespace WolfDesk.Domain.Tickets.Values;

public record TicketId
{
    public TicketId() {
        Value = Guid.NewGuid();
    }

    public TicketId(Guid value) {
        Value = value;
    }

    private Guid Value { get; }

    public static TicketId ParseString(string str) {
        if (!Guid.TryParse(str, out var value)) {
            throw new ArgumentException($"Invalid ticket id: {str}");
        }

        return new TicketId(value);
    }

    public override string ToString() {
        return Value.ToString();
    }
}