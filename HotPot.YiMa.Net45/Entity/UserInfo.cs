namespace HotPot.YiMa.Net45.Entity
{
    /// <summary>
    /// 易码平台用户信息
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 账户等级
        /// </summary>
        public int UserLevel { get; set; }

        /// <summary>
        /// 获取号码最大数量
        /// </summary>
        public int MaxHold { get; set; }

        /// <summary>
        /// 账户折扣
        /// </summary>
        public double Discount { get; set; }

        /// <summary>
        /// 账户余额
        /// </summary>
        public double Balance { get; set; }

        /// <summary>
        /// 账户状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 冻结金额
        /// </summary>
        public double Frozen { get; set; }
    }
}