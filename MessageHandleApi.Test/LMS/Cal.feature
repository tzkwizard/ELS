Feature: Cal
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

@mytag
Scenario: Add two numbers
	Given I have enter 70 into the calculator
	And I also have enter 50 into the calculator
	When I press minus
	Then the result should be 20 
