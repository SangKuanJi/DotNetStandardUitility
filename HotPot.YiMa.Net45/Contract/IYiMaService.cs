using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotPot.YiMa.Net45.Entity;

namespace HotPot.YiMa.Net45.Contract
{
    public interface IYiMaService
    {
        UserInfo UserInfo { get; set; }
        string Message { get; set; }

        string Token { get; set; }
        string Mobile { get; set; }

        /// <summary>
        /// 登录易码
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        IYiMaService Login(string account, string password);

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        IYiMaService GetUserInfo();

        IYiMaService GetMessage(string itemId, string mobile = "", int release = 0);

        IYiMaService GetMobile(string itemId, int isp = 0, int province = 0, int city = 0, string mobile = "",
            string excludeno = "");
    }
}
