Feature: Ticket lifecycle management
    Feature Description

Background: Initialize an agent and a customer
    Given a customer named 'Vincent Jules'
    Given an agent named 'Mr. Wolf'

Scenario: Request assignment of an agent for new tickets
    Given the time is 2022/05/21 11:08:00
    When a new ticket is opened
    Then the ticket should be assigned to an agent

Scenario: Response SLA is set when a ticket is assigned to an agent
    Given a customer opened a low priority bug report
    And the time is 2022/05/18 11:08:00
    And according to Mr. Wolf's department policy, a low priority bug report should be processed within 6 hours
    When the ticket is assigned to Mr. Wolf
    Then the agent should respond by 2022/05/18 17:08:00

Scenario: Customer response deadline is set after the agent replies to a ticket
    Given a new ticket is opened
    And the ticket is assigned to Mr. Wolf
    And according to Mr. Wolf's department policy, customers have to respond to agents' messages within 3 days
    And the time is 2022/05/18 11:08:00
    When the agent replies to the ticket
    Then the customer should respond by 2022/05/21 11:08:00

Scenario: Replying clears the agent's response deadline
    Given a new ticket is opened
    And the ticket is assigned to Mr. Wolf
    When the agent replies to the ticket
    Then the agent has no response deadline

Scenario: Customer doesn't respond within the allocated time
    Given a new ticket is opened
    And the ticket is assigned to Mr. Wolf
    And according to Mr. Wolf's department policy, customers have to respond to agents' messages within 3 days
    And the agent replies to the ticket on 2022/05/18 11:08:00
    When the time is 2022/05/21 11:08:00
    Then the ticket is closed

Scenario: If the agent doesn't reply within the allocated time, the case can be escalated
    Given a customer opened a low priority bug report
    And the time is 2022/05/18 11:08:00
    And according to Mr. Wolf's department policy, a low priority bug report should be processed within 6 hours
    And the ticket is assigned to Mr. Wolf
    When the time is 2022/05/18 17:08:01
    Then the ticket can be escalated

Scenario: Ticket cannot be escalated before the agent's response deadline
    Given a customer opened a low priority bug report
    And the time is 2022/05/18 11:08:00
    And according to Mr. Wolf's department policy, a low priority bug report should be processed within 6 hours
    And the ticket is assigned to Mr. Wolf
    When the time is 2022/05/18 17:07:01
    Then the ticket cannot be escalated    
    
Scenario: If a case is escalated and the assigned agent is not on a shift, the ticket is reassigned
    Given a customer opened a low priority bug report
    And the time is 2022/05/18 11:08:00
    And according to Mr. Wolf's department policy, a low priority bug report should be processed within 6 hours
    And the ticket is assigned to Mr. Wolf
    And the time is 2022/05/18 17:08:01
    And Mr. Wolf is not on a shift
    When the customer escalates the ticket
    Then the ticket should be reassigned to another agent

Scenario: When an escalated case is assigned to an agent, the response SLAs are cut in half
    Given an escalated low priority bug report
    And the time is 2022/05/18 11:08:00
    And according to Mr. Wolf's department policy, a low priority bug report should be processed within 6 hours
    When the ticket is assigned to Mr. Wolf
    Then the agent should respond by 2022/05/18 14:08:00

Scenario: Ticket tracks the correspondence between the customer and the agent
    Given the time is 2022/05/18 10:00:00
    And Vincent Jules opens a ticket "Help me!!!1" saying "I need to hide an entity"
    And the ticket is assigned to Mr. Wolf
    And on 2022/05/18 10:01:00 the agent replies saying "Have you tried hiding it in an aggregate?"
    And on 2022/05/18 10:02:00 the customer replies saying "What's that?"
    And on 2022/05/18 10:03:00 the agent replies saying "It's a cluster of objects sharing a transactional boundary"
    Then when the ticket is displayed its title is "Help me!!!1"
    And it has the following messages:
        | From          | Sent on             | Message                                                    |
        | Vincent Jules | 2022/05/18 10:00:00 | I need to hide an entity                                   |
        | Mr. Wolf      | 2022/05/18 10:01:00 | Have you tried hiding it in an aggregate?                  |
        | Vincent Jules | 2022/05/18 10:02:00 | What's that?                                               |
        | Mr. Wolf      | 2022/05/18 10:03:00 | It's a cluster of objects sharing a transactional boundary |