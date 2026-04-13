//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.IO;
using System.Linq;
using Xarial.XCad.Geometry.Surfaces;
using Xarial.XCad.SolidWorks.Annotations;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Features;
using Xarial.XCad.SolidWorks.Features.CustomFeature;
using Xarial.XCad.SolidWorks.Geometry;
using Xarial.XCad.SolidWorks.Geometry.Curves;
using Xarial.XCad.SolidWorks.Geometry.Surfaces;
using Xarial.XCad.SolidWorks.Sketch;
using Xarial.XCad.Utils.Reflection;

namespace Xarial.XCad.SolidWorks
{
    /// <summary>
    /// xCAD 对象工厂，负责根据 SolidWorks COM 调度对象的实际类型，
    /// 创建对应的 xCAD 封装对象（边、面、体、草图实体、特征、注释等）。
    /// </summary>
    internal static class SwObjectFactory 
    {
        /// <summary>
        /// 从 COM 调度对象创建指定类型的 xCAD 对象。
        /// 若目标类型是可选对象（<see cref="ISwSelObject"/>），则创建 SwSelObject，否则创建 SwObject。
        /// </summary>
        internal static TObj FromDispatch<TObj>(object disp, SwDocument doc, SwApplication app)
            where TObj : IXObject
        {
            if (typeof(ISwSelObject).IsAssignableFrom(typeof(TObj))) 
            {
                return (TObj)FromDispatch(disp, doc, app, d => new SwSelObject(disp, doc, app));
            }
            else
            {
                return (TObj)FromDispatch(disp, doc, app, d => new SwObject(disp, doc, app));
            }
        }

        /// <summary>
        /// 核心分发方法：根据 COM 调度对象的实际接口类型，创建对应的 xCAD 包装对象。
        /// 支持边（IEdge）、面（IFace2）、顶点（IVertex）、轮廓线（ISilhouetteEdge）、
        /// 草图（ISketch）、体（IBody2）、草图段（ISketchSegment）、草图区域（ISketchRegion）、
        /// 草图点（ISketchPoint）、草图图片（ISketchPicture）、尺寸标注（IDisplayDimension）、
        /// 注释（INote/IAnnotation）、图层（ILayer）、配置（IConfiguration）、
        /// 零部件（IComponent2）、工程图图纸（ISheet）、视图（IView）、
        /// 曲线（ICurve）、环（ILoop2）、曲面（ISurface）、模型视图（IModelView）、
        /// 草图块实例/定义、特征（IFeature）等各类 SolidWorks 对象。
        /// </summary>
        private static ISwObject FromDispatch(object disp, SwDocument doc, SwApplication app, Func<object, ISwObject> defaultHandler)
        {
            if (disp == null) 
            {
                throw new ArgumentException("Dispatch is null");
            }

            switch (disp)
            {
                case IEdge edge:
                    // 根据边的底层曲线类型，区分圆形边（SwCircularEdge）、直线边（SwLinearEdge）和通用边（SwEdge）
                    var edgeCurve = edge.IGetCurve();
                    if (edgeCurve.IsCircle())
                    {
                        return new SwCircularEdge(edge, doc, app);
                    }
                    else if (edgeCurve.IsLine())
                    {
                        return new SwLinearEdge(edge, doc, app);
                    }
                    else
                    {
                        return new SwEdge(edge, doc, app);
                    }

                case IFace2 face:
                    // 根据面的曲面类型，创建对应的专用面对象（平面/柱面/锥面/球面/环面/B样条面/混合面/偏移面/拉伸面/旋转面）
                    var faceSurf = face.IGetSurface();
                    var faceSurfIdentity = (swSurfaceTypes_e)faceSurf.Identity();
                    switch (faceSurfIdentity)
                    {
                        case swSurfaceTypes_e.PLANE_TYPE:
                            // 平面（如拉伸体的顶面/底面）
                            return new SwPlanarFace(face, doc, app);

                        case swSurfaceTypes_e.CYLINDER_TYPE:
                            // 柱面（如圆柱体的侧面）
                            return new SwCylindricalFace(face, doc, app);

                        case swSurfaceTypes_e.CONE_TYPE:
                            // 锥面（如锥体的侧面）
                            return new SwConicalFace(face, doc, app);

                        case swSurfaceTypes_e.SPHERE_TYPE:
                            // 球面
                            return new SwSphericalFace(face, doc, app);

                        case swSurfaceTypes_e.TORUS_TYPE:
                            // 环面（圆环体的侧面）
                            return new SwToroidalFace(face, doc, app);

                        case swSurfaceTypes_e.BSURF_TYPE:
                            // B样条曲面（NURBS曲面，自由曲面造型中的通用曲面）
                            return new SwBFace(face, doc, app);

                        case swSurfaceTypes_e.BLEND_TYPE:
                            // 混合/放样曲面（由多个截面曲线生成的过渡曲面）
                            return new SwBlendFace(face, doc, app);

                        case swSurfaceTypes_e.OFFSET_TYPE:
                            // 偏移曲面（与原始曲面保持等距的曲面）
                            return new SwOffsetFace(face, doc, app);

                        case swSurfaceTypes_e.EXTRU_TYPE:
                            // 拉伸曲面（沿指定方向拉伸轮廓线生成的曲面）
                            return new SwExtrudedFace(face, doc, app);

                        case swSurfaceTypes_e.SREV_TYPE:
                            // 旋转曲面（将轮廓线绕轴旋转生成的曲面）
                            return new SwRevolvedFace(face, doc, app);

                        default:
                            throw new NotSupportedException($"Not supported face type: {faceSurfIdentity}");
                    }

                case IVertex vertex:
                    // 几何顶点（三条或多条边的交汇点）
                    return new SwVertex(vertex, doc, app);

                case ISilhouetteEdge silhouetteEdge:
                    // 轮廓线（投影视图中显示的可见轮廓边，工程图中使用）
                    return new SwSilhouetteEdge(silhouetteEdge, doc, app);

                case ISketch sketch:
                    // 草图：区分 2D 草图和 3D 草图
                    if (sketch.Is3D())
                    {
                        // 3D 草图：在三维空间中绘制的草图
                        return new SwSketch3D(sketch, doc, app, true);
                    }
                    else
                    {
                        // 2D 草图：在平面上绘制的普通草图
                        return new SwSketch2D(sketch, doc, app, true);
                    }

                case IBody2 body:

                    var bodyType = (swBodyType_e)body.GetType();
                    var isTemp = body.IsTemporaryBody();

                    switch (bodyType)
                    {
                        case swBodyType_e.swSheetBody:
                            // 片体（只有曲面，没有封闭体积），包括平面片体和一般片体
                            if (body.GetFaceCount() == 1 && body.IGetFirstFace().IGetSurface().IsPlane())
                            {
                                // 单面且为平面的片体（平面片体）
                                if (!isTemp)
                                {
                                    return new SwPlanarSheetBody(body, doc, app);
                                }
                                else
                                {
                                    // 临时平面片体（用于几何计算，不保存到模型中）
                                    return new SwTempPlanarSheetBody(body, app);
                                }
                            }
                            else
                            {
                                // 一般曲面片体
                                if (!isTemp)
                                {
                                    return new SwSheetBody(body, doc, app);
                                }
                                else
                                {
                                    return new SwTempSheetBody(body, app);
                                }
                            }

                        case swBodyType_e.swSolidBody:
                            // 实体（封闭的有体积的几何体）
                            if (!isTemp)
                            {
                                return new SwSolidBody(body, doc, app);
                            }
                            else
                            {
                                // 临时实体（用于几何运算，不保存到模型中）
                                return new SwTempSolidBody(body, app);
                            }

                        case swBodyType_e.swWireBody:
                            // 线框体（只有边和顶点，没有面）
                            if (!isTemp)
                            {
                                return new SwWireBody(body, doc, app);
                            }
                            else
                            {
                                return new SwTempWireBody(body, app);
                            }

                        default:
                            throw new NotSupportedException();
                    }

                case ISketchSegment seg:
                    // 草图段：根据类型分别创建圆弧、椭圆、直线、抛物线、样条线、文字等草图实体
                    switch ((swSketchSegments_e)seg.GetType())
                    {
                        case swSketchSegments_e.swSketchARC:
                            var arc = (ISketchArc)seg;
                            const int CIRCLE = 1;
                            if (arc.IsCircle() == CIRCLE)
                            {
                                // 圆（封闭的圆弧）
                                return new SwSketchCircle(arc, doc, app, true);
                            }
                            else 
                            {
                                // 普通圆弧
                                return new SwSketchArc(arc, doc, app, true);
                            }
                        case swSketchSegments_e.swSketchELLIPSE:
                            // 椭圆草图实体
                            return new SwSketchEllipse((ISketchEllipse)seg, doc, app, true);
                        case swSketchSegments_e.swSketchLINE:
                            // 直线草图实体
                            return new SwSketchLine((ISketchLine)seg, doc, app, true);
                        case swSketchSegments_e.swSketchPARABOLA:
                            // 抛物线草图实体
                            return new SwSketchParabola((ISketchParabola)seg, doc, app, true);
                        case swSketchSegments_e.swSketchSPLINE:
                            // 样条曲线草图实体（用于自由曲线设计）
                            return new SwSketchSpline((ISketchSpline)seg, doc, app, true);
                        case swSketchSegments_e.swSketchTEXT:
                            // 草图文字（可作为特征截面轮廓）
                            return new SwSketchText((ISketchText)seg, doc, app, true);
                        default:
                            throw new NotSupportedException();
                    }

                case ISketchRegion skReg:
                    // 草图区域（封闭轮廓围成的区域，可用于生成特征）
                    return new SwSketchRegion(skReg, doc, app);

                case ISketchPoint skPt:
                    // 草图点（用于约束或参考）
                    return new SwSketchPoint(skPt, doc, app, true);

                case ISketchPicture skPict:
                    // 草图图片（嵌入草图中的参考图片）
                    return new SwSketchPicture(skPict, doc, app, true);

                case IDisplayDimension dispDim:
                    // 显示尺寸（工程图或模型中的尺寸标注）
                    return new SwDimension(dispDim, doc, app);

                case INote note:
                    // 注释文字（工程图中的文本注解）
                    return new SwNote(note, doc, app);

                case IDrSection section:
                    // 剖切线（工程图剖视图的剖切符号）
                    return new SwSectionLine(section, doc, app);

                case IDetailCircle detailCircle:
                    // 详图圆（工程图局部放大视图的圆形边界）
                    return new SwDetailCircle(detailCircle, doc, app);

                case ITableAnnotation tableAnn:
                    // 表格注释（BOM 表、孔表等工程图表格）
                    return new SwTable(tableAnn, doc, app);

                case IAnnotation ann:
                    // 通用注释对象，根据具体注释类型进一步区分
                    switch ((swAnnotationType_e)ann.GetType())
                    {
                        case swAnnotationType_e.swDisplayDimension:
                            return new SwDimension((IDisplayDimension)ann.GetSpecificAnnotation(), doc, app);
                        case swAnnotationType_e.swNote:
                            return new SwNote((INote)ann.GetSpecificAnnotation(), doc, app);
                        case swAnnotationType_e.swTableAnnotation:
                            return new SwTable((ITableAnnotation)ann.GetSpecificAnnotation(), doc, app);
                        default:
                            return new SwAnnotation(ann, doc, app);
                    }

                case ILayer layer:
                    // 图层（用于组织和管理工程图中的对象显示）
                    return new SwLayer(layer, doc, app);

                case IConfiguration conf:
                    // 配置（零件或装配体的不同设计状态，如不同尺寸或零部件组合）
                    switch (doc)
                    {
                        case SwAssembly assm:
                            // 装配体配置
                            return new SwAssemblyConfiguration(conf, assm, app, true);

                        case SwPart part:
                            // 零件配置
                            return new SwPartConfiguration(conf, part, app, true);

                        default:
                            throw new Exception("Owner document must be 3D document or assembly");
                    }

                case IComponent2 comp:
                    // 零部件（装配体中的子零件或子装配体）
                    var compRefModel = comp.GetModelDoc2();

                    if (compRefModel != null)
                    {
                        switch (compRefModel)
                        {
                            case IPartDoc _:
                                // 零件类型的零部件
                                return new SwPartComponent(comp, (SwAssembly)doc, app);

                            case IAssemblyDoc _:
                                // 装配体类型的零部件（子装配体）
                                return new SwAssemblyComponent(comp, (SwAssembly)doc, app);

                            default:
                                throw new NotSupportedException($"Unrecognized component type of '{comp.Name2}'");
                        }
                    }
                    else
                    {
                        // 无法获取模型文档时，根据文件扩展名判断类型
                        var compFilePath = comp.GetPathName();
                        var ext = Path.GetExtension(compFilePath);

                        switch (ext.ToLower())
                        {
                            case ".sldprt":
                                return new SwPartComponent(comp, (SwAssembly)doc, app);
                            case ".sldasm":
                                return new SwAssemblyComponent(comp, (SwAssembly)doc, app);
                            default:
                                throw new NotSupportedException($"Component '{comp.Name2}' file '{compFilePath}' is not recognized");
                        }
                    }

                case ISheet sheet:
                    // 工程图图纸（工程图文档中的一张图纸）
                    return new SwSheet(sheet, (SwDrawing)doc, app);

                case IView view:
                    // 工程图视图：区分展开图视图和普通视图
                    if (view.IsFlatPatternView())
                    {
                        // 钣金展开图视图
                        return new SwFlatPatternDrawingView(view, (SwDrawing)doc);
                    }
                    else
                    {
                        // 根据视图类型创建对应的视图对象
                        switch ((swDrawingViewTypes_e)view.Type)
                        {
                            case swDrawingViewTypes_e.swDrawingProjectedView:
                                // 投影视图（正交投影）
                                return new SwProjectedDrawingView(view, (SwDrawing)doc);
                            case swDrawingViewTypes_e.swDrawingNamedView:
                                // 命名视图（基于模型方向的视图）
                                return new SwModelBasedDrawingView(view, (SwDrawing)doc);
                            case swDrawingViewTypes_e.swDrawingAuxiliaryView:
                                // 辅助视图（沿斜面法向投影的视图）
                                return new SwAuxiliaryDrawingView(view, (SwDrawing)doc);
                            case swDrawingViewTypes_e.swDrawingSectionView:
                                // 剖视图（用剖切面切割后的视图）
                                return new SwSectionDrawingView(view, (SwDrawing)doc);
                            case swDrawingViewTypes_e.swDrawingDetailView:
                                // 局部放大视图（对某一区域放大显示）
                                return new SwDetailDrawingView(view, (SwDrawing)doc);
                            case swDrawingViewTypes_e.swDrawingRelativeView:
                                // 相对视图（由两个正交面定义的视图方向）
                                return new SwRelativeView(view, (SwDrawing)doc);
                            default:
                                return new SwDrawingView(view, (SwDrawing)doc);
                        }
                    }

                case ICurve curve:
                    // 曲线：根据曲线类型创建对应的封装对象
                    switch ((swCurveTypes_e)curve.Identity())
                    {
                        case swCurveTypes_e.LINE_TYPE:
                            // 直线
                            return new SwLineCurve(curve, doc, app, true);
                        case swCurveTypes_e.CIRCLE_TYPE:
                            // 圆或圆弧（根据是否封闭区分）
                            curve.GetEndParams(out _, out _, out bool isClosed, out _);
                            if (isClosed)
                            {
                                // 圆（封闭曲线）
                                return new SwCircleCurve(curve, doc, app, true);
                            }
                            else 
                            {
                                // 圆弧（非封闭）
                                return new SwArcCurve(curve, doc, app, true);
                            }
                        case swCurveTypes_e.BCURVE_TYPE:
                            // B样条曲线（NURBS曲线，自由曲线造型的基础）
                            return new SwBCurve(curve, doc, app, true);
                        case swCurveTypes_e.ELLIPSE_TYPE:
                            // 椭圆曲线
                            return new SwEllipseCurve(curve, doc, app, true);
                        case swCurveTypes_e.SPCURVE_TYPE:
                            // 样条曲线
                            return new SwSplineCurve(curve, doc, app, true);
                        default:
                            return new SwCurve(curve, doc, app, true);
                    }

                case ILoop2 loop:
                    // 环（面的边界环，由一组有序边组成，分内环和外环）
                    return new SwLoop(loop, doc, app);

                case ISurface surf:
                    // 曲面：根据曲面类型创建对应的封装对象（独立曲面，与面 IFace2 不同）
                    var surfIdentity = (swSurfaceTypes_e)surf.Identity();
                    switch (surfIdentity)
                    {
                        case swSurfaceTypes_e.PLANE_TYPE:
                            // 平面曲面
                            return new SwPlanarSurface(surf, doc, app);

                        case swSurfaceTypes_e.CYLINDER_TYPE:
                            // 柱面
                            return new SwCylindricalSurface(surf, doc, app);

                        case swSurfaceTypes_e.CONE_TYPE:
                            // 锥面
                            return new SwConicalSurface(surf, doc, app);

                        case swSurfaceTypes_e.SPHERE_TYPE:
                            // 球面
                            return new SwSphericalSurface(surf, doc, app);

                        case swSurfaceTypes_e.TORUS_TYPE:
                            // 环面
                            return new SwToroidalSurface(surf, doc, app);

                        case swSurfaceTypes_e.BSURF_TYPE:
                            // B样条曲面（NURBS自由曲面）
                            return new SwBSurface(surf, doc, app);

                        case swSurfaceTypes_e.BLEND_TYPE:
                            // 混合/放样曲面
                            return new SwBlendXSurface(surf, doc, app);

                        case swSurfaceTypes_e.OFFSET_TYPE:
                            // 偏移曲面
                            return new SwOffsetSurface(surf, doc, app);

                        case swSurfaceTypes_e.EXTRU_TYPE:
                            // 拉伸曲面
                            return new SwExtrudedSurface(surf, doc, app);

                        case swSurfaceTypes_e.SREV_TYPE:
                            // 旋转曲面
                            return new SwRevolvedSurface(surf, doc, app);

                        default:
                            throw new NotSupportedException($"Not supported surface type: {surfIdentity}");
                    }

                case IModelView modelView:
                    // 模型视图（三维模型中的视角/视图方向）
                    return new SwModelView(modelView, doc, app);

                case ISketchBlockInstance skBlockInst:
                    // 草图块实例（草图中插入的块的具体实例）
                    return new SwSketchBlockInstance((IFeature)skBlockInst, doc, app, true);

                case ISketchBlockDefinition skBlockDef:
                    // 草图块定义（可重复使用的草图块模板定义）
                    return new SwSketchBlockDefinition((IFeature)skBlockDef, doc, app, true);

                case IFeature feat:
                    // 特征：根据特征类型字符串创建对应的专用特征对象
                    switch (feat.GetTypeName())
                    {
                        case SwSketch2D.TypeName:
                            // 2D 草图特征
                            return new SwSketch2D(feat, doc, app, true);
                        case SwSketch3D.TypeName:
                            // 3D 草图特征
                            return new SwSketch3D(feat, doc, app, true);
                        case "CutListFolder":
                            // 切割清单项（钣金或焊接件的材料清单）
                            return new SwCutListItem(feat, (SwDocument3D)doc, app, true);
                        case "CoordSys":
                            // 坐标系特征（定义局部坐标系，用于装配约束或测量基准）
                            return new SwCoordinateSystem(feat, doc, app, true);
                        case SwOrigin.TypeName:
                            // 原点特征（文档坐标系原点）
                            return new SwOrigin(feat, doc, app, true);
                        case SwPlane.TypeName:
                            // 基准面特征（参考平面，用于草图绘制或镜像等操作）
                            return new SwPlane(feat, doc, app, true);
                        case SwFlatPattern.TypeName:
                            // 展开图特征（钣金件的展平状态）
                            return new SwFlatPattern(feat, doc, app, true);
                        case "SketchBlockInst":
                            // 草图块实例特征
                            return new SwSketchBlockInstance(feat, doc, app, true);
                        case "SketchBlockDef":
                            // 草图块定义特征
                            return new SwSketchBlockDefinition(feat, doc, app, true);
                        case "SketchBitmap":
                            // 草图图片（嵌入草图的参考图片特征）
                            return new SwSketchPicture(feat, doc, app, true);
                        case "BaseBody":
                            // 哑实体（不由参数化特征驱动的直接实体）
                            return new SwDumbBody(feat, doc, app, true);
                        case "WeldMemberFeat":
                            // 焊接结构件成员（焊接框架中的型材/结构件）
                            return new SwStructuralMember(feat, doc, app, true);
                        case "MacroFeature":
                            // 宏特征（用户自定义的参数化特征，由 SwMacroFeatureDefinition 派生类实现）
                            if (TryGetParameterType(feat, out Type paramType))
                            {
                                return SwMacroFeature<object>.CreateSpecificInstance(feat, doc, app, paramType);
                            }
                            else
                            {
                                return new SwMacroFeature(feat, doc, app, true);
                            }
                        default:
                            // 其他通用特征
                            return new SwFeature(feat, doc, app, true);
                    }

                default:
                    // 对于未识别的调度类型，调用默认处理器
                    return defaultHandler.Invoke(disp);
            }
        }

        /// <summary>
        /// 尝试从宏特征对象获取其参数类型。
        /// 通过读取宏特征数据的 ProgId，查找对应的 .NET 类型，并获取其泛型参数（参数类定义）。
        /// </summary>
        /// <param name="feat">SolidWorks 特征对象</param>
        /// <param name="paramType">成功时输出参数类型；失败时为 null</param>
        /// <returns>是否成功获取参数类型</returns>
        private static bool TryGetParameterType(IFeature feat, out Type paramType)
        {
            var featData = feat.GetDefinition() as IMacroFeatureData;

            //NOTE: definition of rolled back macro feature is null
            // 注意：处于回滚状态（压缩）的宏特征，其定义可能为 null
            var progId = featData?.GetProgId();

            if (!string.IsNullOrEmpty(progId))
            {
                // 根据 ProgId 查找对应的 .NET 类型（COM 注册的类型）
                var type = Type.GetTypeFromProgID(progId);

                if (type != null)
                {
                    // 检查该类型是否派生自 SwMacroFeatureDefinition<TParams>，并获取泛型参数（参数类型）
                    if (type.IsAssignableToGenericType(typeof(SwMacroFeatureDefinition<>)))
                    {
                        paramType = type.GetArgumentsOfGenericType(typeof(SwMacroFeatureDefinition<>)).First();
                        return true;
                    }
                }
            }

            paramType = null;
            return false;
        }
    }
}