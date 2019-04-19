using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hotPot.Selenium.Net45;
using HotPot.HttpClient.Net45;
using HotPot.HttpClient.Net45.Contract;
using HotPot.Utility;
using OpenQA.Selenium;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            new SeleniumService()
                .GoToUrl("http://172.17.1.246:4330")
                .FindElementById("username").SetValue("1")
                .FindElementById("password").SetValue("123456");

            Console.ReadLine();
        }
    }
}
