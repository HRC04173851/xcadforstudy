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

namespace Xarial.XCad
{
    /// <summary>
    /// Represents the material
    /// 表示材料
    /// </summary>
    public interface IXMaterial : IXTransaction
    {
        /// <summary>
        /// Material database
        /// 材料数据库
        /// </summary>
        IXMaterialsDatabase Database { get; }

        /// <summary>
        /// Category of the material
        /// 材料类别
        /// </summary>
        string Category { get; }

        /// <summary>
        /// Elastic modulus
        /// 弹性模量（杨氏模量）
        /// </summary>
        double ElasticModulus { get; }

        /// <summary>
        /// Poisson's ratio
        /// 泊松比
        /// </summary>
        double PoissonRatio { get; }

        /// <summary>
        /// Shear modulus
        /// 剪切模量
        /// </summary>
        double ShearModulus { get; }

        /// <summary>
        /// Thermal expansion coefficient
        /// 热膨胀系数
        /// </summary>
        double ThermalExpansionCoefficient { get; }

        /// <summary>
        /// Mass density
        /// 质量密度
        /// </summary>
        double MassDensity { get; }

        /// <summary>
        /// Thermal conductivity
        /// 热导率
        /// </summary>
        double ThermalConductivity { get; }

        /// <summary>
        /// Specific heat
        /// 比热容（比熔）
        /// </summary>
        double SpecificHeat { get; }

        /// <summary>
        /// Tensile strength
        /// 抗张强度
        /// </summary>
        double TensileStrength { get; }

        /// <summary>
        /// Yield strength
        /// 屈服强度
        /// </summary>
        double YieldStrength { get; }

        /// <summary>
        /// Hardening factor (0.0-1.0; 0.0=isotropic; 1.0=kinematic)
        /// 硬化因子 (0.0-1.0; 0.0=各向同性; 1.0=随动硬化)
        /// </summary>
        double HardeningFactor { get; }

        /// <summary>
        /// Name of the material
        /// </summary>
        string Name { get; }

        //TODO: Add support for custom properties
    }
}
