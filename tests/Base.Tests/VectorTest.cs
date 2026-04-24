// -*- coding: utf-8 -*-
// tests/Base.Tests/VectorTest.cs

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
    /// 测试三维向量（Vector）的空间变换和垂直向量生成功能。
    /// 向量变换与点变换类似，但不受平移分量影响（仅受旋转和缩放影响）。
    /// </summary>
    public class VectorTest
    {
        /// <summary>
        /// 测试用例目的：验证 Vector 在同时包含旋转和平移的变换矩阵下的正确性。
        /// 向量的平移分量被忽略，仅旋转和缩放影响结果。
        /// 预期结果：向量 (1,3,5) 经矩阵变换后约为 (-2.83, -5.05, 1.23)
        /// </summary>
        [Test]
        public void TransformTest()
        {
            // sw transform data: SOLIDWORKS 导出的 4x4 变换矩阵
            var matrix = new TransformMatrix(-0.708267430806948, 0.347623567688267, 0.614422575794384, 0, -0.705944223333632, -0.348767569783138, -0.616444592651635, 0, 8.88178419700125E-16, -0.870355695940025, 0.492423560103246, 0, 0.232454472653307, 0.28957519769969, 0.0593062288453039, 1);
            var vector = new Vector(1, 3, 5);

            var vector1 = vector.Transform(matrix);

            Assert.That(-2.82610010080784, Is.EqualTo(vector1.X).Within(0.00001).Percent);
            Assert.That(-5.05045762136127, Is.EqualTo(vector1.Y).Within(0.00001).Percent);
            Assert.That(1.22720659835571, Is.EqualTo(vector1.Z).Within(0.00001).Percent);
        }

        /// <summary>
        /// 测试用例目的：验证 Vector 在包含非均匀缩放的变换矩阵下的正确性。
        /// 此矩阵包含较大的缩放因子，向量会被显著放大。
        /// 预期结果：向量 (-0.2, 0.35, -0.7) 经变换后约为 (98.55, -412.45, 287.62)
        /// </summary>
        [Test]
        public void TransformWithScaleTest()
        {
            // sw transform data: 包含大缩放因子的变换矩阵
            var matrix = new TransformMatrix(564.542711973209, 163.719840977085, -238.46583819816, 0, 28.4743581273473, -551.862310440353, -311.473622003293, 0, -287.853006318067, 266.500318846062, -498.494576467766, 0, 724.374297695573, 431.891671932574, 13.2759519466606, 1);
            var vector = new Vector(-0.2, 0.35, -0.7);

            var vector1 = vector.Transform(matrix);

            Assert.That(98.5545873725764, Is.EqualTo(vector1.X).Within(0.00001).Percent);
            Assert.That(-412.446000041784, Is.EqualTo(vector1.Y).Within(0.00001).Percent);
            Assert.That(287.623603465916, Is.EqualTo(vector1.Z).Within(0.00001).Percent);
        }

        /// <summary>
        /// 测试用例目的：验证 Vector 在投影到指定平面场景下的变换行为。
        /// 测试向量为 Y 轴单位向量 (0,1,0)。
        /// 预期结果：经投影矩阵变换后仍保持为单位向量尺度
        /// </summary>
        [Test]
        public void TransformWithScaleToPlaneTest()
        {
            // sw transform data: 正交投影变换矩阵
            var matrix = new TransformMatrix(2.38602721601803, 1.36624109143621, 1.20010808033794, 0, -1.81832643281355, 1.81832643281354, 1.54511422473005, 0, -0.023729899974265, -1.95629094559137, 2.27428288215086, 0, 0.700972369348501, 0.438557054675365, -0.0946540940610755, 1);
            var vector = new Vector(0, 1, 0);

            var vector1 = vector.Transform(matrix);

            // 验证变换后的向量分量
            Assert.That(-1.81832643281355, Is.EqualTo(vector1.X).Within(0.00001).Percent);
            Assert.That(1.81832643281354, Is.EqualTo(vector1.Y).Within(0.00001).Percent);
            Assert.That(1.54511422473005, Is.EqualTo(vector1.Z).Within(0.00001).Percent);
        }

        /// <summary>
        /// 测试用例目的：验证 CreateAnyPerpendicular 方法为任意向量生成垂直向量的正确性。
        /// 测试场景包括：X轴、Z轴、负Z轴以及近零向量。
        /// 预期结果：每个向量都能生成与之垂直的非零向量
        /// </summary>
        [Test]
        public void CreateAnyPerpendicularTest()
        {
            var v1 = new Vector(1, 0, 0);  // X轴方向
            var v2 = new Vector(0, 0, 1);  // Z轴方向
            var v3 = new Vector(0, 0, -1); // 负Z轴方向
            var v4 = new Vector(1E-15, 1E-15, 1); // 近零向量（几乎平行于Z轴）

            var r1 = v1.CreateAnyPerpendicular();
            var r2 = v2.CreateAnyPerpendicular();
            var r3 = v3.CreateAnyPerpendicular();
            var r4 = v4.CreateAnyPerpendicular();

            // 验证 v1 的垂直向量应为 (0,-1,0)
            Assert.AreEqual(0, r1.X);
            Assert.AreEqual(-1, r1.Y);
            Assert.AreEqual(0, r1.Z);

            // 验证 v2 的垂直向量应为 (1,0,0)
            Assert.AreEqual(1, r2.X);
            Assert.AreEqual(0, r2.Y);
            Assert.AreEqual(0, r2.Z);

            // 验证 v3 的垂直向量应为 (1,0,0)（与 v2 相同，因为 v3 是 v2 的反向）
            Assert.AreEqual(1, r3.X);
            Assert.AreEqual(0, r3.Y);
            Assert.AreEqual(0, r3.Z);

            // 验证 v4 的垂直向量应为 (1,0,0)
            Assert.AreEqual(1, r4.X);
            Assert.AreEqual(0, r4.Y);
            Assert.AreEqual(0, r4.Z);
        }
    }
}
