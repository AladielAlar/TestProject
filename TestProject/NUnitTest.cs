using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace WebTests
{
    [TestFixture]
    public class Tests
    {
        private ChromeDriver? driver;

        [SetUp]

        public void SetUp()
        {
            ChromeOptions chromeOptions = new();

            driver = new ChromeDriver(chromeOptions);
            driver.Manage().Window.Maximize();
        }

        [Test] // First

        public void NavigationTest()
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            driver.Navigate().GoToUrl("https://en.ehu.lt/");

            IWebElement aboutLink = driver.FindElement(By.LinkText("About")); // Here according task must be "About EHU" , but on todays site we have only "About" , so i change it .
            if (aboutLink == null)
            {
                Assert.Fail("The 'About' link was not found.");
            }

            Thread.Sleep(2000);
            aboutLink.Click();

            string currentUrl = driver.Url;
            if (currentUrl != "https://en.ehu.lt/about/")
            {
                Assert.Fail($"Expected URL: https://en.ehu.lt/about/, but got: {currentUrl}");
            }

            string pageTitle = driver.Title;
            if (pageTitle != "About") // Here according task must be "About EHU" , but on todays site we have only "About" , so i change it .
            {
                Assert.Fail($"Expected 'About', but got: {pageTitle}");
            }

            IWebElement header = driver.FindElement(By.TagName("h1"));
            if (header == null || header.Text != "About") // Here according task must be "About European Humanities University" , but on todays site we again have only "About" , so i change it .
            {
                Assert.Fail($"The header content does not match the expected value. We got {header.Text}");
            }
        }

        [Test] // Second 

        public void SearchTest()
        {
            driver.Navigate().GoToUrl("https://en.ehu.lt/");

            IWebElement headerSearch = driver.FindElement(By.ClassName("header-search"));

            Actions actions = new Actions(driver);
            actions.MoveToElement(headerSearch).Perform();

            IWebElement searchBar = driver.FindElement(By.ClassName("header-search__form"));
            searchBar.Click();
            Actions l = new Actions(driver); 
            actions.SendKeys("study programs").Build().Perform();

            Thread.Sleep(2000);
            IWebElement sendButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            sendButton.Click();

            string currentUrl = driver.Url;
            if (currentUrl != "https://en.ehu.lt/?s=study+programs")
            {
                Assert.Fail($"Expected URL: https://en.ehu.lt/?s=study+programs, but got: {currentUrl}");
            }
        }

        [Test] // Third
        public void LanguageTest()
        {
            driver.Navigate().GoToUrl("https://en.ehu.lt/");

            IWebElement languageSwitcher = driver.FindElement(By.ClassName("language-switcher"));
            Actions actions = new Actions(driver);
            actions.MoveToElement(languageSwitcher).Perform();

            IWebElement LT = driver.FindElement(By.LinkText("LT"));
            LT.Click();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => driver.Url.Contains("lt.ehu.lt"));

            string pageTitle = driver.FindElement(By.TagName("h2")).Text;
            if (pageTitle != "Apie mus")
            {
                Assert.Fail($"Expected 'Apie mus', but got: {pageTitle}");
            }
        }

        [Test] // fourth
        public void ContactFormTest() // honestly i don't find such form on site so this test is failed
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

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            IWebElement successMessage = wait.Until(driver => driver.FindElement(By.CssSelector(".success-message")));

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
            driver.Quit();
        }
    }
}
