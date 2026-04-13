//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xarial.XCad.Exceptions;
using Xarial.XCad.Features;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.SolidWorks.Documents;

namespace Xarial.XCad.SolidWorks.Features
{
    /// <summary>
    /// SolidWorks 原点特征接口。
    /// 原点是文档坐标系基准点，属于系统内置参考几何元素。
    /// </summary>
    public interface ISwOrigin : IXOrigin, ISwFeature 
    {
    }

    /// <summary>
    /// SolidWorks 原点特征实现类。
    /// 原点为默认系统特征，不支持手动创建。
    /// </summary>
    internal class SwOrigin : SwFeature, ISwOrigin
    {
        /// <summary>
        /// SolidWorks 原点特征类型名。
        /// </summary>
        internal const string TypeName = "OriginProfileFeature";

        internal SwOrigin(IFeature feat, SwDocument doc, SwApplication app, bool created) : base(feat, doc, app, created)
        {
        }

        public override bool IsUserFeature => false;

        protected override IFeature InsertFeature(CancellationToken cancellationToken)
            // 原点属于系统默认特征，不允许通过 API 新建
            => throw new NotSupportedException("Origin is a default feature and cannot be created");
    }
}
