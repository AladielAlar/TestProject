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
        public static IWebDriver GetDriver()
        {
#pragma warning disable CA1840 // Use 'Environment.CurrentManagedThreadId'
            int threadId = Thread.CurrentThread.ManagedThreadId;

            return driverInstances.GetOrAdd(threadId, _ =>
            {
                ChromeOptions options = new();
                var driver = new ChromeDriver(options);
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
