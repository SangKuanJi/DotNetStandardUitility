using Microsoft.VisualStudio.TestTools.UnitTesting;
using HotPot.YiMa.Net45;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotPot.YiMa.Net45.Contract;

namespace HotPot.YiMa.Net45.Tests
{
    [TestClass()]
    public class YiMaServiceTests
    {
        /// <summary>
        /// 易码平台服务
        /// </summary>
        public IYiMaService YiMaService = new YiMaService();

        /// <summary>
        /// 易码平台账号和密码
        /// </summary>
        public string[] YimaInfo = System.IO.File.ReadAllText(@"d:\test\yima.txt").Split('|');

        /// <summary>
        /// 浙江长龙航空项目编码
        /// </summary>
        public int ProjectCode = 6061;

        [TestMethod()]
        public void LoginTest()
        {
            var yimaInfo = System.IO.File.ReadAllText(@"d:\test\yima.txt").Split('|');
            try
            {
                var login = YiMaService.Login(yimaInfo[0], yimaInfo[1]);
                Assert.IsNotNull(login.Token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [TestMethod()]
        public void UserInfoTest()
        {
            try
            {
                var userInfo = YiMaService
                    .Login(YimaInfo[0], YimaInfo[1])
                    .GetUserInfo()
                    .UserInfo;
                Assert.IsNotNull(userInfo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.Fail();
        }

        [TestMethod()]
        public void GetMessageTest()
        {
            try
            {
                var mobile = YiMaService.Login(YimaInfo[0], YimaInfo[1])
                    .GetMobile(ProjectCode.ToString())
                    .Mobile;

                var message = YiMaService
                    .GetMessage(ProjectCode.ToString(), release:1)
                    .Message;
                Assert.IsNotNull(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}