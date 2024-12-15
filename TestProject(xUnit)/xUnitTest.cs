using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Patterns;
using Patterns.WebTests.Builders;
using FluentAssertions;
using Serilog;
using Xunit;
using Xunit.Abstractions;
using Steps;

namespace TestProjectXUnit
{
    [Collection("Parallel Tests")]
    public class XUnitTest : IDisposable
    {
        private readonly IWebDriver driver;
#pragma warning disable IDE0052 // Remove unread private members
        private readonly ITestOutputHelper output;
#pragma warning restore IDE0052 // Remove unread private members

        public XUnitTest(ITestOutputHelper output)
        {
            this.output = output;
            Directory.CreateDirectory("xUnitlogs");
            string logFilePath = "xUnitlogs/tests.log";

            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                .WriteTo.TestOutput(output)
                .CreateLogger();

            driver = DriverSingleton.GetDriver();
            driver.Manage().Window.Maximize();
        }

        [Fact, Trait("Category", "xUnitTest")]
        public void RunxUnitTest()
        {
            Log.Information("Starting xUnit test.");

            var feature = new UserSteps(driver);
#pragma warning disable IDE0042 // Deconstruct variable declaration
            var navigationData = TestDataFactory.NavigationTestData();
            var searchData = TestDataFactory.SearchTestData();
            var languageDataLT = TestDataFactory.LanguageTestDataLT();
            var languageDataEN = TestDataFactory.LanguageTestDataEN();
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            var contactFormData = TestDataFactory.ContactFormTestData();
#pragma warning restore IDE0059 // Unnecessary assignment of a value
            feature.GivenTheUserIsOnTheHomepage();
            feature.WhenTheUserTapOn(navigationData.LinkText);
            feature.VerifyingURL();
            feature.WhenTheUserSwitchesTheSiteLanguageTo(languageDataLT.Language);
            feature.ThenTheUserShouldBeRedirectedTo(languageDataLT.ExpectedUrlFragment);
            feature.ThenThePageTitleShouldBe(languageDataLT.ExpectedHeader);
            feature.WhenTheUserSwitchesTheSiteLanguageTo(languageDataEN.Language);
            feature.ThenTheUserShouldBeRedirectedTo(languageDataEN.ExpectedUrlFragment);
            feature.ThenThePageTitleShouldBe(languageDataEN.ExpectedHeader);
            feature.WhenTheUserSearchesFor(searchData.SearchQuery);
            feature.ThenTheUserShouldBeRedirectedToTheSearchResultsPage();
          //feature.WhenTheUserClicksTheLink("Contact");
          //feature.WhenTheUserFillsInTheNameEmailAndMessage(contactFormData.Name,contactFormData.Email,contactFormData.Message);
          //feature.WhenTheUserSubmitsTheContactForm();
          //feature.ThenTheUserShouldSeeASuccessMessageContaining(contactFormData.ExpectedMessage);

            Log.Information("xUnit test completed.");
        }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        {
            Log.Information("Cleaning up WebDriver.");
            DriverSingleton.QuitDriver();
        }
    }
}
