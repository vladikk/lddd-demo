using WolfDesk.Domain.Users;

namespace WolfDesk.Domain.Tickets.Messages;

public class Message
{
    public Message(MessageId id, UserId author, MessageBody body, DateTime sentOn) {
        this.Id = id;
        this.Body = body;
        this.Author = author;
        this.SentOn = sentOn;
    }

    public MessageId Id { get; }
    public MessageBody Body { get; }
    public DateTime SentOn { get; }
    public UserId Author { get; }
}