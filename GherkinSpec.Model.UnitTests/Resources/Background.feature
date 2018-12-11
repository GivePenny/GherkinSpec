Feature: Add two numbers

Background:
	Given a first step
	And another

Scenario: Add two numbers together
	Given I have 5 apples
	And I have 6 more apples
	When I add the numbers together
	Then the result should be 11
