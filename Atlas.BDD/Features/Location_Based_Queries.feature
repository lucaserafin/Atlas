Feature: Location-Based Queries
  As a user
  I want to perform queries based on location
  So that I can find points of interest (POIs) near me

  Scenario: Find nearby POIs
    Given there are points of interest stored in the system
    And my current location is at latitude "40.7128" and longitude "-74.0060"
    When I send a GET request to "/locations/nearby?lat=40.7128&lon=-74.0060&radius=500"
    Then I should receive a list of POIs within 500 meters of my location

  Scenario: Error when radius is missing in nearby POIs request
    Given the system is running
    And my current location is at latitude "40.7128" and longitude "-74.0060"
    When I send a GET request to "/locations/nearby?lat=40.7128&lon=-74.0060"
    Then the system should respond with a 400 status code
    And an error message "radius parameter is required"

  Scenario: Error when coordinates are invalid
    Given the system is running
    When I send a GET request to "/locations/nearby?lat=200&lon=-74.0060&radius=500"
    Then the system should respond with a 400 status code
    And an error message "Invalid latitude value: must be between -90 and 90"

  Scenario: Query nearby locations efficiently with large datasets
    Given the database contains 1,000,000 points of interest
    When I query for locations within a 1 km radius
    Then the response time should be under 1 second