//-----------------------------------------------------------------------
// <copyright file="DateTimeExtensions.cs" company="Personage Enterprises">
// * Copyright (C) 2018 qinchaoyue 版权所有。
// * version : 1.0
// * author  : qinchaoyue
// * FileName: DateTimeExtensions.cs
// * history : created by qinchaoyue 2018-05-14 06:22:09
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq;

namespace HotPot.Utility
{
    /// <summary>
    /// 时间扩展类
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 当前时间是否周末
        /// </summary>
        /// <param name="dateTime">时间点</param>
        /// <returns></returns>
        public static bool IsWeekend(this DateTime dateTime)
        {
            DayOfWeek[] weeks = { DayOfWeek.Saturday, DayOfWeek.Sunday };
            return weeks.Contains(dateTime.DayOfWeek);
        }

        /// <summary>
        /// 当前时间是否工作日
        /// </summary>
        /// <param name="dateTime">时间点</param>
        /// <returns></returns>
        public static bool IsWeekday(this DateTime dateTime)
        {
            DayOfWeek[] weeks = { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
            return weeks.Contains(dateTime.DayOfWeek);
        }

        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time"> DateTime时间格式</param>
        /// <returns>Unix时间戳格式</returns>
        public static long ConvertDateTimeInt(this DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (long)(time - startTime).TotalMilliseconds;
        }

        /// <summary>
        /// 获取本周的开始时间和结束时间
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="begin">返回本周的开始时间</param>
        /// <param name="end">返回本周的结束时间</param>
        public static void GetWeekBeginAndEndTime(this DateTime time, out DateTime begin, out DateTime end)
        {
            var inWeekCount = time.GetTimeInWeekCount();
            if (inWeekCount == 0) inWeekCount = 7;
            begin = time.AddDays(-(inWeekCount - 1));
            end = time.AddDays((7 - inWeekCount));
        }

        /// <summary>
        /// 获取月份开始和结束时间
        /// </summary>
        /// <param name="time"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        public static void GetMonthBeginAndEndTime(this DateTime time, out DateTime begin, out DateTime end)
        {
            begin = time.GetMonthBegin();
            end = time.GetMonthEnd();
        }

        /// <summary>
        /// 获取今天是本周的第几天
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int GetTimeInWeekCount(this DateTime time)
        {
            return Convert.ToInt16(time.DayOfWeek);
        }

        /// <summary>
        /// 月份开始时间
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime GetMonthBegin(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        /// <summary>
        /// 月份结束时间
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime GetMonthEnd(this DateTime dateTime)
        {
            return new DateTime(dateTime.AddMonths(1).Year, dateTime.AddMonths(1).Month, 1).AddDays(-1);
        }
    }
}
