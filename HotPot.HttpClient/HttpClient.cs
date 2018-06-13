using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HotPot.HttpClient.Contract;
using HotPot.HttpClient.Extension;
using HotPot.Utility.Net45;
using HotPot.Utility.Net45.ConfigurManager;
using HotPot.Utility.Net45.Extension;

namespace HotPot.HttpClient
{
    public class HttpClient : IHttpClient
    {
        /// <summary>
        /// http 请求客户端
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="NCHttpClient"/> class.
        /// </summary>
        public HttpClient()
        {
            this._httpClient = GetHttpClient();
        }

        private HttpClient GetHttpClient()
        {
            return new HttpClient();
        }

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
            var encoding = Encoding.GetEncoding(charset);
            var content = formData == null ? new StringContent(string.Empty, encoding) : new StringContent(formData.ToJson(), encoding);

            this.SetHeaders(content);
            content.Headers.ContentType = new MediaTypeHeaderValue(mediaType) { CharSet = charset };
            return await this.PutStringAsync(url, content);
        }

        /// <summary>
        /// 异步请求Put并返回string类型结果
        /// </summary>
        /// <param name="requestUri">请求地址</param>
        /// <param name="context">请求问</param>
        /// <returns>返回结果</returns>
        public async Task<string> PutStringAsync(string requestUri, HttpContent context)
        {
            var url = this.GetUrl(requestUri);
            try
            {
                var response = await this._httpClient.PostAsync(url, context);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
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
        /// 发起 Get 请求并返回 string 类型请求结果
        /// </summary>
        /// <param name="url"> 请求地址. </param>
        /// <param name="param"> 入参 </param>
        /// <returns> string 类型结果. </returns>
        public string GetString(string url, object param = null, Dictionary<string, IEnumerable<string>> headers = null)
        {
            return Task.Run(async () => await this.GetStringAsync(url, param, headers)).Result;
        }

        /// <summary>
        /// 发起 Get 请求并返回 string 类型请求结果
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="param"> 入参. </param>
        /// <returns>请求结果</returns>
        private string GetString(string url, Dictionary<string, object> param = null, Dictionary<string, IEnumerable<string>> headers = null)
        {
            return Task.Run(async () => await this.GetStringAsync(url, param, headers)).Result;
        }

        public async Task<string> GetStringAsync(string url, object param = null, Dictionary<string, IEnumerable<string>> headers = null)
        {
            if (param != null)
            {
                return await this.GetStringAsync(url, (Dictionary<string, object>)param.ToDictionary(), headers);
            }

            return await this.GetStringAsync(url, new Dictionary<string, object>(), headers);
        }

        private async Task<string> GetStringAsync(string url, Dictionary<string, object> param = null, Dictionary<string, IEnumerable<string>> headers = null)
        {
            if (param != null && param.Count != 0)
            {
                url = $"{url}?{string.Join("&", param.Select(m => m.Key + "=" + m.Value).ToList())}";
            }

            return await this.GetStringAsync(url, headers);
        }

        /// <summary>
        /// 异步 发起 Get 请求并返回 string 类型请求结果
        /// </summary>
        /// <param name="requestUri">请求地址 . </param>
        /// <returns>请求结果</returns>
        public async Task<string> GetStringAsync(string requestUri, Dictionary<string, IEnumerable<string>> headers)
        {
            var url = this.GetUrl(requestUri);
            try
            {
                var httpClient = GetHttpClient();
                this.SetHeaders(httpClient, headers);

                return await httpClient.GetStringAsync(url);
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
        /// 异步请求 post
        /// </summary>
        /// <param name="requestUri"> 请求地址 </param>
        /// <param name="content"> 请求内容 </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            var url = this.GetUrl(requestUri);
            try
            {
                this.SetHeaders(content);
                return await this._httpClient.PostAsync(url, content);
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
        /// <returns>返回结果</returns>
        public async Task<string> PostStringAsync(
            string url,
            object formData = null,
            string charset = "UTF-8",
            string mediaType = "application/json",
           Dictionary<string, IEnumerable<string>> headers = null
            )
        {
            var encoding = Encoding.GetEncoding(charset);
            StringContent content;
            if (formData == null)
            {
                content = new StringContent(string.Empty, encoding);
            }
            else
            {
                if (formData is string)
                {
                    content = new StringContent(formData as string, encoding);
                }
                else
                {
                    if (mediaType == "application/x-www-form-urlencoded")
                        content = new StringContent(string.Join("&", formData.ToDictionary().Select(m => m.Key + "=" + m.Value)), encoding);
                    else
                        content = new StringContent(formData.ToJson(), encoding);
                }
            }

            content.Headers.ContentType = new MediaTypeHeaderValue(mediaType) { CharSet = charset };
            if (headers != null)
            {

                foreach (var header in headers)
                {
                    content.Headers.Add(header.Key, header.Value);
                }
            }

            var resp = await this.PostAsync(url, content);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// 同步请求post
        /// </summary>
        /// <param name="url">网络的地址("/api/values")</param>
        /// <param name="formData"> 请求入参 </param>
        /// <param name="charset">编码格式</param>
        /// <param name="mediaType">头媒体类型</param>
        /// <returns>返回结果</returns>
        public string PostString(
            string url,
            object formData = null,
            string charset = "UTF-8",
            string mediaType = "application/json",
        Dictionary<string, IEnumerable<string>> headers = null)
        {
            return Task.Run(async () => await this.PostStringAsync(url, formData, charset, mediaType, headers)).Result;
        }

        public string TryPostString(string url, string outStr, object formData = null, string charset = "UTF-8",
            string mediaType = "application/json", Dictionary<string, IEnumerable<string>> headers = null)
        {
            try
            {
                return this.PostString(url, formData, charset, mediaType, headers);
            }
            catch (Exception e)
            {
                _logger.Warn(e.ToString(), $"请求 {url} 时出现异常, 返回默认值: {outStr}.");
                return outStr;
            }
        }

        /// <summary>
        /// The set headers.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        private void SetHeaders(HttpContent content)
        {
            try
            {
                if (!NCContext.Current.Token.IsNullOrEmpty())
                {
                    AuthenticationHeaderValue authValue = new AuthenticationHeaderValue("jwt", NCContext.Current.Token);
                    this._httpClient.DefaultRequestHeaders.Authorization = authValue;
                }

                var traceId = ContextManager.GetThisValue("TraceId")?.ToString();
                if (traceId.IsNullOrEmpty())
                {
                    ContextManager.SetThisValue("TraceId", Guid.NewGuid());
                }

                //if (!NCContext.Current.TryGetValue($"AutoDelete_{NCContext.Current.Operator.KeyId}_TraceId", out _))
                //{
                //    NCContext.Current[$"AutoDelete_{NCContext.Current.Operator.KeyId}_TraceId"] = Guid.NewGuid();
                //}

                // content.Headers.Add(TraceId, NCContext.Current[$"AutoDelete_{NCContext.Current.Operator.KeyId}_TraceId"].ToString());
                content.Headers.Add(TraceId, ContextManager.GetThisValue("TraceId")?.ToString());
            }
            catch (Exception e)
            {
                this._logger.Debug(e.ToString(), "this is a framecode bug");
                return;
            }
        }

        /// <summary>
        /// 设置头部信息
        /// </summary>
        private void SetHeaders(HttpClient httpClient, Dictionary<string, IEnumerable<string>> headers = null)
        {
            try
            {
                if (httpClient.DefaultRequestHeaders.TryGetValues(TraceId, out _))
                {
                    httpClient.DefaultRequestHeaders.Remove(TraceId);
                }

                var traceId = ContextManager.GetThisValue("TraceId")?.ToString();
                if (traceId.IsNullOrEmpty())
                {
                    ContextManager.SetThisValue("TraceId", Guid.NewGuid());
                }

                //if (!NCContext.Current.TryGetValue($"AutoDelete_{NCContext.Current.Operator.KeyId}_TraceId", out _))
                //{
                //    NCContext.Current[$"AutoDelete_{NCContext.Current.Operator.KeyId}_TraceId"] = Guid.NewGuid();
                //}

                // httpClient.DefaultRequestHeaders.Add(TraceId, NCContext.Current[$"AutoDelete_{NCContext.Current.Operator.KeyId}_TraceId"].ToString());
                httpClient.DefaultRequestHeaders.Add(TraceId, ContextManager.GetThisValue("TraceId")?.ToString());


                if (!NCContext.Current.Token.IsNullOrEmpty())
                {
                    AuthenticationHeaderValue authValue = new AuthenticationHeaderValue("jwt", NCContext.Current.Token);
                    this._httpClient.DefaultRequestHeaders.Authorization = authValue;
                }


                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Remove(header.Key);
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
            }
            catch (Exception e)
            {
                this._logger.Debug(e.ToString(), "this is a framecode bug");
                return;
            }
        }

        /// <summary>
        /// 获取 请求 url
        /// </summary>
        /// <param name="url">url .  </param>
        /// <returns> 返回url . </returns>
        private string GetUrl(string url)
        {
            var httpType = new ConfigurHandler().GetAppOptionValue("HttpType", @default: "http");
            if (httpType.ToLower() != "http" && httpType.ToLower() != "https")
            {
                httpType = "http";
            }

            if (url.IndexOf("http", StringComparison.Ordinal) == 0)
            {
                // 如果是 http 开头, 则说明是完整的 url 地址 直接返回即可
                return url;
            }

            if (url.IndexOf("api", StringComparison.Ordinal) == 0 || url.IndexOf("/api", StringComparison.Ordinal) == 0)
            {
                if (url.IndexOf('/') != 0)
                {
                    url = $"/{url}";
                }

                // 如果 地址以 api 或者 /api 开头说明是需要从配置文件中获取 请求ip的url
                var valueKey = new Regex(@"(?<=api/)\w+").Match(url).Value;
                var serviceUrl = new ConfigurHandler().GetAppOptionValue<string>("ServiceUrl", valueKey);
                if (serviceUrl.IsNullOrEmpty())
                {
                    return $"{httpType}://{valueKey}-service{url}";
                }

                if (serviceUrl == "{https}")
                {
                    return $"https://{valueKey}-service{url}";
                }

                if (serviceUrl == "{http}")
                {
                    return $"http://{valueKey}-service{url}";
                }

                return serviceUrl + url;
            }

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
                var serviceUrl = configurHandler.GetAppOptionValue<string>("ServiceUrl", v);
                var replace = url.Replace("{" + v + "}", serviceUrl);
                url = replace;
            }

            return url;
        }
    }
}
