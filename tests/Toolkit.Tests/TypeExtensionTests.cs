// -*- coding: utf-8 -*-
// tests/Toolkit.Tests/TypeExtensionTests.cs

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xarial.XCad.Utils.Reflection;

namespace Toolkit.Tests
{
    /// <summary>
    /// 测试 TypeExtension 类型扩展方法，特别是 IsComVisible COM 可视性检测功能。
    /// COM 可视性决定类型是否对 COM 可见，影响类型在非托管环境中的访问性。
    /// </summary>
    public class TypeExtensionTests
    {
        /// <summary>
        /// UserControl1：默认 COM 不可见（无 ComVisibleAttribute）。
        /// </summary>
        public class UserControl1 : UserControl
        {
        }

        /// <summary>
        /// UserControl2：显式标记为 COM 不可见 [ComVisible(false)]。
        /// </summary>
        [ComVisible(false)]
        public class UserControl2 : UserControl
        {
        }

        /// <summary>
        /// UserControl3：显式标记为 COM 可见 [ComVisible(true)]。
        /// </summary>
        [ComVisible(true)]
        public class UserControl3 : UserControl
        {
        }

        /// <summary>
        /// 测试用例目的：验证 IsComVisible 方法正确检测类型的 COM 可视性。
        /// - 默认（无属性）：COM 不可见
        /// - [ComVisible(false)]：COM 不可见
        /// - [ComVisible(true)]：COM 可见
        /// - 程序集级别 [ComVisible(true)]：所有类型 COM 可见
        /// </summary>
        [Test]
        public void IsComVisibleTest()
        {
            var r1 = typeof(UserControl1).IsComVisible();
            var r2 = typeof(UserControl2).IsComVisible();
            var r3 = typeof(UserControl3).IsComVisible();
            var r4 = CreateTypeInComVisibleAssm().IsComVisible();

            Assert.IsFalse(r1); // 默认无属性，COM 不可见
            Assert.IsFalse(r2); // [ComVisible(false)]，COM 不可见
            Assert.IsTrue(r3);  // [ComVisible(true)]，COM 可见
            Assert.IsTrue(r4);  // 程序集级别 [ComVisible(true)]，COM 可见
        }

        /// <summary>
        /// 创建在标记为 [ComVisible(true)] 的动态程序集中的类型。
        /// 用于测试程序集级别 COM 可视性设置。
        /// </summary>
        private Type CreateTypeInComVisibleAssm()
        {
            var assmName = new AssemblyName(Guid.NewGuid().ToString());
            var assmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assmName, AssemblyBuilderAccess.RunAndSave);

            var moduleBuilder = assmBuilder.DefineDynamicModule(assmName.Name, assmName.Name + ".dll");

            var typeBuilder = moduleBuilder.DefineType("UserControl4", TypeAttributes.Public);

            // 创建 [ComVisible(true)] 属性
            var attBuilder = new CustomAttributeBuilder(typeof(ComVisibleAttribute).GetConstructor(new Type[] { typeof(bool) }), new object[] { true });

            // 设置程序集级别的 ComVisible
            assmBuilder.SetCustomAttribute(attBuilder);

            typeBuilder.SetParent(typeof(UserControl));

            return typeBuilder.CreateType();
        }
    }
}
