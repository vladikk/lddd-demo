namespace WolfDesk.Domain;

public record EventId
{
    private EventId(int value) {
        Value = value;
    }

    private int Value { get; }

    public static EventId FromInt(int value) {
        return new EventId(value);
    }

    public static EventId FromString(string str) {
        int value;
        if (!int.TryParse(str, out value)) {
            throw new ArgumentException($"Invalid domain event id: {str}");
        }

        return new EventId(value);
    }

    public static EventId Next(IList<IDomainEvent> current) {
        if (!current.Any()) {
            return FromInt(0);
        }

        return FromInt(current.Max(x => x.EventId.Value) + 1);
    }

    public override string ToString() {
        return Value.ToString();
    }
}