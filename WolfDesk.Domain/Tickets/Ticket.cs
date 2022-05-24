using WolfDesk.Domain.Tickets.Events;
using WolfDesk.Domain.Tickets.Messages;
using WolfDesk.Domain.Tickets.Values;
using WolfDesk.Domain.Users;

namespace WolfDesk.Domain.Tickets;

public class Ticket
{
    private readonly IAgentsInformationService _agentsInformationService;
    private readonly IClock _clock;
    private readonly List<IDomainEvent> _domainEvents = new();
    private readonly List<IDomainEvent> _unpublishedDomainEvents = new();
    private readonly List<Message> _messages = new();

    public Ticket(IAgentsInformationService agentsInformationService, IClock? clock = null) {
        _clock = clock ?? new Clock();
        _agentsInformationService = agentsInformationService;
        Id = new TicketId();
    }

    public IList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public IList<IDomainEvent> UnpublishedDomainEvents => _domainEvents.AsReadOnly();
    public TicketId Id { get; }
    public UserId Customer { get; private set; }
    public Title Title { get; private set; }
    public ProductId Product { get; private set; }
    public TicketPriority Priority { get; private set; }
    public TicketCategory Category { get; private set; }
    public UserId? AssignedAgent { get; private set; }
    public DateTime? ResponseDeadline { get; private set; }
    public bool Escalated { get; private set; }
    public bool CanBeEscalated => Status == TicketStatus.PENDING_AGENT_RESPONSE && _clock.Now > ResponseDeadline!;
    public TicketStatus Status { get; private set; }
    public int Version { get; private set; } = 0;

    public void Open(UserId customer, Title title, MessageBody body, TicketPriority priority, TicketCategory category,
        ProductId product, bool escalated = false) {
        Customer = customer;
        Title = title;
        Priority = priority;
        Category = category;
        Product = product;
        Escalated = escalated;
        ResetAssignment();

        AddMessage(Customer, body);
    }

    public void AssignToAgent(UserId agentId) {
        AssignedAgent = agentId;
        Status = TicketStatus.PENDING_AGENT_RESPONSE;
        var responseTime = _agentsInformationService.GetResponseDeadlineForAgent(agentId, Priority, Category);
        if (Escalated) {
            responseTime = responseTime / 2;
        }

        ResponseDeadline = _clock.Now.Add(responseTime);
    }

    public void AddAgentResponse(MessageBody messageBody) {
        AddMessage(AssignedAgent!, messageBody);
        Status = TicketStatus.PENDING_CUSTOMER_RESPONSE;
        ResponseDeadline = _clock.Now.Add(_agentsInformationService.GetResponseDeadlineForCustomer(AssignedAgent!));
        var @event = new AgentResponded(Id, EventId.Next(_domainEvents), messageBody, Status, ResponseDeadline);
        AddDomainEvent(@event);
    }

    public void AddCustomerResponse(MessageBody messageBody) {
        AddMessage(Customer, messageBody);
        Status = TicketStatus.PENDING_AGENT_RESPONSE;
        ResponseDeadline =
            _clock.Now.Add(_agentsInformationService.GetResponseDeadlineForAgent(AssignedAgent!, Priority, Category));
    }

    public void Escalate() {
        if (!CanBeEscalated) {
            throw new InvalidOperationException("The ticket cannot be escalated");
        }

        Escalated = true;

        if (AssignedAgent != null && !_agentsInformationService.IsOnShift(AssignedAgent!, _clock.Now)) {
            ResetAssignment();
        }
    }

    public void EvaluateTimeBasedRules() {
        if (Status == TicketStatus.PENDING_CUSTOMER_RESPONSE && ResponseDeadline! <= _clock.Now) {
            Status = TicketStatus.CLOSED;
        }
    }

    public void MarkAsCommitted() {
        Version += 1;
        _unpublishedDomainEvents.Clear();
    }

    public IEnumerable<Message> GetMessages() {
        return _messages.AsReadOnly();
    }

    private void ResetAssignment() {
        AssignedAgent = null;
        Status = TicketStatus.UNASSIGNED;
        var @event = new AssignmentRequested(Id, EventId.Next(_domainEvents), Category, Priority, Product, _clock.Now);
        AddDomainEvent(@event);
    }

    private void AddMessage(UserId author, MessageBody body) {
        var id = MessageId.FromInt(_messages.Count);
        var sentOn = _clock.Now;
        var message = new Message(id, author, body, sentOn);
        _messages.Add(message);
    }

    private void AddDomainEvent(IDomainEvent @event) {
        _domainEvents.Add(@event);
        _unpublishedDomainEvents.Add(@event);
    }
}
