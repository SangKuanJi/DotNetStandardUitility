//-----------------------------------------------------------------------
// <copyright file="ConfigurHandler.cs" company="Personage Enterprises">
// * Copyright (C) 2018 qinchaoyue 版权所有。
// * version : 1.0
// * author  : qinchaoyue
// * FileName: ConfigurHandler.cs
// * history : created by qinchaoyue 2018-05-15 03:45:33
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using HotPot.Utility.Extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HotPot.Utility.ConfigurManager
{
    public class ConfigurHandler
    {
        /// <summary>
        /// 配置文件集合
        /// </summary>
        private static readonly Dictionary<string, IConfigurationRoot> ConfigurationRootDictionary = new Dictionary<string, IConfigurationRoot>();

        /// <summary>
        /// 配置文件集合
        /// </summary>
        private static readonly Dictionary<string, IConfiguration> ConfigurationDictionary = new Dictionary<string, IConfiguration>();

        /// <summary>
        /// 从配置文件中获取节点信息
        /// </summary>
        /// <typeparam name="T"> 获取的配置节点类型 </typeparam>
        /// <param name="appSetingKey"> 节点key </param>
        /// <param name="fileName"> 配置文件名称 </param>
        /// <param name="byHostingJson"> 是否根据 hosting.json 获取配置文件 </param>
        /// <returns> 节点值 </returns>
        public T GetAppOptions<T>(string appSetingKey, string fileName = "", bool byHostingJson = true) where T : class, new()
        {
            var configuration = GetConfiguration(fileName, byHostingJson);

            var appconfig = new ServiceCollection()
                .AddOptions()
                .Configure<T>(configuration.GetSection(appSetingKey))
                .BuildServiceProvider()
                .GetService<IOptions<T>>()
                .Value;
            return appconfig;
        }

        /// <summary>
        /// 获取配置信息类
        /// </summary>
        /// <param name="appSetingKey"> 配置节点root key </param>
        /// <param name="fileName"> 配置文件名称 </param>
        /// <param name="byHostingJson"> 是否根据 hosting.json 获取配置文件 </param>
        /// <typeparam name="T"> 节点 class </typeparam>
        /// <returns> The <see cref="IConfigurationSection"/>. </returns>
        public IConfigurationSection GetSection(string appSetingKey, string fileName = "", bool byHostingJson = true)
        {
            IConfigurationRoot configuration = GetConfiguration(fileName, byHostingJson);
            return configuration.GetSection(appSetingKey);
        }

        /// <summary>
        /// 从配置文件中获取项
        /// </summary>
        /// <param name="appSettingKey"> 选项父级 key </param>
        /// <param name="valueKey"> 选项子级 key </param>
        /// <param name="fileName"> 配置文件名称 </param>
        /// <param name="byHostingJson"> 是否根据 hosting.json 获取配置文件 </param>
        /// <typeparam name="T"> 返回类型 </typeparam>
        /// <returns> 获取成功返回数据, 获取失败返回 default(T) <see cref="T"/>. </returns>
        public T GetAppOptionValue<T>(string appSettingKey, string valueKey = "", string fileName = "", bool byHostingJson = true)
        {
            var configuration = GetConfiguration(fileName, byHostingJson);
            var key = $"{appSettingKey}";
            if (!valueKey.IsNullOrEmpty())
            {
                key = $"{appSettingKey}:{valueKey}";
            }

            return configuration.GetValue<T>(key);
        }

        /// <summary>
        /// 从配置文件中获取项
        /// </summary>
        /// <param name="appSettingKey"> 选项父级 key </param>
        /// <param name="valueKey"> 选项子级 key </param>
        /// <param name="default">如果获取失败则返回默认值</param>
        /// <param name="fileName"> 配置文件名称 </param>
        /// <param name="byHostingJson"> 是否根据 hosting.json 获取配置文件 </param>
        /// <returns> 获取成功返回数据, 获取失败返回 default </returns>
        public string GetAppOptionValue(string appSettingKey, string valueKey = "", string @default = "", string fileName = "", bool byHostingJson = true)
        {
            var value = this.GetAppOptionValue<string>(appSettingKey, valueKey, fileName, byHostingJson);
            return value ?? @default;
        }

        /// <summary>
        /// 从配置文件中获取项
        /// </summary>
        /// <param name="appSettingKey"> 选项父级 key </param>
        /// <param name="valueKey"> 选项子级 key </param>
        /// <param name="default">如果获取失败则返回默认值</param>
        /// <param name="fileName"> 配置文件名称 </param>
        /// <param name="byHostingJson"> 是否根据 hosting.json 获取配置文件 </param>
        /// <typeparam name="T"> 返回类型 </typeparam>
        /// <returns> 获取成功返回数据, 获取失败返回 default </returns>
        public int GetAppOptionValue(string appSettingKey, string valueKey = "", int @default = 0, string fileName = "", bool byHostingJson = true)
        {
            var value = this.GetAppOptionValue<int>(appSettingKey, valueKey, fileName, byHostingJson);
            return value == default(int) ? @default : value;
        }

        /// <summary>
        /// 从配置文件中获取项
        /// </summary>
        /// <param name="appSettingKey"> 选项父级 key </param>
        /// <param name="valueKey"> 选项子级 key </param>
        /// <param name="default">如果获取失败则返回默认值</param>
        /// <param name="fileName"> 配置文件名称 </param>
        /// <param name="byHostingJson"> 是否根据 hosting.json 获取配置文件 </param>
        /// <typeparam name="T"> 返回类型 </typeparam>
        /// <returns> 获取成功返回数据, 获取失败返回 default </returns>
        public long GetAppOptionValue(string appSettingKey, string valueKey, long @default = 0, string fileName = "", bool byHostingJson = true)
        {
            var value = this.GetAppOptionValue<long>(appSettingKey, valueKey, fileName, byHostingJson);
            return value == default(long) ? @default : value;
        }

        /// <summary>
        /// 从配置文件中获取项
        /// </summary>
        /// <param name="appSettingKey"> 选项父级 key </param>
        /// <param name="valueKey"> 选项子级 key </param>
        /// <param name="default">如果获取失败则返回默认值</param>
        /// <param name="fileName"> 配置文件名称 </param>
        /// <param name="byHostingJson"> 是否根据 hosting.json 获取配置文件 </param>
        /// <typeparam name="T"> 返回类型 </typeparam>
        /// <returns> 获取成功返回数据, 获取失败返回 default </returns>
        public float GetAppOptionValue(string appSettingKey, string valueKey, float @default = 0, string fileName = "", bool byHostingJson = true)
        {
            var value = this.GetAppOptionValue<float>(appSettingKey, valueKey, fileName, byHostingJson);
            return value == default(float) ? @default : value;
        }

        /// <summary>
        /// 从配置文件中获取项
        /// </summary>
        /// <param name="appSettingKey"> 选项父级 key </param>
        /// <param name="valueKey"> 选项子级 key </param>
        /// <param name="default">如果获取失败则返回默认值</param>
        /// <param name="fileName"> 配置文件名称 </param>
        /// <param name="byHostingJson"> 是否根据 hosting.json 获取配置文件 </param>
        /// <typeparam name="T"> 返回类型 </typeparam>
        /// <returns> 获取成功返回数据, 获取失败返回 default </returns>
        public double GetAppOptionValue(string appSettingKey, string valueKey, double @default = 0, string fileName = "", bool byHostingJson = true)
        {
            var value = this.GetAppOptionValue<double>(appSettingKey, valueKey, fileName, byHostingJson);
            return value == default(double) ? @default : value;
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="fileName"> 配置文件名称 </param>
        /// <param name="byHostingJson"> 是否根据 hosting.json 获取配置文件 </param>
        /// <returns> The <see cref="IConfigurationRoot"/>. </returns>
        private static IConfigurationRoot GetConfiguration(string fileName, bool byHostingJson = true)
        {
            var baseDir = AppContext.BaseDirectory;

            var appFileName = string.Empty;
            if (fileName.IsNullOrEmpty())
            {
                fileName = "appsettings.json";
            }

            if (byHostingJson)
            {
                try
                {
                    var config = GetConfiguration(baseDir);
                    appFileName = $".{config.GetSection("environment").Value}";
                }
                catch (Exception e)
                {

                }
            }

            fileName = fileName.Insert(fileName.IndexOf('.'), appFileName);
            return GetConfigurationRoot(fileName, baseDir);
        }

        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <param name="baseDir"> The base dir.  </param>
        /// <returns> The <see cref="IConfigurationRoot"/>.  </returns>
        private static IConfiguration GetConfiguration(string baseDir)
        {
            if (ConfigurationDictionary.ContainsKey(baseDir))
            {
                return ConfigurationDictionary[baseDir];
            }

            IConfiguration config = new ConfigurationBuilder().SetBasePath(baseDir).AddJsonFile("hosting.json")
                .Build();
            ConfigurationDictionary.Add(baseDir, config);
            return config;
        }

        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <param name="filePath"> The file path.  </param>
        /// <param name="baseDir"> The base dir.  </param>
        /// <returns> The <see cref="IConfigurationRoot"/>.  </returns>
        private static IConfigurationRoot GetConfigurationRoot(string filePath, string baseDir)
        {
            if (ConfigurationRootDictionary.ContainsKey(filePath))
            {
                return ConfigurationRootDictionary[filePath];
            }

            var configuration = new ConfigurationBuilder()
                .SetBasePath(baseDir)
                .Add(new JsonConfigurationSource { Path = filePath, Optional = false, ReloadOnChange = false })
                .Build();
            ConfigurationRootDictionary.Add(filePath, configuration);
            return configuration;
        }
    }
}
