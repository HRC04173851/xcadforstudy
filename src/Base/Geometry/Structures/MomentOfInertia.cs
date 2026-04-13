//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Xarial.XCad.Geometry.Evaluation;

namespace Xarial.XCad.Geometry.Structures
{
    /// <summary>
    /// Moment of intertia used in <see cref="IXMassProperty"/>
    /// 用于 <see cref="IXMassProperty"/> 的惯性矩张量表示
    /// </summary>
    public class MomentOfInertia
    {
        /// <summary>
        /// X moment of intertia (mass * square lemgth )
        /// 绕 X 方向的惯性矩分量（质量 × 长度平方）
        /// </summary>
        public Vector Lx { get; }

        /// <summary>
        /// Y moment of intertia (mass * square lemgth )
        /// 绕 Y 方向的惯性矩分量（质量 × 长度平方）
        /// </summary>
        public Vector Ly { get; }

        /// <summary>
        /// Z moment of intertia (mass * square lemgth )
        /// 绕 Z 方向的惯性矩分量（质量 × 长度平方）
        /// </summary>
        public Vector Lz { get; }

        /// <summary>
        /// Default construcotor
        /// </summary>
        public MomentOfInertia(Vector lx, Vector ly, Vector lz)
        {
            Lx = lx;
            Ly = ly;
            Lz = lz;
        }
    }
}
