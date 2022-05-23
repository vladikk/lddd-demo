namespace WolfDesk.Domain.Tickets.Messages;

public record MessageBody
{
    private const int MAX_LENGTH = 1024;

    private MessageBody(string value) {
        Value = value.Trim();

        if (string.IsNullOrEmpty(Value)) {
            throw new ArgumentException("Body cannot be empty");
        }

        if (Value.Length > MAX_LENGTH) {
            throw new ArgumentException($"Max body length is {MAX_LENGTH} characters");
        }
    }

    private string Value { get; }

    public override string ToString() {
        return Value;
    }

    public static MessageBody FromString(string value) {
        return new MessageBody(value);
    }
}