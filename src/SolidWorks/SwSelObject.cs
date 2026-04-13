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
using System.Runtime.InteropServices;
using System.Threading;
using Xarial.XCad.Base.Enums;
using Xarial.XCad.Exceptions;
using Xarial.XCad.SolidWorks.Documents;

namespace Xarial.XCad.SolidWorks
{
    /// <summary>
    /// 表示可在 SolidWorks 中被选中的对象的基础接口。
    /// 继承自 <see cref="ISwObject"/>（SolidWorks 对象基接口）和 <see cref="IXSelObject"/>（xCAD 可选对象接口）。
    /// </summary>
    public interface ISwSelObject : ISwObject, IXSelObject
    {
    }

    /// <inheritdoc/>
    /// <summary>
    /// SolidWorks 中可被选择/取消选择的对象的内部基类实现。
    /// 封装了 SolidWorks 选择管理器（ISelectionManager）的交互逻辑。
    /// </summary>
    internal class SwSelObject : SwObject, ISwSelObject
    {
        /// <summary>
        /// 表示该对象是否已提交（已存在于 SolidWorks 模型中）。
        /// 对于可选对象，默认认为已提交。
        /// </summary>
        public virtual bool IsCommitted => true;

        /// <summary>
        /// 指示当前对象是否处于选中状态（在选择管理器中的索引不为 -1）。
        /// </summary>
        public bool IsSelected => SelectionIndex != -1;

        /// <summary>
        /// 在 SolidWorks 选择管理器中获取当前对象的选择索引（1-based）。
        /// 若未被选中，则返回 -1。
        /// </summary>
        internal int SelectionIndex
        {
            get 
            {
                // 获取 SolidWorks 选择管理器
                var selMgr = OwnerModelDoc.ISelectionManager;

                // 遍历所有已选对象（索引从 1 开始），查找与当前对象匹配的项
                for (int i = 1; i < selMgr.GetSelectedObjectCount2(-1) + 1; i++)
                {
                    if (IsSameDispatch(selMgr.GetSelectedObject6(i, -1)))
                    {
                        return i;
                    }
                }

                return -1;
            }
        }

        /// <summary>
        /// 构造 SwSelObject 实例。
        /// </summary>
        /// <param name="disp">底层 SolidWorks COM 调度对象</param>
        /// <param name="doc">所属 SolidWorks 文档</param>
        /// <param name="app">SolidWorks 应用程序实例</param>
        internal SwSelObject(object disp, SwDocument doc, SwApplication app) : base(disp, doc, app)
        {
        }

        /// <summary>
        /// 选中当前对象，<paramref name="append"/> 为 true 时追加到当前选择集，否则替换当前选择集。
        /// </summary>
        public void Select(bool append) => Select(append, null);

        /// <summary>
        /// 使用 SolidWorks MultiSelect2 API 选中当前对象，支持附加选择数据（如选择标记）。
        /// </summary>
        /// <param name="append">是否追加到当前选择集</param>
        /// <param name="selData">SolidWorks 选择数据（可包含选择标记 mark 等信息）</param>
        internal virtual void Select(bool append, ISelectData selData) 
        {
            if (OwnerModelDoc != null)
            {
                // 使用 MultiSelect2 实现多选；返回值 1 表示成功
                if (OwnerModelDoc.Extension.MultiSelect2(new DispatchWrapper[] { new DispatchWrapper(Dispatch) }, append, selData) != 1)
                {
                    throw new Exception("Failed to select");
                }
            }
            else
            {
                throw new Exception("Model doc is not initialized");
            }
        }

        /// <summary>
        /// 提交（创建）此对象到 SolidWorks 模型中。
        /// 对于已存在的可选对象，该方法默认为空操作。
        /// </summary>
        public virtual void Commit(CancellationToken cancellationToken)
        {
        }

        /// <summary>
        /// 从 SolidWorks 文档中删除此对象。
        /// 先将其选中，再调用 SolidWorks DeleteSelection2 API 执行删除。
        /// </summary>
        public virtual void Delete()
        {
            // 必须先选中对象才能删除
            Select(false);

            // swDelete_Absorbed 表示同时删除被吸收的（关联的）实体
            if (!OwnerModelDoc.Extension.DeleteSelection2((int)swDeleteSelectionOptions_e.swDelete_Absorbed)) 
            {
                throw new Exception("Failed to delete the object");
            }
        }

        /// <summary>
        /// 判断给定的 COM 调度对象是否与当前对象的调度对象指向同一 SolidWorks 实体。
        /// 使用 SolidWorks IsSame API 进行比较。
        /// </summary>
        protected virtual bool IsSameDispatch(object disp)
            => OwnerApplication.Sw.IsSame(disp, Dispatch) == (int)swObjectEquality.swObjectSame;
    }
}