namespace WolfDesk.Domain.Tickets.Values;

public record ProductId
{
    public ProductId() {
        Value = Guid.NewGuid();
    }

    public ProductId(Guid value) {
        Value = value;
    }

    private Guid Value { get; }

    public static ProductId ParseString(string str) {
        if (!Guid.TryParse(str, out var value)) {
            throw new ArgumentException($"Invalid user id: {str}");
        }

        return new ProductId(value);
    }

    public override string ToString() {
        return Value.ToString();
    }
}