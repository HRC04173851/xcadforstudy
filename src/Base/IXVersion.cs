// -*- coding: utf-8 -*-
// src/Base/IXVersion.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 版本接口，表示应用程序或文件的版本信息，提供版本号、显示名称和版本比较功能。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad
{
    /// <summary>
    /// Represents the version of the application or file
    /// <para>中文：表示应用程序或文件的版本信息</para>
    /// </summary>
    public interface IXVersion : IComparable<IXVersion>, IEquatable<IXVersion>
    {
        /// <summary>
        /// Version number
        /// <para>中文：版本号</para>
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Display name of this version
        /// <para>中文：此版本的显示名称</para>
        /// </summary>
        string DisplayName { get; }
    }

    /// <summary>
    /// Versions equality result
    /// <para>中文：版本比较结果枚举</para>
    /// </summary>
    public enum VersionEquality_e 
    {
        /// <summary>
        /// Versions are the same
        /// <para>中文：版本相同</para>
        /// </summary>
        Same = 0,

        /// <summary>
        /// This version is older to the version it is compared to
        /// <para>中文：此版本比比较版本旧</para>
        /// </summary>
        Older = -1,

        /// <summary>
        /// This version is newer to the version it is compared to
        /// <para>中文：此版本比比较版本新</para>
        /// </summary>
        Newer = 1
    }

    /// <summary>
    /// Additional methods for version
    /// <para>中文：版本的附加扩展方法</para>
    /// </summary>
    public static class XVersionExtension 
    {
        /// <summary>
        /// Compares two versions
        /// <para>中文：比较两个版本</para>
        /// </summary>
        /// <typeparam name="TVersion">Type of the version to compare</typeparam>
        /// <param name="vers">This version</param>
        /// <param name="other">Version to compare to</param>
        /// <returns>Result of comparison</returns>
        public static VersionEquality_e Compare<TVersion>(this TVersion vers, TVersion other)
            where TVersion : IXVersion
            => (VersionEquality_e)vers.CompareTo(other);
    }
}
