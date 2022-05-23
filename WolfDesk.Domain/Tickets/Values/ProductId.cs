namespace WolfDesk.Domain.Tickets.Values;

public record ProductId
{
    private Guid Value { get; }

    public ProductId() {
        this.Value = Guid.NewGuid();
    }

    public ProductId(Guid value) {
        this.Value = value;
    }

    public static ProductId ParseString(string str) {
        if (!Guid.TryParse(str, out var value)) {
            throw new ArgumentException($"Invalid user id: {str}");
        }

        return new ProductId(value);
    }

    public override string ToString() {
        return this.Value.ToString();
    }
}