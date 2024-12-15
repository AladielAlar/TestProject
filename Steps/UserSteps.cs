using System;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using FluentAssertions;
using Patterns;
using Patterns.WebTests.Builders;
using Serilog;

namespace WebTests.StepDefinitions
{
    [Binding]
    public class UserJourneySteps(IWebDriver webDriver)
    {
        private readonly IWebDriver driver = webDriver;

        [Given(@"the user is on the homepage")]
        public void GivenTheUserIsOnTheHomepage()
        {
            Log.Debug("Navigating to the homepage.");
            var (baseUrl, _, _, _) = TestDataFactory.LanguageTestData();
            driver.Navigate().GoToUrl(baseUrl);
            Log.Information("User navigated to the homepage: {Url}", baseUrl);
        }

        [When(@"the user switches the site language to ""(.*)""")]
        public void WhenTheUserSwitchesTheSiteLanguageTo(string language)
        {
            Log.Debug("Switching site language to {Language}.", language);
            var languagePage = new LanguagePage(driver);
            languagePage.SwitchTolithuanian();
            Log.Information("Language switched to: {Language}", language);
        }

        [Then(@"the user should be redirected to ""(.*)""")]
        public void ThenTheUserShouldBeRedirectedTo(string expectedUrlFragment)
        {
            Log.Debug("Verifying URL contains: {ExpectedFragment}.", expectedUrlFragment);
            driver.Url.Should().Contain(expectedUrlFragment);
            Log.Information("URL verified: {Url}", driver.Url);
        }

        [Then(@"the page title should be ""(.*)""")]
        public void ThenThePageTitleShouldBe(string expectedTitle)
        {
            Log.Debug("Verifying page title matches: {ExpectedTitle}.", expectedTitle);
            driver.FindElement(By.TagName("h2")).Text.Should().Be(expectedTitle);
            Log.Information("Page title verified: {Title}", expectedTitle);
        }

        [When(@"the user searches for ""(.*)""")]
        public void WhenTheUserSearchesFor(string searchQuery)
        {
            Log.Debug("Searching for query: {SearchQuery}.", searchQuery);
            var studyProgramPage = new StudyProgramPage(driver);
            studyProgramPage.SearchStudyPrograms(searchQuery);
            Log.Information("Search executed for query: {SearchQuery}.", searchQuery);
        }

        [Then(@"the user should be redirected to the search results page")]
        public void ThenTheUserShouldBeRedirectedToTheSearchResultsPage()
        {
            var (_, _, expectedUrl) = TestDataFactory.SearchTestData();
            Log.Debug("Verifying redirection to the search results page.");
            driver.Url.Should().Be(expectedUrl);
            Log.Information("Search results page verified: {Url}", expectedUrl);
        }

        [When(@"the user clicks the ""(.*)"" link")]
        public void WhenTheUserClicksTheLink(string linkText)
        {
            Log.Debug("Clicking the link: {LinkText}.", linkText);
            var basePage = new BasePage(driver);
            basePage.FindLinkByText(linkText).Click();
            Log.Information("Link clicked: {LinkText}.", linkText);
        }

        [When(@"the user fills in the name ""(.*)"", email ""(.*)"", and message ""(.*)""")]
        public void WhenTheUserFillsInTheNameEmailAndMessage(string name, string email, string message)
        {
            Log.Debug("Filling contact form with Name: {Name}, Email: {Email}, Message: {Message}.", name, email, message);
            _ = new ContactFormBuilder(driver)
                .SetName(name)
                .SetEmail(email)
                .SetMessage(message);
            Log.Information("Contact form filled.");
        }

        [When(@"the user submits the contact form")]
        public void WhenTheUserSubmitsTheContactForm()
        {
            Log.Debug("Submitting the contact form.");
            var contactFormBuilder = new ContactFormBuilder(driver);
            contactFormBuilder.Submit();
            Log.Information("Contact form submitted.");
        }

        [Then(@"the user should see a success message containing ""(.*)""")]
        public void ThenTheUserShouldSeeASuccessMessageContaining(string expectedMessage)
        {
            Log.Debug("Verifying success message contains: {ExpectedMessage}.", expectedMessage);
            var successMessage = driver.FindElement(By.CssSelector(".success-message")).Text;
            successMessage.Should().Contain(expectedMessage);
            Log.Information("Success message verified: {Message}", successMessage);
        }
    }
}
