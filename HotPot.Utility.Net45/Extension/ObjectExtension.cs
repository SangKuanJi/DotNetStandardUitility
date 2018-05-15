//-----------------------------------------------------------------------
// <copyright file="ObjectExtension.cs" company="Personage Enterprises">
// * Copyright (C) 2018 qinchaoyue 版权所有。
// * version : 1.0
// * author  : qinchaoyue
// * FileName: ObjectExtension.cs
// * history : created by qinchaoyue 2018-05-15 10:02:03
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace HotPot.Utility.Net45.Extension
{
    public static class ObjectExtension
    {
        /// <summary>
        /// 转换对象为json字符串
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string ToJson(this object result, JsonSerializerSettings settings = null)
        {
            return result == null ? default(string) : JsonConvert.SerializeObject(result, settings);
        }

        /// <summary>
        /// 转换 json 字符串为 T 对象
        /// </summary>
        /// <param name="json"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T FromJson<T>(this string json)
        {
            return json.IsNullOrEmpty() ? default(T) : JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// obj 转换成 IDictionary
        /// </summary>
        /// <param name="source"> The source.  </param>
        /// <typeparam name="T"> 返回类型 </typeparam>
        /// <returns> The <see cref="IDictionary"/>.  </returns>
        public static IDictionary<string, T> ToDictionary<T>(this object source)
        {
            if (source == null)
            {
                ThrowExceptionWhenSourceArgumentIsNull();
            }

            var dictionary = new Dictionary<string, T>();
            if (source is Dictionary<string, object>)
            {
                foreach (var property in (Dictionary<string, object>)source)
                {
                    dictionary.Add(property.Key, (T)property.Value);
                }
            }
            else
            {
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
                {
                    AddPropertyToDictionary<T>(property, source, dictionary);
                }
            }
            return dictionary;
        }

        /// <summary>
        /// obj 转换成 dictionary
        /// </summary>
        /// <param name="source"> The source.  </param>
        /// <returns> The <see cref="IDictionary"/>.  </returns>
        public static IDictionary<string, object> ToDictionary(this object source)
        {
            return source.ToDictionary<object>();
        }

        private static void AddPropertyToDictionary<T>(PropertyDescriptor property, object source, Dictionary<string, T> dictionary)
        {
            object value = property.GetValue(source);
            if (IsOfType<T>(value))
                dictionary.Add(property.Name, (T)value);
        }
        private static bool IsOfType<T>(object value)
        {
            return value is T;
        }
        private static void ThrowExceptionWhenSourceArgumentIsNull()
        {
            throw new ArgumentNullException("source", "Unable to convert object to a dictionary. The source object is null.");
        }
    }
}
