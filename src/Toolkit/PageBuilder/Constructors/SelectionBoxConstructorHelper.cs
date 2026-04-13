//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Xarial.XCad.Annotations;
using Xarial.XCad.Base.Enums;
using Xarial.XCad.Documents;
using Xarial.XCad.Features;
using Xarial.XCad.Geometry;
using Xarial.XCad.Sketch;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.UI.PropertyPage.Enums;
using Xarial.XCad.UI.PropertyPage.Services;
using Xarial.XCad.Utils.PageBuilder.Base;
using Xarial.XCad.Utils.Reflection;

namespace Xarial.XCad.Toolkit.PageBuilder.Constructors
{
    /// <summary>
    /// Helper methods for selection-box defaults in page builder.
    /// <para>页面构建器中用于选择框默认行为的辅助方法。</para>
    /// </summary>
    public static class SelectionBoxConstructorHelper
    {
        /// <summary>
        /// Gets element type from a type or enumerable type.
        /// <para>从类型或可枚举类型中提取元素类型。</para>
        /// </summary>
        public static Type GetElementType(Type type) 
        {
            if (type.IsAssignableToGenericType(typeof(IEnumerable<>)))
            {
                return type.GetArgumentsOfGenericType(typeof(IEnumerable<>)).First();
            }
            else 
            {
                return type;
            }
        }

        /// <summary>
        /// Returns default bitmap label by selection context type.
        /// <para>根据选择上下文类型返回默认位图标签。</para>
        /// </summary>
        public static BitmapLabelType_e? GetDefaultBitmapLabel(IAttributeSet atts)
        {
            var type = atts.ContextType;

            if (type.IsAssignableToGenericType(typeof(IEnumerable<>)))
            {
                type = type.GetArgumentsOfGenericType(typeof(IEnumerable<>)).First();
            }

            if (IsOfType<IXFace>(type))
            {
                return BitmapLabelType_e.SelectFace;
            }
            else if (IsOfType<IXEdge>(type))
            {
                return BitmapLabelType_e.SelectEdge;
            }
            else if (IsOfType<IXComponent>(type))
            {
                return BitmapLabelType_e.SelectComponent;
            }
            else if (IsOfType<IXDimension>(type))
            {
                return BitmapLabelType_e.LinearDistance;
            }

            return null;
        }

        /// <summary>
        /// Checks assignability against requested CAD interface type.
        /// <para>检查是否可赋值为指定 CAD 接口类型。</para>
        /// </summary>
        private static bool IsOfType<T>(Type t) => typeof(T).IsAssignableFrom(t);
    }
}
