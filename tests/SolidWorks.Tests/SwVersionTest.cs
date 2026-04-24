// -*- coding: utf-8 -*-
// tests/SolidWorks.Tests/SwVersionTest.cs

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.SolidWorks;
using Xarial.XCad;
using SolidWorks.Interop.sldworks;
using Moq;
using Xarial.XCad.SolidWorks.Enums;

namespace SolidWorks.Tests
{
    /// <summary>
    /// 测试 SwVersion 版本比较功能，包括 IsVersionNewerOrEqual、Equals、CompareTo 等方法。
    /// SOLIDWORKS 版本格式：主版本.服务版本.补丁版本（如 "25.2.3" 表示 SW2023 SP2.3）
    /// </summary>
    public class SwVersionTest
    {
        /// <summary>
        /// 测试用例目的：验证 IsVersionNewerOrEqual 方法对各种版本比较场景的正确性。
        /// 测试场景：
        /// - r1: SW2023 (25) >= SW2017 (23) → true
        /// - r2: SW2023 SP2 (25.2) >= SW2017 SP2 (23.2) → true
        /// - r3: SW2023 SP2.3 (25.2.3) >= SW2017 SP2.3 (23.2.3) → true
        /// - r4: SW2023 SP3 (25.3) >= SW2017 SP2 (23.2) → false（主版本相同但 SP3 < SP 需求检查）
        /// - r5: SW2023 SP1 (25.1) >= SW2017 (23) → true（主版本更新）
        /// - r6: SW2023 (25) >= SW2016 SP4 (24.4) → true（主版本更新）
        /// - r7: SW2024 (26) >= SW2018 (25) → false（26 < 25 的某种比较逻辑）
        /// </summary>
        [Test]
        public void IsVersionNewerOrEqualTest()
        {
            // 模拟 SOLIDWORKS 版本 "25.2.3" (SW2023 SP2.3)
            var sw2017sp23Mock = new Mock<SldWorks>();
            sw2017sp23Mock.Setup(m => m.RevisionNumber()).Returns("25.2.3");
            var app = SwApplicationFactory.FromPointer(sw2017sp23Mock.Object);

            var r1 = app.IsVersionNewerOrEqual(SwVersion_e.Sw2017);
            var r2 = app.IsVersionNewerOrEqual(SwVersion_e.Sw2017, 2);
            var r3 = app.IsVersionNewerOrEqual(SwVersion_e.Sw2017, 2, 3);
            var r4 = app.IsVersionNewerOrEqual(SwVersion_e.Sw2017, 3);
            var r5 = app.IsVersionNewerOrEqual(SwVersion_e.Sw2017, 1);
            var r6 = app.IsVersionNewerOrEqual(SwVersion_e.Sw2016, 4);
            var r7 = app.IsVersionNewerOrEqual(SwVersion_e.Sw2018, 0);

            Assert.IsTrue(r1);
            Assert.IsTrue(r2);
            Assert.IsTrue(r3);
            Assert.IsFalse(r4);
            Assert.IsTrue(r5);
            Assert.IsTrue(r6);
            Assert.IsFalse(r7);
            // 验证 null + 非 null 的补丁版本组合会抛出异常
            Assert.Throws<ArgumentException>(() => app.IsVersionNewerOrEqual(SwVersion_e.Sw2017, null, 1));
        }

        /// <summary>
        /// 测试用例目的：验证 SwVersion.Equals 方法比较两个相同版本的结果。
        /// </summary>
        [Test]
        public void EqualityTest()
        {
            var v1 = SwApplicationFactory.CreateVersion(Xarial.XCad.SolidWorks.Enums.SwVersion_e.Sw2020);
            var v2 = SwApplicationFactory.CreateVersion(Xarial.XCad.SolidWorks.Enums.SwVersion_e.Sw2020);
            var v3 = SwApplicationFactory.CreateVersion(Xarial.XCad.SolidWorks.Enums.SwVersion_e.Sw2019);

            Assert.That(v1.Equals(v2));
            Assert.That(!v1.Equals(v3));
        }

        /// <summary>
        /// 测试用例目的：验证 SwVersion.CompareTo 方法的排序比较结果。
        /// CompareTo 返回：-1（小于）、0（等于）、1（大于）。
        /// </summary>
        [Test]
        public void CompareTest()
        {
            var v1 = SwApplicationFactory.CreateVersion(Xarial.XCad.SolidWorks.Enums.SwVersion_e.Sw2020);
            var v2 = SwApplicationFactory.CreateVersion(Xarial.XCad.SolidWorks.Enums.SwVersion_e.Sw2020);
            var v3 = SwApplicationFactory.CreateVersion(Xarial.XCad.SolidWorks.Enums.SwVersion_e.Sw2019);

            Assert.AreEqual(0, v1.CompareTo(v2));
            Assert.AreEqual(1, v1.CompareTo(v3));
            Assert.AreEqual(-1, v3.CompareTo(v2));
        }

        /// <summary>
        /// 测试用例目的：验证 Compare 扩展方法返回枚举类型的版本比较结果。
        /// VersionEquality_e 枚举：Same、Newer、Older。
        /// </summary>
        [Test]
        public void CompareExtensionTest()
        {
            var v1 = SwApplicationFactory.CreateVersion(Xarial.XCad.SolidWorks.Enums.SwVersion_e.Sw2020);
            var v2 = SwApplicationFactory.CreateVersion(Xarial.XCad.SolidWorks.Enums.SwVersion_e.Sw2020);
            var v3 = SwApplicationFactory.CreateVersion(Xarial.XCad.SolidWorks.Enums.SwVersion_e.Sw2019);

            Assert.AreEqual(VersionEquality_e.Same, v1.Compare(v2));
            Assert.AreEqual(VersionEquality_e.Newer, v1.Compare(v3));
            Assert.AreEqual(VersionEquality_e.Older, v3.Compare(v2));
        }
    }
}
