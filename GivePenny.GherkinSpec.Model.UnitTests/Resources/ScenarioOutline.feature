Feature: Add two numbers

Scenario Outline: Example scenario outline
	Given a first step <columnA>
	And another <columnB>
Examples:
	| columnA | columnB |
	| A1      | B1      |
	| A2      | B2      |

Scenario: Scenario after an outline
	When I do nothing
	Then nothing happens
