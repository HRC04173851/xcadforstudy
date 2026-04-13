//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.SolidWorks.Utils
{
    /// <summary>
    /// Utility to transform <see cref="TransformMatrix"/> and <see cref="IMathTransform"/>
    /// <para>中文：用于在 xCAD 变换矩阵与 SolidWorks 数学变换之间进行双向转换。</para>
    /// </summary>
    public static class TransformConverter
    {
        /// <summary>
        /// Transforms SOLIDWORKS matrix to xCAD matrix
        /// </summary>
        /// <param name="transform">Matrix to transform</param>
        /// <returns>Transformed matrix</returns>
        public static TransformMatrix ToTransformMatrix(this IMathTransform transform)
            => ToTransformMatrix(transform.ArrayData as double[]);

        /// <summary>
        /// Transforms data from SOLIDWORKS matrix <see cref="IMathTransform.ArrayData"/> to xCAD matrix
        /// </summary>
        /// <param name="data">Data tro transform</param>
        /// <returns>Transformed matrix</returns>
        public static TransformMatrix ToTransformMatrix(double[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length != 16)
            {
                throw new Exception("Array size must be 16 (4x4 matrix)");
            }

            // SolidWorks 变换数组第 13 位（索引 12）为统一缩放因子
            var scale = data[12];

            return new TransformMatrix(
                data[0] * scale, data[1] * scale, data[2] * scale, 0,
                data[3] * scale, data[4] * scale, data[5] * scale, 0,
                data[6] * scale, data[7] * scale, data[8] * scale, 0,
                data[9], data[10], data[11], 1);
        }


        /// <summary>
        /// Transforms xCAD matrix to SOLIDWORKS transform
        /// </summary>
        /// <param name="mathUtils">SOLIDWORKS math utility</param>
        /// <param name="matrix">Matrix to transform</param>
        /// <returns>SOLIDWORKS transform</returns>
        public static IMathTransform ToMathTransform(this IMathUtility mathUtils, TransformMatrix matrix)
            => mathUtils.CreateTransform(ToMathTransformData(matrix)) as IMathTransform;

        /// <summary>
        /// Transforms xCAD matrix to SOLIDWORKS transform data <see cref="IMathTransform.ArrayData"/>
        /// </summary>
        /// <param name="matrix">Matrix to transform</param>
        /// <returns>SOLIDWORKS transform data</returns>
        public static double[] ToMathTransformData(this TransformMatrix matrix)
        {
            var transX = matrix.M41;
            var transY = matrix.M42;
            var transZ = matrix.M43;

            var scaleVec = matrix.Scale;

            var scaleX = scaleVec.X;
            var scaleY = scaleVec.X; //only uniform scale is supported in SOLIDWORKS
            var scaleZ = scaleVec.X; //only uniform scale is supported in SOLIDWORKS
            // 中文：SolidWorks 仅支持统一缩放，因此 Y/Z 缩放与 X 保持一致

            return new double[]
            {
                matrix.M11 / scaleX, matrix.M12 / scaleY, matrix.M13 / scaleZ,
                matrix.M21 / scaleX, matrix.M22 / scaleY, matrix.M23 / scaleZ,
                matrix.M31 / scaleX, matrix.M32 / scaleY, matrix.M33 / scaleZ,
                transX, transY, transZ,
                scaleX,
                0, 0, 0
            };
        }
    }
}