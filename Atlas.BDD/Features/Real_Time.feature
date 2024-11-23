Feature: Real-Time Location Updates
  As a user
  I want to receive real-time updates about nearby locations
  So that I can get alerts when something of interest is close to me

  Scenario: Receive proximity alert
    Given I am subscribed to real-time location updates
    And I am currently at latitude "40.7128" and longitude "-74.0060"
    When a point of interest enters a 100-meter radius from my location
    Then I should receive a proximity alert with the POI details
