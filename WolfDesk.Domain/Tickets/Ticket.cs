using WolfDesk.Domain.Tickets.Events;
using WolfDesk.Domain.Tickets.Messages;
using WolfDesk.Domain.Tickets.Values;
using WolfDesk.Domain.Users;

namespace WolfDesk.Domain.Tickets
{
    public class Ticket
    {
        private IClock _clock;
        private IAgentsInformationService _agentsInformationService;
        private List<IDomainEvent> _domainEvents = new();
        private List<Message> _messages = new();
        public IList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
        public TicketId Id { get; }
        public UserId Customer { get; private set; }
        public Title Title { get; private set; }
        public TicketPriority Priority { get; private set; }
        public TicketCategory Category { get; private set; }
        public UserId? AssignedAgent { get; private set; }
        public DateTime? ResponseDeadline { get; private set; }
        public bool Escalated { get; private set; } = false;
        public bool CanBeEscalated => Status == TicketStatus.PENDING_AGENT_RESPONSE && _clock.Now > ResponseDeadline!;
        public TicketStatus Status { get; private set; }
        
        public Ticket(IAgentsInformationService agentsInformationService, IClock? clock = null)
        {
            _clock = clock ?? new Clock();
            _agentsInformationService = agentsInformationService; 
            Id = new TicketId();
        }

        public void Open(UserId customer, Title title, MessageBody body, TicketPriority priority, TicketCategory category, bool escalated = false)
        {
            Customer = customer;
            Title = title;
            Priority = priority;
            Category = category;
            Escalated = escalated;
            ResetAssignment();
            
            AddMessage(Customer, body);
        }

        public void AssignToAgent(UserId agentId)
        {
            AssignedAgent = agentId;
            Status = TicketStatus.PENDING_AGENT_RESPONSE;
            var responseTime = _agentsInformationService.GetResponseDeadlineForAgent(agentId, Priority, Category);
            if (Escalated)
            {
                responseTime = responseTime / 2;
            }
            ResponseDeadline = _clock.Now.Add(responseTime);
        }

        public void AddAgentResponse(MessageBody messageBody)
        {
            AddMessage(AssignedAgent!, messageBody);
            Status = TicketStatus.PENDING_CUSTOMER_RESPONSE;
            ResponseDeadline = _clock.Now.Add(_agentsInformationService.GetResponseDeadlineForCustomer(AssignedAgent!));
        }
        
        public void AddCustomerResponse(MessageBody messageBody)
        {
            AddMessage(Customer, messageBody);
            Status = TicketStatus.PENDING_AGENT_RESPONSE;
            ResponseDeadline = _clock.Now.Add(_agentsInformationService.GetResponseDeadlineForAgent(AssignedAgent!, Priority, Category));
        }

        public void Escalate()
        {
            if (!CanBeEscalated)
            {
                throw new InvalidOperationException("The ticket cannot be escalated");
            }
            
            Escalated = true;

            if (AssignedAgent != null && !_agentsInformationService.IsOnShift(AssignedAgent!, _clock.Now))
            {
                ResetAssignment();
            }
        }

        public void EvaluateTimeBasedRules()
        {
            if (Status == TicketStatus.PENDING_CUSTOMER_RESPONSE && ResponseDeadline! <= _clock.Now)
            {
                Status = TicketStatus.CLOSED;
            }
        }

        public IEnumerable<Message> GetMessages() => _messages.AsReadOnly();

        private void ResetAssignment()
        {
            AssignedAgent = null;
            Status = TicketStatus.UNASSIGNED;
            _domainEvents.Add(new AssignmentRequested(EventId.Next(_domainEvents), Id, DateTime.Now));
        }

        private void AddMessage(UserId author, MessageBody body)
        {
            var id = MessageId.FromInt(_messages.Count);
            var sentOn = _clock.Now;
            var message = new Message(id, author, body, sentOn);
            _messages.Add(message);
        }
    }
}