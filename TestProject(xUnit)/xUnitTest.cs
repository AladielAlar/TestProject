using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Patterns;
using Patterns.WebTests.Builders;
using FluentAssertions;
using Serilog;

namespace TestProject_xUnit_
{
    [Collection("Parallel Tests")]
    public class Tests : IDisposable
    {
        private readonly IWebDriver driver;
        public Tests()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/tests.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            driver = DriverSingleton.GetDriver();
            driver.Manage().Window.Maximize();
        }

        [Fact , Trait("Category", "Navigation")]
        public void NavigationTest()
        {
            Log.Debug("NavigationTest initialized.");
            try
            {
                var (baseUrl, linkText, expectedUrl, expectedTitle) = TestDataFactory.NavigationTestData();
                var basePage = new BasePage(driver);
                var aboutPage = new AboutPage(driver);

                basePage.NavigateTo(baseUrl);
                Log.Debug("Navigated to {BaseUrl}", baseUrl);

                var link = basePage.FindLinkByText(linkText);
                link.Click();
                Log.Debug("Clicked link with text {LinkText}", linkText);

                driver.Url.Should().Be(expectedUrl, "URL should match the expected value");
                driver.Title.Should().Be(expectedTitle, "Title should match the expected value");

                aboutPage.GetAboutHeader().Should().Be(expectedTitle, "Header should match the expected title");
                Log.Information("NavigationTest passed.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "NavigationTest failed.");
                throw;
            }
        }

        [Fact ,Trait("Category", "Search")] // Category
        public void SearchTest()
        {
            Log.Debug("SearchTest initialized.");
            try
            {
                var (baseUrl, searchQuery, expectedUrl) = TestDataFactory.SearchTestData();
                var basePage = new BasePage(driver);
                var studyProgramPage = new StudyProgramPage(driver);

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

        [Fact, Trait("Category", "Localization")] // Category
        public void LanguageTest()
        {
            Log.Debug("LanguageTest initialized.");
            try
            {
                var (baseUrl, language, expectedUrlFragment, expectedHeader) = TestDataFactory.LanguageTestData();
                var basePage = new BasePage(driver);
                var languagePage = new LanguagePage(driver);

                basePage.NavigateTo(baseUrl);
                Log.Debug("Navigated to {BaseUrl}", baseUrl);

                languagePage.SwitchTolithuanian();
                Log.Debug("Switched to language {Language}", language);

                string pageTitle = driver.FindElement(By.TagName("h2")).Text;
                pageTitle.Should().Be(expectedHeader, "Header should match the expected value");
                Log.Information("LanguageTest passed.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "LanguageTest failed.");
                throw;
            }
        }

#pragma warning disable xUnit1004 // Test methods should not be skipped
        [Fact(Skip = "We cannot run it, it is just an example of test with form"), Trait("Category", "ContactForm")] // Skip
#pragma warning restore xUnit1004 // Test methods should not be skipped
        public void ContactFormTest()
        {
            Log.Debug("ContactFormTest initialized.");
            try
            {
                var (baseUrl, name, email, message, expectedMessage) = TestDataFactory.ContactFormTestData();
                var basePage = new BasePage(driver);

                basePage.NavigateTo(baseUrl);
                Log.Debug("Navigated to {BaseUrl}", baseUrl);

                var contactFormBuilder = new ContactFormBuilder(driver)
                    .SetName(name)
                    .SetEmail(email)
                    .SetMessage(message);

                contactFormBuilder.Submit();
                Log.Debug("Submitted the contact form.");

                WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
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
        public void Dispose()
        {
            Log.Information("Tests disposed.");
            DriverSingleton.QuitDriver();
        }
    }
}