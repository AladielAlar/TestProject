using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Patterns;
using Patterns.WebTests.Builders;
using FluentAssertions;
using Serilog;
using Steps; 

namespace TestProjectNUnit
{
    [TestFixture, Parallelizable(ParallelScope.All)]

    public class NUnitTest
    {
        private ThreadLocal<IWebDriver> driver;

        [OneTimeSetUp]
        public void SetupLogging()
        {
            driver = new ThreadLocal<IWebDriver>(() => DriverSingleton.GetDriver());

            Directory.CreateDirectory("NUnitlogs");
            string logFilePath = "NUnitlogs/tests.log";

            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        [SetUp]

        public void SetUp()
        {
            Log.Information("Test started: {TestName}", TestContext.CurrentContext.Test.Name);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            driver.Value.Manage().Window.Maximize();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        [Test]
        public void RunNUnitTest()
        {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable IDE0042 // Deconstruct variable declaration
#pragma warning disable CS8604 // Possible null reference argument.
            var feature = new UserSteps(driver.Value);
            var navigationData = TestDataFactory.NavigationTestData();
            var searchData = TestDataFactory.SearchTestData();
            var languageDataLT = TestDataFactory.LanguageTestDataLT();
            var languageDataEN = TestDataFactory.LanguageTestDataEN();
            var contactFormData = TestDataFactory.ContactFormTestData();
#pragma warning restore IDE0059 // Unnecessary assignment of a value
#pragma warning restore CS8604 // Possible null reference argument.
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
//          feature.WhenTheUserClicksTheLink("Contact");
//          feature.WhenTheUserFillsInTheNameEmailAndMessage(contactFormData.Name,contactFormData.Email,contactFormData.Message);
//          feature.WhenTheUserSubmitsTheContactForm();
//          feature.ThenTheUserShouldSeeASuccessMessageContaining(contactFormData.ExpectedMessage);
            Log.Information("NUnit test completed.");
        }


        [OneTimeTearDown]
        public void TearDown()
        {
            Log.Information("Cleaning up WebDriver.");
            driver.Value?.Quit();
            driver.Dispose();
        }
    }
}
