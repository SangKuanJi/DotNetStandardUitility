using System;
using System.Threading;

namespace HotPot.Utility.Net45.Helper
{
    /// <summary>
    /// 重试操作辅助类
    /// </summary>
    public static class RetryHelper
    {
        /// <summary>
        /// 重试
        /// </summary>
        /// <param name="func">重试方法</param>
        /// <param name="times">重试次数</param>
        /// <param name="millisecondsTimeout">重试休眠时间</param>
        /// <returns>是否执行成功</returns>
        public static bool Retry(Func<int, bool> func, int times, int millisecondsTimeout = 0)
        {
            if (times <= 1 && times >= 100)
            {
                throw new ArgumentOutOfRangeException("times 参数有误。只能是介于 1 与 100（包含1和100）之间的整数。");
            }

            var i = 1;
            while (times > 0)
            {
                if (millisecondsTimeout!=0)
                {
                    Thread.Sleep(millisecondsTimeout);
                }
                if (func(i)) return true;
                times--;
                i++;
            }
            return false;
        }
    }
}
