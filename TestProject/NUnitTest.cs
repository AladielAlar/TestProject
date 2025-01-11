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
using OpenQA.Selenium.DevTools.V129.HeadlessExperimental;
using System.Linq;

namespace TestProjectNUnit
{
    [TestFixture, Parallelizable(ParallelScope.All)]

    public class NUnitTest
    {
        private ThreadLocal<IWebDriver> driver;
        private ExtentReports extent;

        private static string Screenshot(IWebDriver driver, string testName)
        {
            string screenPath = Path.Combine($"Error Screenshot on {DateTime.Now:yyyy.MM.dd_HH-mm-ss}, {testName}.png");
            ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile(screenPath);
            return screenPath;
        }

        private static string Inform()
        {
            string result = $"1)OS : {Environment.OSVersion}\n " +
                $"2)Machine Name : {Environment.MachineName}\n " +
                $"3)User : {Environment.UserName}\n " +
                $"4).NET Version : {Environment.Version}\n";
            return result;
        }


        [OneTimeSetUp]
        public void SetupLogging()
        {
            driver = new ThreadLocal<IWebDriver>(() => DriverSingleton.GetDriver());

            extent = new ExtentReports();

            string projectRootPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\..\..\"));

            string folderName = "LogsAndReports";
            string outputFolderPath = "";
            if (File.Exists(Path.Combine(projectRootPath, folderName)))
            {
                File.Delete(Path.Combine(projectRootPath, folderName));
                outputFolderPath = Path.Combine(projectRootPath, folderName);
            }
            else
            {
                outputFolderPath = Path.Combine(projectRootPath, folderName);
            }
            
            Directory.SetCurrentDirectory(outputFolderPath);
            string currentprocessfolder = Path.Combine(outputFolderPath, $"Test Process on {DateTime.Now:yyyy.MM.dd_HH-mm-ss}");
            Directory.CreateDirectory(currentprocessfolder);
            Directory.SetCurrentDirectory(currentprocessfolder);

            string reportfolder = Path.Combine(currentprocessfolder, "Report");
            Directory.CreateDirectory("Report");
            Directory.SetCurrentDirectory(reportfolder);

            var sparkReporter = new ExtentSparkReporter(Path.Combine(reportfolder , $"TestReport on {DateTime.Now:yyyy.MM.dd_HH-mm-ss}.html"));
            extent.AttachReporter(sparkReporter);

            Directory.SetCurrentDirectory(currentprocessfolder);


            string logfolder = Path.Combine(currentprocessfolder, "Log");
            Directory.CreateDirectory(logfolder);
            Directory.SetCurrentDirectory(logfolder);

            string logFilePath = $"TestLog on .log";
            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Directory.SetCurrentDirectory(currentprocessfolder);
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
            ExtentTest passedtest = extent.CreateTest("RunNUnitTest");
            string info = Inform();
            passedtest.Info($"Environmental information :{info}");
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
            passedtest.Pass("Test completed successfully.");
            extent.Flush();
            Log.Information("NUnit Pass test completed.");
        }

        [Test]
        public void RunSkippedTest()
        {
            ExtentTest skippedtest = extent.CreateTest("RunSkippedTest");
            string info = Inform();
            skippedtest.Info($"Environmental information :{info}");
            skippedtest.Skip("Test skipped.");
            extent.Flush();

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

            ExtentTest failedtest = extent.CreateTest("RunFailTest");
            string info = Inform();
            failedtest.Info($"Environmental information :{info}");
            var languageDataFr = TestDataFactory.LanguageTestDataFR();
            try
            {
                feature.GivenTheUserIsOnTheHomepage();
                feature.WhenTheUserSwitchesTheSiteLanguageTo(languageDataFr.Language);
                feature.ThenTheUserShouldBeRedirectedTo(languageDataFr.ExpectedUrlFragment);
            }
            catch (Exception ex)
            {
                string screenshotPath = Screenshot(driver.Value, "RunFailTest");
                failedtest.Fail($"Test fail. Issue message: {ex}")
                    .AddScreenCaptureFromPath(screenshotPath);
                throw;
            }

            Log.Information("NUnit Fail test completed.");
            extent.Flush();
        }

        [TearDown]
        public void TearDown()
        {
            Log.Information("Cleaning up WebDriver");
            driver.Value?.Quit();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Log.Information("Disposing WebDriver.");
            driver.Dispose();
        }
    }
}
