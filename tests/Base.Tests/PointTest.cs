// -*- coding: utf-8 -*-
// tests/Base.Tests/PointTest.cs

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
    /// 测试三维点（Point）通过变换矩阵（TransformMatrix）进行空间变换的功能。
    /// 包括纯旋转平移、缩放以及投影到平面的变换场景。
    /// </summary>
    public class PointTest
    {
        /// <summary>
        /// 测试用例目的：验证 Point 在同时包含旋转和平移的变换矩阵下的正确性。
        /// 测试数据来源于 SOLIDWORKS 的真实变换矩阵（16元素格式）。
        /// 预期结果：点 (1,3,5) 经过矩阵变换后得到坐标 (-2.59, -4.76, 1.29)，误差容许 0.00001%。
        /// </summary>
        [Test]
        public void TransformTest()
        {
            // sw transform data: 从 SOLIDWORKS 导出的 4x4 变换矩阵，按列主序存储
            // 矩阵包含旋转分量（前3x3）和平移分量（第4列）
            var matrix = new TransformMatrix(-0.708267430806948, 0.347623567688267, 0.614422575794384, 0, -0.705944223333632, -0.348767569783138, -0.616444592651635, 0, 8.88178419700125E-16, -0.870355695940025, 0.492423560103246, 0, 0.232454472653307, 0.28957519769969, 0.0593062288453039, 1);
            var point = new Point(1, 3, 5);

            var point1 = point.Transform(matrix);

            // 验证变换后的坐标与预期值匹配（0.00001% 相对误差容许）
            Assert.That(-2.59364562815453, Is.EqualTo(point1.X).Within(0.00001).Percent);
            Assert.That(-4.76088242366158, Is.EqualTo(point1.Y).Within(0.00001).Percent);
            Assert.That(1.28651282720101, Is.EqualTo(point1.Z).Within(0.00001).Percent);
        }

        /// <summary>
        /// 测试用例目的：验证 Point 在包含缩放、旋转和平移的复杂变换矩阵下的正确性。
        /// 此矩阵包含较大的缩放因子和非均匀缩放分量。
        /// 预期结果：点 (-0.2, 0.35, -0.7) 经变换后坐标约为 (822.93, 19.45, 300.90)
        /// </summary>
        [Test]
        public void TransformWithScaleTest()
        {
            // sw transform data: 包含非均匀缩放的变换矩阵
            var matrix = new TransformMatrix(564.542711973209, 163.719840977085, -238.46583819816, 0, 28.4743581273473, -551.862310440353, -311.473622003293, 0, -287.853006318067, 266.500318846062, -498.494576467766, 0, 724.374297695573, 431.891671932574, 13.2759519466606, 1);
            var point = new Point(-0.2, 0.35, -0.7);

            var point1 = point.Transform(matrix);

            Assert.That(822.92888506815, Is.EqualTo(point1.X).Within(0.00001).Percent);
            Assert.That(19.4456718907902, Is.EqualTo(point1.Y).Within(0.00001).Percent);
            Assert.That(300.899555412576, Is.EqualTo(point1.Z).Within(0.00001).Percent);
        }

        /// <summary>
        /// 测试用例目的：验证 Point 在投影到指定平面场景下的变换行为。
        /// 矩阵为正交投影矩阵（最后一行 [3,0,0,0] 表示 w 分量为 0 的投影）
        /// 预期结果：原点 (0,0,0) 经变换后仍接近原点，仅受平移分量影响
        /// </summary>
        [Test]
        public void TransformWithScaleToPlaneTest()
        {
            // sw transform data: 正交投影变换矩阵
            var matrix = new TransformMatrix(2.38602721601803, 1.36624109143621, 1.20010808033794, 0, -1.81832643281355, 1.81832643281354, 1.54511422473005, 0, -0.023729899974265, -1.95629094559137, 2.27428288215086, 0, 0.700972369348501, 0.438557054675365, -0.0946540940610755, 1);
            var point = new Point(0, 0, 0);

            var point1 = point.Transform(matrix);

            // 原点变换后仅受平移分量影响，结果应为平移向量
            Assert.That(0.700972369348501, Is.EqualTo(point1.X).Within(0.00001).Percent);
            Assert.That(0.438557054675365, Is.EqualTo(point1.Y).Within(0.00001).Percent);
            Assert.That(-9.46540940610755E-02, Is.EqualTo(point1.Z).Within(0.00001).Percent);
        }
    }
}
