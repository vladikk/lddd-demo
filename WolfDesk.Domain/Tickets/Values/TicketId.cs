namespace WolfDesk.Domain.Tickets.Values;

public record TicketId
{
    private Guid Value { get; }

    public TicketId() {
        this.Value = Guid.NewGuid();
    }

    public TicketId(Guid value) {
        this.Value = value;
    }

    public static TicketId ParseString(string str) {
        if (!Guid.TryParse(str, out var value)) {
            throw new ArgumentException($"Invalid ticket id: {str}");
        }

        return new TicketId(value);
    }

    public override string ToString() {
        return this.Value.ToString();
    }
}