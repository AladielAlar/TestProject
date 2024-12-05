using NUnit.Framework;
using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace Patterns
{
    public class BasePage
    {
        protected readonly IWebDriver driver;

        public BasePage(IWebDriver driver)
        {
            this.driver = driver;
        }
        public void NavigateTo(string url)
        {
            driver.Navigate().GoToUrl(url);
        }
        public IWebElement FindLinkByText(string linkText)
        {
            return driver.FindElement(By.LinkText(linkText));
        }
        public string GetPageTitle()
        {
            return driver.Title;
        }
    }

    public class AboutPage : BasePage
    {
        public AboutPage(IWebDriver driver) : base(driver) { }

        public string GetAboutHeader()
        {
            return driver.FindElement(By.TagName("h1")).Text;
        }
    }

    public class StudyProgramPage : BasePage
    {
        public StudyProgramPage(IWebDriver driver) : base(driver) { }
        public void SearchStudyPrograms(string query)
        {
            IWebElement headerSearch = driver.FindElement(By.ClassName("header-search"));

            Actions actions = new(driver);
            actions.MoveToElement(headerSearch).Perform();

            IWebElement searchBar = driver.FindElement(By.ClassName("header-search__form"));
            searchBar.Click();
            actions.SendKeys(query).Build().Perform();

            IWebElement sendButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            sendButton.Click();
        }
        public string GetCurrentUrl()
        {
            return driver.Url;
        }
    }
    public class LanguagePage : BasePage
    {
        public LanguagePage(IWebDriver driver) : base(driver) { }
        public void SwitchTolithuanian()
        {
            IWebElement languageSwitcher = driver.FindElement(By.ClassName("language-switcher"));
            Actions actions = new(driver);
            actions.MoveToElement(languageSwitcher).Perform();

            IWebElement lang = driver.FindElement(By.LinkText("LT"));
            lang.Click();
        }
    }
}
