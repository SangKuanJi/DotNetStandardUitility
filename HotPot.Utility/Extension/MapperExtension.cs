//-----------------------------------------------------------------------
// <copyright file="MapperExtension.cs" company="Personage Enterprises">
// * Copyright (C) 2018 qinchaoyue 版权所有。
// * version : 1.0
// * author  : qinchaoyue
// * FileName: MapperExtension.cs
// * history : created by qinchaoyue 2018-05-16 02:35:51
// </copyright>
//-----------------------------------------------------------------------

using System;
using AutoMapper;

namespace HotPot.Utility.Extension
{
    public static class MapperExtension
    {
        /// <summary>
        /// 将对象映射为指定类型
        /// </summary>
        /// <typeparam name="TTarget">要映射的目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <returns>目标类型的对象</returns>
        public static TTarget MapTo<TTarget>(this object source)
        {
            return Mapper.Map<TTarget>(source);
        }

        /// <summary>
        /// 更新映射，使用源对象更新目标对象
        /// </summary>
        /// <typeparam name="TSource"> 源类型 </typeparam>
        /// <typeparam name="TTarget"> 目标类型 </typeparam>
        /// <param name="source"> 源对象 </param>
        /// <param name="configure"> 配置信息 </param>
        /// <returns> 更新后的目标对象 </returns>
        public static TTarget MapTo<TSource, TTarget>(this TSource source, Action<IMapperConfigurationExpression> configure = null)
        {
            Action<IMapperConfigurationExpression> configureMe = null;
            if (configure == null)
            {
                configureMe = cfg => cfg.CreateMap<TSource, TTarget>();
            }
            else
            {
                configureMe = configure;
            }

            return new MapperConfiguration(configureMe).CreateMapper().Map<TTarget>(source);
        }


        /// <summary>
        /// 使用源类型的对象更新目标类型的对象
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="target">待更新的目标对象</param>
        /// <returns>更新后的目标类型对象</returns>
        public static TTarget MapTo<TSource, TTarget>(this TSource source, TTarget target)
        {
            return Mapper.Map<TSource, TTarget>(source);
        }
    }
}
