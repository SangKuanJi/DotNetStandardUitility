//-----------------------------------------------------------------------
// <copyright file="StringExtension.cs" company="Personage Enterprises">
// * Copyright (C) 2018 qinchaoyue 版权所有。
// * version : 1.0
// * author  : qinchaoyue
// * FileName: StringExtension.cs
// * history : created by qinchaoyue 2018-05-14 06:18:11
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace HotPot.Utility.Extension
{
    /// <summary>
    /// string 扩展类
    /// </summary>
    public static class StringExtension
    {
        public static int TryToInt(this string value)
        {
            try
            {
                return int.Parse(value);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取字符串中的数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static decimal GetNumber(this string str)
        {
            decimal result = 0;
            if (!string.IsNullOrEmpty(str))
            {
                // 正则表达式剔除非数字字符（不包含小数点.） 
                str = Regex.Replace(str, @"[^\d.\d]", "");
                // 如果是数字，则转换为decimal类型 
                if (Regex.IsMatch(str, @"^[+-]?\d*[.]?\d*$"))
                {
                    result = decimal.Parse(str);
                }
            }
            return result;
        }

        /// <summary>
        /// 替换为空
        /// </summary>
        /// <param name="value">原始字符串</param>
        /// <param name="oldValue">替换字符串</param>
        /// <returns></returns>
        public static string ReplaceEmpty(this string value, string oldValue)
        {
            return value.Replace(oldValue, string.Empty);
        }
        /// <summary>
        /// 获取身份证生日
        /// 失败返回 DateTime.MinValue
        /// </summary>
        /// <param name="idCard">身份证号码</param>
        /// <returns>生日yyyy-MM-dd, 失败返回 DateTime.MinValue</returns>
        public static string IdCardBirthday(this string idCard)
        {
            if (!idCard.IsIdentityCard())
                return DateTime.MinValue.ToString("yyyy-MM-dd");

            return idCard.Substring(6, 4) + "-" + idCard.Substring(10, 2) + "-" + idCard.Substring(12, 2);
        }

        /// <summary>
        /// 身份证性别
        /// 1: 男
        /// 0: 女
        /// </summary>
        /// <param name="idCard">身份证</param>
        /// <returns>1: 男, 0: 女, -1: 不是有效的身份证</returns>
        public static string IdCardSex(this string idCard)
        {
            if (!idCard.IsIdentityCard())
                return "-1";
            var sex = idCard.Substring(14, 3);
            if (int.Parse(sex) % 2 == 0)//性别代码为偶数是女性奇数为男性
            {
                return "0";
            }
            else
            {
                return "1";
            }

        }

        /// <summary>
        /// 用正则表达式判断字符是不是汉字
        /// </summary>
        /// <param name="text">待判断字符或字符串</param>
        /// <returns>真：是汉字；假：不是</returns>
        public static bool IsChinese(this string text)
        {
            return Regex.IsMatch(text, @"[\u4e00-\u9fbb]+$");
        }

        public static string UrlEncode(this string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        public static string UrlDecode(this string str)
        {
            return HttpUtility.UrlDecode(str);
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DesEncrypt(string str)
        {
            byte[] keys = { 12, 23, 34, 45, 56, 67, 78, 89 };
            byte[] ivs = { 120, 230, 10, 1, 10, 20, 30, 40 };
            //加密  
            var strs = Encoding.Unicode.GetBytes(str);

            var desc = new DESCryptoServiceProvider();
            var mStream = new MemoryStream();

            var transform = desc.CreateEncryptor(keys, ivs);//加密对象  
            var cStream = new CryptoStream(mStream, transform, CryptoStreamMode.Write);
            cStream.Write(strs, 0, strs.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DesDecode(this string str)
        {
            byte[] keys = { 12, 23, 34, 45, 56, 67, 78, 89 };
            byte[] ivs = { 120, 230, 10, 1, 10, 20, 30, 40 };

            //解密  
            byte[] strs = Convert.FromBase64String(str);

            DESCryptoServiceProvider desc = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();

            ICryptoTransform transform = desc.CreateDecryptor(keys, ivs);//解密对象  

            CryptoStream cStream = new CryptoStream(mStream, transform, CryptoStreamMode.Write);
            cStream.Write(strs, 0, strs.Length);
            cStream.FlushFinalBlock();
            return Encoding.Unicode.GetString(mStream.ToArray());
        }

        public static bool IsDecimal(this string value)
        {
            try
            {
                Convert.ToDecimal(value);
                return true;
            }
            catch (Exception)
            {
                //throw new ArgumentException("值不是Decimal");
                return false;
            }
        }

        /// <summary>
        /// 指示所指定的正则表达式在指定的输入字符串中是否找到了匹配项
        /// </summary>
        /// <param name="value">要搜索匹配项的字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式</param>
        /// <returns>如果正则表达式找到匹配项，则为 true；否则，为 false</returns>
        public static bool IsMatch(this string value, string pattern)
        {
            return value != null && Regex.IsMatch(value, pattern);
        }

        /// <summary>
        /// 是否电子邮件
        /// </summary>
        public static bool IsEmail(this string value)
        {
            const string pattern = @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";
            return value.IsMatch(pattern);
        }

        /// <summary>
        /// 是否是IP地址
        /// </summary>
        public static bool IsIpAddress(this string value)
        {
            const string pattern = @"^(d{1,2}|1dd|2[0-4]d|25[0-5]).(d{1,2}|1dd|2[0-4]d|25[0-5]).(d{1,2}|1dd|2[0-4]d|25[0-5]).(d{1,2}|1dd|2[0-4]d|25[0-5])$";
            return value.IsMatch(pattern);
        }

        /// <summary>
        /// 是否是整数
        /// </summary>
        public static bool IsNumeric(this string value)
        {
            const string pattern = @"^\-?[0-9]+$";
            return value.IsMatch(pattern);
        }

        /// <summary>
        /// 是否是Unicode字符串
        /// </summary>
        public static bool IsUnicode(this string value)
        {
            const string pattern = @"^[\u4E00-\u9FA5\uE815-\uFA29]+$";
            return value.IsMatch(pattern);
        }

        /// <summary>
        /// 是否Url字符串
        /// </summary>
        public static bool IsUrl(this string value)
        {
            const string pattern = @"^(http|https|ftp|rtsp|mms):(\/\/|\\\\)[A-Za-z0-9%\-_@]+\.[A-Za-z0-9%\-_@]+[A-Za-z0-9\.\/=\?%\-&_~`@:\+!;]*$";
            return value.IsMatch(pattern);
        }

        /// <summary>
        /// 是否身份证号，验证如下3种情况：
        /// 1.身份证号码为15位数字；
        /// 2.身份证号码为18位数字；
        /// 3.身份证号码为17位数字+1个字母
        /// </summary>
        public static bool IsIdentityCard(this string value)
        {
            const string pattern = @"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$";
            return value.IsMatch(pattern);
        }

        /// <summary>
        /// 是否手机号码
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isRestrict">是否按严格格式验证</param>
        public static bool IsMobileNumber(this string value, bool isRestrict = false)
        {
            var pattern = isRestrict ? @"^[1][3-8]\d{9}$" : @"^[1]\d{10}$";
            return value.IsMatch(pattern);
        }

        /// <summary>
        /// 无条件进位, 保留小数点后两位
        /// </summary>
        /// <param name="dec"></param>
        /// <returns></returns>
        public static string CeillingD2(this decimal dec)
        {
            return (dec + 0.004m).ToString("#0.00");
        }

        /// <summary>
        /// 判断是否空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string Get32MD5(this string str)
        {
            return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(UTF8Encoding.Default.GetBytes(str))).Replace("-", "");
        }

        /// <summary>  
        /// 将传入字符串以GZip算法压缩后，返回Base64编码字符  
        /// </summary>  
        /// <param name="rawString">需要压缩的字符串</param>  
        /// <returns>压缩后的Base64编码的字符串</returns>  
        public static string GZipCompressString(string rawString)
        {
            if (string.IsNullOrEmpty(rawString) || rawString.Length == 0)
            {
                return "";
            }
            else
            {
                byte[] rawData = System.Text.Encoding.UTF8.GetBytes(rawString.ToString());
                byte[] zippedData = Compress(rawData);
                return (string)(Convert.ToBase64String(zippedData));
            }
        }

        /// <summary>  
        /// GZip压缩  
        /// </summary>  
        /// <param name="rawData"></param>  
        /// <returns></returns>  
        public static byte[] Compress(byte[] rawData)
        {
            MemoryStream ms = new MemoryStream();
            GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true);
            compressedzipStream.Write(rawData, 0, rawData.Length);
            compressedzipStream.Close();
            return ms.ToArray();
        }

        /// <summary>
        /// 返回随机字符串
        /// </summary>
        /// <param name="length">字符串长度</param>
        /// <returns></returns>
        public static string GenerateRandomString(int length)
        {
            char[] constant = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            var checkCode = string.Empty;
            var rd = new Random();
            for (var i = 0; i < length; i++)
            {
                checkCode += constant[rd.Next(36)].ToString();
            }
            return checkCode;
        }

        /// <summary>
        /// 解压 GZip
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GZipDecompress(byte[] data)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (GZipStream gZipStream = new GZipStream(new MemoryStream(data), CompressionMode.Decompress))
                    {
                        byte[] bytes = new byte[40960];
                        int n;
                        while ((n = gZipStream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            stream.Write(bytes, 0, n);
                        }
                        gZipStream.Close();
                    }

                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            }
            catch
            {
                return Encoding.UTF8.GetString(data);
            }
        }
    }
}
