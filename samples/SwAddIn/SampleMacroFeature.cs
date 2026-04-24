// -*- coding: utf-8 -*- // samples/SwAddIn/SampleMacroFeature.cs
//*********************************************************************
//xCAD
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xenal.com/license/
//*********************************************************************
//说明：
//本文件演示了如何在 xCAD 框架中创建 SOLIDWORKS 宏特征（Macro Feature）。
//宏特征是一种自定义特征，允许开发者通过代码定义复杂的几何形状和行为。
//SampleMacroFeature 展示了如何：
//1. 定义宏特征的基本结构（继承 SwMacroFeatureDefinition）
//2. 实现 OnRebuild 方法来动态生成几何体
//3. 配置参数编辑器（通过 AlignDimensionDelegate）
//4. 使用 MemoryGeometryBuilder 创建临时几何体（圆弧、线条、扫掠体）

using System.Runtime.InteropServices;
using Xarial.XCad;
using SwAddInExample.Properties;
using Xarial.XCad.Base.Attributes;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Features.CustomFeature.Structures;
using Xarial.XCad.Features.CustomFeature;
using Xarial.XCad.Features.CustomFeature.Delegates;
using Xarial.XCad.Geometry;
using Xarial.XCad.SolidWorks.Features.CustomFeature;
using Xarial.XCad.SolidWorks;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.Features.CustomFeature.Attributes;
using System.Linq;
using Xarial.XCad.SolidWorks.Geometry.Primitives;
using Xarial.XCad.Geometry.Primitives;
using Xarial.XCad.SolidWorks.Geometry;
using Xarial.XCad.Base;

namespace SwAddInExample
{
    /// <summary>
    /// SimpleMacroFeature 是宏特征的最小化实现示例。
    /// // 中文：展示最基础的宏特征定义，不处理任何参数或几何重建逻辑。
    /// 当模型重建时，直接调用基类实现，不做任何自定义处理。
    /// </summary>
    /// <remarks>
    /// 此类的目的仅作为学习起点，实际应用中通常需要像 SampleMacroFeature 那样
    /// 实现完整的三维几何体生成和参数关联逻辑。
    /// </remarks>
    [ComVisible(true)]
    [MissingDefinitionErrorMessage("xCAD. Download the add-in")]
    public class SimpleMacroFeature : SwMacroFeatureDefinition
    {
        /// <summary>
        /// 宏特征重建回调方法。
        /// // 中文：当特征需要重建时被 SOLIDWORKS 调用。
        /// 此处直接调用基类实现，不执行任何自定义几何生成逻辑。
        /// </summary>
        /// <param name="app">SOLIDWORKS 应用程序实例（ISwApplication）</param>
        /// <param name="model">包含此特征的文档对象（ISwDocument）</param>
        /// <param name="feature">宏特征本身（ISwMacroFeature）</param>
        /// <returns>标准的重建结果，表示操作成功</returns>
        public override CustomFeatureRebuildResult OnRebuild(ISwApplication app, ISwDocument model, ISwMacroFeature feature)
        {
            return base.OnRebuild(app, model, feature);
        }
    }

    /// <summary>
    /// SampleMacroFeature 展示了如何创建一个功能完整的宏特征。
    /// // 中文：此宏特征接受两个可编辑参数（Number 和 Angle），并在重建时动态生成扫掠体几何。
    /// 它演示了 xCAD 框架中宏特征的核心概念：
    /// - 参数定义：通过 PmpMacroFeatData 类定义用户可编辑的参数
    /// - 几何生成：在 OnRebuild 中使用 MemoryGeometryBuilder 创建三维几何
    /// - 尺寸对齐：通过 AlignDimensionDelegate 将参数与特征尺寸关联
    /// </summary>
    /// <remarks>
    /// 工作流程：用户修改参数值 → SOLIDWORKS 调用 OnRebuild → 创建新的几何体替换旧几何
    /// </remarks>
    [ComVisible(true)]
    [Icon(typeof(Resources), nameof(Resources.xarial))]
    [MissingDefinitionErrorMessage("xCAD. Download the add-in")]
    public class SampleMacroFeature : SwMacroFeatureDefinition<PmpMacroFeatData>
    {
        /// <summary>
        /// 核心重建方法 - 当宏特征需要更新时被调用。
        /// // 中文：此方法负责根据当前参数值生成新的几何体。
        /// 它执行以下操作：
        /// 1. 获取当前参数值
        /// 2. 配置尺寸对齐委托（用于在 PropertyManager 中显示尺寸标注）
        /// 3. 创建临时几何体（圆弧、线条、扫掠体）
        /// 4. 修改参数值（演示参数读写）
        /// 5. 返回生成的几何体
        /// </summary>
        /// <param name="app">SOLIDWORKS 应用程序实例，提供 MemoryGeometryBuilder 等服务</param>
        /// <param name="model">包含此宏特征的零件或装配体文档</param>
        /// <param name="feature">宏特征对象，包含参数和特征数据</param>
        /// <param name="alignDim">输出参数：尺寸对齐委托，用于在参数编辑时提供视觉反馈</param>
        /// <returns>包含生成几何体的重建结果</returns>
        public override CustomFeatureRebuildResult OnRebuild(ISwApplication app, ISwDocument model, ISwMacroFeature<PmpMacroFeatData> feature,
            out AlignDimensionDelegate<PmpMacroFeatData> alignDim)
        {
            //中文：获取宏特征的当前参数值
            var parameters = feature.Parameters;

            //中文：定义尺寸对齐回调 - 当用户在 PropertyManager 中编辑参数时提供视觉引导
            // AlignDimensionDelegate 决定了尺寸标注在图形区域中的显示位置和方向
            alignDim = (n, d) =>
            {
                switch (n)
                {
                    //中文：Number 参数（线性尺寸） - 对齐到原点，沿 Y 轴方向
                    case nameof(PmpMacroFeatData.Number):
                        // AlignLinearDimension: 创建线性尺寸标注，起点为原点，方向为 Y 轴正方向
                        this.AlignLinearDimension(d, new Point(0, 0, 0), new Vector(0, 1, 0));
                        break;

                    //中文：Angle 参数（角度尺寸） - 沿 X 轴负方向作为角度参考线
                    case nameof(PmpMacroFeatData.Angle):
                        // AlignAngularDimension: 创建角度尺寸标注，顶点在原点，参考线沿 X 轴负方向
                        this.AlignAngularDimension(d, new Point(0, 0, 0), new Point(-0.1, 0, 0), new Vector(0, 1, 0));
                        break;
                }
            };

            //中文：步骤 1 - 创建用于扫掠的圆弧轮廓（作为截面）
            // MemoryGeometryBuilder.WireBuilder 用于创建线框几何（边、曲线）
            var sweepArc = app.MemoryGeometryBuilder.WireBuilder.PreCreateCircle();
            //中文：定义圆弧几何 - 以 Z 轴为旋转轴，原点为圆心，半径 0.01 单位
            // Circle 由旋转轴（Axis）和半径定义：Axis 包含圆心和方向向量
            sweepArc.Geometry = new Circle(new Axis(new Point(0, 0, 0), new Vector(0, 0, 1)), 0.01);
            //中文：Commit 将创建的线框几何提交到内存几何构建器，使其可被后续操作使用
            sweepArc.Commit();

            //中文：步骤 2 - 创建扫掠路径（直线）
            var sweepLine = app.MemoryGeometryBuilder.WireBuilder.PreCreateLine();
            //中文：定义直线几何 - 从原点到点 (1,1,1) 的三维直线
            sweepLine.Geometry = new Line(new Point(0, 0, 0), new Point(1, 1, 1));
            sweepLine.Commit();

            //中文：步骤 3 - 创建扫掠特征（Sweep）
            // MemoryGeometryBuilder.SolidBuilder 用于创建实体几何（拉伸、旋转、扫掠等）
            var sweep = (ISwTempSweep)app.MemoryGeometryBuilder.SolidBuilder.PreCreateSweep();

            //中文：设置扫掠的截面轮廓 - 将圆弧转换为平面片（Planar Sheet）
            // CreatePlanarSheet: 从闭合的线框轮廓创建平面片
            // CreateRegionFromSegments: 将曲线段转换为区域（Region），用于生成实体
            // .Bodies.OfType<ISwTempPlanarSheetBody>().First(): 提取平面片中的实体几何
            sweep.Profiles = new ISwTempRegion[] { app.MemoryGeometryBuilder.CreatePlanarSheet(
                app.MemoryGeometryBuilder.CreateRegionFromSegments(sweepArc)).Bodies.OfType<ISwTempPlanarSheetBody>().First() };

            //中文：设置扫掠的路径 - 刚才创建的直线作为导引路径
            sweep.Path = sweepLine;
            //中文：提交扫掠特征，此时生成完整的三维几何体
            sweep.Commit();

            //中文：演示参数修改 - 将 Number 参数值加 1
            // 这展示了参数的双向绑定：用户界面修改参数会传递到此，代码也可在此修改参数
            parameters.Number = parameters.Number + 1;

            //中文：返回重建结果，包含生成的几何体
            // CustomFeatureBodyRebuildResult 用于传递生成的几何体给 SOLIDWORKS
            return new CustomFeatureBodyRebuildResult() { Bodies = sweep.Bodies };
        }
    }
}
