//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Xarial.XCad.SwDocumentManager
{
    /// <summary>
    /// Maps SOLIDWORKS release years to the numeric major version used by Document Manager.
    /// 把 SOLIDWORKS 发布年份映射到 Document Manager 使用的数值主版本号。
    /// </summary>
    public enum SwDmVersion_e
    {
        Sw2000 = 1500,
        Sw2001 = 1750,
        Sw2001Plus = 1950,
        Sw2003 = 2200,
        Sw2004 = 2500,
        Sw2005 = 2800,
        Sw2006 = 3100,
        Sw2007 = 3400,
        Sw2008 = 3800,
        Sw2009 = 4100,
        Sw2010 = 4400,
        Sw2011 = 4700,
        Sw2012 = 5000,
        Sw2013 = 6000,
        Sw2014 = 7000,
        Sw2015 = 8000,
        Sw2016 = 9000,
        Sw2017 = 10000,
        Sw2018 = 11000,
        Sw2019 = 12000,
        Sw2020 = 13000,
        Sw2021 = 14000,
        Sw2022 = 15000,
        Sw2023 = 16000,
        Sw2024 = 17000,
        Sw2025 = 18000
    }

    /// <summary>
    /// xCAD version abstraction for SOLIDWORKS Document Manager.
    /// 面向 SOLIDWORKS Document Manager 的 xCAD 版本抽象。
    /// </summary>
    public interface ISwDmVersion : IXVersion
    {
        SwDmVersion_e Major { get; }
    }

    /// <summary>
    /// Concrete version object used for comparison and display.
    /// 用于版本比较与显示的具体版本对象。
    /// </summary>
    internal class SwDmVersion : ISwDmVersion
    {
        public SwDmVersion_e Major { get; }

        public string DisplayName
            => $"SOLIDWORKS {Major.ToString().Substring("Sw".Length)}";

        public Version Version { get; }

        /// <summary>
        /// Builds the strongly typed version object from a raw .NET version.
        /// 根据原始 .NET `Version` 构建强类型的 SOLIDWORKS 版本对象。
        /// </summary>
        internal SwDmVersion(Version version)
        {
            Version = version;
            Major = (SwDmVersion_e)version.Major;
        }

        /// <summary>
        /// Compares two SOLIDWORKS Document Manager versions.
        /// 比较两个 SOLIDWORKS Document Manager 版本的先后关系。
        /// </summary>
        public int CompareTo(IXVersion other)
        {
            if (other is ISwDmVersion)
            {
                return Version.CompareTo(other.Version);
            }
            else
            {
                throw new InvalidCastException("Can only compare SOLIDWORKS versions");
            }
        }

        public override int GetHashCode() => (int)Major;

        public override bool Equals(object obj)
        {
            if (!(obj is ISwDmVersion))
            {
                return false;
            }

            return IsSame((ISwDmVersion)obj);
        }

        private bool IsSame(ISwDmVersion other) => Major == other.Major;

        public bool Equals(IXVersion other) => Equals((object)other);

        public static bool operator ==(SwDmVersion version1, SwDmVersion version2)
            => version1.IsSame(version2);

        public static bool operator !=(SwDmVersion version1, SwDmVersion version2)
            => !version1.IsSame(version2);

        public override string ToString() => DisplayName;
    }

    /// <summary>
    /// Helper extensions for version capability checks.
    /// 用于能力判定的版本扩展方法，例如判断某个 API 是否在指定版本及以上可用。
    /// </summary>
    public static class SwDmVersionExtension 
    {
        public static bool IsVersionNewerOrEqual(this ISwDmVersion vers, SwDmVersion_e version)
            => vers.Major >= version;
    }
}
