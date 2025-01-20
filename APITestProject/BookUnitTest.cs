using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using System.Reflection;
using Serilog;
using OpenQA.Selenium.DevTools.V129.HeadlessExperimental;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text.Json;
using System.Text;

namespace APITestProject
{
    public class BaseTest
    {
        protected HttpClient Client;
#pragma warning disable CA2211 // Non-constant fields should not be visible
        protected static ExtentReports extent;
#pragma warning restore CA2211 // Non-constant fields should not be visible
        private ExtentTest? test;
        private string? token;
        private string? bookId;
        private string? bookTitle;
        private string? bookAuthor;
        private string? bookISBN;
        private string? bookPublishedDate;

        private static string Inform()
        {
            string result = $"1)OS : {Environment.OSVersion}\n " +
                $"2)Machine Name : {Environment.MachineName}\n " +
                $"3)User : {Environment.UserName}\n " +
                $"4).NET Version : {Environment.Version}\n";
            return result;
        }

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            extent = new ExtentReports();
            Client = new HttpClient
            {
                BaseAddress = new Uri("https://lecture-books-api.azurewebsites.net")
            };

            var tokenRequest = new FormUrlEncodedContent(
            [
               new KeyValuePair<string, string>("client_Id", "a8d2586b-e933-4623-b654-736e4a5d4d5e"),
               new KeyValuePair<string, string>("client_Secret", "Knb8Q~poM-r_T~geHJkoDAflxpI5D6NZ3F~IJcDt"),
               new KeyValuePair<string, string>("scope", "api://598eeaa3-cb83-43a0-9980-15c0da758bab/.default"),
               new KeyValuePair<string, string>("grant_type", "client_credentials")
            ]);

            var tokenResponse = await Client.PostAsync("https://login.microsoftonline.com/d0931c35-94f4-49e6-8587-c0715af471f3/oauth2/v2.0/token", tokenRequest);

            if (tokenResponse.IsSuccessStatusCode)
            {
                var responseBody = await tokenResponse.Content.ReadAsStringAsync();
                var jsonDocument = JsonDocument.Parse(responseBody);
                token = jsonDocument.RootElement.GetProperty("access_token").GetString();

                if (string.IsNullOrEmpty(token))
                {
                    throw new Exception("Failed to retrieve access token.");
                }

                Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",token);
            }
            else
            {
                var errorResponse = await tokenResponse.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve token. Status: {tokenResponse.StatusCode}, Response: {errorResponse}");
            }
            string projectRootPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\"));

            string folderName = "LogsAndReportsAPI";
            string outputFolderPath = Path.Combine(projectRootPath, folderName);
            Directory.SetCurrentDirectory(outputFolderPath);
            string currentprocessfolder = Path.Combine(outputFolderPath, $"APITest Process on {DateTime.Now:yyyy.MM.dd_HH-mm-ss}");
            Directory.CreateDirectory(currentprocessfolder);
            Directory.SetCurrentDirectory(currentprocessfolder);

            string reportfolder = Path.Combine(currentprocessfolder, "Report");
            Directory.CreateDirectory("Report");
            Directory.SetCurrentDirectory(reportfolder);

            var sparkReporter = new ExtentSparkReporter(Path.Combine(reportfolder, $"APITestReport on {DateTime.Now:yyyy.MM.dd_HH-mm-ss}.html"));
            extent.AttachReporter(sparkReporter);

            Directory.SetCurrentDirectory(currentprocessfolder);


            string logfolder = Path.Combine(currentprocessfolder, "Log");
            Directory.CreateDirectory(logfolder);
            Directory.SetCurrentDirectory(logfolder);

            string logFilePath = $"TestLog on .log";
            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        [SetUp]
        public void BeforeEachTest()
        {
            Log.Information("Test started: {TestName}", TestContext.CurrentContext.Test.Name);
        }

        [Test, Order(1)]
        public async Task CreateBook()
        {
            test = extent.CreateTest("CreateBook");
            string info = Inform();
            test.Info($"Environmental information :{info}");
            var newBook = new 
            {
                title = "C Book",
                author = "Author C",
                isbn = "1234567890193",
                publishedDate = "2023-01-20T11:04:40.841Z"
            };

            var bookscheck = await Client.GetAsync("/Books");
            if (bookscheck.IsSuccessStatusCode)
            {
                var responseBodyCheck = await bookscheck.Content.ReadAsStringAsync();
                using JsonDocument documentCheck = JsonDocument.Parse(responseBodyCheck);
                var books = documentCheck.RootElement.EnumerateArray();

                foreach (var book in books)
                {
                    if (book.GetProperty("title").GetString() == newBook.title)
                    {
                        test.Warning($"Book with Title '{newBook.title}' already exists.");
                        return;
                    }
                }
            }
            else
            {
                test.Warning($"Status Code: {(int)bookscheck.StatusCode}");
            }

#pragma warning disable CA1869 // Cache and reuse 'JsonSerializerOptions' instances
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
#pragma warning restore CA1869 // Cache and reuse 'JsonSerializerOptions' instances
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(newBook, options), Encoding.UTF8, "application/json");
            var expectedValues = new Dictionary<string, string> { { "author", newBook.author }, { "isbn", newBook.isbn }, { "publishedDate", newBook.publishedDate }, { "title", newBook.title } };
            var response = await Client.PostAsync("/Books", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            Log.Information($"Created Book data : {responseBody}");
            if ((int)response.StatusCode == 201)
            {
                using JsonDocument document = JsonDocument.Parse(responseBody);
                JsonElement root = document.RootElement;

                bookId = root.GetProperty("id").GetString();
                bookAuthor = root.GetProperty("author").GetString();
                bookISBN = root.GetProperty("isbn").GetString();
                bookPublishedDate = root.GetProperty("publishedDate").GetString();
                bookTitle = root.GetProperty("title").GetString();

#pragma warning disable CS8604 // Possible null reference argument.
                var actualValues = new Dictionary<string, string> { { "author", bookAuthor }, { "isbn", bookISBN }, { "publishedDate", bookPublishedDate }, { "title", bookTitle } };
#pragma warning restore CS8604 // Possible null reference argument.

                bool allMatch = true;
                foreach (var key in expectedValues.Keys)
                {
                    if (!expectedValues[key].Equals(actualValues[key]))
                    {
                        test.Fail($"Mismatch: we need {expectedValues[key]} but we have {actualValues[key]} for {key}");
                        allMatch = false;
                    }
                }

                if (allMatch)
                {
                    test.Pass("All data match");
                }
            }
            else
            {
                test.Fail($"Expected status code 201 but got {(int)response.StatusCode}.");
            }
        }

        [Test, Order(2)]
        public async Task GetAllBooks()
        {
            test = extent.CreateTest("GetAllBooks");
            string info = Inform();
            test.Info($"Environmental information :{info}");

            var response = await Client.GetAsync("/Books");
            var responseBody = await response.Content.ReadAsStringAsync();

            if ((int)response.StatusCode == 200 && responseBody.Contains("title"))
            {
                test.Pass("Successfully retrieved list of books.");
            }
            else
            {
                test.Fail($"Expected status code 200 and response body containing 'title' but got status code {(int)response.StatusCode}.");
            }
        }

        [Test, Order(3)]
        public async Task GetBookById()
        {
            test = extent.CreateTest("GetBookById");
            string info = Inform();
            test.Info($"Environmental information :{info}");

            var response = await Client.GetAsync($"/Books/{bookId}");

            if ((int)response.StatusCode == 200)
            {
                test.Pass("Successfully retrieved book by ID.");
            }
            else
            {
                test.Fail($"Expected status code 200 but got {(int)response.StatusCode}.");
            }
        }

        [Test, Order(4)]
        public async Task DeleteBookById()
        {
            test = extent.CreateTest("DeleteBookById");
            string info = Inform();
            test.Info($"Environmental information :{info}");
            var response = await Client.DeleteAsync($"/Books/{bookId}");

            if ((int)response.StatusCode == 204)
            {
                test.Pass("Successfully deleted book.");
            }
            else
            {
                test.Fail($"Expected status code 204 but got {(int)response.StatusCode}.");
            }
        }


        [TearDown]
        public void AfterEachTest()
        {
            var testStatus = TestContext.CurrentContext.Result.Outcome.Status;
            var testMessage = TestContext.CurrentContext.Result.Message;

            switch (testStatus)
            {
                case NUnit.Framework.Interfaces.TestStatus.Passed:
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    test.Pass("Test passed.");
                    Log.Information("Test passed.");
                    break;

                case NUnit.Framework.Interfaces.TestStatus.Failed:
                    test.Fail($"Test failed. Message: {testMessage}");
                    Log.Error($"Test failed. Message: {testMessage}");
                    break;

                case NUnit.Framework.Interfaces.TestStatus.Skipped:
                    test.Skip("Test skipped.");
                    Log.Information("Test skipped.");
                    break;
            }
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Client.Dispose();
            extent.Flush();
        }
    }
}