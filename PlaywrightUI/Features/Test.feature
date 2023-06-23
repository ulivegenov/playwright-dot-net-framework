@UI
Feature: Test

A short summary of the feature

@Browser:Chromium
@Browser:Chrome
@Browser:Edge
@Browser:Firefox
@Browser:Webkit
Scenario: Verify I can search for Endava successfully in Google
	Given I open Google's home page
	And I accept cookies
	When I fill "Endava" in the seach field
	And I click on Google search button
	Then Then I verify the first result is "Endava"