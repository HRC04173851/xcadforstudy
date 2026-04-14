// -*- coding: utf-8 -*-
// src/Toolkit/Windows/RotHelper.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现访问运行对象表（ROT）的辅助工具类 RotHelper。
// 根据 Moniker 名称从 ROT 中查找并返回 COM 对象。
// 用于获取正在运行的 COM 对象实例。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Xarial.XCad.Base;

namespace Xarial.XCad.Toolkit.Windows
{
    /// <summary>
    /// Utilities for accessing Running Object Table
    /// <para>访问运行对象表（ROT）的辅助工具。</para>
    /// </summary>
    public static class RotHelper
    {
        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(uint reserved, out IBindCtx ppbc);

        /// <summary>
        /// Returns the COM object by moniker name
        /// <para>根据 Moniker 名称从 ROT 中返回 COM 对象。</para>
        /// </summary>
        /// <typeparam name="TComObj">Type of COM object<para>COM 对象类型。</para></typeparam>
        /// <param name="monikerName">Name of the moniker or an empty string to iterate all monikers<para>Moniker 名称；为空时遍历所有 Moniker。</para></param>
        /// <param name="logger">Custom logger<para>自定义日志器。</para></param>
        /// <param name="predicate">Predicate<para>可选筛选条件。</para></param>
        /// <returns>COM objects<para>匹配的 COM 对象；未找到时返回默认值。</para></returns>
        public static TComObj TryGetComObjectByMonikerName<TComObj>(string monikerName, IXLogger logger, Predicate<TComObj> predicate = null)
        {
            IBindCtx context = null;
            IRunningObjectTable rot = null;
            IEnumMoniker monikers = null;

            try
            {
                CreateBindCtx(0, out context);

                context.GetRunningObjectTable(out rot);
                rot.EnumRunning(out monikers);

                var moniker = new IMoniker[1];

                while (monikers.Next(1, moniker, IntPtr.Zero) == 0)
                {
                    var curMoniker = moniker.First();

                    if (curMoniker != null)
                    {
                        try
                        {
                            curMoniker.GetDisplayName(context, null, out var name);

                            if (string.IsNullOrEmpty(monikerName) || string.Equals(monikerName,
                                name, StringComparison.CurrentCultureIgnoreCase))
                            {
                                object app;
                                rot.GetObject(curMoniker, out app);

                                if (app is TComObj)
                                {
                                    if (predicate == null || predicate.Invoke((TComObj)app))
                                    {
                                        return (TComObj)app;
                                    }
                                }
                            }
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            logger?.Log(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.Log(ex);
            }
            finally
            {
                if (monikers != null)
                {
                    while (Marshal.ReleaseComObject(monikers) > 0) ;
                }

                if (rot != null)
                {
                    while (Marshal.ReleaseComObject(rot) > 0) ;
                }

                if (context != null)
                {
                    while (Marshal.ReleaseComObject(context) > 0) ;
                }
            }

            return default;
        }
    }
}
