//-----------------------------------------------------------------------
// <copyright file="SeleniumService.cs" company="Personage Enterprises">
// * Copyright (C) 2018 qinchaoyue 版权所有。
// * version : 1.0
// * author  : qinchaoyue
// * FileName: SeleniumService.cs
// * history : created by qinchaoyue 2018-05-28 04:48:12
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using HotPot.Selenium.Contract;
using HotPot.Selenium.Entity;
using HotPot.Selenium.Extension;
using HotPot.Utility.Extension;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace HotPot.Selenium
{
    public class SeleniumService : IDisposable, ISeleniumService
    {
        public Queue<SeleniumEntity> SeleniumEntities;

        // public PhantomJSDriver DriverService;

        public RemoteWebDriver DriverService;

        public void Quit()
        {
            this.DriverService.Quit();
            this.DriverService.Dispose();
        }

        public SeleniumService(string proxyIp = "", string proxyPort = "", string driverPath = "")
        {
            SeleniumEntities = new Queue<SeleniumEntity>();
            GetDriver(proxyIp, proxyPort, driverPath);
        }

        public void GetDriver(string proxyIp, string proxyPort, string driverPath)
        {
            //            var phantomJsDriverService = PhantomJSDriverService.CreateDefaultService();
            //            phantomJsDriverService.IgnoreSslErrors = true;
            //            phantomJsDriverService.LoadImages = true;
            //            phantomJsDriverService.ProxyType = "none";
            //#if !DEBUG
            //            phantomJsDriverService.HideCommandPromptWindow = true;
            //#endif
            //DriverService = new PhantomJSDriver(phantomJsDriverService);
            var chromeDriverService = ChromeDriverService.CreateDefaultService(driverPath);
            var options = new ChromeOptions();
            options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
            var argument = $"disable-infobars";
            if (!proxyIp.IsNullOrEmpty() && !proxyPort.IsNullOrEmpty())
            {
                argument = $"disable-infobars --proxy-server=socks5://{proxyIp}:{proxyPort}";
            }

            options.AddArgument(argument);
#if !DEBUG

            options.AddArgument("headless");
            chromeDriverService.HideCommandPromptWindow = true;
#endif
            this.DriverService = new ChromeDriver(chromeDriverService, options);
            DriverService.Manage().Window.Size = new Size(1920, 1080);
        }

        public ISeleniumService GoToUrl(string url)
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.GoUrl,
                Url = url,
            });
            return this;
        }

        public ISeleniumService Sleep(int millisecondsTimeout)
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.Sleep,
                MillisecondsTimeout = millisecondsTimeout
            });
            return this;
        }

        public ISeleniumService FindElement(By by)
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.Find,
                By = by
            });
            return this;
        }

        public ISeleniumService FindElementByLinkText(string linkText, int millsecondsTimeout = 0)
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.Find,
                By = By.LinkText(linkText),
                MillisecondsTimeout = millsecondsTimeout
            });
            return this;
        }

        public ISeleniumService Click(By by = null, string debugKey = "")
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.Click,
                By = by == null ? null : by,
                DebugKey = debugKey
            });
            return this;
        }

        public ISeleniumService Clear()
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.Clear,
            });
            return this;
        }

        public ISeleniumService SendKeys(string keys)
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.SendKeys,
                Keys = keys
            });
            return this;
        }

        public int ToScrollY()
        {
            IJavaScriptExecutor js = this.DriverService;
            var executeScript = js.ExecuteScript($"return window.scrollY;");
            return int.Parse(executeScript.ToString());
        }

        public void DoTask()
        {
            this.DoTask<string>();
        }

        public T DoTask<T>(Func<IWebElement, ReadOnlyCollection<IWebElement>, T> func = null)
        {
            try
            {
                IWebElement findElement = null;
                ReadOnlyCollection<IWebElement> findElements = null;
                byte[] screenByteArray = new byte[] { };
                while (SeleniumEntities.Count > 0)
                {
                    //foreach (var seleniumEntity in SeleniumEntities)
                    //for (int i = 0; i < SeleniumEntities.Count; i++)
                    var seleniumEntity = SeleniumEntities.Dequeue();
                    switch (seleniumEntity.ActionType)
                    {
                        case ActionType.GoUrl:
                            this.DriverService.Navigate().GoToUrl(seleniumEntity.Url);
                            // this.DriverService.PageSource.WriteLine();
                            break;
                        case ActionType.Click:
                            if (seleniumEntity.By == null)
                                findElement?.Click();
                            else
                                this.DriverService.FindElement(seleniumEntity.By).Click();
                            break;
                        case ActionType.Find:
                            if (seleniumEntity.IsChild)
                                findElement = findElement.FindElement(this.DriverService, seleniumEntity.By, seleniumEntity.MillisecondsTimeout);
                            else
                                findElement = this.DriverService.FindElement(seleniumEntity.By, seleniumEntity.MillisecondsTimeout);
                            break;
                        case ActionType.SendKeys:
                            if (findElement != null) findElement.SendKeys(seleniumEntity.Keys);
                            break;
                        case ActionType.Screenshot:
                            screenByteArray = this.DriverService.GetScreenshot().AsByteArray;
                            if (findElement != null && seleniumEntity.IsChild)
                                screenByteArray = screenByteArray.CropImage(new Rectangle(findElement.Location, findElement.Size))?.ImageToByte();

                            if (!seleniumEntity.Path.IsNullOrEmpty())
                                if (screenByteArray != null)
                                    ((Image)new ImageConverter().ConvertFrom(screenByteArray))?.Save(seleniumEntity.Path);
                            break;
                        case ActionType.Sleep:
                            Thread.Sleep(seleniumEntity.MillisecondsTimeout);
                            break;
                        case ActionType.Lambda:
                            var funcCompile = seleniumEntity.Expression.Compile();
                            findElement = findElements.First(funcCompile);
                            break;
                        case ActionType.Finds:
                            if (seleniumEntity.IsChild && findElement != null)
                                findElements = findElement.FindElements(this.DriverService, seleniumEntity.By,
                                    seleniumEntity.MillisecondsTimeout);
                            else
                                findElements = this.DriverService.FindElements(seleniumEntity.By,
                                    seleniumEntity.MillisecondsTimeout);
                            break;
                        case ActionType.RemoveReadonly:
                            DoRemoveReadonly(findElement);
                            break;
                        case ActionType.Scroll:
                            DoScrollTo(seleniumEntity.Height);
                            break;
                        case ActionType.Clear:
                            findElement.Clear();
                            break;
                        case ActionType.SetAttribute:
                            DoSetValue(findElement, seleniumEntity.Value);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (func != null)
                {
                    return func(findElement, findElements);
                }

                if (typeof(T).Name.ToUpper() == "POINT" && findElement != null)
                {
                    object value = findElement.Location;
                    return (T)Convert.ChangeType(value, typeof(T));
                }

                if (typeof(T).Name.ToUpper() == "BYTE[]")
                {
                    return (T)Convert.ChangeType(screenByteArray, typeof(T));
                }


                if (typeof(T).Name.IndexOf("List", StringComparison.Ordinal) != -1)
                // if (typeof(T).Name.ToUpper() == "IWEBELEMENT" && findElement != null)
                {
                    var value = new List<IWebElement>
                    {
                        findElement
                    };
                    return (T)Convert.ChangeType(value, typeof(T));
                }

                if (typeof(T).Name.IndexOf("ReadOnlyCollection", StringComparison.Ordinal) != -1)
                {
                    return (T)Convert.ChangeType(findElements, typeof(T));
                }

                if (typeof(T).Name.ToUpper() == "STRING")
                {
                    return (T)Convert.ChangeType(this.DriverService.PageSource, typeof(T));
                }

                return default(T);
            }
            catch (Exception e)
            {
                this.SeleniumEntities.Clear();
                return default(T);
            }
        }

        private void DoSetValue(IWebElement findElement, string value)
        {
            IJavaScriptExecutor js = this.DriverService;
            js.ExecuteScript($"arguments[0].value=arguments[1]", findElement, value);
        }

        public ISeleniumService ScrollToBottom()
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.Scroll,
                Height = 9999
            });
            return this;
        }

        private void DoScrollTo(int height)
        {
            IJavaScriptExecutor js = this.DriverService;
            js.ExecuteScript($"window.scrollTo(0, {height});");
        }

        public ISeleniumService ScrollTo(int height)
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.Scroll,
                Height = height
            });
            return this;
        }

        private void DoRemoveReadonly(IWebElement element)
        {
            var elementId = element.GetAttribute("id");
            String jsString = $"document.getElementById('{elementId}').removeAttribute('readonly')";

            IJavaScriptExecutor js = this.DriverService;
            js.ExecuteScript(jsString);
        }

        public ISeleniumService RemoveReadonly()
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.RemoveReadonly
            });
            return this;
        }

        public ISeleniumService SetValue(string value)
        {
            this.SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.SetAttribute,
                Value = value
            });
            return this;
        }

        public Point GetPoint()
        {
            return this.DoTask<Point>();
        }

        public void Dispose()
        {
            this.DriverService.Quit();
            this.DriverService.Close();
            this.DriverService.Dispose();
        }

        public ISeleniumService FindElementById(string id, int millisecondsTimeout = 0, string debugKey = "")
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.Find,
                By = By.Id(id),
                MillisecondsTimeout = millisecondsTimeout,
                DebugKey = debugKey
            });
            return this;
        }

        public ISeleniumService FindElementByClassName(string className, int millisecondsTimeout = 0)
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.Find,
                By = By.ClassName(className),
                MillisecondsTimeout = millisecondsTimeout
            });
            return this;
        }

        public ISeleniumService FindChildElementByTagName(string tagName)
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.Find,
                By = By.TagName(tagName),
                IsChild = true,
            });
            return this;
        }

        public byte[] GetScreenshot(bool isChild)
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.Screenshot,
                IsChild = isChild
            });
            return this.DoTask<byte[]>();
        }

        public ISeleniumService SaveScreenshot(string path = "", bool isChild = true)
        {
            if (path.IsNullOrEmpty())
            {
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img");
                if (!System.IO.Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                path = Path.Combine(path, DateTime.Now.ToString("yyyyMMddhhmmssss") + ".jpg");
            }
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.Screenshot,
                Path = path,
                IsChild = isChild
            });

            return this;
        }

        //public NormalResult SaveScreenshot(string path)
        //{
        //    SeleniumEntities.Enqueue(new SeleniumEntity
        //    {
        //        ActionType = ActionType.Screenshot
        //    });
        //    var normalResult = this.DoTask<byte[]>();

        //    ((Image)new ImageConverter().ConvertFrom(normalResult))?.Save(path);
        //    return new NormalResult("保存成功.");
        //}

        public IReadOnlyCollection<IWebElement> ToElements()
        {
            return DoTask<ReadOnlyCollection<IWebElement>>();
        }

        public ISeleniumService FindElementByName(string name, int millisecondsTimeout = 0)
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.Find,
                By = By.Name(name),
                MillisecondsTimeout = millisecondsTimeout
            });
            return this;
        }

        public ISeleniumService FindElementsByName(string name, int millisecondsTimeout = 0)
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.Finds,
                By = By.Name(name),
                MillisecondsTimeout = millisecondsTimeout
            });
            return this;
        }

        public string GetPageSource()
        {
            return DoTask<string>();
        }

        public ISeleniumService FindElementsByClassName(string className, int millisecondsTimeout = 0)
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.Finds,
                By = By.ClassName(className),
                MillisecondsTimeout = millisecondsTimeout
            });
            return this;
        }

        public ISeleniumService First(Expression<Func<IWebElement, bool>> expression)
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.Lambda,
                Expression = expression
            });
            return this;
        }

        public string TryAlertText()
        {
            try
            {
                return this.DriverService.SwitchTo().Alert().Text;
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public void TryAlertAccept()
        {
            try
            {
                this.DriverService.SwitchTo().Alert().Accept();
            }
            catch (Exception e)
            {

            }
        }

        public IWebElement ToElement()
        {
            return DoTask<List<IWebElement>>()?.FirstOrDefault();
        }

        public string ToText()
        {
            return this.DoTask<string>((element, elements) => element != null ? element.Text : string.Empty);
        }
    }
}
