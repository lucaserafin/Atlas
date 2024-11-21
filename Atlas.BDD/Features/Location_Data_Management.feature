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


  Scenario: Error when user_id is missing in POST request
    Given a user with user_id "12345" doesn't exists
    When I send a POST request to "/locations" with:
      | latitude   | longitude  |
      | 40.7128    | -74.0060   |
    Then the system should respond with a 400 status code
    And an error message "user_id is required"


  Scenario: Error when requesting locations for a non-existent user
    Given a user with user_id "99999" exists
    When I send a GET request to "/locations?user_id=99999"
    Then the system should respond with a 404 status code
    And an error message "No locations found for user_id 99999"
