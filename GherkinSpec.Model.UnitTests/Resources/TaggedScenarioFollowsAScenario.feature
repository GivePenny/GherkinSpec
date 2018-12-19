Feature: Feature file in which a tagged scenario follows a scenario

Scenario: First scenario
	Given a tagged scenario follows this one
	When GherkinSpec parses the feature file
	Then parsing should succeed

@ignore
Scenario Outline: Tagged scenario outline follows
	Given this scenario outline has a tag
	When GherkinSpec parses the feature file
	Then parsing should <result>
Examples:
	| result  |
	| succeed |
