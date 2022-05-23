#nullable enable
using Gherkin.Ast;
using Moq;
using Xunit;
using Xunit.Gherkin.Quick;
using WolfDesk.Domain.Tickets;
using WolfDesk.Domain.Tickets.Events;
using WolfDesk.Domain.Tickets.Messages;
using WolfDesk.Domain.Tickets.Values;
using WolfDesk.Domain.Users;
using Feature = Xunit.Gherkin.Quick.Feature;

namespace WolfDesk.Domain.Tests.Tickets;

[FeatureFile("./Tickets/Ticket.feature")]
public sealed class TicketLifecycle : Feature
{
    private FakeClock _fakeClock = new();
    private Mock<IAgentsInformationService> _agentsInformationService = new();
    private Dictionary<string, UserId> _customers = new();
    private Dictionary<string, UserId> _agents = new();
    private ProductId _product = new();

    private Ticket? _theTicket;
    private Ticket TheTicket => _theTicket!;

    [Given(@"a customer named '(.+)'")]
    public void InitCustomer(string customerName)
    {
        _customers[customerName] = new UserId();
    }

    [Given(@"an agent named '(.+)'")]
    public void InitAgent(string agentName)
    {
        _agents[agentName] = new UserId();
    }

    [Given(@"a new ticket is opened")]
    [When(@"a new ticket is opened")]
    public void InitTicket()
    {
        var aCustomer = _customers.Values.First();
        _theTicket = BuildTicket(aCustomer);
    }

    [Given(@"a customer opened a (.+) priority (.+)")]
    public void InitTicket(string priorityName, string categoryName)
    {
        var priority = ParsePriority(priorityName);
        var category = ParseCategory(categoryName);
        var aCustomer = _customers.Values.First();
        
        _theTicket = BuildTicket(aCustomer, priority: priority, category: category);
    }
    
    [And(@"(.+) opens a ticket ""(.+)"" saying ""(.+)""")]
    public void InitTicket(string customerName, string title, string body)
    {
        var customer = _customers[customerName];
        
        _theTicket = BuildTicket(
            customer: customer,
            title: title,
            body: body);
    }

    [Given(@"an escalated (.+) priority (.+)")]
    public void InitEscalatedTicket(string priorityName, string categoryName)
    {
        var priority = ParsePriority(priorityName);
        var category = ParseCategory(categoryName);
        var customer = _customers.Values.First();

        _theTicket = BuildTicket(
            customer: customer,
            priority: priority,
            category: category,
            escalated: true);
    }

    [Given(@"the time is (.+)")]
    [And(@"the time is (.+)")]
    public void SetTime(DateTime time)
    {
        _fakeClock.FakeTime = time;
    }
    
    [When(@"the time is (.+)")]
    public void EvaluateTimeBasedRules(DateTime time)
    {
        _fakeClock.FakeTime = time;
        TheTicket.EvaluateTimeBasedRules();
    }

    [Given(@"according to (.+)'s department policy, a (.+) priority (.+) should be processed within (.+) hours")]
    [And(@"according to (.+)'s department policy, a (.+) priority (.+) should be processed within (.+) hours")]
    public void SetSLAPolicy(string agentName, string priorityName, string categoryName, int hours)
    {
        var agentId = _agents[agentName];
        var priority = ParsePriority(priorityName);
        var category = ParseCategory(categoryName);

        _agentsInformationService.Setup(x => x.GetResponseDeadlineForAgent(agentId, priority, category)).Returns(TimeSpan.FromHours(hours));
    }

    [And(@"according to (.+)'s department policy, customers have to respond to agents' messages within (.+) days")]
    public void SetCustomerResponseTime(string agentName, int days)
    {
        var agentId = _agents[agentName];

        _agentsInformationService.Setup(x => x.GetResponseDeadlineForCustomer(agentId)).Returns(TimeSpan.FromDays(days));
    }

    [And(@"the agent replies to the ticket on (.+)")]
    public void AddAgentReply(DateTime date)
    {
        _fakeClock.FakeTime = date;
        TheTicket.AddAgentResponse(MessageBody.FromString("have you tried turned it off and back on again?"));
    }

    [When(@"the agent replies to the ticket")]
    public void AddAgentReply()
    {
        TheTicket.AddAgentResponse(MessageBody.FromString("have you tried turned it off and back on again?"));
    }

    [And(@"(.+) is not on a shift")]
    public void SetNotOnShift(string agentName)
    {
        var agentId = _agents[agentName];
        _agentsInformationService.Setup(x => x.IsOnShift(agentId, _fakeClock.Now)).Returns(false);
    }

    [And(@"on (.+) the agent replies saying ""(.+)""")]
    public void AgentResponds(DateTime time, string message)
    {
        _fakeClock.FakeTime = time;
        TheTicket.AddAgentResponse(MessageBody.FromString(message));
    }
    
    [And(@"on (.+) the customer replies saying ""(.+)""")]
    public void CustomerResponds(DateTime time, string message)
    {
        _fakeClock.FakeTime = time;
        TheTicket.AddCustomerResponse(MessageBody.FromString(message));
    }

    [When(@"the ticket is assigned to (.+)")]
    [And(@"the ticket is assigned to (.+)")]
    public void AssignTicket(string agentName)
    {
        var agentId = _agents[agentName];
        TheTicket.AssignToAgent(agentId);
    }

    [When(@"the customer escalates the ticket")]
    public void EscalateCase()
    {
        TheTicket.Escalate();
    }

    [Then(@"the ticket should be assigned to an agent")]
    [Then(@"the ticket should be reassigned to another agent")]
    public void AssertAssignmentRequired()
    {
        foreach (var e in TheTicket.DomainEvents.Where(x => x is AssignmentRequested).Cast<AssignmentRequested>())
        {
            if (e.TicketId == TheTicket.Id &&
                e.Category == TheTicket.Category &&
                e.Priority == TheTicket.Priority &&
                e.Product == TheTicket.Product &&
                e.Timestamp == _fakeClock.Now)
            {
                return;
            }
        }
        Assert.True(false);
    }

    [Then(@"the agent should respond by (.+)")]
    public void AssertResponseSLA(DateTime responseBy)
    {
        Assert.Equal(responseBy, TheTicket.ResponseDeadline);
        Assert.Equal(TicketStatus.PENDING_AGENT_RESPONSE,  TheTicket.Status);
    }

    [Then(@"the agent has no response deadline")]
    public void AssertNoAgentResponseSLA()
    {
        Assert.NotEqual(TicketStatus.PENDING_AGENT_RESPONSE, TheTicket.Status);
    }

    [Then(@"the customer should respond by (.+)")]
    public void AssertCustomerResponseDeadline(DateTime respondBy)
    {
        Assert.Equal(respondBy, TheTicket.ResponseDeadline);
        Assert.Equal(TicketStatus.PENDING_CUSTOMER_RESPONSE,  TheTicket.Status);
    }

    [Then(@"the ticket is closed")]
    public void AssertTicketClosed()
    {
        Assert.Equal(TicketStatus.CLOSED, TheTicket.Status);
    }

    [Then(@"the ticket can be escalated")]
    public void AssertTicketCanBeEscalated()
    {
        Assert.True(TheTicket.CanBeEscalated);
        TheTicket.Escalate();
        Assert.True(TheTicket.Escalated);
    }
    
    [Then(@"the ticket cannot be escalated")]
    public void AssertTicketCannotBeEscalated()
    {
        Assert.False(TheTicket.CanBeEscalated);
        Assert.Throws<InvalidOperationException>(() => TheTicket.Escalate());
        Assert.False(TheTicket.Escalated);
    }

    [Then(@"when the ticket is displayed its title is ""(.+)""")]
    public void AssertTitle(string title)
    {
        Assert.Equal(Title.FromString(title), TheTicket.Title);
    }
    
    [And(@"it has the following messages:")]
    public void AssertMessages(DataTable messagesTable)
    {
        var messages = TheTicket.GetMessages().ToArray();
        var expectedRows = messagesTable.Rows.Skip(1).ToArray();

        for (var i=0; i<expectedRows.Length; i++)
        {
            var message = messages[i];
            var expectedRow = expectedRows[i];

            var from = FindUser(expectedRow.Cells.ElementAt(0).Value);
            var sentOn = DateTime.Parse(expectedRow.Cells.ElementAt(1).Value);
            var messageBody = MessageBody.FromString(expectedRow.Cells.ElementAt(2).Value);
            
            Assert.Equal(MessageId.FromInt(i), message.Id);
            Assert.Equal(from, message.Author);
            Assert.Equal(sentOn, message.SentOn);
            Assert.Equal(messageBody, message.Body);
        }
    }

    private Ticket BuildTicket(
        UserId customer,
        string title = "a title",
        string body = "ticket body",
        TicketPriority priority = TicketPriority.MEDIUM,
        TicketCategory category = TicketCategory.GENERAL_GUIDANCE,
        bool escalated = false,
        ProductId? product = null)
    {
        var result = new Ticket(_agentsInformationService.Object,  _fakeClock);
        result.Open(
            customer,
            Title.FromString(title),
            MessageBody.FromString(body),
            priority,
            category,
            product ?? _product,
            escalated);

        return result;
    }

    private TicketPriority ParsePriority(string name)
    {
        if (!Enum.TryParse(name.ToUpper().Replace(' ', '_'), out TicketPriority result))
        {
            throw new ArgumentException("Invalid priority value", name);
        }

        return result;
    }

    private TicketCategory ParseCategory(string name)
    {
        if (!Enum.TryParse(name.ToUpper().Replace(' ', '_'), out TicketCategory result))
        {
            throw new ArgumentException("Invalid category value", name);
        }

        return result;
    }

    private UserId FindUser(string name) => _agents.ContainsKey(name) ? _agents[name] : _customers[name];
}
