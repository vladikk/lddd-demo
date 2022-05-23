namespace WolfDesk.Domain.Tickets.Messages;

public record MessageBody
{
    private const int MAX_LENGTH = 1024;
    private string Value { get; }

    private MessageBody(string value) {
        this.Value = value.Trim();

        if (string.IsNullOrEmpty(this.Value)) {
            throw new ArgumentException($"Body cannot be empty");
        }

        if (this.Value.Length > MAX_LENGTH) {
            throw new ArgumentException($"Max body length is {MAX_LENGTH} characters");
        }
    }

    public override string ToString() {
        return this.Value;
    }

    public static MessageBody FromString(string value) {
        return new MessageBody(value);
    }
}