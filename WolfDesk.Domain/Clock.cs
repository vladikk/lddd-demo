namespace WolfDesk.Domain;

public interface IClock
{
    DateTime Now { get; }
}

public class Clock : IClock
{
    public DateTime Now => DateTime.Now;
}