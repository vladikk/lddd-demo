namespace WolfDesk.Domain.Users;

public record UserId
{
    private Guid Value { get; }

    public UserId() {
        this.Value = Guid.NewGuid();
    }

    public UserId(Guid value) {
        this.Value = value;
    }

    public static UserId ParseString(string str) {
        if (!Guid.TryParse(str, out var value)) {
            throw new ArgumentException($"Invalid user id: {str}");
        }

        return new UserId(value);
    }

    public override string ToString() {
        return this.Value.ToString();
    }
}