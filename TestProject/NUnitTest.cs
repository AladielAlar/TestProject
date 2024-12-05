using System;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Xml.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Patterns;
using Patterns.WebTests.Builders;

namespace WebTests
{
    [TestFixture, Parallelizable(ParallelScope.Fixtures)]

    public class Tests
    {
        private static readonly ThreadLocal<IWebDriver> driver = new(() => DriverSingleton.GetDriver());

        [SetUp]

        public void SetUp()
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            driver.Value.Manage().Window.Maximize();
        }

        [Test, Parallelizable(ParallelScope.Children)]
        [TestCase("https://en.ehu.lt/", "About", "https://en.ehu.lt/about/", "About")]
        [Category("Navigation")]
        public void NavigationTest(string baseUrl, string linkText, string expectedUrl, string expectedTitle)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            var basePage = new BasePage(driver.Value);
            var aboutPage = new AboutPage(driver.Value);

            basePage.NavigateTo(baseUrl);

            IWebElement link = basePage.FindLinkByText(linkText);
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

            string header = aboutPage.GetAboutHeader();
            if (header != expectedTitle)
            {
                throw new Exception($"Expected title: {expectedTitle}, but got: {header}");
            }
        }

        [Test, Parallelizable(ParallelScope.Children)]
        [TestCase("https://en.ehu.lt/", "study programs", "https://en.ehu.lt/?s=study+programs")]
        [Category("Search")] // Second 

        public void SearchTest(string baseUrl , string query, string expectedUrl)
        {
            var basePage = new BasePage(driver.Value);
            var studyProgramPage = new StudyProgramPage(driver.Value);

            basePage.NavigateTo(baseUrl);
            studyProgramPage.SearchStudyPrograms(query);

            string currentUrl = studyProgramPage.GetCurrentUrl();

            if (currentUrl != expectedUrl)
            {
                Assert.Fail($"Expected URL: {expectedUrl}, but got: {currentUrl}");
            }
        }

        [Test, Parallelizable(ParallelScope.Children)]
        [TestCase("https://en.ehu.lt/", "en.ehu.lt", "About")] 
        [Category("Localization")] // Third
        public void LanguageTest(string baseUrl , string expectedUrlFragment, string expectedHeader)
        { 
            var basePage = new BasePage(driver.Value);
            var languagePage = new LanguagePage(driver.Value);
            basePage.NavigateTo(baseUrl);
            languagePage.SwitchToEnglish();

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
            var basePage = new BasePage(driver.Value);
            basePage.NavigateTo(baseUrl);

            var contactFormBuilder = new ContactFormBuilder(driver.Value)
                .SetName(nameField)
                .SetEmail(emailField)
                .SetMessage(messageField);

            contactFormBuilder.Submit();

            WebDriverWait wait = new(driver.Value, TimeSpan.FromSeconds(10));
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
            DriverSingleton.QuitDriver();
        }
    }
}
