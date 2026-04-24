// -*- coding: utf-8 -*-
// tests/Base.Tests/MatrixTest.cs

using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Geometry.Structures;

namespace Base.Tests
{
    /// <summary>
    /// 测试 4x4 变换矩阵（TransformMatrix）的各种数学运算功能。
    /// 包括：从轴旋转创建矩阵、矩阵乘法、行列式求逆、欧拉角提取。
    /// </summary>
    public class MatrixTest
    {
        /// <summary>
        /// 测试用例目的：验证 CreateFromRotationAroundAxis 方法通过轴和角度创建旋转矩阵的正确性。
        /// 测试数据来源于 SOLIDWORKS 的真实变换矩阵。
        /// 预期结果：生成的旋转矩阵与预期矩阵相等
        /// </summary>
        [Test]
        public void CreateFromRotationAroundAxisTest()
        {
            // sw transform data: SOLIDWORKS 导出的纯旋转矩阵（无平移和缩放）
            var matrix = new TransformMatrix(0.900202513118371, -0.424567507790233, -0.096839386120319, 0, 0.383554841948467, 0.878329091336096, -0.285348366966756, 0, 0.206206495031693, 0.219728101619931, 0.953518978712666, 0, 0, 0, 0, 1);

            // 使用轴 (-0.5, 0.3, -0.8) 和 30度（0.523599弧度）创建旋转矩阵
            var rotMatrix = TransformMatrix.CreateFromRotationAroundAxis(new Vector(-0.5, 0.3, -0.8), 0.523599, new Point(0, 0, 0));

            CollectionAssert.AreEqual(matrix.ToArray(), rotMatrix.ToArray(), new DoubleComparer());
        }

        /// <summary>
        /// 测试用例目的：验证 TransformMatrix.Multiply 方法进行矩阵乘法的正确性。
        /// 矩阵乘法顺序：result = matrix1 * matrix2
        /// 预期结果：乘积矩阵与预期矩阵相等
        /// </summary>
        [Test]
        public void MultiplyTest()
        {
            // sw transform data: 第一个变换矩阵
            var matrix1 = new TransformMatrix(-0.708267430806948, 0.347623567688267, 0.614422575794384, 0, -0.705944223333632, -0.348767569783138, -0.616444592651635, 0, 8.88178419700125E-16, -0.870355695940025, 0.492423560103246, 0, 0.232454472653307, 0.28957519769969, 0.0593062288453039, 1);

            // sw transform data: 第二个变换矩阵
            var matrix2 = new TransformMatrix(0.974788926484518, 0, -0.223128995881668, 0, 0.223128995881668, 0, 0.974788926484518, 0, 0, -1, 0, 0, 0.18810566875436, 0.01, 0.123668090384383, 1);

            // sw transform data: matrix1 * matrix2 的预期结果矩阵
            var expMatrix = new double[] { -0.612846350937167, -0.614422575794384, 0.496894605019207, 0, -0.765966769263138, 0.616444592651635, -0.182458139240622, 0, -0.194201592494987, -0.492423560103246, -0.848413094505063, 0, 0.479312337703569, -0.0493062288453039, 0.354075453415271, 1 };

            var resMatrix = matrix1.Multiply(matrix2);

            CollectionAssert.AreEqual(expMatrix, resMatrix.ToArray(), new DoubleComparer());
        }

        /// <summary>
        /// 测试用例目的：验证 TransformMatrix.Determinant 行列式计算的正确性。
        /// 行列式用于判断矩阵是否可逆（行列式不为零时可逆）。
        /// 预期结果：行列式值约为 -366.875
        /// </summary>
        [Test]
        public void DeterminantTest()
        {
            var matrix = new TransformMatrix(
                -1, 3, 3.5, 9,
                1, 2, 1.5, 1,
                -3, 7.6, 9, 7.45,
                5, 2, 4, 9);

            var d = matrix.Determinant;

            Assert.That(-366.875, Is.EqualTo(d).Within(0.00001).Percent);
        }

        /// <summary>
        /// 测试用例目的：验证 TransformMatrix.Inverse 方法求矩阵逆的正确性。
        /// 矩阵求逆常用于坐标变换的逆向计算。
        /// 预期结果：逆矩阵与预期矩阵相等
        /// </summary>
        [Test]
        public void InverseTest()
        {
            var matrix = new TransformMatrix(
                -1, 3, 3.5, 9,
                1, 2, 1.5, 1,
                -3, 7.6, 9, 7.45,
                5, 2, 4, 9);

            var inversedMatrix = matrix.Inverse();

            var expMatrix = new double[] { -0.0941737649063033, 0.212470187393527, -0.0477001703577513, 0.110051107325383, 0.246882453151618, 0.924906303236797, -0.149914821124361, -0.225553662691652, -0.376149914821124, -0.699829642248722, 0.272572402044293, 0.228279386712095, 0.164633730834753, -0.0125383304940375, -0.0613287904599659, -0.00136286201022147 };

            CollectionAssert.AreEqual(expMatrix, inversedMatrix.ToArray(), new DoubleComparer());
        }

        /// <summary>
        /// 测试用例目的：验证 TransformMatrix 从欧拉角（Yaw/Pitch/Roll）创建矩阵的正确性。
        /// 以及从矩阵提取欧拉角的逆过程。
        /// 预期结果：提取的 Yaw=20°, Pitch=-15°, Roll=60°
        /// </summary>
        [Test]
        public void AngleTest()
        {
            // 创建具有特定欧拉角的旋转矩阵：Yaw=20°, Pitch=-15°, Roll=60°
            var matrix1 = TransformMatrix.CreateFromRotation(20 * Math.PI / 180, -15 * Math.PI / 180, 60 * Math.PI / 180);

            var yaw1 = matrix1.Yaw;
            var pitch1 = matrix1.Pitch;
            var roll1 = matrix1.Roll;

            // 验证提取的欧拉角与原始值匹配（极小误差容许）
            Assert.That(yaw1, Is.EqualTo(20 * Math.PI / 180).Within(0.00000000001).Percent);
            Assert.That(pitch1, Is.EqualTo(-15 * Math.PI / 180).Within(0.00000000001).Percent);
            Assert.That(roll1, Is.EqualTo(60 * Math.PI / 180).Within(0.00000000001).Percent);
        }

        /// <summary>
        /// DoubleComparer：用于比较两个 double 数组，允许 14 位小数精度差异。
        /// 这是因为浮点数运算存在舍入误差，直接比较往往失败。
        /// </summary>
        public class DoubleComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                var d1 = (double)x;
                var d2 = (double)y;

                // 四舍五入到 14 位小数后比较
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
