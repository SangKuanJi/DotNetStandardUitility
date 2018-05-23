using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using hotPot.Selenium.Net45.Entity;
using hotPot.Selenium.Net45.Extension;
using HotPot.Utility.Net45.Extension;
using HotPot.Utility.Net45.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;

namespace hotPot.Selenium.Net45
{
    public class SeleniumService : IDisposable
    {
        public Queue<SeleniumEntity> SeleniumEntities;
        public PhantomJSDriver DriverService;

        public void Quit()
        {
            this.DriverService.Quit();
            // this.DriverService.Close();
            this.DriverService.Dispose();
        }

        public SeleniumService()
        {
            SeleniumEntities = new Queue<SeleniumEntity>();
            GetDriver();
        }

        public void GetDriver()
        {
            var phantomJsDriverService = PhantomJSDriverService.CreateDefaultService();
            phantomJsDriverService.IgnoreSslErrors = true;
            phantomJsDriverService.LoadImages = true;
            phantomJsDriverService.ProxyType = "none";
#if !DEBUG
            phantomJsDriverService.HideCommandPromptWindow = true;
#endif
            DriverService = new PhantomJSDriver(phantomJsDriverService);
            DriverService.Manage().Window.Size = new Size(1920, 1080);
        }

        public SeleniumService GoToUrl(string url)
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.GoUrl,
                Url = url,
            });
            return this;
        }

        public SeleniumService Sleep(int millisecondsTimeout)
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.Sleep,
                MillisecondsTimeout = millisecondsTimeout
            });
            return this;
        }

        public SeleniumService FindElement(By by)
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.Find,
                By = by
            });
            return this;
        }

        public SeleniumService FindElementByLinkText(string linkText, int millsecondsTimeout = 0)
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.Find,
                By = By.LinkText(linkText),
                MillisecondsTimeout = millsecondsTimeout
            });
            return this;
        }

        public SeleniumService Click(By by = null)
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.Click,
                By = by == null ? null : by
            });
            return this;
        }

        public SeleniumService SendKeys(string keys)
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.SendKeys,
                Keys = keys
            });
            return this;
        }

        public void DoTask()
        {
            this.DoTask<string>();
        }

        public T DoTask<T>()
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
                            this.DriverService.PageSource.WriteLine();
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
                            var func = seleniumEntity.Expression.Compile();
                            findElement = findElements.First(func);
                            break;
                        case ActionType.Finds:
                            if (seleniumEntity.IsChild && findElement != null)
                                findElements = findElement.FindElements(this.DriverService, seleniumEntity.By,
                                    seleniumEntity.MillisecondsTimeout);
                            else
                                findElements = this.DriverService.FindElements(seleniumEntity.By,
                                    seleniumEntity.MillisecondsTimeout);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
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

                if (typeof(T).Name.ToUpper() == "IWEBELEMENT" && findElement != null)
                {
                    object value = findElement;
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
                return default(T);
            }
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

        public SeleniumService FindElementById(string id, int millisecondsTimeout = 0, string debugKey = "")
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

        public SeleniumService FindElementByClassName(string className, int millisecondsTimeout = 0)
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.Find,
                By = By.ClassName(className),
                MillisecondsTimeout = millisecondsTimeout
            });
            return this;
        }

        public SeleniumService FindChildElementByTagName(string tagName)
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

        public SeleniumService SaveScreenshot(string path, bool isChild = true)
        {
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

        public ReadOnlyCollection<IWebElement> ToElements()
        {
            return DoTask<ReadOnlyCollection<IWebElement>>();
        }

        public SeleniumService FindElementByName(string name, int millisecondsTimeout = 0)
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.Find,
                By = By.Name(name),
                MillisecondsTimeout = millisecondsTimeout
            });
            return this;
        }

        public SeleniumService FindElementsByName(string name, int millisecondsTimeout = 0)
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

        public SeleniumService FindElementsByClassName(string className, int millisecondsTimeout = 0)
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.Finds,
                By = By.ClassName(className),
                MillisecondsTimeout = millisecondsTimeout
            });
            return this;
        }

        public SeleniumService First(Expression<Func<IWebElement, bool>> expression)
        {
            SeleniumEntities.Enqueue(new SeleniumEntity
            {
                ActionType = ActionType.Lambda,
                Expression = expression
            });
            return this;
        }

        public IWebElement ToElement()
        {
            return DoTask<ReadOnlyCollection<IWebElement>>()?.FirstOrDefault();
        }
    }
}
