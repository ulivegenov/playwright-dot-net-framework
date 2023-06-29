@API
Feature: ApiTests

A short summary of the feature

Scenario: Verify I can get posts list
	When I send GET request to endpoint "/posts"
	Then I receive responce with status code "200"
