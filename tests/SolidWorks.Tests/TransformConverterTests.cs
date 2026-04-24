// -*- coding: utf-8 -*-
// tests/SolidWorks.Tests/TransformConverterTests.cs

using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.SolidWorks.Utils;

namespace SolidWorks.Tests
{
    /// <summary>
    /// 测试 TransformConverter 变换转换器的 SOLIDWORKS 格式与 xCAD 格式之间的转换功能。
    /// SOLIDWORKS 使用 12 元素数组（3x4 矩阵），而 xCAD 使用 16 元素 4x4 矩阵。
    /// </summary>
    public class TransformConverterTests
    {
        /// <summary>
        /// 测试用例目的：验证旋转+平移矩阵的双向转换（Roundtrip）是否保持数据一致。
        /// 先将 SOLIDWORKS 格式转换为 xCAD 格式，再转回 SOLIDWORKS 格式，结果应相同。
        /// </summary>
        [Test]
        public void RoundtripRotationTranslationTest()
        {
            // SOLIDWORKS 16元素变换数据：4x4 矩阵格式
            // 包括旋转分量（前9个元素，按列主序）和平移分量
            var swTransformData = new double[]
            {
                0.499999999999998, 0.5, -0.707106781186549, -0.146446609406726,  // 第1列
                0.853553390593274, 0.5, 0.853553390593275, -0.146446609406726,   // 第2列
                0.499999999999998, -0.0974873734152917, -0.072487373415292,       // 第3列
                0.122487373415293, 1, 0, 0, 0                                     // 第4列（平移）
            };

            var matrix = TransformConverter.ToTransformMatrix(swTransformData);

            var res = TransformConverter.ToMathTransformData(matrix);

            // 验证 Roundtrip 转换后数据一致
            CollectionAssert.AreEqual(swTransformData, res);
        }

        /// <summary>
        /// 测试用例目的：验证纯旋转矩阵的转换正确性。
        /// 测试场景：绕 X 轴旋转 30 度。
        /// 预期结果：转换后的数据与预期旋转矩阵匹配。
        /// </summary>
        [Test]
        public void RotationTest()
        {
            // 30 degs rotation around X
            // 构建绕 X 轴旋转 30 度的 4x4 矩阵
            var matrix = new TransformMatrix(
                1,      0,              0,              0,
                0,      0.86602540378,  -0.5,           0,
                0,      0.5,            0.86602540378,  0,
                0,      0,              0,              1);

            var data = TransformConverter.ToMathTransformData(matrix);

            // 预期结果：绕 X 轴旋转的标准矩阵
            CollectionAssert.AreEqual(new double[] { 1, 0, 0, 0, 0.86602540378, -0.5, 0, 0.5, 0.86602540378, 0, 0, 0, 1, 0, 0, 0 }, data);
        }

        /// <summary>
        /// 测试用例目的：验证包含缩放的矩阵的双向转换是否保持数据一致。
        /// 测试场景：复杂的非均匀缩放+旋转+平移矩阵。
        /// </summary>
        [Test]
        public void RoundtripScaleTest()
        {
            var swTransformData = new double[]
            {
                -0.173281870700371,
                -0.568901317857332,
                -0.803943209329347,
                 0.871391797781904,
                -0.468966056249135,
                 0.144038789374787,
                -0.458965933425347,
                -0.675590207615774,
                 0.576999257650021,
                 0.118738080204305,
                 3.07942116552466E-03,
                -1.31754673526319E-02,
                 3,
                 0,
                 0,
                 0
            };

            var matrix = TransformConverter.ToTransformMatrix(swTransformData);

            var res = TransformConverter.ToMathTransformData(matrix);

            // 验证 Roundtrip 转换（使用自定义比较器处理浮点精度）
            CollectionAssert.AreEqual(swTransformData, res, new DoubleComparer());
        }

        /// <summary>
        /// 测试用例目的：验证 ToMathTransformData 方法的转换正确性。
        /// 对比转换结果与预期值。
        /// </summary>
        [Test]
        public void ToMathTransformTest()
        {
            var matrix = new TransformMatrix(0.227240687239813, 0.130118199184401, 0.114296007651233, 0, -0.173173945982243, 0.173173945982242, 0.147153735688577, 0, -0.00225999047373953, -0.186313423389655, 0.216598369728654, 0, 0.202001019294273, 0.304139434520347, -0.00901467562486433, 1);
            var transformData = TransformConverter.ToMathTransformData(matrix);

            var expTransform = new double[] { 0.795342405339345, 0.455413697145403, 0.400036026779314, -0.606108810937849, 0.606108810937846, 0.515038074910018, -7.90996665808835E-03, -0.652096981863791, 0.758094294050287, 0.202001019294273, 0.304139434520347, -9.01467562486433E-03, 0.285714285714286, 0, 0, 0 };
            CollectionAssert.AreEqual(expTransform, transformData, new DoubleComparer());
        }

        /// <summary>
        /// 测试用例目的：验证单位矩阵的转换正确性。
        /// 单位矩阵是最基础的变换（无旋转、无平移、无缩放）。
        /// </summary>
        [Test]
        public void IdentityTest()
        {
            var matrix = TransformConverter.ToTransformMatrix(new double[]
            {
                1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0
            });

            // 验证转换为 TransformMatrix 后与单位矩阵相同
            CollectionAssert.AreEqual(TransformMatrix.Identity.ToArray(), matrix.ToArray(), new DoubleComparer());
        }

        /// <summary>
        /// 测试用例目的：验证纯平移变换的提取正确性。
        /// 测试场景：X+0.01, Y+0.02, Z+0.03 的平移。
        /// </summary>
        [Test]
        public void TranslationTest()
        {
            var matrix = TransformConverter.ToTransformMatrix(new double[]
            {
                1, 0, 0, 0, 1, 0, 0, 0, 1, 0.01, 0.02, 0.03, 1, 0, 0, 0
            });

            // 验证提取的平移分量
            Assert.AreEqual(matrix.M41, 0.01);
            Assert.AreEqual(matrix.M42, 0.02);
            Assert.AreEqual(matrix.M43, 0.03);
        }

        /// <summary>
        /// DoubleComparer：用于浮点数数组比较，允许 14 位小数精度差异。
        /// 这是因为 SOLIDWORKS 和 xCAD 的矩阵存储精度略有不同。
        /// </summary>
        public class DoubleComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                var d1 = (double)x;
                var d2 = (double)y;

                if (Math.Round(d1, 14) == Math.Round(d2, 14))
                {
                    return 0;
                }
                else
                {
                    return d1.CompareTo(d2);
                }
            }
        }
    }
}