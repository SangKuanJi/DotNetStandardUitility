﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hotPot.Selenium.Net45;
using HotPot.Utility;
using OpenQA.Selenium;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var seleniumService = new SeleniumService();
            var normalResult = seleniumService
                .GoToUrl("http://www.shanglv51.com/")
                .FindElement(By.Id("slide-page"))
                .DoTask<Point>();
            Console.WriteLine($"X: {normalResult.Data.X}  Y: {normalResult.Data.Y}");

            normalResult = seleniumService
                .GoToUrl("https://www.airkunming.com/")
                .FindElement(By.Id("orgCityLabel"))
                .DoTask<Point>();

            Console.WriteLine($"X: {normalResult.Data.X}  Y: {normalResult.Data.Y}");
            Console.ReadLine();
        }
    }
}
