//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Base;

namespace Xarial.XCad.Geometry.Evaluation
{
    /// <summary>
    /// Represents the measure utility
    /// 表示测量评估工具
    /// </summary>
    public interface IXMeasure : IEvaluation
    {
        /// <summary>
        /// Angle
        /// 角度
        /// </summary>
        double Angle { get; }
        
        /// <summary>
        /// Area
        /// 面积
        /// </summary>
        double Area { get; }
        
        /// <summary>
        /// Diameter
        /// 直径
        /// </summary>
        double Diameter { get; }
        
        /// <summary>
        /// Distance
        /// 距离
        /// </summary>
        double Distance { get; }
        
        /// <summary>
        /// Length
        /// 长度
        /// </summary>
        double Length { get; }

        /// <summary>
        /// Perimeter
        /// 周长
        /// </summary>
        double Perimeter { get; }

        /// <summary>
        /// Scope of entities to measure
        /// 参与测量的实体范围
        /// </summary>
        /// <remarks>Specify null to measure selected entities</remarks>
        new IXEntity[] Scope { get; set; }
    }
}
