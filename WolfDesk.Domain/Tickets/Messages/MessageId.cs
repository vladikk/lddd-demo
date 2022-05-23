namespace WolfDesk.Domain.Tickets.Messages;

public record MessageId
{
    private MessageId(int value) {
        Value = value;
    }

    private int Value { get; }

    public static MessageId FromString(string str) {
        int value;
        if (!int.TryParse(str, out value)) {
            throw new ArgumentException($"Invalid message id: {str}");
        }

        return new MessageId(value);
    }

    public static MessageId FromInt(int value) {
        return new MessageId(value);
    }

    public override string ToString() {
        return Value.ToString();
    }
}