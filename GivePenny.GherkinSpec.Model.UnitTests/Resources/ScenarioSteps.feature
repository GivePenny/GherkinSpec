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
	Given I have a table
		| column 1 | column 2 |
		| value 1  | value 2  |
	When I do nothing
	Then nothing happens
	But this reads well

Scenario: A scenario
	Given I have some markdown
		"""
		A very long document can go here, but
		  watch that the indentation is correct.
		"""
	And documents can be followed by tables
		"""
		Document here
		"""
		| column 1 | column 2 |
		| value 1  | value 2  |