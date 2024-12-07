﻿using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Patterns
{
    public class DriverSingleton
    {

        private static readonly ConcurrentDictionary<int, IWebDriver> driverInstances = new();

        private DriverSingleton() { }

        public static IWebDriver GetDriver()
        {
#pragma warning disable CA1840 // Use 'Environment.CurrentManagedThreadId'
            int threadId = Thread.CurrentThread.ManagedThreadId;

            return driverInstances.GetOrAdd(threadId, _ =>
            {
                ChromeOptions options = new();
#pragma warning disable CA1859 // Use concrete types when possible for improved performance
                IWebDriver driver = new ChromeDriver(options);
#pragma warning restore CA1859 // Use concrete types when possible for improved performance
                driver.Manage().Window.Maximize();
                return driver;
            });
        }

        public static void QuitDriver()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            if (driverInstances.TryRemove(threadId, out var driver))
            {
                driver.Quit();
                driver.Dispose();
            }
        }
    }
}
