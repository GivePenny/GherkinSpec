Feature: Eventually Consistent Scenario

@eventuallyConsistent(within=00:00:20;retryInterval=00:00:05)
Scenario: One eventually consistent When step followed by two Then steps, failing on the first and then the second
	Given an asynchronous create result event has been published
	And an asynchronous update result event has been published
	When I call the eventually consistent method with that GUID
	Then the result should not be null
	And the result should reflect the update event