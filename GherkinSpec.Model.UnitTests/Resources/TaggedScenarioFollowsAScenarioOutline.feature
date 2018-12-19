Feature: Feature file in which a tagged scenario follows a scenario

Scenario Outline: Scenario outline
	Given a tagged scenario follows this one
	When GherkinSpec parses the feature file
	Then parsing should <result>
Examples:
	| result  |
	| succeed |

@ignore
Scenario: Tagged scenario follows
	Given this scenario has a tag
	When GherkinSpec parses the feature file
	Then parsing should succeed
