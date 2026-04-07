//*********************************************************************
//xCAD
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad;
using Xarial.XCad.Annotations;
using Xarial.XCad.Base;
using Xarial.XCad.Base.Enums;
using Xarial.XCad.Documents;
using Xarial.XCad.Documents.Extensions;
using Xarial.XCad.Documents.Structures;
using Xarial.XCad.Enums;
using Xarial.XCad.Features;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Curves;
using Xarial.XCad.Geometry.Primitives;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Wires;
using Xarial.XCad.Sketch;
using Xarial.XCad.SolidWorks;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Features;
using Xarial.XCad.SolidWorks.Geometry;
using Xarial.XCad.SolidWorks.Geometry.Curves;
using Xarial.XCad.SwDocumentManager;
using Xarial.XCad.Toolkit;
using Xarial.XCad.Toolkit.Utils;

namespace StandAlone
{
    // Custom logger implementation for the xCAD framework
    // 中文：xCAD 框架的自定义日志记录器实现
    // Register this in CustomServices to replace the default xCAD logger behavior
    // 中文：在 CustomServices 中注册此类以替换 xCAD 框架的默认日志行为
    public class MyLogger : IXLogger
    {
        // Log method intentionally left empty in this sample; in production, write to a file or console
        // 中文：此示例中 Log 方法故意留空；在生产环境中应将消息写入文件或控制台
        public void Log(string msg, LoggerMessageSeverity_e severity = LoggerMessageSeverity_e.Information)
        {
        }
    }

    class Program
    {
        // Entry point for the standalone console application that connects to SolidWorks
        // 中文：连接到 SolidWorks 的独立控制台应用程序入口点
        static void Main(string[] args)
        {
            try
            {
                // Launch a new SolidWorks instance in Default state and connect to it via the xCAD API
                // 中文：以默认状态启动新的 SolidWorks 实例，并通过 xCAD API 连接
                var app = SwApplicationFactory.Create(null, ApplicationState_e.Default);
                app.ShowMessageBox("Hello World!");

                // Alternative: connect to an already-running SolidWorks process by process name
                // 中文：备选方案：通过进程名称连接到已在运行的 SolidWorks 进程
                //var app = SwApplicationFactory.FromProcess(Process.GetProcessesByName("SLDWORKS").First());

                // Alternative: use the SolidWorks Document Manager (DM) API with a license key
                // 中文：备选方案：使用 SolidWorks 文档管理器（DM）API 和许可证密钥连接
                //var dmApp = SwDmApplicationFactory.Create(
                //    System.Environment.GetEnvironmentVariable("SW_DM_KEY", EnvironmentVariableTarget.Machine));

                // Uncomment one of the sample methods below to run a specific demo:
                // 中文：取消注释以下任一示例方法以运行对应的演示功能

                //RenameFiles(app);

                //ParseViewPolylines(app);

                //StructuralMembersTest(app);

                //AnnotationColor(app);

                //CreateLoftFromSelection(app);

                //CustomServices();

                //Progress(app);

                //SketchSegmentColors(app);

                //CreateDrawingView(app);

                // Alternative: create a SolidWorks COM object directly and wrap it with xCAD API
                // 中文：备选方案：直接创建 SolidWorks COM 对象并用 xCAD API 包装
                //var sw = Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application")) as ISldWorks;
                //sw.Visible = true;

                //var app = SwApplication.FromPointer(sw);

                //CreateSketchEntities(app);

                //TraverseSelectedFaces(app);

                //CreateSweepFromSelection(app);

                // Currently active: creates several types of temporary in-memory geometry and adds to the active part
                // 中文：当前激活的方法：创建多种类型的临时（内存中的）几何体并将其添加到活动零件
                CreateTempGeometry(app);

                //CreateSweepFromSelection(app);
            }
            catch 
            {
            }

            // Wait for user to press Enter before closing the console window
            // 中文：等待用户按 Enter 键后再关闭控制台窗口
            Console.ReadLine();
        }

        // Demonstrates renaming all dependency files of an assembly by prepending "_" to each file name
        // 中文：演示如何通过在每个文件名前添加"_"前缀来重命名装配体的所有依赖文件
        private static void RenameFiles(IXApplication app)
        {
            // Local recursive function: ensures a document and all its nested dependencies are committed (loaded)
            // 中文：本地递归函数：确保文档及其所有嵌套依赖项均已提交（完全加载到内存）
            void CommitAllDependencies(IXDocument doc) 
            {
                // Commit loads the document into the xCAD model if it has not been loaded yet
                // 中文：若文档尚未加载，提交操作会将其载入 xCAD 模型
                if (!doc.IsCommitted) 
                {
                    doc.Commit();
                }

                // Recursively commit all child dependencies of this document
                // 中文：递归提交此文档的所有子依赖项
                foreach (var dep in doc.Dependencies) 
                {
                    CommitAllDependencies(dep);
                }
            }

            // Pre-create an assembly reference without opening SolidWorks; set the file path
            // 中文：在不打开 SolidWorks 的情况下预创建装配体引用；设置文件路径
            var assm = app.Documents.PreCreateAssembly();
            assm.Path = @"D:\Assem1\Assem1.SLDASM";

            // The Document Manager (DM) API requires explicit commit to resolve the dependency tree
            // 中文：文档管理器（DM）API 需要显式提交才能解析完整的依赖树
            if (app is ISwDmApplication)
            {
                CommitAllDependencies(assm);
            }

            // Replace all dependency file paths by prepending "_" to each file name; copies the file if needed
            // 中文：通过在每个文件名前添加"_"来替换所有依赖项的文件路径；如目标不存在则复制原文件
            assm.Dependencies.ReplaceAll(x => 
            {
                var newPath = Path.Combine(Path.GetDirectoryName(x), "_" + Path.GetFileName(x));

                // Copy the original file to the new "_" prefixed path only if it does not already exist
                // 中文：仅当带"_"前缀的目标文件不存在时，才将原文件复制到新路径
                if (!File.Exists(newPath)) 
                {
                    File.Copy(x, newPath);
                }

                return newPath;
            });

            // For DM application: save and close all dependencies and the assembly after renaming
            // 中文：对于文档管理器应用：重命名后保存并关闭所有依赖项及装配体
            if (app is ISwDmApplication)
            {
                foreach (var dep in assm.Dependencies.TryIterateAll()) 
                {
                    dep.Save();
                    dep.Close();
                }

                assm.Save();
                assm.Close();
            }
        }

        // Reads polyline data from a selected drawing view and recreates the outlines as 3D sketch lines
        // and also as lines drawn directly on the active drawing sheet
        // 中文：从选定的工程图视图中读取折线数据，并将轮廓线重新创建为三维草图线段
        // 中文：同时也在活动工程图图纸上直接绘制对应的线段
        private static void ParseViewPolylines(ISwApplication app) 
        {
            // Get the currently active drawing document
            // 中文：获取当前活动的工程图文档
            var drw = (ISwDrawing)app.Documents.Active;

            // Get the first selected drawing view from the current selection
            // 中文：从当前选择中获取第一个选定的工程图视图
            var view = drw.Selections.OfType<ISwDrawingView>().First();

            // Alternative: get the first view from the active drawing sheet
            // 中文：备选方案：从活动图纸中获取第一个工程图视图
            //var view = (ISwDrawingView)drw.Sheets.Active.DrawingViews.First();

            // Retrieve polyline data from the drawing view (edge silhouettes represented as polylines)
            // 中文：从工程图视图中获取折线数据（边线轮廓以折线形式表示）
            var polylinesData = view.Polylines;

            //select
            // Select all polyline entities in the drawing view for visual verification
            // 中文：在工程图视图中选择所有折线实体以便可视化验证
            foreach (var polylineData in polylinesData)
            {
                view.DrawingView.SelectEntity(((ISwEntity)polylineData.Entity).Entity, true);
            }
            //

            //create in 3D view
            // Recreate polylines as 3D sketch line segments in the referenced 3D document (part or assembly)
            // 中文：在引用的三维文档（零件或装配体）中将折线重新创建为三维草图线段
            var sketch = view.ReferencedDocument.Features.PreCreate3DSketch();

            var ents = new List<IXWireEntity>();

            // Convert each polyline into a sequence of connected sketch lines between adjacent points
            // 中文：将每条折线转换为相邻点之间相互连接的草图线段序列
            foreach (var polylineData in polylinesData)
            {
                for (int i = 0; i < polylineData.Points.Length - 1; i++)
                {
                    var line = sketch.Entities.PreCreateLine();
                    line.Geometry = new Line(polylineData.Points[i], polylineData.Points[i + 1]);
                    ents.Add(line);
                }
            }

            // Add all line entities to the 3D sketch and commit to create the feature in the FeatureManager
            // 中文：将所有线段实体添加到三维草图并提交，在特征管理器中创建该草图特征
            sketch.Entities.AddRange(ents);

            sketch.Commit();
            //

            //create in sheet
            // Also recreate the polylines as lines on the active drawing sheet's sketch
            // 中文：同样在活动工程图图纸草图中将折线重新创建为线段
            var sheetSketch = drw.Sheets.Active.Sketch;

            // Edit the sheet sketch in a using block to ensure the sketch editor is properly closed afterward
            // 中文：在 using 块内编辑图纸草图，确保编辑器在操作完成后正确关闭
            using (var editor = sheetSketch.Edit())
            {
                var sheetEnts = new List<IXWireEntity>();

                foreach (var polylineData in polylinesData)
                {
                    for (int i = 0; i < polylineData.Points.Length - 1; i++)
                    {
                        var line = sheetSketch.Entities.PreCreateLine();
                        line.Geometry = new Line(polylineData.Points[i], polylineData.Points[i + 1]);
                        sheetEnts.Add(line);
                    }
                }

                sheetSketch.Entities.AddRange(ents);
            }
            //
        }
        
        // Iterates all groups and pieces of a selected structural member and visualizes each profile plane
        // 中文：遍历选定结构构件的所有组和段，并可视化每个段的截面平面方向
        private static void StructuralMembersTest(ISwApplication app)
        {
            // Get the active document (expected to be a part containing a structural member feature)
            // 中文：获取活动文档（应为包含结构构件特征的零件文档）
            var doc = app.Documents.Active;

            // Get the first selected structural member feature from the selection
            // 中文：从当前选择中获取第一个结构构件特征
            var feat = doc.Selections.OfType<ISwStructuralMember>().First();

            // Iterate each group and piece within the structural member
            // 中文：遍历结构构件中的每个组（Group）和每个段（Piece）
            foreach (var grp in feat.Groups)
            {
                foreach (var piece in grp.Pieces)
                {
                    // Draw colored axis lines to visualize the profile plane orientation of each piece
                    // 中文：绘制彩色坐标轴线段以可视化每个段的截面平面方向
                    DrawPlaneOrientation(doc, piece.ProfilePlane);
                }
            }
        }

        // Creates a 3D sketch with colored axis lines to visually represent a plane's coordinate system
        // X-axis = red (plane direction), Y-axis = green (reference direction), Z-axis = blue (normal)
        // 中文：创建三维草图，用彩色坐标轴线段直观表示平面坐标系
        // 中文：X 轴=红色（平面主方向），Y 轴=绿色（参考方向），Z 轴=蓝色（法线方向）
        private static void DrawPlaneOrientation(IXDocument doc, Plane plane) 
        {
            // Pre-create a 3D sketch to hold the visualization geometry (not yet in FeatureManager)
            // 中文：预创建三维草图以容纳可视化几何体（尚未出现在特征管理器中）
            var sketch = doc.Features.PreCreate3DSketch();
            
            // Mark the plane's origin point in the sketch
            // 中文：在草图中标记平面原点
            var pt = sketch.Entities.PreCreatePoint();
            pt.Coordinate = plane.Point;

            // X-axis line (red, length 0.1 m): shows the plane's primary Direction vector
            // 中文：X 轴线段（红色，长度 0.1 m）：显示平面的主方向向量
            var xLine = (IXSketchLine)sketch.Entities.PreCreateLine();
            xLine.Geometry = new Line(plane.Point, plane.Point.Move(plane.Direction, 0.1));
            xLine.Color = System.Drawing.Color.Red;

            // Y-axis line (green, length 0.2 m): shows the plane's Reference (secondary) direction vector
            // 中文：Y 轴线段（绿色，长度 0.2 m）：显示平面的参考（次）方向向量
            var yLine = (IXSketchLine)sketch.Entities.PreCreateLine();
            yLine.Geometry = new Line(plane.Point, plane.Point.Move(plane.Reference, 0.2));
            yLine.Color = System.Drawing.Color.Green;

            // Z-axis line (blue, length 0.3 m): shows the plane's Normal vector
            // 中文：Z 轴线段（蓝色，长度 0.3 m）：显示平面的法向量
            var zLine = (IXSketchLine)sketch.Entities.PreCreateLine();
            zLine.Geometry = new Line(plane.Point, plane.Point.Move(plane.Normal, 0.3));
            zLine.Color = System.Drawing.Color.Blue;

            // Add all entities to the 3D sketch and commit to create the sketch feature in the model
            // 中文：将所有实体添加到三维草图并提交，在模型中创建该草图特征
            sketch.Entities.AddRange(new IXWireEntity[] { pt, xLine, yLine, zLine });

            sketch.Commit();
        }

        // Demonstrates how to read and modify the color of a selected annotation note, then reset it to default
        // 中文：演示如何读取和修改选定注释的颜色，然后将其重置为文档默认颜色
        private static void AnnotationColor(ISwApplication app)
        {
            // Get the first selected annotation note from the active document
            // 中文：从活动文档的当前选择中获取第一个文字注释
            var note = app.Documents.Active.Selections.OfType<IXNote>().First();
            // Read the current color of the annotation note
            // 中文：读取注释的当前颜色
            var c = note.Color;
            // Change the note color to green
            // 中文：将注释颜色更改为绿色
            note.Color = System.Drawing.Color.Green;
            // Reset the note color to the document default (null means use document-level color setting)
            // 中文：将注释颜色重置为文档默认值（null 表示使用文档级别的颜色设置）
            note.Color = null;
        }

        // Demonstrates how to register custom services (e.g., a custom logger) before launching SolidWorks
        // 中文：演示如何在启动 SolidWorks 之前注册自定义服务（例如自定义日志记录器）
        private static void CustomServices() 
        {
            // PreCreate returns a pre-configured (uncommitted) application instance without launching SolidWorks yet
            // 中文：PreCreate 返回预配置（未提交）的应用程序实例，此时 SolidWorks 尚未启动
            var app = SwApplicationFactory.PreCreate();
            // Create a service collection and register our custom logger to replace the default
            // 中文：创建服务集合并注册自定义日志记录器以替换默认实现
            var svcColl = new ServiceCollection();
            svcColl.Add<IXLogger, MyLogger>();
            // Assign the service collection before committing (this must be done before Commit())
            // 中文：在提交之前分配服务集合（必须在调用 Commit() 之前完成）
            app.CustomServices = svcColl;
            // Commit actually launches SolidWorks with the custom services registered
            // 中文：Commit 以已注册的自定义服务启动 SolidWorks
            app.Commit();
        }

        // Demonstrates the xCAD progress indicator API: displays a SolidWorks status bar progress from 0 to 100%
        // 中文：演示 xCAD 进度指示器 API：在 SolidWorks 状态栏中显示从 0% 到 100% 的进度条
        private static void Progress(IXApplication app) 
        {
            // CreateProgress returns an IXProgress that controls the SolidWorks status bar progress display
            // 中文：CreateProgress 返回 IXProgress，用于控制 SolidWorks 状态栏的进度显示
            using (var prg = app.CreateProgress())
            {
                for (int i = 0; i < 100; i++) 
                {
                    // Report progress as a normalized value between 0.0 (0%) and 1.0 (100%)
                    // 中文：以 0.0（0%）到 1.0（100%）之间的归一化值报告进度
                    prg.Report((double)i / 100);
                    // Update the status text shown alongside the progress bar
                    // 中文：更新进度条旁边显示的状态文本
                    prg.SetStatus(i.ToString());
                    System.Threading.Thread.Sleep(100);
                }
            }
        }

        // Demonstrates how to read and modify the display color of a selected sketch segment
        // 中文：演示如何读取和修改选定草图段（草图线）的显示颜色
        private static void SketchSegmentColors(IXApplication app) 
        {
            // Get the first selected entity and cast it to a sketch segment (line, arc, spline, etc.)
            // 中文：获取第一个选定实体并将其转换为草图段（直线、圆弧、样条曲线等）
            var seg = app.Documents.Active.Selections.First() as IXSketchSegment;
            // Read the current display color of the sketch segment
            // 中文：读取草图段的当前显示颜色
            var color = seg.Color;
            // Change the sketch segment's display color to purple
            // 中文：将草图段的显示颜色更改为紫色
            seg.Color = System.Drawing.Color.Purple;
        }

        // Demonstrates how to create a new drawing document and populate it with a model view
        // 中文：演示如何创建新工程图文档并用三维模型的标准视图填充它
        private static void CreateDrawingView(IXApplication app) 
        {
            // Get the active 3D document (part or assembly) as the source for the drawing view
            // 中文：将活动三维文档（零件或装配体）作为工程图视图的数据来源
            var partDoc = app.Documents.Active as IXDocument3D;
            // Retrieve the predefined "Right" standard view from the 3D document's model views
            // 中文：从三维文档的模型视图中获取预定义的"右视图"标准视图
            var view = partDoc.ModelViews[StandardViewType_e.Right];
            // Create a new blank drawing document in SolidWorks
            // 中文：在 SolidWorks 中创建新的空白工程图文档
            var drw = app.Documents.NewDrawing();
            // Add a drawing view to the active sheet based on the retrieved model view
            // 中文：在活动图纸上基于获取的模型视图添加工程图视图
            var drwView = drw.Sheets.Active.DrawingViews.CreateModelViewBased(view);
        }

        // Demonstrates creating, adding to the FeatureManager, and subsequently editing 3D sketch entities
        // 中文：演示创建三维草图实体、将其添加到特征管理器，以及后续对草图进行编辑的完整流程
        private static void CreateSketchEntities(IXApplication app)
        {
            // Pre-create a 3D sketch feature (in memory only, not yet in the FeatureManager)
            // 中文：预创建三维草图特征（仅在内存中，尚未添加到特征管理器）
            var sketch3D = app.Documents.Active.Features.PreCreate3DSketch();
            // Pre-create a line entity; coordinates are in meters (SolidWorks internal units)
            // 中文：预创建线段实体；坐标单位为米（SolidWorks 内部单位）
            var line = (IXSketchLine)sketch3D.Entities.PreCreateLine();
            line.Color = System.Drawing.Color.Green;
            line.Geometry = new Line(new Point(0.1, 0.1, 0.1), new Point(0.2, 0.2, 0.2));
            sketch3D.Entities.AddRange(new IXSketchEntity[] { line });

            // Add the sketch feature to the document to commit it into the FeatureManager tree
            // 中文：将草图特征添加到文档，使其提交并出现在特征管理器树中
            app.Documents.Active.Features.Add(sketch3D);

            // Read the end point coordinate of the now-committed line
            // 中文：读取已提交线段的终点坐标
            var c = line.EndPoint.Coordinate;

            // Open the sketch for editing to modify the existing line's end point position
            // 中文：打开草图进行编辑，修改已有线段的终点位置
            using (var editor = sketch3D.Edit()) 
            {
                line.EndPoint.Coordinate = new Point(0.3, 0.3, 0.3);
            }

            // Open the sketch for editing again to add a second line entity
            // 中文：再次打开草图进行编辑，以添加第二条线段实体
            using (var editor = sketch3D.Edit())
            {
                var line2 = (IXSketchLine)sketch3D.Entities.PreCreateLine();
                line2.Geometry = new Line(new Point(0, 0, 0), new Point(0.1, 0.2, 0.3));
                // Commit the line within the editor to add it to the existing sketch immediately
                // 中文：在编辑器上下文中提交线段，将其立即添加到现有草图中
                line2.Commit();
            }
        }

        // Iterates all selected faces and prints their surface area (in square meters) to the console
        // 中文：遍历所有选定的面并将其表面积（平方米）输出到控制台
        private static void TraverseSelectedFaces(IXApplication app) 
        {
            // Filter the current selection to only IXFace objects (faces of solid or surface bodies)
            // 中文：将当前选择过滤为仅 IXFace 对象（实体或曲面体的面）
            foreach (var face in app.Documents.Active.Selections.OfType<IXFace>()) 
            {
                // Area is returned in square meters (SolidWorks internal SI unit)
                // 中文：面积以平方米为单位返回（SolidWorks 内部 SI 单位制）
                Console.WriteLine(face.Area);
            }
        }

        // Creates a solid sweep body using an in-memory triangular profile and a selected sketch segment as the path,
        // then adds the resulting body to the active part document
        // 中文：使用内存中的三角形截面折线和选定草图段作为路径创建实体扫描体，
        // 中文：然后将生成的实体添加到活动零件文档
        private static void CreateSweepFromSelection(ISwApplication app) 
        {
            var doc = app.Documents.Active;
            
            // Create a closed triangular polyline in memory to serve as the sweep cross-section profile
            // 中文：在内存中创建封闭的三角形折线，作为扫描的横截面轮廓
            var polyline = app.MemoryGeometryBuilder.WireBuilder.PreCreatePolyline();
            polyline.Points = new Point[]
            {
                new Point(0, 0, 0),
                new Point(0.01, 0.01, 0),
                new Point(0.02, 0, 0),
                new Point(0, 0, 0)     // Close the polyline by returning to the start point
                                        // 中文：回到起点以封闭折线
            };
            polyline.Commit();

            // Convert the closed polyline into a planar region (flat face) for the sweep profile
            // 中文：将封闭折线转换为平面区域（平面面），用作扫描截面
            var reg = app.MemoryGeometryBuilder.CreatePlanarSheet(
                app.MemoryGeometryBuilder.CreateRegionFromSegments(polyline)).Bodies.First();

            // Get the last selected sketch segment and use its underlying curve definition as the sweep path
            // 中文：获取最后一个选定的草图段，使用其底层曲线定义作为扫描路径
            var pathSeg = app.Documents.Active.Selections.Last() as IXSketchSegment;

            var pathCurve = pathSeg.Definition;

            // Build the sweep solid body in memory using the triangular profile and the path curve
            // 中文：在内存中使用三角形截面和路径曲线构建扫描实体
            var sweep = app.MemoryGeometryBuilder.SolidBuilder.PreCreateSweep();
            sweep.Profiles = new IXPlanarRegion[] { reg };
            sweep.Path = pathCurve;
            sweep.Commit();

            // Extract the underlying SolidWorks IBody2 object and add it to the active part as a feature
            // 中文：提取底层 SolidWorks IBody2 对象并将其作为特征添加到活动零件
            var body = (sweep.Bodies.First() as ISwBody).Body;

            (app.Documents.Active as ISwPart).Part.CreateFeatureFromBody3(body, false, 0);
        }

        // Demonstrates creating multiple types of temporary (in-memory) geometry bodies and adding them to a part:
        // sweep, cone, revolve, box, extrusion, solid cylinder, and surface cylinder
        // 中文：演示创建多种类型的临时（内存中的）几何体并将它们添加到零件文档：
        // 中文：扫描体、圆锥体、旋转体、长方体、拉伸体、实体圆柱体和曲面圆柱体
        private static void CreateTempGeometry(IXApplication app) 
        {
            // --- Sweep: circular cross-section swept along a diagonal line ---
            // 中文：扫描体：圆形截面沿对角线路径扫描
            // Create a circular wire (sweep profile): radius 0.01 m at origin, normal along Z-axis
            // 中文：创建圆形线框（扫描截面）：半径 0.01 m，位于原点，法线沿 Z 轴
            var sweepArc = app.MemoryGeometryBuilder.WireBuilder.PreCreateCircle();
            sweepArc.Geometry = new Circle(new Axis(new Point(0, 0, 0), new Vector(0, 0, 1)), 0.01);
            sweepArc.Commit();

            // Create a diagonal line as the sweep path
            // 中文：创建对角线段作为扫描路径
            var sweepLine = app.MemoryGeometryBuilder.WireBuilder.PreCreateLine();
            sweepLine.Geometry = new Line(new Point(0, 0, 0), new Point(1, 1, 1));
            sweepLine.Commit();

            // Build the sweep solid: circular face swept along the diagonal line
            // 中文：构建扫描实体：将圆形面沿对角线扫描生成实体
            var sweep = app.MemoryGeometryBuilder.SolidBuilder.PreCreateSweep();
            sweep.Profiles = new IXPlanarRegion[] { app.MemoryGeometryBuilder.CreatePlanarSheet(
                app.MemoryGeometryBuilder.CreateRegionFromSegments(sweepArc)).Bodies.First() };
            sweep.Path = sweepLine;
            sweep.Commit();

            // Add the sweep body to the active part as a feature
            // 中文：将扫描体作为特征添加到活动零件
            var body = (sweep.Bodies.First() as ISwBody).Body;

            (app.Documents.Active as ISwPart).Part.CreateFeatureFromBody3(body, false, 0);

            // --- Cone: origin at (0,0,0), axis along (1,1,1), top radius 0.1 m, bottom radius 0.05 m, height 0.2 m ---
            // 中文：圆锥体：原点在 (0,0,0)，轴沿 (1,1,1)，顶面半径 0.1 m，底面半径 0.05 m，高度 0.2 m
            var cone = app.MemoryGeometryBuilder.CreateSolidCone(
                new Point(0, 0, 0), 
                new Vector(1, 1, 1), 
                0.1, 0.05, 0.2);
            
            body = (cone.Bodies.First() as ISwBody).Body;

            (app.Documents.Active as ISwPart).Part.CreateFeatureFromBody3(body, false, 0);

            // --- Revolve: circle at (-0.1,0,0) with radius 0.01 m, revolved 360° around the Y-axis ---
            // 中文：旋转体：圆形位于 (-0.1,0,0)，半径 0.01 m，绕 Y 轴旋转 360°
            var arc = app.MemoryGeometryBuilder.WireBuilder.PreCreateCircle();
            arc.Geometry = new Circle(new Axis(new Point(-0.1, 0, 0), new Vector(0, 0, 1)), 0.01);
            arc.Commit();

            // Create the rotation axis along the Y-axis
            // 中文：创建沿 Y 轴方向的旋转轴
            var axis = app.MemoryGeometryBuilder.WireBuilder.PreCreateLine();
            axis.Geometry = new Line(new Point(0, 0, 0), new Point(0, 1, 0));
            axis.Commit();

            // Build the revolve solid: rotate the circular face 360° around the Y-axis
            // 中文：构建旋转实体：将圆形面绕 Y 轴旋转 360°（2π 弧度）
            var rev = app.MemoryGeometryBuilder.SolidBuilder.PreCreateRevolve();
            rev.Angle = Math.PI * 2;
            rev.Axis = axis;
            rev.Profiles = new IXPlanarRegion[] { app.MemoryGeometryBuilder.CreatePlanarSheet(
                app.MemoryGeometryBuilder.CreateRegionFromSegments(arc)).Bodies.First() };
            rev.Commit();

            body = (rev.Bodies.First() as ISwBody).Body;

            (app.Documents.Active as ISwPart).Part.CreateFeatureFromBody3(body, false, 0);

            // --- Box: origin at (0,0,0), primary axis (1,1,1), dimensions 0.1 x 0.2 x 0.3 m ---
            // 中文：长方体：原点在 (0,0,0)，主轴方向 (1,1,1)，尺寸 0.1 x 0.2 x 0.3 m
            var box = app.MemoryGeometryBuilder.CreateSolidBox(
                new Point(0, 0, 0), 
                new Vector(1, 1, 1),
                new Vector(1, 1, 1).CreateAnyPerpendicular(),  // perpendicular direction for the box width
                                                                // 中文：盒体宽度方向的垂直向量
                0.1, 0.2, 0.3);

            body = (box.Bodies.First() as ISwBody).Body;

            (app.Documents.Active as ISwPart).Part.CreateFeatureFromBody3(body, false, 0);

            // --- Extrusion: triangular polyline profile extruded 0.5 m along direction (1,1,1) ---
            // 中文：拉伸体：三角形折线截面沿 (1,1,1) 方向拉伸 0.5 m
            var polyline = app.MemoryGeometryBuilder.WireBuilder.PreCreatePolyline();
            polyline.Points = new Point[] 
            {
                new Point(0, 0, 0),
                new Point(0.1, 0.1, 0),
                new Point(0.2, 0, 0),
                new Point(0, 0, 0)     // Close the profile polyline
                                        // 中文：封闭截面折线
            };
            polyline.Commit();

            // Build the extrusion: triangular face extruded 0.5 m along direction (1,1,1)
            // 中文：构建拉伸实体：三角形面沿 (1,1,1) 方向拉伸 0.5 m
            var extr = app.MemoryGeometryBuilder.SolidBuilder.PreCreateExtrusion();
            extr.Depth = 0.5;
            extr.Direction = new Vector(1, 1, 1);
            extr.Profiles = new IXPlanarRegion[] { app.MemoryGeometryBuilder.CreatePlanarSheet(
                app.MemoryGeometryBuilder.CreateRegionFromSegments(polyline)).Bodies.First() };
            extr.Commit();

            body = (extr.Bodies.First() as ISwBody).Body;

            (app.Documents.Active as ISwPart).Part.CreateFeatureFromBody3(body, false, 0);

            // --- Solid cylinder: origin (0,0,0), axis along X-axis, radius 0.1 m, height 0.2 m ---
            // 中文：实体圆柱体：原点 (0,0,0)，轴线沿 X 轴，半径 0.1 m，高度 0.2 m
            var cyl = app.MemoryGeometryBuilder.CreateSolidCylinder(
                new Point(0, 0, 0), new Vector(1, 0, 0), 0.1, 0.2);

            // Overwrite with a surface (hollow) cylinder using the same parameters
            // 中文：用相同参数创建曲面（中空）圆柱体覆盖之前的实体圆柱体
            cyl = app.MemoryGeometryBuilder.CreateSurfaceCylinder(
                new Point(0, 0, 0), new Vector(1, 0, 0), 0.1, 0.2);

            body = (cyl.Bodies.First() as ISwBody).Body;

            (app.Documents.Active as ISwPart).Part.CreateFeatureFromBody3(body, false, 0);
        }

        // Demonstrates creating a lofted surface body from the boundary edges of two selected planar faces,
        // then adds the trimmed sheet body to the active part using the SolidWorks Modeler API
        // 中文：演示从两个选定平面的边界边线创建放样曲面体，
        // 中文：然后使用 SolidWorks Modeler API 将修剪后的片体添加到活动零件
        private static void CreateLoftFromSelection(ISwApplication app)
        {
            var part = (ISwPart)app.Documents.Active;

            // Get the two selected planar faces to use as loft profiles
            // 中文：获取两个选定的平面作为放样截面轮廓
            var faces = part.Selections.OfType<IXPlanarFace>().ToArray();

            // Access the underlying SolidWorks Modeler (geometry kernel) object
            // 中文：访问底层 SolidWorks Modeler（几何内核）对象
            var modeler = app.Sw.IGetModeler();

            var curves = new List<ICurve>();

            // Get the boundary edge curves of the first face and create copies for manipulation
            // 中文：获取第一个面的边界边线曲线并创建副本以便操作
            var firstProfile = faces[0].AdjacentEntities.OfType<ISwEdge>().Select(e => e.Definition.Curves.First().ICopy());
            // Get the boundary edge curves of the second face
            // 中文：获取第二个面的边界边线曲线
            var secondProfile = faces[1].AdjacentEntities.OfType<ISwEdge>().Select(e => e.Definition.Curves.First().ICopy());

            // Merge all edge curves of each profile into single composite curves
            // 中文：将每个截面的所有边线曲线合并为单一复合曲线
            var c1 = modeler.MergeCurves(firstProfile.Select(c => c.ICopy()).ToArray());
            var c2 = modeler.MergeCurves(secondProfile.Select(c => c.ICopy()).ToArray());

            // Get parameter range of the merged curves to determine start/end points for trimming
            // 中文：获取合并曲线的参数范围，以确定修剪的起始点和终点
            c1.GetEndParams(out var start, out var end, out var isClosed, out var isPer);
            c2.GetEndParams(out var start1, out var end1, out var isClosed1, out var isPer1);

            // Evaluate the start and end 3D positions of curve c1
            // 中文：计算曲线 c1 起点和终点的三维坐标
            var startPt = (double[])c1.Evaluate2(start, 0);
            var endPt = (double[])c1.Evaluate2(end, 0);

            // Trim curve c1 to its full extent (start to end points)
            // 中文：将曲线 c1 修剪到其完整范围（从起点到终点）
            c1 = c1.CreateTrimmedCurve2(startPt[0], startPt[1], startPt[2], endPt[0], endPt[1], endPt[2]);

            startPt = (double[])c2.Evaluate2(start, 0);
            endPt = (double[])c2.Evaluate2(end, 0);

            // Trim curve c2 to its full extent
            // 中文：将曲线 c2 修剪到其完整范围
            c2 = c2.CreateTrimmedCurve2(startPt[0], startPt[1], startPt[2], endPt[0], endPt[1], endPt[2]);

            // Convert trimmed curves to B-spline representation required by the loft surface API
            // 中文：将修剪后的曲线转换为放样曲面 API 所需的 B 样条曲线表示
            curves.Add(c1.MakeBsplineCurve2());
            curves.Add(c2.MakeBsplineCurve2());

            //curves.AddRange(firstProfile.Select(c => c.MakeBsplineCurve2()));
            //curves.AddRange(secondProfile.Select(c => c.MakeBsplineCurve2()));

            //var vec = (IMathVector)app.Sw.IGetMathUtility().CreateVector(new double[] { 0, 0, 1 });

            // Create the loft surface from the two B-spline profile curves using the SolidWorks Modeler
            // 中文：使用 SolidWorks Modeler 从两条 B 样条截面曲线创建放样曲面
            var surf = (ISurface)modeler.CreateLoftSurface(curves.ToArray(), false, true, new ICurve[0], 0, 0, null, null, new IFace2[0], new IFace2[0], false, false, null, null, -1, -1, -1, -1);

            curves.Clear();

            // Rebuild curve list for trimming: include copies of c1 and c2 with a null separator
            // 中文：重新构建曲线列表用于修剪：包含 c1 和 c2 的副本，中间以 null 分隔
            //curves.AddRange(firstProfile.Select(c => c.ICopy()));
            curves.Add(c1.ICopy());
            curves.Add(null);
            curves.Add(c2.ICopy());
            //curves.AddRange(secondProfile.Select(c => c.ICopy()));

            // Trim the loft surface to create a bounded sheet body
            // 中文：修剪放样曲面以创建有界片体
            var body = (IBody2)surf.CreateTrimmedSheet4(curves.ToArray(), true);

            // Add the sheet body to the active part feature tree with simplification enabled
            // 中文：将片体添加到活动零件的特征树中，并启用几何简化选项
            part.Part.CreateFeatureFromBody3(body, false, (int)swCreateFeatureBodyOpts_e.swCreateFeatureBodySimplify);
        }
    }
}
