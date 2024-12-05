using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns
{
    using OpenQA.Selenium;

    namespace WebTests.Builders
    {
        public class ContactFormBuilder
        {
            private readonly IWebDriver Driver;
            private readonly IWebElement nameField;
#pragma warning disable CS0649 // Field 'ContactFormBuilder.emailField' is never assigned to, and will always have its default value null
            private readonly IWebElement emailField;
            private readonly IWebElement messageField;
            private readonly IWebElement submitButton;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            public ContactFormBuilder(IWebDriver driver)
            {
                Driver = driver;
                nameField = driver.FindElement(By.Name("name"));
                nameField = driver.FindElement(By.Name("email"));
                nameField = driver.FindElement(By.Name("message"));
                nameField = driver.FindElement(By.CssSelector("button[type='submit']"));
            }

            public ContactFormBuilder SetName(string name)
            {
                nameField.Clear();
                nameField.SendKeys(name);
                return this;
            }

            public ContactFormBuilder SetEmail(string email)
            {
                emailField.Clear();
                emailField.SendKeys(email);
                return this;
            }

            public ContactFormBuilder SetMessage(string message)
            {
                messageField.Clear();
                messageField.SendKeys(message);
                return this;
            }

            public void Submit()
            {
                submitButton.Click();
            }
        }
    }

}
