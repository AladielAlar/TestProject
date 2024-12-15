Feature: User Pass
 In order to provide an optimal user experience
  As a user
  I want to navigate through the application, switch the language, search for study programs, and submit a contact form.
  Scenario: Complete Full Test
    Given the user is on the homepage
    When the user tap on About
    Then the URL should be "https://en.ehu.lt/about/"
    And  the user should be redirected to the search results page

    When the user switches the site language to "LT"
    Then the user should be redirected to "lt.ehu.lt"
    And the page title should be "Apie mus"

    When the user switches the site language to "EN"
    Then the user should be redirected to "en.ehu.lt"
    And the page title should be "About"

    When the user searches for "study programs"
    Then the user should be redirected to the search results page
    And the URL should be "https://en.ehu.lt/?s=study+programs"

#    When the user clicks the "Contact" link
#    And the user fills in the name "Test User", email "testuser@example.com", and message "This is a test message."
#    And the user submits the contact form
#    Then the user should see a success message containing "Thank you for your message. It has been sent."
