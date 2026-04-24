// -*- coding: utf-8 -*-
// tests/Base.Tests/Rect2DTest.cs

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Geometry.Structures;

namespace Base.Tests
{
    /// <summary>
    /// 测试二维矩形（Rect2D）的交集判断功能。
    /// Rect2D 由宽度、高度和中心点定义。
    /// </summary>
    public class Rect2DTest
    {
        /// <summary>
        /// 测试用例目的：验证 Rect2D.Intersects 方法对各种矩形相交情况的判断正确性。
        /// 测试场景：
        /// - i1: r2 完全在 r1 内部（中心点重合）
        /// - i2: r3 与 r1 部分重叠
        /// - i3: r4 与 r1 完全不相交（r1 的右下方）
        /// - i4: r5 与 r1 完全不相交（r1 的下方）
        /// - i5: r6 与 r1 完全不相交（r1 的右侧）
        /// - i6, i7: r7、r8 与 r1 部分重叠
        /// </summary>
        [Test]
        public void IntersectsTest()
        {
            // 创建基准矩形 r1: 宽10，高10，中心点 (5,5)
            var r1 = new Rect2D(10, 10, new Point(5, 5, 0));
            // r2 宽5，高5，中心点 (5,5) - 完全在 r1 内部
            var r2 = new Rect2D(5, 5, new Point(5, 5, 0));
            // r3 宽10，高10，中心点 (7,7) - 与 r1 部分重叠
            var r3 = new Rect2D(10, 10, new Point(7, 7, 0));
            // r4 宽10，高10，中心点 (20,20) - 与 r1 完全不相交
            var r4 = new Rect2D(10, 10, new Point(20, 20, 0));
            // r5 宽10，高10，中心点 (5,20) - 与 r1 完全不相交
            var r5 = new Rect2D(10, 10, new Point(5, 20, 0));
            // r6 宽10，高10，中心点 (20,5) - 与 r1 完全不相交
            var r6 = new Rect2D(10, 10, new Point(20, 5, 0));
            // r7 宽10，高10，中心点 (15,5) - 与 r1 部分重叠
            var r7 = new Rect2D(10, 10, new Point(15, 5, 0));
            // r8 宽10，高10，中心点 (5,15) - 与 r1 部分重叠
            var r8 = new Rect2D(10, 10, new Point(5, 15, 0));

            var i1 = r1.Intersects(r2);
            var i2 = r1.Intersects(r3);
            var i3 = r1.Intersects(r4);
            var i4 = r1.Intersects(r5);
            var i5 = r1.Intersects(r6);
            var i6 = r1.Intersects(r7);
            var i7 = r1.Intersects(r8);

            // 验证交集判断结果
            Assert.IsTrue(i1);   // r2 完全在 r1 内部
            Assert.IsTrue(i2);   // r3 与 r1 部分重叠
            Assert.IsFalse(i3); // r4 与 r1 不相交
            Assert.IsFalse(i4); // r5 与 r1 不相交
            Assert.IsFalse(i5); // r6 与 r1 不相交
            Assert.IsTrue(i6);  // r7 与 r1 部分重叠
            Assert.IsTrue(i7);  // r8 与 r1 部分重叠
        }
    }
}
