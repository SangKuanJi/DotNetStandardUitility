﻿//-----------------------------------------------------------------------
// <copyright file="HttpClient.cs" company="Personage Enterprises">
// * Copyright (C) 2018 qinchaoyue 版权所有。
// * version : 1.0
// * author  : qinchaoyue
// * FileName: HttpClient.cs
// * history : created by qinchaoyue 2018-05-15 03:40:24
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Extreme.Net;
using HotPot.HttpClient.Net45.Contract;
using HotPot.Utility.Net45.ConfigurManager;
using HotPot.Utility.Net45.Extension;

namespace HotPot.HttpClient.Net45
{
    public class HttpClient : IHttpClient
    {
        public IList<Cookie> Cookies { get; set; } = new List<Cookie>();
        public string MediaType { get; set; } = string.Empty;
        public string AcceptMediaType { get; set; } = string.Empty;

        /// <summary>
        /// http 请求工具类
        /// </summary>
        public HttpClient() { }

        /// <summary>
        /// 请求Put并返回string类型结果
        /// </summary>
        /// <param name="url"> 请求地址 </param>
        /// <param name="formData">请求数据. </param>
        /// <param name="charset">请求编码</param>
        /// <param name="mediaType">请求类型</param>
        /// <returns>string类型结果</returns>
        public string PutString(string url, object formData = null, string charset = "UTF-8", string mediaType = "application/json")
        {
            return Task.Run(async () => await this.PutStringAsync(url, formData, charset, mediaType)).Result;
        }

        /// <summary>
        /// 异步请求Put并返回string类型结果
        /// </summary>
        /// <param name="url"> 请求地址 </param>
        /// <param name="formData">请求数据. </param>
        /// <param name="charset">请求编码</param>
        /// <param name="mediaType">请求类型</param>
        /// <returns>string类型结果</returns>
        public async Task<string> PutStringAsync(string url, object formData = null, string charset = "UTF-8", string mediaType = "application/json")
        {
            return await HttpRequest(new Uri(url), method: HttpMethod.Put, formData: formData, charset: charset, mediaType: mediaType);
        }

        /// <summary>
        /// 发起 Get 请求并返回 string 类型请求结果
        /// </summary>
        /// <param name="url"> 请求地址. </param>
        /// <param name="param"> 入参 </param>
        /// <returns> string 类型结果. </returns>
        public string GetString(string url, object param = null,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> acceptMediaTypes = null
            )
        {
            return Task.Run(async () => await this.GetStringAsync(url, param, headers, acceptMediaTypes)).Result;
        }

        /// <summary>
        /// 发起 Get 请求并返回 string 类型请求结果
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="param"> 入参. </param>
        /// <returns>请求结果</returns>
        private string GetString(string url, Dictionary<string, object> param = null, Dictionary<string, string> headers = null)
        {
            return Task.Run(async () => await this.GetStringAsync(url, param, headers)).Result;
        }

        public async Task<string> GetStringAsync(string url, object param = null, Dictionary<string, string> headers = null,
            Dictionary<string, string> acceptMediaTypes = null)
        {
            if (param != null)
            {
                var dictionary = (Dictionary<string, object>)param.ToDictionary();
                if (dictionary.Count != 0)
                {
                    url = $"{url}?{string.Join("&", dictionary.Select(m => m.Key + "=" + m.Value).ToList())}";
                }
            }

            // return await this.GetStringAsync(url, new Dictionary<string, object>(), headers);
            url = this.GetUrl(url);
            try
            {
                return await HttpRequest(new Uri(url), headers, false, HttpMethod.Get);
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException($"E00001: 请求地址: {url}", e);
            }
            catch (Exception e)
            {
                throw new Exception($"请求地址: {url}", e);
            }

        }

        /// <summary>
        /// 异步请求post
        /// </summary>
        /// <param name="url">网络的地址("/api/values")</param>
        /// <param name="formData"> 请求入参 </param>
        /// <param name="charset">编码格式</param>
        /// <param name="mediaType">头媒体类型</param>
        /// <param name="headers">头部信息</param>
        /// <param name="acceptMediaType">accept媒体信息</param>
        /// <returns>返回结果</returns>
        public async Task<string> PostStringAsync(
            string url,
            object formData = null,
            string charset = "UTF-8",
            string mediaType = "application/json",
           Dictionary<string, string> headers = null,
            string acceptMediaType = "*/*",
            Dictionary<string, string> acceptMediaTypes = null
            )
        {
            url = GetUrl(url);
            return await HttpRequest(new Uri(url), headers, false, HttpMethod.Post, formData,
                acceptMediaType,
                mediaType,
                charset,
                acceptMediaTypes);
        }

        /// <summary>
        /// 同步请求post
        /// </summary>
        /// <param name="url">网络的地址("/api/values")</param>
        /// <param name="formData"> 请求入参 </param>
        /// <param name="charset">编码格式</param>
        /// <param name="mediaType">头媒体类型</param>
        /// <param name="headers">头部信息</param>
        /// <param name="acceptMediaType">accept媒体信息</param>
        /// <param name="acceptMediaTypes">accept媒体信息集合</param>
        /// <returns>返回结果</returns>
        public string PostString(
            string url,
            object formData = null,
            string charset = "UTF-8",
            string mediaType = "application/json",
        Dictionary<string, string> headers = null,
            string acceptMediaType = "*/*",
            Dictionary<string, string> acceptMediaTypes = null
            )
        {
            return Task.Run(async () => await this.PostStringAsync(url, formData, charset, mediaType, headers, acceptMediaType, acceptMediaTypes)).Result;
        }

        public string TryPostString(string url,
            string outStr,
            object formData = null,
            string charset = "UTF-8",
            string mediaType = "application/json",
            Dictionary<string, string> headers = null,
            string acceptMediaType = "*/*",
            Dictionary<string, string> acceptMediaTypes = null
            )
        {
            try
            {
                return this.PostString(url,
                    formData,
                    charset,
                    mediaType,
                    headers,
                    acceptMediaType,
                    acceptMediaTypes
                    );
            }
            catch (Exception e)
            {
                return outStr;
            }
        }

        /// <summary>
        /// 获取 请求 url
        /// </summary>
        /// <param name="url">url .  </param>
        /// <returns> 返回url . </returns>
        private string GetUrl(string url)
        {
            return FormatUrl(url);
        }

        /// <summary>
        /// 格式化 url 地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string FormatUrl(string url)
        {
            var configurHandler = new ConfigurHandler();
            foreach (Match urlKey in new Regex(@"(?<={)\w+(?=})").Matches(url))
            {
                var v = urlKey.Value;
                var serviceUrl = configurHandler.GetAppOptionValue(v, "");
                var replace = url.Replace("{" + v + "}", serviceUrl);
                url = replace;
            }

            return url;
        }

        public byte[] GetByte(string url, string proxyIp = "", int proxyPort = 0)
        {
            return HttpRequestByte(new Uri(url), proxyIp, proxyPort).Result;
        }

        public async Task<byte[]> HttpRequestByte(Uri baseAddress, string proxyIp = "", int proxyPort = 0)
        {
            try
            {
                // var webProxy = new WebProxy($"socks5://{proxyIp}:{proxyPort}");
                var socksProxy = new Socks5ProxyClient(proxyIp, proxyPort);
                // var webProxy = new ProxyHandler(socksProxy);
                if (proxyIp.IsNullOrEmpty())
                {
                    socksProxy = null;
                }

                using (var handler = new ProxyHandler(socksProxy))
                using (var client = new System.Net.Http.HttpClient(handler) { BaseAddress = baseAddress })
                {
                    var message = new HttpRequestMessage(HttpMethod.Get, baseAddress.PathAndQuery);
                    var cookieValue = GetCookie();
                    if (!cookieValue.IsNullOrEmpty())
                    {
                        message.Headers.Add("Cookie", cookieValue);
                    }
                    var result = client.SendAsync(message).Result;
                    if (result.Headers.TryGetValues("Set-Cookie", out var cookie))
                    {
                        cookie.ToList().ForEach(m => SetCookie(m));
                    }
                    result.EnsureSuccessStatusCode();
                    return await result.Content.ReadAsByteArrayAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
         
        }

        private async Task<string> HttpRequest(Uri baseAddress, Dictionary<string, string> heads = null, bool isGzip = false, HttpMethod method = null, object formData = null,
    string acceptMediaType = "*/*",
    string mediaType = "application/json",
        string charset = "UTF-8",
            Dictionary<string, string> acceptMediaTypes = null
    )
        {
            if (!MediaType.IsNullOrEmpty()) // 如果 MediaType 不为空, 说明用户设置了全局 MediaType
            {
                if (mediaType == "application/json") // 如果 MediaType 和默认值不同, 说明用户设置了 MediaType
                {
                    mediaType = MediaType;
                }
            }
            if (method == null)
            {
                method = HttpMethod.Get;
            }
            using (var handler = new HttpClientHandler { UseCookies = false })
            using (var client = new System.Net.Http.HttpClient(handler) { BaseAddress = baseAddress })
            {
                var message = new HttpRequestMessage(method, baseAddress.PathAndQuery);
                var encoding = Encoding.GetEncoding(charset);

                if (method == HttpMethod.Post)
                {
                    StringContent content;

                    if (formData == null)
                    {
                        content = new StringContent(string.Empty, encoding, mediaType);
                        message.Content = content;
                    }
                    else
                    {
                        if (formData is string)
                        {
                            content = new StringContent(formData as string, encoding, mediaType);
                        }
                        else
                        {
                            if (mediaType == "application/x-www-form-urlencoded")
                                content = new StringContent(string.Join("&", formData.ToDictionary().Select(m => m.Key + "=" + m.Value)), encoding, mediaType);
                            else
                                content = new StringContent(formData.ToJson(), encoding, mediaType);
                        }
                        message.Content = content;
                    }

                    // 
                    message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptMediaType));
                    if (acceptMediaTypes != null)
                    {
                        foreach (var mediaTypeItem in acceptMediaTypes)
                        {
                            if (!mediaTypeItem.Value.IsNullOrEmpty())
                            {
                                message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaTypeItem.Key, Convert.ToDouble(mediaTypeItem.Value)));
                            }
                            else
                            {
                                message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaTypeItem.Key));
                            }
                        }
                    }
                }
                if (heads != null)
                {
                    foreach (var head in heads)
                    {
                        message.Headers.Add(head.Key, head.Value);
                    }
                }

                message.Headers.Add("Cookie", GetCookie());
                var result = client.SendAsync(message).Result;
                result.EnsureSuccessStatusCode();
                if (result.Headers.TryGetValues("Set-Cookie", out var cookie))
                {
                    cookie.ToList().ForEach(m => SetCookie(m));
                }

                string httpContent;
                if (isGzip)
                {
                    httpContent = StringExtension.GZipDecompress(await result.Content.ReadAsByteArrayAsync());
                }
                else
                {
                    httpContent = await result.Content.ReadAsStringAsync();
                }

                return httpContent;
            }
        }

        private string GetCookie()
        {
            var cookieString = new StringBuilder();
            foreach (var cookie in this.Cookies)
            {
                if (cookie == null) continue;

                cookieString.Append(cookie.Name + "=" + cookie.Value + ";");
            }

            var a = cookieString.ToString().TrimEnd(';');
            return a;
        }

        private void SetCookie(string cookieString)
        {
            var cookie = new Cookie();
            var regexMatches = new Regex(@"(?<name>.*?)=(?<ip>.*?); path=(?<path>.*?); expires=(?<expires>.*)")
                .Matches(cookieString);
            foreach (Match regexMatch in regexMatches)
            {
                cookie.Name = regexMatch.Groups["name"].ToString();
                var ipMatch = Regex.Matches(regexMatch.Groups["ip"].ToString(), @"(?<ip>((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?))\.(?<timeStamp>\d*)");
                foreach (Match match in ipMatch)
                {
                    cookie.Domain = match.Groups["ip"].ToString();
                    var timeStamp = match.Groups["timeStamp"].ToString();
                    cookie.Expires = DateTimeExtensions.ConvertStringToDateTime(timeStamp).AddDays(30);
                }
                cookie.Path = regexMatch.Groups["path"].ToString();
                cookie.Value = regexMatch.Groups["ip"].ToString();
                cookie.Expired = true;
            }

            if (cookie.Name.IsNullOrEmpty())
            {
                regexMatches = new Regex(@"(?<name>.*?)=(?<value>.*?); [Pp]ath=(?<path>.*)")
                    .Matches(cookieString);
                foreach (Match regexMatch in regexMatches)
                {
                    cookie.Name = regexMatch.Groups["name"].ToString();
                    cookie.Value = regexMatch.Groups["value"].ToString();
                    cookie.Path = regexMatch.Groups["path"].ToString();
                }
            }

            var deleteCookie = this.Cookies.SingleOrDefault(m => m.Name == cookie.Name);
            if (deleteCookie != null)
            {
                this.Cookies.Remove(deleteCookie);
            }

            this.Cookies.Add(cookie);
        }

    }
}
