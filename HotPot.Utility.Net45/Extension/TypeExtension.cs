//-----------------------------------------------------------------------
// <copyright file="TypeExtension.cs" company="Personage Enterprises">
// * Copyright (C) 2018 qinchaoyue 版权所有。
// * version : 1.0
// * author  : qinchaoyue
// * FileName: TypeExtension.cs
// * history : created by qinchaoyue 2018-05-14 06:19:08
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace HotPot.Utility.Net45.Extension
{
    /// <summary>
    /// type 扩展类
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        /// 从类型成员获取指定Attribute特性
        /// </summary>
        /// <typeparam name="T">Attribute特性类型</typeparam>
        /// <param name="memberInfo">类型类型成员</param>
        /// <param name="inherit">是否从继承中查找</param>
        /// <returns>存在返回第一个，不存在返回null</returns>
        public static T GetAttribute<T>(this MemberInfo memberInfo, bool inherit = false) where T : Attribute
        {
            var descripts = memberInfo.GetCustomAttributes(typeof(T), inherit);
            return descripts.FirstOrDefault() as T;
        }

        /// <summary>
        /// 获取成员元数据的Description特性描述信息
        /// </summary>
        /// <param name="member">成员元数据对象</param>
        /// <param name="inherit">是否搜索成员的继承链以查找描述特性</param>
        /// <returns>返回Description特性描述信息，如不存在则返回成员的名称</returns>
        public static string ToDescription(this MemberInfo member, bool inherit = false)
        {
            var desc = member.GetAttribute<DescriptionAttribute>(inherit);
            if (desc != null)
            {
                return desc.Description;
            }
            var displayName = member.GetAttribute<DisplayNameAttribute>(inherit);
            return displayName != null ? displayName.DisplayName : member.Name;
        }

        /// <summary>
        /// 检查指定指定类型成员中是否存在指定的Attribute特性
        /// </summary>
        /// <typeparam name="T">要检查的Attribute特性类型</typeparam>
        /// <param name="memberInfo">要检查的类型成员</param>
        /// <param name="inherit">是否从继承中查找</param>
        /// <returns>是否存在</returns>
        public static bool HasAttribute<T>(this MemberInfo memberInfo, bool inherit = false) where T : Attribute
        {
            return memberInfo.IsDefined(typeof(T), inherit);
        }

        public static IList<string> GetProperties(this MemberInfo memberInfo)
        {
            return memberInfo.GetProperties().ToList();
        }
    }
}
