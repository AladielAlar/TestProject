using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns
{
    public class BasePage(IWebDriver driver)
    {
        protected readonly IWebDriver driver = driver;

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
}
