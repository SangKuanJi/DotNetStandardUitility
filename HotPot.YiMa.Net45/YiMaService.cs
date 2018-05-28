using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotPot.HttpClient.Net45.Contract;
using HotPot.Utility.Net45.Extension;
using HotPot.Utility.Net45.Helper;
using HotPot.YiMa.Net45.Contract;
using HotPot.YiMa.Net45.Entity;

namespace HotPot.YiMa.Net45
{
    public class YiMaService : IYiMaService
    {
        public string ServiceUrl = "http://api.fxhyd.cn/";
        public IHttpClient HttpClient = new HttpClient.Net45.HttpClient();
        public string Token { get; set; }
        private const string Success = "success";

        /// <summary>
        /// 获取手机号码
        /// 登录过期会自动登录
        /// </summary>
        /// <param name="itemId">项目编码</param>
        /// <param name="isp">手机运营商. 1:移动, 2:联通, 3:电信</param>
        /// <param name="province">省代码</param>
        /// <param name="city">市代码</param>
        /// <param name="mobile">获取指定的手机号码</param>
        /// <param name="excludeno">排除指定的手机号码</param>
        /// <returns></returns>
        public IYiMaService GetMobile(string itemId, int isp = 0, int province = 0, int city = 0, string mobile = "", string excludeno = "")
        {
            //UserInterface.aspx?action=getmobile&token=TOKEN&itemid=项目编号&excludeno=排除号段
            if (this.Token.IsNullOrEmpty())
            {
                if (this.UserInfo == null)
                {
                    return this;
                }

                this.Login(this.UserInfo.UserName, this.UserInfo.Password);
            }
            try
            {
                var url = $"{ServiceUrl}UserInterface.aspx";
                var param = new {action = "getmobile", this.Token, itemid = itemId}.ToDictionary();
                if (isp != 0)
                {
                    param.Add("isp", isp);
                }

                if (province != 0)
                {
                    param.Add("province", province);
                }

                if (city != 0)
                {
                    param.Add("city", city);
                }

                if (!mobile.IsNullOrEmpty())
                {
                    param.Add("mobile", mobile);
                }

                if (!excludeno.IsNullOrEmpty())
                {
                    param.Add("excludeno", excludeno);
                }

                var mobileResult = HttpClient.GetString(url, param);
                if (mobileResult.IndexOf(Success) != -1)
                {
                    this.Mobile = mobileResult.Split('|')[1];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return this;
        }

        public string Mobile { get; set; }

        /// <summary>
        /// 获取短信
        /// </summary>
        /// <param name="itemId">项目id</param>
        /// <param name="mobile">手机号码</param>
        /// <param name="release">1: 获取到短信后自动释放手机号码</param>
        /// <returns></returns>
        public IYiMaService GetMessage(string itemId, string mobile = "", int release = 0)
        {
            if (mobile.IsNullOrEmpty())
            {
                mobile = this.Mobile;
            }
            try
            {
                //UserInterface.aspx?action=getsms&token=TOKEN&itemid=项目编号&mobile=手机号码&release=1
                var url = $"{ServiceUrl}UserInterface.aspx";
                var param = new {action = "getsms",mobile, this.Token, itemId}.ToDictionary();
                if (release != 0)
                {
                    param.Add("release", release);
                }

                var smsOk = RetryHelper.Retry(
                () => {
                    var messageResult = HttpClient.GetString(url, param);
                    messageResult.WriteLine();
                    if (messageResult == "2007")
                    {
                        return true;
                    }
                    if (messageResult.IndexOf(Success) == -1) return false;
                    this.Message = messageResult.Split('|')[1];
                    return true;

                }, 99);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return this;
        }

        public string Message { get; set; }

        public IYiMaService GetUserInfo()
        {
            try
            {
                var url = $"{ServiceUrl}UserInterface.aspx";
                //UserInterface.aspx?action=getaccountinfo&token=TOKEN
                var userInfoStr = HttpClient.GetString(url, new {action = "getaccountinfo", this.Token, format = 1});
                if (userInfoStr.IndexOf("success") == -1)
                {
                    return this;
                }

                this.UserInfo = userInfoStr.Split('|')[1].ToString().FromJson<UserInfo>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return this;
        }

        public UserInfo UserInfo { get; set; }

        public IYiMaService Login(string account, string password)
        {
            try
            {
                var url = $"{ServiceUrl}UserInterface.aspx";
                var loginResult = HttpClient.GetString(url, new { action = "login", username = account, password });
                this.Token = loginResult.ToLower().IndexOf("success", StringComparison.Ordinal) == -1 ? string.Empty : loginResult.Split('|')[1];
                GetUserInfo();
                this.UserInfo.Password = password;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return this;
        }
    }
}
