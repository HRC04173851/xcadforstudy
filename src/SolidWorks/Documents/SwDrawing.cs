//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Linq;
using System.Threading;
using Xarial.XCad.Base;
using Xarial.XCad.Documents;
using Xarial.XCad.Documents.Structures;
using Xarial.XCad.Utils.Diagnostics;

namespace Xarial.XCad.SolidWorks.Documents
{
    /// <summary>
    /// Represents a SolidWorks engineering drawing document.
    /// <para>中文：表示 SolidWorks 工程图文档的接口。</para>
    /// </summary>
    public interface ISwDrawing : ISwDocument, IXDrawing 
    {
        /// <summary>
        /// Gets the underlying SolidWorks <see cref="IDrawingDoc"/> COM object.
        /// <para>中文：获取底层 SolidWorks <see cref="IDrawingDoc"/> COM 对象。</para>
        /// </summary>
        IDrawingDoc Drawing { get; }
    }

    /// <summary>
    /// Represents SolidWorks-specific options for an engineering drawing document.
    /// <para>中文：表示 SolidWorks 工程图文档专用选项的接口。</para>
    /// </summary>
    public interface ISwDrawingOptions : ISwDocumentOptions, IXDrawingOptions 
    {
    }

    /// <summary>
    /// Represents SolidWorks-specific detailing options for an engineering drawing document.
    /// <para>中文：表示 SolidWorks 工程图文档详图选项的接口。</para>
    /// </summary>
    public interface ISwDrawingDetailingOptions : IXDrawingDetailingOptions 
    {
    }

    /// <summary>
    /// Implementation of detailing options for a SolidWorks engineering drawing document.
    /// <para>中文：SolidWorks 工程图文档详图选项的实现类，封装各项详图用户偏好设置。</para>
    /// </summary>
    internal class SwDrawingDetailingOptions : ISwDrawingDetailingOptions 
    {
        // Reference to the owning drawing document for accessing user preference toggles
        // 中文：对所属工程图文档的引用，用于读写用户偏好开关
        private readonly SwDrawing m_Draw;

        /// <summary>
        /// Initializes a new instance bound to the given drawing document.
        /// <para>中文：初始化绑定到指定工程图文档的新实例。</para>
        /// </summary>
        internal SwDrawingDetailingOptions(SwDrawing draw) 
        {
            m_Draw = draw;
        }

        /// <summary>
        /// Gets or sets whether cosmetic threads are displayed in the drawing.
        /// <para>中文：获取或设置是否在工程图中显示装饰螺纹线。</para>
        /// </summary>
        public bool DisplayCosmeticThreads 
        {
            get => m_Draw.GetUserPreferenceToggle(swUserPreferenceToggle_e.swDisplayCosmeticThreads);
            set => m_Draw.SetUserPreferenceToggle(swUserPreferenceToggle_e.swDisplayCosmeticThreads, value);
        }

        /// <summary>
        /// Gets or sets whether center marks for slots are automatically inserted.
        /// <para>中文：获取或设置是否自动为槽孔插入中心标记。</para>
        /// </summary>
        public bool AutoInsertCenterMarksForSlots
        {
            get => m_Draw.GetUserPreferenceToggle(swUserPreferenceToggle_e.swDetailingAutoInsertCenterMarksForSlots);
            set => m_Draw.SetUserPreferenceToggle(swUserPreferenceToggle_e.swDetailingAutoInsertCenterMarksForSlots, value);
        }

        /// <summary>
        /// Gets or sets whether center marks for fillets are automatically inserted.
        /// <para>中文：获取或设置是否自动为圆角插入中心标记。</para>
        /// </summary>
        public bool AutoInsertCenterMarksForFillets
        {
            get => m_Draw.GetUserPreferenceToggle(swUserPreferenceToggle_e.swDetailingAutoInsertCenterMarksForFillets);
            set => m_Draw.SetUserPreferenceToggle(swUserPreferenceToggle_e.swDetailingAutoInsertCenterMarksForFillets, value);
        }

        /// <summary>
        /// Gets or sets whether center marks for holes are automatically inserted.
        /// <para>中文：获取或设置是否自动为孔特征插入中心标记。</para>
        /// </summary>
        public bool AutoInsertCenterMarksForHoles
        {
            get => m_Draw.GetUserPreferenceToggle(swUserPreferenceToggle_e.swDetailingAutoInsertCenterMarksForHoles);
            set => m_Draw.SetUserPreferenceToggle(swUserPreferenceToggle_e.swDetailingAutoInsertCenterMarksForHoles, value);
        }

        /// <summary>
        /// Gets or sets whether dowel symbols are automatically inserted.
        /// <para>中文：获取或设置是否自动插入销钉符号。</para>
        /// </summary>
        public bool AutoInsertDowelSymbols
        {
            get => m_Draw.GetUserPreferenceToggle(swUserPreferenceToggle_e.swDetailingAutoInsertDowelSymbols);
            set => m_Draw.SetUserPreferenceToggle(swUserPreferenceToggle_e.swDetailingAutoInsertDowelSymbols, value);
        }
    }

    /// <summary>
    /// Implementation of document options for a SolidWorks engineering drawing.
    /// <para>中文：SolidWorks 工程图文档选项的实现类，包含详图子选项。</para>
    /// </summary>
    internal class SwDrawingOptions : SwDocumentOptions, ISwDrawingOptions 
    {
        /// <summary>
        /// Initializes a new instance and creates the detailing sub-options.
        /// <para>中文：初始化新实例并创建详图子选项对象。</para>
        /// </summary>
        internal SwDrawingOptions(SwDrawing draw) : base(draw) 
        {
            Detailing = new SwDrawingDetailingOptions(draw);
        }

        /// <summary>
        /// Gets the detailing options for this drawing document.
        /// <para>中文：获取工程图文档的详图选项。</para>
        /// </summary>
        public IXDrawingDetailingOptions Detailing { get; }
    }

    /// <summary>
    /// SolidWorks engineering drawing document implementation.
    /// <para>中文：SolidWorks 工程图文档的实现类。</para>
    /// </summary>
    internal class SwDrawing : SwDocument, ISwDrawing
    {
        // Explicit interface: return drawing options via xCAD base interface
        // 中文：显式接口：通过 xCAD 基础接口返回工程图选项
        IXDrawingOptions IXDrawing.Options => m_Options;

        /// <summary>
        /// Gets the underlying SolidWorks drawing document COM object.
        /// <para>中文：获取底层 SolidWorks 工程图文档 COM 对象。</para>
        /// </summary>
        public IDrawingDoc Drawing => Model as IDrawingDoc;

        /// <summary>
        /// Gets the sheet repository containing all drawing sheets (图纸) in this document.
        /// <para>中文：获取包含该工程图文档所有图纸的图纸存储库。</para>
        /// </summary>
        public IXSheetRepository Sheets => m_SheetsLazy.Value;

        /// <summary>
        /// Gets the SolidWorks document type identifier for a drawing document.
        /// <para>中文：获取工程图文档对应的 SolidWorks 文档类型标识符。</para>
        /// </summary>
        internal protected override swDocumentTypes_e? DocumentType => swDocumentTypes_e.swDocDRAWING;

        // Lazy-initialized sheet collection to defer SolidWorks API calls
        // 中文：延迟初始化的图纸集合，推迟 SolidWorks API 调用
        private readonly Lazy<SwSheetCollection> m_SheetsLazy;

        /// <summary>
        /// Returns true when any drawing view on any sheet is in lightweight mode.
        /// <para>中文：当任意图纸上的任意工程图视图处于轻量化模式时返回 true。</para>
        /// </summary>
        protected override bool IsLightweightMode => Sheets.Any(s => s.DrawingViews.Any(v => ((ISwDrawingView)v).DrawingView.IsLightweight()));

        /// <summary>
        /// Returns true when the drawing is opened in detailing (rapid) mode.
        /// Only available from SolidWorks 2020 onwards.
        /// <para>中文：当工程图以详图（快速）模式打开时返回 true。该功能仅在 SolidWorks 2020 及以上版本可用。</para>
        /// </summary>
        protected override bool IsRapidMode 
        {
            get 
            {
                // Detailing mode (快速模式) was introduced in SolidWorks 2020
                // 中文：详图模式（快速模式）在 SolidWorks 2020 中引入
                if (OwnerApplication.IsVersionNewerOrEqual(Enums.SwVersion_e.Sw2020))
                {
                    return Drawing.IsDetailingMode();
                }
                else 
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the document options for this drawing.
        /// <para>中文：获取工程图文档选项。</para>
        /// </summary>
        public override IXDocumentOptions Options => m_Options;

        /// <summary>
        /// Gets the layer repository containing all drawing layers (图层) in this document.
        /// <para>中文：获取包含该工程图文档所有图层的图层存储库。</para>
        /// </summary>
        public IXLayerRepository Layers { get; }

        // Drawing-specific options instance (wraps user preference toggles)
        // 中文：工程图专用选项实例（封装用户偏好开关）
        private SwDrawingOptions m_Options;

        /// <summary>
        /// Initializes a new instance of <see cref="SwDrawing"/>.
        /// <para>中文：初始化 <see cref="SwDrawing"/> 的新实例，延迟创建图纸集合，并初始化选项和图层集合。</para>
        /// </summary>
        internal SwDrawing(IDrawingDoc drawing, SwApplication app, IXLogger logger, bool isCreated)
            : base((IModelDoc2)drawing, app, logger, isCreated)
        {
            // Lazily initialize the sheet collection to defer SolidWorks COM calls
            // 中文：延迟初始化图纸集合，推迟 SolidWorks COM 调用
            m_SheetsLazy = new Lazy<SwSheetCollection>(() => new SwSheetCollection(this, OwnerApplication));
            m_Options = new SwDrawingOptions(this);
            Layers = new SwLayersCollection(this, app);
        }

        /// <summary>
        /// Commits any pending cache changes, including the sheet collection if initialized.
        /// <para>中文：提交所有待处理的缓存变更，包括图纸集合（若已初始化）。</para>
        /// </summary>
        protected override void CommitCache(IModelDoc2 model, CancellationToken cancellationToken)
        {
            base.CommitCache(model, cancellationToken);

            // Only commit sheet cache if sheets were already accessed/created
            // 中文：仅在图纸集合已被访问/创建时才提交图纸缓存
            if (m_SheetsLazy.IsValueCreated) 
            {
                m_SheetsLazy.Value.CommitCache(cancellationToken);
            }
        }

        /// <summary>
        /// Returns true only if the given document type is a drawing document.
        /// <para>中文：仅当给定文档类型为工程图文档时返回 true。</para>
        /// </summary>
        protected override bool IsDocumentTypeCompatible(swDocumentTypes_e docType) => docType == swDocumentTypes_e.swDocDRAWING;

        /// <summary>
        /// Creates the annotation collection for this drawing document.
        /// <para>中文：为工程图文档创建注解集合。</para>
        /// </summary>
        protected override SwAnnotationCollection CreateAnnotations() => new SwDrawingAnnotationCollection(this);

        /// <summary>
        /// Pre-creates a save-as operation for the drawing, dispatching by file extension.
        /// <para>中文：为工程图文档预创建另存为操作，根据文件扩展名分发到对应的保存操作类。</para>
        /// </summary>
        IXDrawingSaveOperation IXDrawing.PreCreateSaveAsOperation(string filePath)
        {
            // Extract the file extension to determine which save operation to use
            // 中文：提取文件扩展名，以确定要使用哪种保存操作
            var ext = System.IO.Path.GetExtension(filePath);

            switch (ext.ToLower())
            {
                case ".pdf":
                    // Save drawing as PDF document
                    // 中文：将工程图另存为 PDF 文档
                    return new SwDrawingPdfSaveOperation(this, filePath);

                case ".dxf":
                case ".dwg":
                    // Save drawing in DXF/DWG CAD exchange format
                    // 中文：将工程图另存为 DXF/DWG CAD 交换格式
                    return new SwDxfDwgSaveOperation(this, filePath);

                default:
                    // Save in native SolidWorks drawing format
                    // 中文：以 SolidWorks 原生工程图格式保存
                    return new SwDrawingSaveOperation(this, filePath);
            }
        }

        /// <summary>
        /// Retrieves the paper size of the drawing, using the first sheet if sheets are initialized.
        /// <para>中文：获取工程图的图纸尺寸；如果图纸集合已初始化，则使用第一张图纸的图纸尺寸。</para>
        /// </summary>
        protected override void GetPaperSize(out swDwgPaperSizes_e size, out double width, out double height)
        {
            if (m_SheetsLazy.IsValueCreated)
            {
                // Use the paper size of the first sheet in the drawing
                // 中文：使用工程图中第一张图纸的图纸尺寸
                PaperSizeHelper.ParsePaperSize(Sheets.First().PaperSize, out size, out _, out width, out height);
            }
            else
            {
                // Sheets not yet loaded; return default/null paper size
                // 中文：图纸集合尚未加载，返回默认/空图纸尺寸
                PaperSizeHelper.ParsePaperSize(null, out size, out _, out width, out height);
            }
        }

        /// <summary>
        /// Pre-creates a save-as operation, delegating to the drawing interface implementation.
        /// <para>中文：预创建另存为操作，委托给工程图接口的实现。</para>
        /// </summary>
        public override IXSaveOperation PreCreateSaveAsOperation(string filePath) => ((IXDrawing)this).PreCreateSaveAsOperation(filePath);
    }
}