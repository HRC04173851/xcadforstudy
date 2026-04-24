// -*- coding: utf-8 -*-
// tests/Toolkit.Tests/PageBuilderTests.cs

//*********************************************************************
//xCAD
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad;
using Xarial.XCad.Base;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.Utils.PageBuilder;
using Xarial.XCad.Utils.PageBuilder.Attributes;
using Xarial.XCad.Utils.PageBuilder.Base;
using Xarial.XCad.Utils.PageBuilder.Binders;
using Xarial.XCad.Utils.PageBuilder.Constructors;
using Xarial.XCad.Utils.PageBuilder.Core;
using Xarial.XCad.Utils.PageBuilder.PageElements;

namespace Toolkit.Tests
{
    /// <summary>
    /// 测试 PageBuilder 页面构建器的控件 ID 分配功能。
    /// PageBuilder 根据数据模型的属性自动创建控件，并分配唯一 ID。
    /// </summary>
    public class PageBuilderTests
    {
        #region Mocks

        /// <summary>
        /// ControlMock：控件的测试用实现。
        /// </summary>
        public class ControlMock : Control<object>
        {
            public override bool Enabled { get; set; }
            public override bool Visible { get; set; }

#pragma warning disable CS0067
            protected override event ControlValueChangedDelegate<object> ValueChanged;
#pragma warning restore

            public ControlMock(int id, object tag) : base(id, tag, null)
            {
            }

            protected override object GetSpecificValue()
            {
                return null;
            }

            protected override void SetSpecificValue(object value)
            {
            }

            public override void ShowTooltip(string title, string msg)
            {
            }

            public override void Focus()
            {
            }
        }

        /// <summary>
        /// GroupMock：组的测试用实现。
        /// </summary>
        public class GroupMock : Group
        {
            public GroupMock(int id, object tag) : base(id, tag, null)
            {
            }

            public override bool Enabled { get; set; }
            public override bool Visible { get; set; }

            public override void ShowTooltip(string title, string msg)
            {
            }
        }

        /// <summary>
        /// PageMock：页面的测试用实现，包含控件列表。
        /// </summary>
        public class PageMock : Page
        {
            public List<ControlMock> Controls { get; } = new List<ControlMock>();

            public override bool Enabled { get; set; }
            public override bool Visible { get; set; }

            public override void ShowTooltip(string title, string msg)
            {
            }
        }

        /// <summary>
        /// ControlMockConstructor：控件构造器的测试用实现。
        /// 支持自定义 ID 范围选择器。
        /// </summary>
        [DefaultType(typeof(SpecialTypes.AnyType))]
        public class ControlMockConstructor : PageElementConstructor<ControlMock, GroupMock, PageMock>
        {
            private readonly Func<int> m_IdRangeSelector;

            public ControlMockConstructor(Func<int> idRangeSelector = null)
            {
                m_IdRangeSelector = idRangeSelector;
            }

            protected override ControlMock Create(IGroup parentGroup, IAttributeSet atts, IMetadata[] metadata, ref int numberOfUsedIds)
            {
                if (m_IdRangeSelector != null)
                {
                    numberOfUsedIds = m_IdRangeSelector.Invoke();
                }

                switch (parentGroup)
                {
                    case PageMock page:
                        var ctrl = new ControlMock(atts.Id, atts.Tag);
                        page.Controls.Add(ctrl);
                        return ctrl;

                    case GroupMock grp:
                        return new ControlMock(atts.Id, atts.Tag);

                    default:
                        throw new NotSupportedException();
                }
            }
        }

        /// <summary>
        /// PageMockConstructor：页面构造器的测试用实现。
        /// </summary>
        public class PageMockConstructor : PageConstructor<PageMock>
        {
            protected override PageMock Create(IAttributeSet atts)
            {
                return new PageMock();
            }
        }

        /// <summary>
        /// PageBuilderMock：PageBuilder 的测试用实现，使用所有 Mock 对象。
        /// </summary>
        public class PageBuilderMock : Xarial.XCad.Utils.PageBuilder.PageBuilderBase<PageMock, GroupMock, ControlMock>
        {
            public PageBuilderMock(Func<int> idRangeSelector = null)
                : base(new Moq.Mock<IXApplication>().Object,
                      new TypeDataBinder(new Mock<IXLogger>().Object),
                      new PageMockConstructor(),
                      new ControlMockConstructor(idRangeSelector))
            {
            }
        }

        /// <summary>
        /// DataModel1：包含 3 个属性的测试数据模型。
        /// </summary>
        public class DataModel1
        {
            public string Prp1 { get; set; }
            public string Prp2 { get; set; }
            public string Prp3 { get; set; }
        }

        #endregion

        /// <summary>
        /// 测试用例目的：验证默认情况下控件 ID 从 0 开始连续分配。
        /// DataModel1 有 3 个属性，应创建 3 个控件，ID 分别为 0, 1, 2。
        /// </summary>
        [Test]
        public void CreatePageIdsTest()
        {
            var builder = new PageBuilderMock();
            var page = builder.CreatePage<DataModel1>(x => null, new Mock<IContextProvider>().Object);

            Assert.AreEqual(3, page.Controls.Count);
            Assert.AreEqual(0, page.Controls[0].Id);
            Assert.AreEqual(1, page.Controls[1].Id);
            Assert.AreEqual(2, page.Controls[2].Id);
        }

        /// <summary>
        /// 测试用例目的：验证自定义 ID 范围选择器对 ID 分配的影响。
        /// 测试场景：第 2 个控件（索引=1）使用不同的 ID 范围。
        /// 预期结果：控件 ID 为 0, 1, 4（而非 0, 1, 2）
        /// </summary>
        [Test]
        public void CreatePageIdsRangeTest()
        {
            int ctrlIndex = 0;
            var builder = new PageBuilderMock(() =>
            {
                int idRange = 1;

                if (ctrlIndex == 1)
                {
                    idRange = 3; // 第 2 个控件跳过 2 个 ID
                }

                ctrlIndex++;
                return idRange;
            });
            var page = builder.CreatePage<DataModel1>(x => null, new Mock<IContextProvider>().Object);

            Assert.AreEqual(3, page.Controls.Count);
            Assert.AreEqual(0, page.Controls[0].Id);
            Assert.AreEqual(1, page.Controls[1].Id);
            Assert.AreEqual(4, page.Controls[2].Id); // 第 3 个控件使用 ID=4（跳过了 2,3）
        }
    }
}
