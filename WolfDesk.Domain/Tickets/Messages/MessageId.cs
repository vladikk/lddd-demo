namespace WolfDesk.Domain.Tickets.Messages;

public record MessageId
{
    private int Value { get; }

    private MessageId(int value) {
        this.Value = value;
    }

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
        return this.Value.ToString();
    }
}