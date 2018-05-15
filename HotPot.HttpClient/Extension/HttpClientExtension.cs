//-----------------------------------------------------------------------
// <copyright file="HttpClientExtension.cs" company="Personage Enterprises">
// * Copyright (C) 2018 qinchaoyue 版权所有。
// * version : 1.0
// * author  : qinchaoyue
// * FileName: HttpClientExtension.cs
// * history : created by qinchaoyue 2018-05-15 03:22:20
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotPot.HttpClient.Contract;
using HotPot.Utility.Net45;

namespace HotPot.HttpClient.Extension
{
    public static class HttpClientExtension
    {
        /// <summary>
        /// 发起 Get 请求并返回 T 类型请求结果
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="url"> 请求地址. </param>
        /// <param name="param"> 入参 </param>
        /// <param name="headers"></param>
        /// <returns> string 类型结果. </returns>
        public static T Get<T>(this IHttpClient httpClient, string url, object param = null,
            Dictionary<string, IEnumerable<string>> headers = null)
        {
            return Task.Run(() => httpClient.GetAsync<T>(url, param, headers)).Result;
        }

        public static async Task<T> GetAsync<T>(this IHttpClient httpClient, string url, object param = null,
            Dictionary<string, IEnumerable<string>> headers = null)
        {
            return (await httpClient.GetStringAsync(url, param, headers)).FromJson<T>();
        }

        /// <summary>
        /// 发起 Get 请求并返回 T 类型请求结果. 如果请求发生错误, 返回 outT 值
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="httpClient"></param>
        /// <param name="url">请求地址</param>
        /// <param name="outT">请求失败时返回默认值</param>
        /// <param name="param">请求入参</param>
        /// <param name="headers"></param>
        /// <returns>T</returns>
        public static T TryGet<T>(this IHttpClient httpClient, string url, T outT, object param = null,
        Dictionary<string, IEnumerable<string>> headers = null)
        {
            try
            {
                return httpClient.Get<T>(url, param, headers);
            }
            catch (Exception e)
            {
                return outT;
            }
        }

        public static async Task<T> TryGetAsync<T>(this IHttpClient httpClient, string url, T outT, object param = null,
            Dictionary<string, IEnumerable<string>> headers = null)
        {
            try
            {
                return await httpClient.GetAsync<T>(url, param, headers);
            }
            catch (Exception e)
            {
                return outT;
            }
        }


        public static string TryGetString(this IHttpClient httpClient, string url, string outStr, object param = null, Dictionary<string, IEnumerable<string>> headers = null)
        {
            try
            {
                return httpClient.GetString(url, param, headers);
            }
            catch (Exception e)
            {
                return outStr;
            }
        }

        public static async Task<T> PostStringAsync<T>(this IHttpClient httpClient, string url, object formData = null, string charset = "UTF-8",
            string mediaType = "application/json", Dictionary<string, IEnumerable<string>> headers = null)
        {
            return (await httpClient.PostStringAsync(url, formData, charset, mediaType, headers)).FromJson<T>();
        }

        /// <summary>
        /// 异步发送 Post 请求并返回 T 类型结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpClient"></param>
        /// <param name="url"></param>
        /// <param name="formData"></param>
        /// <param name="charset"></param>
        /// <param name="mediaType"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<T> PostAsync<T>(this IHttpClient httpClient,
            string url,
            object formData = null,
            string charset = "UTF-8",
            string mediaType = "application/json",
            Dictionary<string, IEnumerable<string>> headers = null
        )
        {
            return (await httpClient.PostStringAsync(url, formData, charset, mediaType, headers)).FromJson<T>();
        }

        public static T Post<T>(this IHttpClient httpClient,
            string url,
            object formData = null,
            string charset = "UTF-8",
            string mediaType = "application/json",
            Dictionary<string, IEnumerable<string>> headers = null
        )
        {
            return Task.Run(() => httpClient.PostAsync<T>(url, formData, charset, mediaType, headers))
                .Result;
        }

        public static T TryPost<T>(this IHttpClient httpClient, string url, T outT, object formData = null, string charset = "UTF-8",
            string mediaType = "application/json", Dictionary<string, IEnumerable<string>> headers = null)
        {
            try
            {
                return httpClient.Post<T>(url, formData, charset, mediaType, headers);
            }
            catch (Exception e)
            {
                return outT;
            }
        }

        public static T Put<T>(this IHttpClient httpClient, string url, object formData = null, string charset = "UTF-8", string mediaType = "application/json")
        {
            return httpClient.PutString(url, formData, charset, mediaType).FromJson<T>();
        }

        public static string TryPutString(this IHttpClient httpClient, string url, string outStr, object formData = null, string charset = "UTF-8",
            string mediaType = "application/json")
        {
            try
            {
                return httpClient.PutString(url, formData, charset, mediaType);
            }
            catch (Exception e)
            {
                return outStr;
            }
        }

    }
}
