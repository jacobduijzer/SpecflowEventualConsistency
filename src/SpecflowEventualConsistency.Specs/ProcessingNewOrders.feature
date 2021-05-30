Feature: Processing New Orders
	TODO

@mytag
Scenario: Unprocessed orders will be queued and processed
	Given the user has unprocessed orders
	When he sends this orders to the api
	Then the orders will be processed and added to the database