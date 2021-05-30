Feature: Processing New Orders
  As a order entry clerk
  I want to send orders to the ordering system
  So the system can process the orders for the order pickers

  Scenario: Unprocessed orders will be queued and processed
    Given the user has these unprocessed orders
      | CustomerId | ProductID | Amount |
      | 1          | 1         | 10     |
      | 1          | 2         | 5      |
      | 1          | 3         | 7      |
      | 1          | 4         | 50     |
    When he sends this orders to the api
    Then the orders will be processed and added to the database