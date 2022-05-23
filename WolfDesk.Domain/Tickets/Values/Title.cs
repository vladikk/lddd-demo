namespace WolfDesk.Domain.Tickets.Values;

public record Title
{
    private const int MAX_LENGTH = 100;
    private string Value { get; }

    private Title(string value) {
        this.Value = value.Trim();

        if (string.IsNullOrEmpty(this.Value)) {
            throw new ArgumentException($"Ticket title cannot be empty");
        }

        if (this.Value.Length > MAX_LENGTH) {
            throw new ArgumentException($"Max ticket title length is {MAX_LENGTH} characters");
        }
    }

    public static Title FromString(string value) {
        return new Title(value);
    }

    public override string ToString() {
        return this.Value;
    }
}