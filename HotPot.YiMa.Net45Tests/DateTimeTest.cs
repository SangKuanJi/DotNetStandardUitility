using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPot.YiMa.Net45.Tests
{
    [TestClass()]
    public class DateTimeTest
    {
        [TestMethod()]
        public void LoginTest()
        {
            var now = DateTime.Now;
            var today = DateTime.Today;
            var utcNow = DateTime.UtcNow;
            Console.WriteLine();
        }
    }
}
