using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns
{
    public static class TestDataFactory
    {
        public static (string BaseUrl, string LinkText, string ExpectedUrl, string ExpectedTitle) NavigationTestData()
        {
            return ("https://en.ehu.lt/", "About", "https://en.ehu.lt/about/", "About");
        }

        public static (string BaseUrl, string SearchQuery, string ExpectedUrl) SearchTestData()
        {
            return ("https://en.ehu.lt/", "study programs", "https://en.ehu.lt/?s=study+programs");
        }

        public static (string BaseUrl, string Language, string ExpectedUrlFragment, string ExpectedHeader) LanguageTestData()
        {
            return ("https://en.ehu.lt/", "LT", "lt.ehu.lt", "Apie mus");
        }

        public static (string BaseUrl, string Name, string Email, string Message, string ExpectedMessage) ContactFormTestData()
        {
            return ("https://en.ehu.lt/contact/", "Test User", "testuser@example.com",
                "This is a test message.", "Thank you for your message. It has been sent.");
        }
    }
}
