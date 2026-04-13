//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.Utils.PageBuilder.Base;

namespace Xarial.XCad.Utils.PageBuilder.PageElements
{
    /// <summary>
    /// Base group control wrapper with generic value type.
    /// <para>带泛型值类型的分组控件包装器基类。</para>
    /// </summary>
    /// <typeparam name="TVal">Group value type.<para>分组值类型。</para></typeparam>
    public abstract class Group<TVal> : Control<TVal>, IGroup
    {
#pragma warning disable CS0067

        protected override event ControlValueChangedDelegate<TVal> ValueChanged;

#pragma warning restore CS0067

        /// <summary>
        /// Initializes group control wrapper.
        /// <para>初始化分组控件包装对象。</para>
        /// </summary>
        protected Group(int id, object tag, IMetadata[] metadata) : base(id, tag, metadata)
        {
        }

        protected override TVal GetSpecificValue() => default(TVal);

        protected override void SetSpecificValue(TVal value)
        {
        }
    }

    /// <summary>
    /// Non-generic group base implementation.
    /// <para>非泛型分组基类实现。</para>
    /// </summary>
    public abstract class Group : Group<object>
    {
        /// <summary>
        /// Initializes non-generic group.
        /// <para>初始化非泛型分组。</para>
        /// </summary>
        protected Group(int id, object tag, IMetadata[] metadata) : base(id, tag, metadata)
        {
        }

        public override void Focus()
        {
        }
    }
}