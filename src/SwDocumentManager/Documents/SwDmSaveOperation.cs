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
using System.Threading;
using System.Threading.Tasks;
using Xarial.XCad.Documents;
using Xarial.XCad.Documents.Enums;
using Xarial.XCad.Services;

namespace Xarial.XCad.SwDocumentManager.Documents
{
    /// <summary>
    /// Deferred save-as operation used by xCAD save pipelines.
    /// xCAD 保存流程中使用的延迟另存为操作。
    /// </summary>
    internal class SwDmSaveOperation : IXSaveOperation
    {
        public string FilePath { get; }

        public bool IsCommitted => m_Creator.IsCreated;

        protected readonly ElementCreator<bool?> m_Creator;

        protected readonly SwDmDocument m_Doc;

        /// <summary>
        /// Creates a save operation bound to a target document and file path.
        /// 创建绑定到目标文档与目标文件路径的保存操作。
        /// </summary>
        internal SwDmSaveOperation(SwDmDocument doc, string filePath)
        {
            m_Doc = doc;
            FilePath = filePath;

            m_Creator = new ElementCreator<bool?>(SaveAs, null, false);
        }

        /// <summary>
        /// Executes the Save As pipeline through the owning document wrapper.
        /// 通过所属文档包装器执行另存为流程。
        /// </summary>
        private bool? SaveAs(CancellationToken cancellationToken)
        {
            m_Doc.PerformSave(DocumentSaveType_e.SaveAs, FilePath, f => true, (d, f) => d.SaveAs(f));
            return true;
        }

        public void Commit(CancellationToken cancellationToken) => m_Creator.Create(cancellationToken);
    }
}
