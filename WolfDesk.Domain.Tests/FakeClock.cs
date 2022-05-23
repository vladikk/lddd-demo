namespace WolfDesk.Domain.Tests;

public class FakeClock : IClock
{
    public DateTime? FakeTime { get; set; }
    public DateTime Now => FakeTime ?? DateTime.Now;
}