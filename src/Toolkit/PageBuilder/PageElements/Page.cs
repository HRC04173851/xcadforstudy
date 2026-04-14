// -*- coding: utf-8 -*-
// src/Toolkit/PageBuilder/PageElements/Page.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现页面基类 Page 和页面扩展 PageExtension。
// 继承自 Group，实现 IPage 接口，是 PropertyManager 页面的根元素。
// 提供绑定管理器的延迟初始化和帮助链接打开功能。
//*********************************************************************

using System;
using System.IO;
using Xarial.XCad.Exceptions;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.Utils.PageBuilder.Base;
using Xarial.XCad.Utils.PageBuilder.Core;

namespace Xarial.XCad.Utils.PageBuilder.PageElements
{
    /// <summary>
    /// Base page element implementation.
    /// <para>页面元素基类实现。</para>
    /// </summary>
    public abstract class Page : Group, IPage
    {
        private IBindingManager m_Binding;

        /// <summary>
        /// Initializes page with framework-reserved id.
        /// <para>使用框架保留标识符初始化页面。</para>
        /// </summary>
        public Page() : base(-1, null, null)
        {
        }

        /// <summary>
        /// Gets lazy-initialized binding manager.
        /// <para>获取延迟初始化的数据绑定管理器。</para>
        /// </summary>
        public IBindingManager Binding
        {
            get
            {
                return m_Binding ?? (m_Binding = new BindingManager());
            }
        }

        public override void Focus()
        {
        }
    }

    /// <summary>
    /// Extension helpers for page behavior.
    /// <para>页面行为的扩展辅助方法。</para>
    /// </summary>
    public static class PageExtension 
    {
        private class OpenHelpLinkException : Exception, IUserException 
        {
            internal OpenHelpLinkException(string err) : base(err)
            {
            }

            internal OpenHelpLinkException(string err, Exception inner) : base(err, inner) 
            {
            }
        }

        /// <summary>
        /// Attempts to open help link and reports user-friendly message on failure.
        /// <para>尝试打开帮助链接，失败时显示用户友好的提示消息。</para>
        /// </summary>
        public static void TryOpenLink(this Page page, string link, IXApplication app)
        {
            try
            {
                if (!string.IsNullOrEmpty(link))
                {
                    if (!IsUrl(link))
                    {
                        if (!Path.IsPathRooted(link)) 
                        {
                            link = Path.Combine(Path.GetDirectoryName(page.GetType().Assembly.Location), link);
                        }
                    }

                    try
                    {
                        System.Diagnostics.Process.Start(link);
                    }
                    catch(Exception ex)
                    {
                        throw new OpenHelpLinkException("Help link is not available", ex);
                    }
                }
                else
                {
                    throw new OpenHelpLinkException("Help link is not specified");
                }
            }
            catch (Exception ex)
            {
                string err;

                if (ex is IUserException)
                {
                    err = ex.Message;
                }
                else 
                {
                    err = "Failed to open help link";
                }

                app.ShowMessageBox(err, XCad.Base.Enums.MessageBoxIcon_e.Warning);
            }
        }

        /// <summary>
        /// Determines whether input string is a non-file absolute URL.
        /// <para>判断输入字符串是否为非文件类型的绝对 URL。</para>
        /// </summary>
        private static bool IsUrl(string input)
        {
            if (Uri.TryCreate(input, UriKind.Absolute, out var uri))
            {
                return !uri.IsFile;
            }

            return false;
        }
    }
}