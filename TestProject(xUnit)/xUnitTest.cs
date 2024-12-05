using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Xunit;
using Patterns;
using Patterns.WebTests.Builders;

namespace TestProject_xUnit_
{
    [Collection("Parallel Tests")]
    public class Tests : IDisposable
    {
        private readonly IWebDriver driver;
        public Tests()
        {
            driver = DriverSingleton.GetDriver();
            driver.Manage().Window.Maximize();
        }

        [Fact , Trait("Category", "Navigation")]
        public void NavigationTest()
        {
            var (baseUrl, linkText, expectedUrl, expectedTitle) = TestDataFactory.NavigationTestData();

            var basePage = new BasePage(driver);
            var aboutPage = new AboutPage(driver);

            basePage.NavigateTo(baseUrl);

            IWebElement link = basePage.FindLinkByText(linkText);
            link.Click();

            Assert.Equal(expectedUrl, driver.Url);
            Assert.Equal(expectedTitle, driver.Title);

            string header = aboutPage.GetAboutHeader();
            Assert.Equal(expectedTitle, header);
        }

        [Fact ,Trait("Category", "Search")] // Category
        public void SearchTest()
        {
            var (baseUrl, searchQuery, expectedUrl) = TestDataFactory.SearchTestData();
            var basePage = new BasePage(driver);
            var studyProgramPage = new StudyProgramPage(driver);

            basePage.NavigateTo(baseUrl);
            studyProgramPage.SearchStudyPrograms(searchQuery);

            string currentUrl = studyProgramPage.GetCurrentUrl();
            Assert.Equal(expectedUrl, currentUrl);
        }

        [Fact, Trait("Category", "Localization")] // Category
        public void LanguageTest()
        {
            var (baseUrl, language, expectedUrlFragment, expectedHeader) = TestDataFactory.LanguageTestData();

            var basePage = new BasePage(driver);
            var languagePage = new LanguagePage(driver);

            basePage.NavigateTo(baseUrl);
            languagePage.SwitchTolithuanian();

            string pageTitle = driver.FindElement(By.TagName("h2")).Text;
            Assert.Equal(expectedHeader, pageTitle);
        }

#pragma warning disable xUnit1004 // Test methods should not be skipped
        [Fact(Skip = "We cannot run it, it is just an example of test with form"), Trait("Category", "ContactForm")] // Skip
#pragma warning restore xUnit1004 // Test methods should not be skipped
        public void ContactFormTest()
        {
            var (baseUrl, name, email, message, expectedMessage) = TestDataFactory.ContactFormTestData();
            var basePage = new BasePage(driver);
            basePage.NavigateTo(baseUrl);

            var contactFormBuilder = new ContactFormBuilder(driver)
                .SetName(name)
                .SetEmail(email)
                .SetMessage(message);

            contactFormBuilder.Submit();

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(2));
            IWebElement successMessage = wait.Until(driver => driver.FindElement(By.CssSelector(".success-message")));

            Assert.True(successMessage.Displayed);
            Assert.Contains(expectedMessage, successMessage.Text);
        }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public void Dispose()
        {
            Console.WriteLine("Selenium WebDriver quit");
            DriverSingleton.QuitDriver();
        }
    }
}