using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace WebTests
{
    [TestFixture, Parallelizable(ParallelScope.Fixtures)]
    public class Tests
    {
        private static readonly ThreadLocal<ChromeDriver> driver = new(() => new ChromeDriver()); // here i a bit rebuild code because if we create driver in SetUp there we have a problem with running each test in parallel

        [SetUp]

        public void SetUp()
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            driver.Value.Manage().Window.Maximize();
        }

        [Test , Parallelizable(ParallelScope.Children)]
        [TestCase("https://en.ehu.lt/", "About", "https://en.ehu.lt/about/", "About")]
        [Category("Navigation")]
        public void NavigationTest(string baseUrl, string linkText, string expectedUrl, string expectedTitle)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            driver.Value.Navigate().GoToUrl(baseUrl);

            IWebElement link = driver.Value.FindElement(By.LinkText(linkText)) ?? throw new Exception($"The link '{linkText}' was not found.");
            link.Click();

            string currentUrl = driver.Value.Url;
            if (currentUrl != expectedUrl)
            {
                throw new Exception($"Expected URL: {expectedUrl}, but got: {currentUrl}");
            }

            string pageTitle = driver.Value.Title;
            if (pageTitle != expectedTitle)
            {
                throw new Exception($"Expected title: {expectedTitle}, but got: {pageTitle}");
            }
        }

        [Test, Parallelizable(ParallelScope.Children)]
        [TestCase("https://en.ehu.lt/", "study programs", "https://en.ehu.lt/?s=study+programs")]
        [Category("Search")] // Second 

        public void SearchTest(string baseUrl , string query, string expectedUrl)
        {
            driver.Value.Navigate().GoToUrl(baseUrl);

            IWebElement headerSearch = driver.Value.FindElement(By.ClassName("header-search"));

            Actions actions = new(driver.Value);
            actions.MoveToElement(headerSearch).Perform();

            IWebElement searchBar = driver.Value.FindElement(By.ClassName("header-search__form"));
            searchBar.Click(); 
            actions.SendKeys(query).Build().Perform();

            IWebElement sendButton = driver.Value.FindElement(By.CssSelector("button[type='submit']"));
            sendButton.Click();

            string currentUrl = driver.Value.Url;
            if (currentUrl != expectedUrl)
            {
                Assert.Fail($"Expected URL: {expectedUrl}, but got: {currentUrl}");
            }
        }

        [Test, Parallelizable(ParallelScope.Children)]
        [TestCase("https://en.ehu.lt/", "LT", "lt.ehu.lt", "Apie mus")]
        [TestCase("https://en.ehu.lt/", "EN", "en.ehu.lt", "About")] // i also add English language check 
        [Category("Localization")] // Third
        public void LanguageTest(string baseUrl , string language, string expectedUrlFragment, string expectedHeader)
        {
            driver.Value.Navigate().GoToUrl(baseUrl);

            IWebElement languageSwitcher = driver.Value.FindElement(By.ClassName("language-switcher"));
            Actions actions = new(driver.Value);
            actions.MoveToElement(languageSwitcher).Perform();

            IWebElement lang = driver.Value.FindElement(By.LinkText(language));
            lang.Click();

            WebDriverWait wait = new(driver.Value, TimeSpan.FromSeconds(2));
            wait.Until(driver => driver.Url.Contains(expectedUrlFragment));

            string pageTitle = driver.Value.FindElement(By.TagName("h2")).Text;
            if (pageTitle != expectedHeader)
            {
                Assert.Fail($"Expected {expectedHeader}, but got: {pageTitle}");
            }
        }

        [Test , Ignore("We cannot run it, it is just an example of test with form")] // fourth
        [TestCase("https://en.ehu.lt/contact/", "name", "email", "message", "button[type='submit']", ".success-message")]
        [Category("ContactForm")]
        public void ContactFormTest(string baseUrl, string nameField, string emailField, string messageField, string button, string resultMessage)
        {
            driver.Value.Navigate().GoToUrl(baseUrl);

            IWebElement name = driver.Value.FindElement(By.Name(nameField));
            name.SendKeys("Test User");

            IWebElement email = driver.Value.FindElement(By.Name(emailField));
            email.SendKeys("testuser@example.com");

            IWebElement message = driver.Value.FindElement(By.Name(messageField));
            message.SendKeys("This is a test message for verification purposes.");

            IWebElement sendButton = driver.Value.FindElement(By.CssSelector(button));
            sendButton.Click();

            WebDriverWait wait = new(driver.Value, TimeSpan.FromSeconds(10));
            IWebElement successMessage = wait.Until(driver => driver.FindElement(By.CssSelector(resultMessage)));

            if (successMessage.Displayed)
            {
            }
            else
            {
                Assert.Fail("Test Failed: message is not displayed.");
            }

            string expectedMessage = "Thank you for your message. It has been sent.";
            if (successMessage.Text.Contains(expectedMessage))
            {
            }
            else
            {
                Assert.Fail($"Test Failed: Expected message: '{expectedMessage}', but got: '{successMessage.Text}'");
            }
        }

        [TearDown]
        public void TearDown()
        {
            Console.WriteLine("Selenium webdriver quit");
            driver.Value.Quit();
        }
    }
}
