Feature: Check calculating rank of the text
  Scenario: Sending text and check rank
    Given User open web application
    When User sending text "a1"
    Then Application returns rank = 0,5