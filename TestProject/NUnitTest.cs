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

        [Test, Parallelizable(ParallelScope.Self)]
        [Category("Navigation")]
        public void NavigationTest()
        {
#pragma warning disable CS8604 // Possible null reference argument.
            var (baseUrl, linkText, expectedUrl, expectedTitle) = TestDataFactory.NavigationTestData();

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

        [Test, Parallelizable(ParallelScope.Self)]
        [Category("Search")] // Second 

        public void SearchTest()
        {
            var (baseUrl, searchQuery, expectedUrl) = TestDataFactory.SearchTestData();
            var basePage = new BasePage(driver.Value);
            var studyProgramPage = new StudyProgramPage(driver.Value);

            basePage.NavigateTo(baseUrl);
            studyProgramPage.SearchStudyPrograms(searchQuery);

            string currentUrl = studyProgramPage.GetCurrentUrl();

            if (currentUrl != expectedUrl)
            {
                Assert.Fail($"Expected URL: {expectedUrl}, but got: {currentUrl}");
            }
        }

        [Test, Parallelizable(ParallelScope.Self)]
        [Category("Localization")] // Third
        public void LanguageTest()
        {
            var (baseUrl, language, expectedUrlFragment, expectedHeader) = TestDataFactory.LanguageTestData();
            var basePage = new BasePage(driver.Value);
            var languagePage = new LanguagePage(driver.Value);
            basePage.NavigateTo(baseUrl);
            languagePage.SwitchTolithuanian();
            string pageTitle = driver.Value.FindElement(By.TagName("h2")).Text;

            if (pageTitle != expectedHeader)
            {
                Assert.Fail($"Expected {expectedHeader}, but got: {pageTitle}");
            }
        }

        [Test , Ignore("We cannot run it, it is just an example of test with form")] // fourth
        [Category("ContactForm")]
        public void ContactFormTest()
        {
            var (baseUrl, name, email, message, expectedMessage) = TestDataFactory.ContactFormTestData();
            var basePage = new BasePage(driver.Value);
            basePage.NavigateTo(baseUrl);

            var contactFormBuilder = new ContactFormBuilder(driver.Value)
                .SetName(name)
                .SetEmail(email)
                .SetMessage(message);

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
