using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Patterns;
using Patterns.WebTests.Builders;
using FluentAssertions;
using Serilog;

#pragma warning disable CS8604 // Possible null reference argument.

namespace WebTests
{
    [TestFixture, Parallelizable(ParallelScope.Fixtures)]

    public class Tests
    {
        private static readonly ThreadLocal<IWebDriver> driver = new(() => DriverSingleton.GetDriver());

        [OneTimeSetUp]
        public void SetupLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/tests.log", rollingInterval: RollingInterval.Day)
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

        [Test, Parallelizable(ParallelScope.Self)]
        [Category("Navigation")]
        public void NavigationTest()
        {
            Log.Debug("NavigationTest initialized.");
            try
            {
                var (baseUrl, linkText, expectedUrl, expectedTitle) = TestDataFactory.NavigationTestData();
                var basePage = new BasePage(driver.Value);
                var aboutPage = new AboutPage(driver.Value);

                basePage.NavigateTo(baseUrl);
                Log.Debug("Navigated to {BaseUrl}", baseUrl);

                var link = basePage.FindLinkByText(linkText);
                link.Click();
                Log.Debug("Clicked link with text {LinkText}", linkText);

                driver.Value.Url.Should().Be(expectedUrl, "URL should match the expected value");
                driver.Value.Title.Should().Be(expectedTitle, "Title should match the expected value");

                aboutPage.GetAboutHeader().Should().Be(expectedTitle, "Header should match the expected title");
                Log.Information("NavigationTest passed.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "NavigationTest failed.");
                throw;
            }
        }

        [Test, Parallelizable(ParallelScope.Self)]
        [Category("Search")]
        public void SearchTest()
        {
            Log.Debug("SearchTest initialized.");
            try
            {
                var (baseUrl, searchQuery, expectedUrl) = TestDataFactory.SearchTestData();
                var basePage = new BasePage(driver.Value);
                var studyProgramPage = new StudyProgramPage(driver.Value);

                basePage.NavigateTo(baseUrl);
                Log.Debug("Navigated to {BaseUrl}", baseUrl);

                studyProgramPage.SearchStudyPrograms(searchQuery);
                Log.Debug("Searched for {SearchQuery}", searchQuery);

                string currentUrl = studyProgramPage.GetCurrentUrl();
                currentUrl.Should().Be(expectedUrl, "URL should match the expected value");
                Log.Information("SearchTest passed.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "SearchTest failed.");
                throw;
            }
        }

        [Test, Parallelizable(ParallelScope.Self)]
        [Category("Localization")]
        public void LanguageTest()
        {
            Log.Debug("LanguageTest initialized.");
            try
            {
                var (baseUrl, language, expectedUrlFragment, expectedHeader) = TestDataFactory.LanguageTestData();
                var basePage = new BasePage(driver.Value);
                var languagePage = new LanguagePage(driver.Value);

                basePage.NavigateTo(baseUrl);
                Log.Debug("Navigated to {BaseUrl}", baseUrl);

                languagePage.SwitchTolithuanian();
                Log.Debug("Switched to language {Language}", language);

                string pageTitle = driver.Value.FindElement(By.TagName("h2")).Text;
                pageTitle.Should().Be(expectedHeader, "Header should match the expected value");
                Log.Information("LanguageTest passed.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "LanguageTest failed.");
                throw;
            }
        }

        [Test, Ignore("We cannot run it, it is just an example of test with form")]
        [Category("ContactForm")]
        public void ContactFormTest()
        {
            Log.Debug("ContactFormTest initialized.");
            try
            {
                var (baseUrl, name, email, message, expectedMessage) = TestDataFactory.ContactFormTestData();
                var basePage = new BasePage(driver.Value);

                basePage.NavigateTo(baseUrl);
                Log.Debug("Navigated to {BaseUrl}", baseUrl);

                var contactFormBuilder = new ContactFormBuilder(driver.Value)
                    .SetName(name)
                    .SetEmail(email)
                    .SetMessage(message);

                contactFormBuilder.Submit();
                Log.Debug("Submitted the contact form.");

                WebDriverWait wait = new(driver.Value, TimeSpan.FromSeconds(10));
                var successMessage = wait.Until(d => d.FindElement(By.CssSelector(".success-message")));

                successMessage.Displayed.Should().BeTrue("Success message should be displayed");
                successMessage.Text.Should().Contain(expectedMessage, "Message should contain the expected text");
                Log.Information("ContactFormTest passed.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ContactFormTest failed.");
                throw;
            }
        }

        [TearDown]
        public void TearDown()
        {
            Log.Information("Test finished: {TestName}", TestContext.CurrentContext.Test.Name);
            DriverSingleton.QuitDriver();
        }
    }
}
