@API
Feature: ApiTests

A short summary of the feature

Scenario: Verify I can get users list
	When I send GET request to endpoint "/api/users"
	Then I receive responce with status code "200"
