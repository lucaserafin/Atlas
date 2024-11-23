Feature: Location-Based Queries
  As a user
  I want to perform queries based on location
  So that I can find points of interest (POIs) near me

  Scenario: Find nearby POIs
    Given there are points of interest stored in the system
    And my current location is at latitude "40.7128" and longitude "-74.0060"
    When I send a GET request to "/locations/nearby?lat=40.7128&lon=-74.0060&radius=500"
    Then I should receive a list of POIs within 500 meters of my location

  Scenario: Query nearby locations efficiently with large datasets
    Given the database contains 1,000,000 points of interest
    When I query for locations within a 1 km radius
    Then the response time should be under 1 second