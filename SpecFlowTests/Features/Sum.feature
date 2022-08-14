Feature: Sum
  As a lazy mathematician
  I want to be able to sum two numbers
  So that I can find out the result
  
  Scenario: One plus one
    Given my first number is 1
    And my second number is 1
    When I sum my numbers
    Then the result is 2
  
  Scenario: Two plus two
    Given my first number is 2
    And my second number is 2
    When I sum my numbers
    Then the result is 4
    And the result is not 5