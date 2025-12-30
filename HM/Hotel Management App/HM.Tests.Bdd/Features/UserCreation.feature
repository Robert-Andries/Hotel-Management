Feature: UserCreation
	As a system administrator
	I want to create new users
	So that they can access the system

Scenario: Successful user creation
	Given I have valid user data
	When I request to create a user
	Then the user should be created successfully
	And the user details should match the input data

Scenario: Failed user creation due to invalid email
	Given I have user data with an invalid email
	When I request to create a user
	Then the user creation should fail
	And an error should be returned
