Feature: Location Data Management for Users
  As a system user
  I want to store and retrieve my location data
  So that I can use personalized location-based features


  Scenario: Add a new location for a user
    Given a user with user_id "12345" exists
    When I send a POST request to "/locations" with:
      | user_id    | latitude   | longitude  |
      | 12345      | 40.7128    | -74.0060   |
    Then the system should store the location data in the database
    And associate it with the user_id "12345"
    And the system should respond with a confirmation message
