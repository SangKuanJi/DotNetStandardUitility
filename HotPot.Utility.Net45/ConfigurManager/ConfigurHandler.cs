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
using System.Configuration;

namespace HotPot.Utility.Net45.ConfigurManager
{
    public class ConfigurHandler
    {
        public string GetAppOptionValue(string key, string @default)
        {
            try
            {
                return ConfigurationManager.AppSettings[key];
            }
            catch (Exception e)
            {
                return @default;
            }
        }
    }
}
