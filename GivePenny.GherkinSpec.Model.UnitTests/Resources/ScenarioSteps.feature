Feature: Add two numbers
	In order to count how many apples I've collected
	As an apple-hoarder
	I want to add two numbers together

Scenario: Add two numbers together
	Given I have 5 apples
	And I have 6 more apples
	When I add the numbers together
	Then the result should be 11

Scenario: Add two numbers together when one of them is zero
	When I do nothing
	Then nothing happens