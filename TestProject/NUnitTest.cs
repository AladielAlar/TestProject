using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Patterns;
using Patterns.WebTests.Builders;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using FluentAssertions;
using System.Reflection;
using Serilog;
using Steps; 

namespace TestProjectNUnit
{
    [TestFixture, Parallelizable(ParallelScope.All)]

    public class NUnitTest
    {
        private ThreadLocal<IWebDriver> driver;
        private ExtentReports extent;
        private ExtentTest? test;

        [OneTimeSetUp]
        public void SetupLogging()
        {
            driver = new ThreadLocal<IWebDriver>(() => DriverSingleton.GetDriver());

            extent = new ExtentReports();

            // Initialize the ExtentSparkReporter (Recommended for v5.x)
            if (File.Exists("TestReport.html"))
            {
                File.Delete("TestReport.html");
            }
            var sparkReporter = new ExtentSparkReporter("TestReport.html");
            extent.AttachReporter(sparkReporter);

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
            test = extent.CreateTest("RunNUnitTest");
            var feature = new UserSteps(driver.Value);
            var navigationData = TestDataFactory.NavigationTestData();
            var searchData = TestDataFactory.SearchTestData();
            var languageDataLT = TestDataFactory.LanguageTestDataLT();
            var languageDataEN = TestDataFactory.LanguageTestDataEN();
            var contactFormData = TestDataFactory.ContactFormTestData();
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
            test.Pass("Test completed successfully.");
            Log.Information("NUnit Pass test completed.");
        }

        [Test]
        public void RunSkippedTest()
        {
            test = extent.CreateTest("RunSkippedTest");
            test.Skip("Test skipped.");
            var feature = new UserSteps(driver.Value);
            var contactFormData = TestDataFactory.ContactFormTestData();
            bool skip = true;

            if (skip)
            {
                Assert.Inconclusive("skipped test");
            }
            feature.WhenTheUserClicksTheLink("Contact");
            feature.WhenTheUserFillsInTheNameEmailAndMessage(contactFormData.Name, contactFormData.Email, contactFormData.Message);
            feature.WhenTheUserSubmitsTheContactForm();
            feature.ThenTheUserShouldSeeASuccessMessageContaining(contactFormData.ExpectedMessage);
            Log.Information("NUnit Skipped test");
        }

        [Test]
        public void RunFailTest()
        {
            var feature = new UserSteps(driver.Value);

            test = extent.CreateTest("RunFailTest");
            var languageDataFr = TestDataFactory.LanguageTestDataFR();
            feature.GivenTheUserIsOnTheHomepage();
            feature.WhenTheUserSwitchesTheSiteLanguageTo(languageDataFr.Language);
            feature.ThenTheUserShouldBeRedirectedTo(languageDataFr.ExpectedUrlFragment);

            try
            {
                feature.ThenTheUserShouldBeRedirectedTo(languageDataFr.ExpectedUrlFragment);
                test.Pass("Test passed.");
            }
            catch (AssertionException ex)
            {
                test.Fail($"Test failed with message: {ex.Message}");
                Log.Error($"Test failed: {ex.Message}");
            }

            Log.Information("NUnit Fail test completed.");
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            extent.Flush();
            Log.Information("Cleaning up WebDriver.");
            driver.Value?.Quit();
            driver.Dispose();
        }
    }
}
