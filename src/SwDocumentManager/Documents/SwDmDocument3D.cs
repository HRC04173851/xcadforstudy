//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.swdocumentmgr;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Documents;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.UI;

namespace Xarial.XCad.SwDocumentManager.Documents
{
    /// <summary>
    /// Common contract for part and assembly documents handled by Document Manager.
    /// Document Manager 中零件与装配体文档共享的三维文档约定。
    /// </summary>
    public interface ISwDmDocument3D : ISwDmDocument, IXDocument3D
    {
        new ISwDmConfigurationCollection Configurations { get; }
    }

    /// <summary>
    /// Base implementation for 3D model documents.
    /// 三维模型文档的基础实现，供零件和装配体共用。
    /// </summary>
    internal abstract class SwDmDocument3D : SwDmDocument, ISwDmDocument3D
    {
        #region Not Supported

        public new IXModelView3DRepository ModelViews => throw new NotSupportedException();
        TSelObject IXObjectContainer.ConvertObject<TSelObject>(TSelObject obj) => throw new NotSupportedException();
        IXDocument3DSaveOperation IXDocument3D.PreCreateSaveAsOperation(string filePath) => throw new NotSupportedException();
        public IXDocumentEvaluation Evaluation => throw new NotSupportedException();
        public IXDocumentGraphics Graphics => throw new NotSupportedException();

        #endregion

        IXConfigurationRepository IXDocument3D.Configurations => Configurations;

        public abstract ISwDmConfigurationCollection Configurations { get; }

        /// <summary>
        /// Initializes a 3D document wrapper without deciding whether it is a part or assembly yet.
        /// 初始化三维文档包装器，但暂不关心其具体是零件还是装配体。
        /// </summary>
        public SwDmDocument3D(SwDmApplication dmApp, ISwDMDocument doc, bool isCreated,
            Action<ISwDmDocument> createHandler, Action<ISwDmDocument> closeHandler,
            bool? isReadOnly)
            : base(dmApp, doc, isCreated, createHandler, closeHandler, isReadOnly)
        {
        }
    }

    /// <summary>
    /// Helper methods for titles of virtual documents created by SOLIDWORKS.
    /// 处理 SOLIDWORKS 虚拟文档标题的辅助工具，主要用于去除内部临时前缀。
    /// </summary>
    internal class SwDmVirtualDocumentHelper
    {
        /// <summary>
        /// Removes the temporary prefix that SOLIDWORKS adds to virtual document names.
        /// 去掉 SOLIDWORKS 为虚拟文档名称添加的临时前缀。
        /// </summary>
        internal static string GetTitle(string fileName)
        {
            const string PREFIX = "_temp_";

            if (fileName.StartsWith(PREFIX, StringComparison.CurrentCultureIgnoreCase))
            {
                return fileName.Substring(PREFIX.Length);
            }
            else 
            {
                return fileName;
            }
        }
    }
}
