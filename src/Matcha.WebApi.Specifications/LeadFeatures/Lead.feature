Feature: Manage Leads
	In to build a sales pipeline
	As a sales user
	I want to be able to receive and manage leads

Scenario: Create a lead
	Given an anonymous user using the api
	When they submit their contact details
	Then the new Id is returned
	And a SalesAdmin user can retrieve the lead
	And LeadCreated event is raised

Scenario: Continue to add to a lead
	Given an anonymous user using the api
	And they have submited their contact details
	When they modify their contact details
	Then a SalesAdmin user can retrieve the lead
	And LeadUpdated event is raised

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
	And no UpdateLead event is raised

Scenario: Attempt to Update a lead without a name
	Given an anonymous user using the api
	And they have submited their contact details
	When they modify their contact details with no name
	Then a BadRequest is returned
	And no UpdateLead event is raised

Scenario: Delete A non existant Lead
	Given an sales user using the api
	And a valid Lead exists
	When a delete command is made with an invalid id,
	Then a NotFound is returned
	And no DeleteLead event is raised
	And a SalesAdmin user can retrieve the lead
      
#
#
#Scenario: Delete A Lead
#	Given a sales user is logged on
#	And a Lead has been previously deleted
#	When a delete command is made for that lead
#	Then a 404 is returned
#	Then no Event is raised
#	And the lead is still deleted
#
#Scenario: Delete A deleted Lead
#	Given a valid Lead
#	When it is deleted
#	Then LeadWithdrawn event is raised
#	And the lead can no longer be retrieved by id
#	And the lead does not appear in the lead list
#
#
#Scenario: Update a Lead
#
#Given a sales user is logged on
#And a valid Lead
#When the lead is updated
#Then LeadCreated Event is raised
#And the Lead is updated
#
#Given a sales user is logged on
#And a valid Lead
#When an update command is made with an invalid id
#Then no Event is raised
#And the Lead is not updated
#
#
#Scenario: Vetting a Lead
#
#Given a sales user is logged on
#And a valid Lead
#When the lead is vetted
#Then LeadVetted Event is raised
#And OpportunityCreated Event is raised
#And the Lead is no longer available
#And the Opportunity will be created
#
#Given a sales user is logged on
#And a Lead is vetted
#When the lead is updated, an EntityNotFoundException is thrown
#And no event is raised