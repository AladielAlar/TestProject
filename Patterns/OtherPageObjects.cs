using NUnit.Framework;
using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace Patterns
{
    public class AboutPage(IWebDriver driver) : BasePage(driver)
    {
        public string GetAboutHeader()
        {
            return driver.FindElement(By.TagName("h1")).Text;
        }
    }

    public class StudyProgramPage(IWebDriver driver) : BasePage(driver)
    {
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
    public class LanguagePage(IWebDriver driver) : BasePage(driver)
    {
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
