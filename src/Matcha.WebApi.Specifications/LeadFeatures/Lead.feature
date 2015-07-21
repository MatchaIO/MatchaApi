Feature: Manage Leads
	In to build a sales pipeline
	As a sales user
	I want to be able to receive and manage leads

Scenario: Create a lead
	Given an anonymous user using the api
	When they submit their contact details
	Then the new Id is returned
	And a Created is returned
	And a SalesAdmin user can retrieve the lead
	And LeadCreated event is raised

Scenario: Create a lead with opportunity details
	Given an anonymous user using the api
	When they submit a fullly populated lead
	Then the new Id is returned
	And a Created is returned
	And a SalesAdmin user can retrieve the lead
	And the opportunity details are on the lead
	And LeadCreated event is raised

Scenario: Continue to add to a lead
	Given an anonymous user using the api
	And they have submited their contact details
	When an update command is made with the lead id
	Then a SalesAdmin user can retrieve the lead
	And a Ok is returned
	And LeadUpdated event is raised

Scenario: Sales user update a Lead
	Given an sales user using the api
	And a valid Lead exists
	When an update command is made with the lead id
	Then a Ok is returned
	And LeadUpdated event is raised
	And a SalesAdmin user can retrieve the lead

Scenario: Attempt to Create a lead without a name
	Given an anonymous user using the api
	When they submit their contact details with no name
	Then a BadRequest is returned
	And no leads are created
	And no events are raised

Scenario: Attempt to Create a lead without a contact
	Given an anonymous user using the api
	When they submit their contact details with no contact
	Then a BadRequest is returned
	And no leads are created
	And no events are raised

Scenario: Attempt to Update a lead without a contact
	Given an anonymous user using the api
	And they have submited their contact details
	When they modify their contact details with no contact
	Then a BadRequest is returned
	And no LeadUpdated event is raised

Scenario: Attempt to Update a lead without a name
	Given an anonymous user using the api
	And they have submited their contact details
	When they modify their contact details with no name
	Then a BadRequest is returned
	And no LeadUpdated event is raised

Scenario: Attempt to Update an invalid lead 
	Given an sales user using the api
	And a valid Lead exists
	When an update command is made with an invalid id
	Then a NotFound is returned
	And no LeadUpdated event is raised

Scenario: Delete A non existant Lead
	Given an sales user using the api
	And a valid Lead exists
	When a delete command is made with an invalid id
	Then a NotFound is returned
	And no LeadDeleted event is raised
	And a SalesAdmin user can retrieve the lead

Scenario: Delete A deleted Lead
	Given an sales user using the api
	And a Lead has been previously deleted
	When a delete command is made with the lead id
	Then a NotFound is returned
	And no LeadDeleted event is raised
	And the lead can not be retrieved by id

Scenario: Delete A Lead
	Given an sales user using the api
	And a valid Lead exists
	When a delete command is made with the lead id
	Then a NoContent is returned
	And the lead can not be retrieved by id
	And the lead does not appear in the lead list
	And LeadDeleted event is raised


Scenario: Vetting a Lead
	Given an sales user using the api
	And a valid Lead exists
	When the lead is vetted
	Then a Created is returned
	And the lead can not be retrieved by id
	And the lead does not appear in the lead list
	And the opportunity can be retrieved by id
	And LeadVetted event is raised
	And OpportunityCreated event is raised
#
#Given a sales user is logged on
#And a Lead is vetted
#When the lead is updated, an EntityNotFoundException is thrown
#And no event is raised
