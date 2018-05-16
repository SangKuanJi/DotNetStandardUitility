using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HotPot.HttpClient.Net45.Contract
{
    public interface IHttpClient
    {
        byte[] GetByte(string url);

        /// <summary>
        /// 同步发起Put请求并返回string类型结果
        /// </summary>
        /// <param name="url"> 请求地址 </param>
        /// <param name="formData">请求数据. </param>
        /// <param name="charset">请求编码</param>
        /// <param name="mediaType">请求类型</param>
        /// <returns>string类型结果</returns>
        string PutString(string url, object formData = null, string charset = "UTF-8", string mediaType = "application/json");

        /// <summary>
        /// 异步发起Put请求并返回string类型结果
        /// </summary>
        /// <param name="url"> 请求地址 </param>
        /// <param name="formData">请求数据. </param>
        /// <param name="charset">请求编码</param>
        /// <param name="mediaType">请求类型</param>
        /// <returns>string类型结果</returns>
        Task<string> PutStringAsync(string url, object formData = null, string charset = "UTF-8", string mediaType = "application/json");

        /// <summary>
        /// 发起一个请求并返回string类型的请求结果
        /// </summary>
        /// <param name="url"> The url.  </param>
        /// <param name="param"> The param.  </param>
        /// <param name="headers">请求头部</param>
        /// <returns> The <see cref="string"/>.  </returns>
        string GetString(string url, object param = null, Dictionary<string, string> headers = null);

        /// <summary>
        /// 异步 发起 Get 请求并返回 string 类型请求结果
        /// </summary>
        /// <param name="requestUri">请求地址 . </param>
        /// <param name="headers">请求头部</param>
        /// <returns>请求结果</returns>
        Task<string> GetStringAsync(string requestUri, Dictionary<string, string> headers = null);

        Task<string> GetStringAsync(string url, object param = null,
            Dictionary<string, string> headers = null);

        /// <summary>
        /// 异步请求post
        /// </summary>
        /// <param name="url">网络的地址("/api/values")</param>
        /// <param name="formData">入参</param>
        /// <param name="charset">编码格式</param>
        /// <param name="mediaType">头媒体类型</param>
        /// <param name="headers">请求头部</param>
        /// <returns></returns>
        Task<string> PostStringAsync(string url, object formData = null, string charset = "UTF-8", string mediaType = "application/json", Dictionary<string, string> headers = null,
        string acceptMediaType = "application/json"
        );

        /// <summary>
        /// 同步请求post
        /// </summary>
        /// <param name="url">网络的地址("/api/values")</param>
        /// <param name="formData">入参</param>
        /// <param name="charset">编码格式</param>
        /// <param name="mediaType">头媒体类型</param>
        /// <param name="headers">请求头部</param>
        /// <returns></returns>
        string PostString(string url, object formData = null, string charset = "UTF-8", string mediaType = "application/json", Dictionary<string, string> headers = null, string acceptMediaType = "application/json");

        /// <summary>
        /// 同步请求post, 请求失败返回 outStr
        /// </summary>
        /// <param name="url">网络的地址</param>
        /// <param name="outStr">请求失败返回值</param>
        /// <param name="formData">入参</param>
        /// <param name="charset">编码格式</param>
        /// <param name="mediaType">头媒体类型</param>
        /// <param name="headers">请求头部</param>
        /// <returns></returns>
        string TryPostString(string url, string outStr, object formData = null, string charset = "UTF-8", string mediaType = "application/json", Dictionary<string, string> headers = null);
    }
}
