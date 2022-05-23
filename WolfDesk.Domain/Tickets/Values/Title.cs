namespace WolfDesk.Domain.Tickets.Values;

public record Title
{
    private const int MAX_LENGTH = 100;

    private Title(string value) {
        Value = value.Trim();

        if (string.IsNullOrEmpty(Value)) {
            throw new ArgumentException("Ticket title cannot be empty");
        }

        if (Value.Length > MAX_LENGTH) {
            throw new ArgumentException($"Max ticket title length is {MAX_LENGTH} characters");
        }
    }

    private string Value { get; }

    public static Title FromString(string value) {
        return new Title(value);
    }

    public override string ToString() {
        return Value;
    }
}