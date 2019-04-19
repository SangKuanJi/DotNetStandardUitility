//-----------------------------------------------------------------------
// <copyright file="ActionType.cs" company="Personage Enterprises">
// * Copyright (C) 2018 qinchaoyue 版权所有。
// * version : 1.0
// * author  : qinchaoyue
// * FileName: ActionType.cs
// * history : created by qinchaoyue 2018-05-28 04:04:13
// </copyright>
//-----------------------------------------------------------------------

namespace hotPot.Selenium.Net45.Entity
{
    public enum ActionType
    {
        GoUrl,
        Clear,
        Click,
        Scroll,
        RemoveReadonly,
        Find,
        Finds,
        SendKeys,
        Screenshot,
        Sleep,
        Lambda,
        SetAttribute
    }
}