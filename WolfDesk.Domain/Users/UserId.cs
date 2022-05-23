namespace WolfDesk.Domain.Users;

public record UserId
{
    public UserId() {
        Value = Guid.NewGuid();
    }

    public UserId(Guid value) {
        Value = value;
    }

    private Guid Value { get; }

    public static UserId ParseString(string str) {
        if (!Guid.TryParse(str, out var value)) {
            throw new ArgumentException($"Invalid user id: {str}");
        }

        return new UserId(value);
    }

    public override string ToString() {
        return Value.ToString();
    }
}