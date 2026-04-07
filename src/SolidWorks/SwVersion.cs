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
using Xarial.XCad.SolidWorks.Enums;

namespace Xarial.XCad.SolidWorks
{
    /// <summary>
    /// Represents a SolidWorks version with major version, service pack, and service pack revision.
    /// <para>中文：表示 SolidWorks 版本信息，包含主版本号、服务包（SP）及服务包修订号。</para>
    /// </summary>
    public interface ISwVersion : IXVersion
    {
        /// <summary>
        /// Major version (e.g., Sw2023)
        /// <para>中文：主版本号（例如 Sw2023）</para>
        /// </summary>
        SwVersion_e Major { get; }

        /// <summary>
        /// Service Pack number
        /// <para>中文：服务包编号（SP 编号）</para>
        /// </summary>
        int ServicePack { get; }

        /// <summary>
        /// Service Pack revision number
        /// <para>中文：服务包修订版本号</para>
        /// </summary>
        int ServicePackRevision { get; }
    }

    /// <summary>
    /// Internal implementation of <see cref="ISwVersion"/>. Supports comparison and equality checks between versions.
    /// <para>中文：ISwVersion 的内部实现，支持 SolidWorks 版本之间的比较与相等判断。</para>
    /// </summary>
    internal class SwVersion : ISwVersion
    {
        /// <summary>
        /// Major version enum value
        /// <para>中文：主版本枚举值</para>
        /// </summary>
        public SwVersion_e Major { get; }

        /// <summary>
        /// Human-readable display name (e.g., "SOLIDWORKS 2023")
        /// <para>中文：人类可读的显示名称（例如"SOLIDWORKS 2023"）</para>
        /// </summary>
        public string DisplayName
            => $"SOLIDWORKS {Major.ToString().Substring("Sw".Length)}";

        /// <summary>
        /// Full version as a <see cref="System.Version"/> object
        /// <para>中文：以 System.Version 对象表示的完整版本号</para>
        /// </summary>
        public Version Version { get; }

        /// <summary>Service Pack number
        /// <para>中文：服务包编号</para></summary>
        public int ServicePack { get; }

        /// <summary>Service Pack revision number
        /// <para>中文：服务包修订版本号</para></summary>
        public int ServicePackRevision { get; }

        /// <summary>
        /// Initializes a new SwVersion from a Version object and SP/SP revision values.
        /// <para>中文：从 Version 对象及服务包值初始化新的 SwVersion 实例。</para>
        /// </summary>
        internal SwVersion(Version version, int sp, int spRev) 
        {
            Version = version;
            // 将 Version.Major 整数值转换为 SwVersion_e 枚举
            Major = (SwVersion_e)version.Major;

            ServicePack = sp;
            ServicePackRevision = spRev;
        }

        /// <summary>
        /// Compares this version to another. Only supports comparison between <see cref="ISwVersion"/> instances.
        /// <para>中文：将此版本与另一版本进行比较，仅支持与 ISwVersion 实例的比较。</para>
        /// </summary>
        public int CompareTo(IXVersion other)
        {
            const int EQUAL = 0;

            if (other is ISwVersion)
            {
                //NOTE: cannot compare Version as for the pre-release SP and SP Rev can be negative which is not supported for the Version
                // 中文：不能直接比较 Version 对象，预发布版本的 SP 和 SP Rev 可能为负数，Version 类不支持负值

                var res = Major.CompareTo(((ISwVersion)other).Major);

                if (res == EQUAL)
                {
                    res = ServicePack.CompareTo(((ISwVersion)other).ServicePack);

                    if (res == EQUAL)
                    {
                        res = ServicePack.CompareTo(((ISwVersion)other).ServicePackRevision);
                    }
                }

                return res;
            }
            else 
            {
                throw new InvalidCastException("Can only compare SOLIDWORKS versions");
            }
        }

        /// <summary>
        /// Returns hash code based on major version only.
        /// <para>中文：基于主版本号返回哈希码。</para>
        /// </summary>
        public override int GetHashCode() => (int)Major;

        /// <summary>
        /// Checks equality with another object; only equal to objects that are also <see cref="ISwVersion"/>.
        /// <para>中文：与另一对象比较是否相等，仅与同为 ISwVersion 的对象相等。</para>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is ISwVersion))
            {
                return false;
            }

            return IsSame((ISwVersion)obj);
        }

        // 版本相等性判断基于主版本号
        private bool IsSame(ISwVersion other) => Major == other.Major;

        /// <inheritdoc/>
        public bool Equals(IXVersion other) => Equals((object)other);

        /// <summary>Equality operator for two SwVersion instances.
        /// <para>中文：两个 SwVersion 实例的相等运算符。</para></summary>
        public static bool operator ==(SwVersion version1, SwVersion version2)
            => version1.IsSame(version2);

        /// <summary>Inequality operator for two SwVersion instances.
        /// <para>中文：两个 SwVersion 实例的不等运算符。</para></summary>
        public static bool operator !=(SwVersion version1, SwVersion version2)
            => !version1.IsSame(version2);

        /// <summary>
        /// Returns the display name of this version.
        /// <para>中文：返回此版本的显示名称（如 "SOLIDWORKS 2023"）。</para>
        /// </summary>
        public override string ToString() => DisplayName;
    }
}
