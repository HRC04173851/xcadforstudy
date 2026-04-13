//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Xarial.XCad.Features.CustomFeature;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Features.CustomFeature.Structures
{
    /// <summary>
    /// Information of the selection of <see cref="IXCustomFeature"/>
    /// <see cref="IXCustomFeature"/> 选择对象信息
    /// </summary>
    public class CustomFeatureSelectionInfo 
    {
        /// <summary>
        /// Selection
        /// 选择对象
        /// </summary>
        public IXSelObject Selection { get; }
        
        /// <summary>
        /// Transformation of this selection
        /// 该选择对象的变换矩阵
        /// </summary>
        public TransformMatrix Transformation { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="selection">Selection</param>
        /// <param name="transformation">Transformation of the selection</param>
        public CustomFeatureSelectionInfo(IXSelObject selection, TransformMatrix transformation)
        {
            Selection = selection;
            Transformation = transformation;
        }
    }
}