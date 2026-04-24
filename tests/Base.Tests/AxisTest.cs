// -*- coding: utf-8 -*-
// tests/Base.Tests/AxisTest.cs

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
    /// 测试轴（Axis）的共线性判断功能。
    /// 共线性指两条轴是否位于同一直线上（方向相同或相反）。
    /// </summary>
    public class AxisTest
    {
        /// <summary>
        /// 测试用例目的：验证 Axis.IsCollinear 方法对各种共线/非共线情况的判断正确性。
        /// 测试场景包括：
        /// - r1: a1 和 a2 完全共线（方向向量相同）
        /// - r2, r3: a3 和 a4 位于从同一点出发的一条直线上
        /// - r4: a4 的方向向量与 a1 相同但位置不同（不共线）
        /// - r5: a5 的方向向量与 a1 不共线
        /// - r6: a6 与 a1 起点几乎相同（浮点误差范围内）
        /// - r7: a7 与 a6 类似情况
        /// </summary>
        [Test]
        public void IsCollinearTest()
        {
            // 创建用于测试的点坐标
            var pt1 = new Point(-0.1409493395, -0.07924862951, -0.08122498726);
            var pt2 = new Point(0.29214638898, 0.10968929136, 0);
            var pt3 = new Point(0.0932228447, 0.02290893682, -0.03730713856);
            var pt4 = new Point(0.36209354558, 0.14020372378, 0.01311824738);
            var pt5 = new Point(0.02541124962, -0.06801307203, -0.14991221502);
            // pt6 是 pt1 的微小偏移点（用于测试浮点精度边界）
            var pt6 = new Point(-0.1409493395 + 1E-15, -0.07924862951 + 1E-15, -0.08122498726 + 1E-15);

            // 创建轴：起点 + 方向向量
            var a1 = new Axis(pt1, pt2 - pt1);
            var a2 = new Axis(pt3, pt2 - pt3);
            var a3 = new Axis(pt4, pt4 - pt1);
            var a4 = new Axis(pt5, pt2 - pt1);
            var a5 = new Axis(pt5, pt5 - pt1);
            var a6 = new Axis(pt6, pt2 - pt6);
            var a7 = new Axis(pt6, pt2 - pt1);

            var r1 = a1.IsCollinear(a2);
            var r2 = a1.IsCollinear(a3);
            var r3 = a1.IsCollinear(a3);
            var r4 = a1.IsCollinear(a4);
            var r5 = a1.IsCollinear(a5);
            var r6 = a1.IsCollinear(a6);
            var r7 = a1.IsCollinear(a7);

            // 验证共线性判断结果
            Assert.IsTrue(r1);  // 共线：方向向量相同
            Assert.IsTrue(r2);  // 共线：a3 在 a1 的延长线上
            Assert.IsTrue(r3);  // 共线：同上
            Assert.IsFalse(r4); // 不共线：起点不同
            Assert.IsFalse(r5); // 不共线：方向向量不同
            Assert.IsTrue(r6);  // 共线：起点微小偏移在容许范围内
            Assert.IsTrue(r7);  // 共线：方向向量相同
        }
    }
}
