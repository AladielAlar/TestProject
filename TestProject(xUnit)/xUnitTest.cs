using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace TestProject_xUnit_
{
    [Collection("Parallel Tests")]
    public class Tests : IDisposable
    {
        private readonly ChromeDriver driver;

        public Tests()
        {
            ChromeOptions chromeOptions = new();
            driver = new ChromeDriver(chromeOptions);
            driver.Manage().Window.Maximize();
        }

        [Theory]
        [InlineData("https://en.ehu.lt/", "About", "https://en.ehu.lt/about/", "About")]
        public void NavigationTest(string baseUrl, string linkText, string expectedUrl, string expectedTitle)
        {
            driver.Navigate().GoToUrl(baseUrl);

            IWebElement aboutLink = driver.FindElement(By.LinkText(linkText));
            Assert.NotNull(aboutLink);

            aboutLink.Click();

            Assert.Equal(expectedUrl, driver.Url);
            Assert.Equal(expectedTitle, driver.Title);

            IWebElement header = driver.FindElement(By.TagName("h1"));
            Assert.NotNull(header);
            Assert.Equal(expectedTitle, header.Text);
        }

        [Fact ,Trait("Category", "Search")] // Category
        public void SearchTest()
        {
            driver.Navigate().GoToUrl("https://en.ehu.lt/");

            IWebElement headerSearch = driver.FindElement(By.ClassName("header-search"));
            Actions actions = new(driver);
            actions.MoveToElement(headerSearch).Perform();

            IWebElement searchBar = driver.FindElement(By.ClassName("header-search__form"));
            searchBar.Click();
            actions.SendKeys("study programs").Build().Perform();

            IWebElement sendButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            sendButton.Click();

            string currentUrl = driver.Url;
            Assert.Equal("https://en.ehu.lt/?s=study+programs", currentUrl);
        }

        [Fact, Trait("Category", "Localization")] // Category
        public void LanguageTest()
        {
            driver.Navigate().GoToUrl("https://en.ehu.lt/");

            IWebElement languageSwitcher = driver.FindElement(By.ClassName("language-switcher"));
            Actions actions = new(driver);
            actions.MoveToElement(languageSwitcher).Perform();

            IWebElement LT = driver.FindElement(By.LinkText("LT"));
            LT.Click();

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.Url.Contains("lt.ehu.lt"));

            string pageTitle = driver.FindElement(By.TagName("h2")).Text;
            if (pageTitle != "Apie mus")
            {
                Assert.Fail($"Expected 'Apie mus', but got: {pageTitle}");
            }
        }

#pragma warning disable xUnit1004 // Test methods should not be skipped
        [Fact(Skip = "We cannot run it, it is just an example of test with form"), Trait("Category", "ContactForm")] // Skip
#pragma warning restore xUnit1004 // Test methods should not be skipped
        public void ContactFormTest()
        {
            driver.Navigate().GoToUrl("https://en.ehu.lt/contact/");

            IWebElement nameField = driver.FindElement(By.Name("name"));
            nameField.SendKeys("Test User");

            IWebElement emailField = driver.FindElement(By.Name("email"));
            emailField.SendKeys("testuser@example.com");

            IWebElement messageField = driver.FindElement(By.Name("message"));
            messageField.SendKeys("This is a test message for verification purposes.");

            IWebElement sendButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            sendButton.Click();

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            IWebElement successMessage = wait.Until(driver => driver.FindElement(By.CssSelector(".success-message")));

            Assert.True(successMessage.Displayed);

            string expectedMessage = "Thank you for your message. It has been sent.";
            Assert.Contains(expectedMessage, successMessage.Text);
        }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public void Dispose()
        {
            Console.WriteLine("Selenium webdriver quit");
            driver.Quit();
        }
    }
}