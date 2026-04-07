//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Linq;
using Xarial.XCad.Base;
using Xarial.XCad.Documents;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.SolidWorks.Geometry;
using Xarial.XCad.SolidWorks.Services;
using Xarial.XCad.SolidWorks.UI;
using Xarial.XCad.SolidWorks.Utils;
using Xarial.XCad.UI;
using Xarial.XCad.Utils.Diagnostics;

namespace Xarial.XCad.SolidWorks.Documents
{
    /// <summary>
    /// Represents a SolidWorks 3D document (part or assembly).
    /// <para>中文：表示 SolidWorks 3D文档（零件文档或装配体文档）的接口。</para>
    /// </summary>
    public interface ISwDocument3D : ISwDocument, IXDocument3D
    {
        /// <summary>
        /// Gets the configuration collection of this 3D document.
        /// <para>中文：获取该3D文档的配置集合。</para>
        /// </summary>
        new ISwConfigurationCollection Configurations { get; }

        /// <summary>
        /// Gets the 3D model view collection of this document.
        /// <para>中文：获取该文档的3D模型视图集合。</para>
        /// </summary>
        new ISwModelViews3DCollection ModelViews { get; }

        /// <summary>
        /// Converts a selection object pointer to the equivalent object in this document context.
        /// <para>中文：将选择对象的指针转换为当前文档上下文中对应的对象。</para>
        /// </summary>
        new TSelObject ConvertObject<TSelObject>(TSelObject obj)
            where TSelObject : ISwSelObject;
    }

    /// <summary>
    /// Abstract base class for SolidWorks 3D documents (part and assembly).
    /// <para>中文：SolidWorks 3D文档（零件文档和装配体文档）的抽象基类。</para>
    /// </summary>
    internal abstract class SwDocument3D : SwDocument, ISwDocument3D
    {
        // Explicit interface implementation: exposes configuration collection via xCAD base interface
        // 中文：显式接口实现：通过 xCAD 基础接口公开配置集合
        IXConfigurationRepository IXDocument3D.Configurations => Configurations;

        // Explicit interface implementation: exposes 3D model view repository
        // 中文：显式接口实现：公开3D模型视图存储库
        IXModelView3DRepository IXDocument3D.ModelViews => (IXModelView3DRepository)ModelViews;

        /// <summary>
        /// Gets the model views collection, delegating to the 3D-specific implementation.
        /// <para>中文：获取模型视图集合，委托给3D专用实现。</para>
        /// </summary>
        public override ISwModelViewsCollection ModelViews => ((ISwDocument3D)this).ModelViews;

        // Returns the lazily-initialized 3D model views collection
        // 中文：返回延迟初始化的3D模型视图集合
        ISwModelViews3DCollection ISwDocument3D.ModelViews => m_ModelViewsLazy.Value;

        // Explicit xCAD container interface: convert object using boxed helper
        // 中文：显式 xCAD 容器接口：使用装箱辅助方法转换对象
        TSelObject IXObjectContainer.ConvertObject<TSelObject>(TSelObject obj) => ConvertObjectBoxed(obj) as TSelObject;

        /// <summary>
        /// Initializes a new instance of <see cref="SwDocument3D"/>.
        /// <para>中文：初始化 <see cref="SwDocument3D"/> 的新实例，延迟创建配置集合和模型视图集合，并初始化图形对象。</para>
        /// </summary>
        internal SwDocument3D(IModelDoc2 model, SwApplication app, IXLogger logger, bool isCreated) : base(model, app, logger, isCreated)
        {
            // Lazily initialize the configuration collection to avoid unnecessary loading
            // 中文：延迟初始化配置集合，避免不必要的加载开销
            m_Configurations = new Lazy<ISwConfigurationCollection>(CreateConfigurations);

            // Lazily initialize the 3D model views collection
            // 中文：延迟初始化3D模型视图集合
            m_ModelViewsLazy = new Lazy<ISwModelViews3DCollection>(() => new SwModelViews3DCollection(this, app));

            Graphics = new SwDocumentGraphics(this);
        }

        // Lazy-initialized backing field for the configuration collection
        // 中文：配置集合的延迟初始化后备字段
        private Lazy<ISwConfigurationCollection> m_Configurations;

        // Lazy-initialized backing field for the 3D model views collection
        // 中文：3D模型视图集合的延迟初始化后备字段
        private Lazy<ISwModelViews3DCollection> m_ModelViewsLazy;

        /// <summary>
        /// Gets the configuration collection for this 3D document.
        /// <para>中文：获取该3D文档的配置集合。</para>
        /// </summary>
        public ISwConfigurationCollection Configurations => m_Configurations.Value;

        /// <summary>
        /// Gets the document evaluation object (e.g. mass properties, bounding box).
        /// <para>中文：获取文档的评估对象（例如质量属性、包围盒等）。</para>
        /// </summary>
        public abstract IXDocumentEvaluation Evaluation { get; }

        /// <summary>
        /// Gets the document graphics (3D viewport rendering) object.
        /// <para>中文：获取文档图形（3D视口渲染）对象。</para>
        /// </summary>
        public IXDocumentGraphics Graphics { get; }

        /// <summary>
        /// Disposes managed resources, including the configuration collection if it was created.
        /// <para>中文：释放托管资源，如果配置集合已被创建则一并释放。</para>
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                // Only dispose the configuration collection if it was actually instantiated
                // 中文：仅在配置集合已实际实例化时才进行释放
                if (m_Configurations.IsValueCreated)
                {
                    m_Configurations.Value.Dispose();
                }
            }
        }

        /// <summary>
        /// Factory method: creates the concrete configuration collection for this document type.
        /// <para>中文：工厂方法：为当前文档类型创建具体的配置集合实例。</para>
        /// </summary>
        protected abstract SwConfigurationCollection CreateConfigurations();

        /// <summary>
        /// Pre-creates a save-as operation for a 3D document, dispatching by file extension.
        /// <para>中文：为3D文档预创建另存为操作，根据文件扩展名分发到对应的保存操作类。</para>
        /// </summary>
        IXDocument3DSaveOperation IXDocument3D.PreCreateSaveAsOperation(string filePath)
        {
            // Extract the file extension to determine which save operation to use
            // 中文：提取文件扩展名，以确定要使用哪种保存操作
            var ext = System.IO.Path.GetExtension(filePath);

            switch (ext.ToLower())
            {
                case ".pdf":
                    // Save as PDF document
                    // 中文：另存为 PDF 文档
                    return new SwDocument3DPdfSaveOperation(this, filePath);

                case ".step":
                case ".stp":
                    // Save as STEP neutral format
                    // 中文：另存为 STEP 中性格式
                    return new SwStepSaveOperation(this, filePath);

                default:
                    // Save in native SolidWorks 3D document format
                    // 中文：以 SolidWorks 原生3D文档格式保存
                    return new SwDocument3DSaveOperation(this, filePath);
            }
        }

        /// <summary>
        /// Converts a typed selection object to the corresponding object in this document context.
        /// <para>中文：将类型化的选择对象转换为当前文档上下文中对应的对象。</para>
        /// </summary>
        public TSelObject ConvertObject<TSelObject>(TSelObject obj)
            where TSelObject : ISwSelObject
            => (TSelObject)ConvertObjectBoxed(obj);

        /// <summary>
        /// Internal helper that converts a SolidWorks selection object dispatch pointer
        /// to the equivalent object within this document using GetCorresponding.
        /// <para>中文：内部辅助方法，使用 GetCorresponding 将 SolidWorks 选择对象的 dispatch 指针
        /// 转换为当前文档中对应的对象。</para>
        /// </summary>
        private ISwSelObject ConvertObjectBoxed(object obj)
        {
            if (obj is SwSelObject)
            {
                // Retrieve the underlying COM dispatch pointer of the selection object
                // 中文：获取选择对象底层的 COM dispatch 指针
                var disp = (obj as SwSelObject).Dispatch;

                // Use SolidWorks API to get the corresponding pointer in this document context
                // 中文：使用 SolidWorks API 获取该指针在当前文档上下文中对应的指针
                var corrDisp = Model.Extension.GetCorresponding(disp);

                if (corrDisp != null)
                {
                    return this.CreateObjectFromDispatch<ISwSelObject>(corrDisp);
                }
                else
                {
                    throw new Exception("Failed to convert the pointer of the object");
                }
            }
            else
            {
                throw new InvalidCastException("Object is not SOLIDWORKS object");
            }
        }

        /// <summary>
        /// Pre-creates a save-as operation, delegating to the 3D document interface implementation.
        /// <para>中文：预创建另存为操作，委托给3D文档接口的实现。</para>
        /// </summary>
        public override IXSaveOperation PreCreateSaveAsOperation(string filePath) => ((IXDocument3D)this).PreCreateSaveAsOperation(filePath);
    }
}