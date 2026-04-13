//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Xarial.XCad.Base;
using Xarial.XCad.Base.Enums;

namespace Xarial.XCad
{
    /// <summary>
    /// Represents objects which can be selected by the user
    /// 表示可被用户选中的对象
    /// </summary>
    public interface IXSelObject : IXObject, IXTransaction
    {
        /// <summary>
        /// Identifies if this object is currently selected
        /// 标识此对象当前是否已被选中
        /// </summary>
        bool IsSelected { get; }

        /// <summary>
        /// Selects object in the document
        /// 在文档中选中此对象
        /// </summary>
        /// <param name="append">True to add selection to the current list, false to clear existing selection 为 true 则添加选中，false 则清除当前选中</param>
        void Select(bool append);

        /// <summary>
        /// Deletes this object
        /// 删除此对象
        /// </summary>
        void Delete();
    }
}