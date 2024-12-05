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

        [Theory]
        [InlineData("https://en.ehu.lt/", "About", "https://en.ehu.lt/about/", "About")]
        public void NavigationTest(string baseUrl, string linkText, string expectedUrl, string expectedTitle)
        {
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
            var basePage = new BasePage(driver);
            var studyProgramPage = new StudyProgramPage(driver);

            basePage.NavigateTo("https://en.ehu.lt/");
            studyProgramPage.SearchStudyPrograms("study programs");

            string currentUrl = studyProgramPage.GetCurrentUrl();
            Assert.Equal("https://en.ehu.lt/?s=study+programs", currentUrl);
        }

        [Fact, Trait("Category", "Localization")] // Category
        public void LanguageTest()
        {
            var basePage = new BasePage(driver);
            var languagePage = new LanguagePage(driver);

            basePage.NavigateTo("https://en.ehu.lt/");
            languagePage.SwitchToEnglish();

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => driver.Url.Contains("en.ehu.lt"));

            string pageTitle = driver.FindElement(By.TagName("h2")).Text;
            Assert.Equal("About", pageTitle);
        }

#pragma warning disable xUnit1004 // Test methods should not be skipped
        [Fact(Skip = "We cannot run it, it is just an example of test with form"), Trait("Category", "ContactForm")] // Skip
#pragma warning restore xUnit1004 // Test methods should not be skipped
        public void ContactFormTest()
        {
            var basePage = new BasePage(driver);
            basePage.NavigateTo("https://en.ehu.lt/contact/");

            var contactFormBuilder = new ContactFormBuilder(driver)
                .SetName("Test User")
                .SetEmail("testuser@example.com")
                .SetMessage("This is a test message.");

            contactFormBuilder.Submit();

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            IWebElement successMessage = wait.Until(driver => driver.FindElement(By.CssSelector(".success-message")));

            Assert.True(successMessage.Displayed);
            Assert.Contains("Thank you for your message. It has been sent.", successMessage.Text);
        }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public void Dispose()
        {
            Console.WriteLine("Selenium WebDriver quit");
            DriverSingleton.QuitDriver();
        }
    }
}