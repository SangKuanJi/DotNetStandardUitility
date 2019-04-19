using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using HotPot.Selenium.Entity;
using HotPot.Selenium.Extension;
using HotPot.Utility.Extension;
using OpenQA.Selenium;

namespace HotPot.Selenium.Contract
{
    public interface ISeleniumService
    {
        ISeleniumService GoToUrl(string url);

        ISeleniumService Sleep(int millisecondsTimeout);

        ISeleniumService FindElement(By by);

        ISeleniumService FindElementByLinkText(string linkText, int millsecondsTimeout = 0);

        ISeleniumService Click(By by = null, string debugKey = "");

        ISeleniumService Clear();

        ISeleniumService SendKeys(string keys);

        int ToScrollY();

        void DoTask();

        T DoTask<T>(Func<IWebElement, ReadOnlyCollection<IWebElement>, T> func = null);

        ISeleniumService ScrollToBottom();

        ISeleniumService ScrollTo(int height);

        ISeleniumService RemoveReadonly();

        ISeleniumService SetValue(string value);

        Point GetPoint();

        ISeleniumService FindElementById(string id, int millisecondsTimeout = 0, string debugKey = "");

        ISeleniumService FindElementByClassName(string className, int millisecondsTimeout = 0);

        ISeleniumService FindChildElementByTagName(string tagName);

        byte[] GetScreenshot(bool isChild);

        ISeleniumService SaveScreenshot(string path = "", bool isChild = true);

        IReadOnlyCollection<IWebElement> ToElements();

        ISeleniumService FindElementByName(string name, int millisecondsTimeout = 0);

        ISeleniumService FindElementsByName(string name, int millisecondsTimeout = 0);

        string GetPageSource();

        ISeleniumService FindElementsByClassName(string className, int millisecondsTimeout = 0);

        ISeleniumService First(Expression<Func<IWebElement, bool>> expression);

        string TryAlertText();

        void TryAlertAccept();

        IWebElement ToElement();

        string ToText();
    }
}
