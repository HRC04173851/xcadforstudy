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
using Xarial.XCad.Features;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Utils;

namespace Xarial.XCad.SolidWorks.Features
{
    /// <summary>
    /// SolidWorks 坐标系特征接口，提供坐标系变换矩阵访问。
    /// </summary>
    public interface ISwCoordinateSystem : IXCoordinateSystem, ISwFeature
    {
        ICoordinateSystemFeatureData CoordSys { get; }
    }

    /// <summary>
    /// SolidWorks 坐标系特征实现类，封装坐标系特征数据及变换矩阵读取。
    /// </summary>
    internal class SwCoordinateSystem : SwFeature, ISwCoordinateSystem
    {
        public ICoordinateSystemFeatureData CoordSys { get; }

        internal SwCoordinateSystem(IFeature feat, SwDocument doc, SwApplication app, bool created) : base(feat, doc, app, created)
        {
            CoordSys = feat.GetDefinition() as ICoordinateSystemFeatureData;
        }

        public TransformMatrix Transform
            => CoordSys.Transform.ToTransformMatrix();
    }
}
