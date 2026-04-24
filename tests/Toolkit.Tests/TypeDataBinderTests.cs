// -*- coding: utf-8 -*-
// tests/Toolkit.Tests/TypeDataBinderTests.cs

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
using Xarial.XCad.Base;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.Utils.PageBuilder;
using Xarial.XCad.Utils.PageBuilder.Base;
using Xarial.XCad.Utils.PageBuilder.Binders;

namespace Toolkit.Tests
{
    /// <summary>
    /// 测试 TypeDataBinder 类型数据绑定器的功能。
    /// TypeDataBinder 负责将数据模型的属性绑定到 UI 控件，并处理嵌套组结构。
    /// </summary>
    public class TypeDataBinderTests
    {
        #region Mocks

        /// <summary>
        /// DataModelMock1：简单数据模型，包含 3 个基本类型属性。
        /// </summary>
        public class DataModelMock1
        {
            public string Field1 { get; set; }
            public int Field2 { get; set; }
            public double Field3 { get; set; }
        }

        /// <summary>
        /// DataModelMock2：包含嵌套组的数据模型。
        /// Group1 属性是另一个数据模型类型的嵌套对象。
        /// </summary>
        public class DataModelMock2
        {
            public string Field1 { get; set; }
            public int Field2 { get; set; }
            public double Field3 { get; set; }

            public DataModelMock1 Group1 { get; set; }
        }

        /// <summary>
        /// DataModelMock3：包含多层嵌套组的数据模型。
        /// Group2 属性包含 DataModelMock2，而 DataModelMock2 又包含 DataModelMock1。
        /// </summary>
        public class DataModelMock3
        {
            public string Field1 { get; set; }

            public DataModelMock2 Group2 { get; set; }
        }

        #endregion

        /// <summary>
        /// 测试用例目的：验证简单数据模型（无嵌套）的绑定功能。
        /// 预期结果：
        /// - 创建 3 个绑定（对应 3 个属性）
        /// - 每个绑定的控件描述符包含正确的名称和数据类型
        /// </summary>
        [Test]
        public void TestBindSimple()
        {
            var binder = new TypeDataBinder(new Mock<IXLogger>().Object);
            IEnumerable<IBinding> bindings;

            IRawDependencyGroup dependencies;

            binder.Bind<DataModelMock1>(
                a =>
                {
                    return new Mock<IPage>().Object;
                },
                (Type t, IAttributeSet a, IGroup p, IMetadata[] md, out int r) =>
                {
                    r = 1;
                    return new Mock<IControl>().Object;
                }, x => null, new Mock<IContextProvider>().Object, out bindings, out dependencies, out _);

            var d1 = (bindings.ElementAt(0) as PropertyInfoBinding<DataModelMock1>).ControlDescriptor;
            var d2 = (bindings.ElementAt(1) as PropertyInfoBinding<DataModelMock1>).ControlDescriptor;
            var d3 = (bindings.ElementAt(2) as PropertyInfoBinding<DataModelMock1>).ControlDescriptor;

            Assert.AreEqual(3, bindings.Count());

            // 验证第 1 个属性的绑定信息
            Assert.AreEqual("Field1", d1.Name);
            Assert.AreEqual(typeof(string), d1.DataType);

            // 验证第 2 个属性的绑定信息
            Assert.AreEqual("Field2", d2.Name);
            Assert.AreEqual(typeof(int), d2.DataType);

            // 验证第 3 个属性的绑定信息
            Assert.AreEqual("Field3", d3.Name);
            Assert.AreEqual(typeof(double), d3.DataType);
        }

        /// <summary>
        /// 测试用例目的：验证包含嵌套组的数据模型的绑定功能。
        /// 嵌套组内的属性也会被递归绑定，生成展平的绑定列表。
        /// 预期结果：DataModelMock2 产生 7 个绑定（3 个顶层属性 + 4 个 Group1 的 3 个属性）
        /// </summary>
        [Test]
        public void TestBindGroup()
        {
            var binder = new TypeDataBinder(new Mock<IXLogger>().Object);
            IEnumerable<IBinding> bindings;

            IRawDependencyGroup dependencies;

            binder.Bind<DataModelMock2>(
                a =>
                {
                    return new Mock<IPage>().Object;
                },
                (Type t, IAttributeSet a, IGroup p, IMetadata[] md, out int r) =>
                {
                    r = 1;
                    if (t == typeof(DataModelMock1))
                    {
                        return new Mock<IGroup>().Object;
                    }
                    else
                    {
                        return new Mock<IControl>().Object;
                    }
                }, x => null, new Mock<IContextProvider>().Object, out bindings, out dependencies, out _);

            var d1 = (bindings.ElementAt(0) as PropertyInfoBinding<DataModelMock2>).ControlDescriptor;
            var d2 = (bindings.ElementAt(1) as PropertyInfoBinding<DataModelMock2>).ControlDescriptor;
            var d3 = (bindings.ElementAt(2) as PropertyInfoBinding<DataModelMock2>).ControlDescriptor;
            var d4 = (bindings.ElementAt(3) as PropertyInfoBinding<DataModelMock2>).ControlDescriptor;
            var d5 = (bindings.ElementAt(4) as PropertyInfoBinding<DataModelMock2>).ControlDescriptor;
            var d6 = (bindings.ElementAt(5) as PropertyInfoBinding<DataModelMock2>).ControlDescriptor;
            var d7 = (bindings.ElementAt(6) as PropertyInfoBinding<DataModelMock2>).ControlDescriptor;

            Assert.AreEqual(7, bindings.Count());

            // 验证顶层属性（Field1, Field2, Field3）
            Assert.AreEqual("Field1", d1.Name);
            Assert.AreEqual(typeof(string), d1.DataType);

            Assert.AreEqual("Field2", d2.Name);
            Assert.AreEqual(typeof(int), d2.DataType);

            Assert.AreEqual("Field3", d3.Name);
            Assert.AreEqual(typeof(double), d3.DataType);

            // 验证嵌套组（Group1）
            Assert.AreEqual("Group1", d4.Name);
            Assert.AreEqual(typeof(DataModelMock1), d4.DataType);

            // 验证 Group1 内的属性
            Assert.AreEqual("Field1", d5.Name);
            Assert.AreEqual(typeof(string), d5.DataType);

            Assert.AreEqual("Field2", d6.Name);
            Assert.AreEqual(typeof(int), d6.DataType);

            Assert.AreEqual("Field3", d7.Name);
            Assert.AreEqual(typeof(double), d7.DataType);
        }

        /// <summary>
        /// 测试用例目的：验证嵌套组的父子关系（Parent）正确建立。
        /// 顶层属性应属于 Page，嵌套的属性应属于对应的 Group。
        /// </summary>
        [Test]
        public void TestBindParent()
        {
            var binder = new TypeDataBinder(new Mock<IXLogger>().Object);
            IEnumerable<IBinding> bindings;

            IPage page = null;
            IGroup grp1 = null;
            IGroup grp2 = null;

            var parents = new Dictionary<IControl, IGroup>();

            IRawDependencyGroup dependencies;

            binder.Bind<DataModelMock3>(
                a =>
                {
                    page = new Moq.Mock<IPage>().Object;
                    return page;
                },
                (Type t, IAttributeSet a, IGroup p, IMetadata[] md, out int r) =>
                {
                    r = 1;
                    if (t == typeof(DataModelMock1))
                    {
                        grp1 = new Mock<IGroup>().Object;
                        parents.Add(grp1, p);
                        return grp1;
                    }
                    if (t == typeof(DataModelMock2))
                    {
                        grp2 = new Mock<IGroup>().Object;
                        parents.Add(grp2, p);
                        return grp2;
                    }
                    else
                    {
                        var ctrl = new Mock<IControl>().Object;
                        parents.Add(ctrl, p);
                        return ctrl;
                    }
                }, x => null, new Mock<IContextProvider>().Object, out bindings, out dependencies, out _);

            // 验证 DataModelMock3 的顶层属性属于 Page
            Assert.AreEqual(page,
                parents[(bindings.ElementAt(0) as PropertyInfoBinding<DataModelMock3>).Control]);
            Assert.AreEqual(page,
                parents[(bindings.ElementAt(1) as PropertyInfoBinding<DataModelMock3>).Control]);

            // 验证 DataModelMock2 的属性属于 grp2
            Assert.AreEqual(grp2,
                parents[(bindings.ElementAt(2) as PropertyInfoBinding<DataModelMock3>).Control]);
            Assert.AreEqual(grp2,
                parents[(bindings.ElementAt(3) as PropertyInfoBinding<DataModelMock3>).Control]);
            Assert.AreEqual(grp2,
                parents[(bindings.ElementAt(4) as PropertyInfoBinding<DataModelMock3>).Control]);
            Assert.AreEqual(grp2,
                parents[(bindings.ElementAt(5) as PropertyInfoBinding<DataModelMock3>).Control]);

            // 验证 DataModelMock1 的属性属于 grp1
            Assert.AreEqual(grp1,
                parents[(bindings.ElementAt(6) as PropertyInfoBinding<DataModelMock3>).Control]);
            Assert.AreEqual(grp1,
                parents[(bindings.ElementAt(7) as PropertyInfoBinding<DataModelMock3>).Control]);
            Assert.AreEqual(grp1,
                parents[(bindings.ElementAt(8) as PropertyInfoBinding<DataModelMock3>).Control]);
        }

        /// <summary>
        /// 测试用例目的：验证控件 ID 的自动分配（从 0 开始递增）。
        /// 每个属性绑定时都会传入 AttributeSet，其中包含 Id。
        /// </summary>
        [Test]
        public void TestBindIds()
        {
            var binder = new TypeDataBinder(new Mock<IXLogger>().Object);
            IEnumerable<IBinding> bindings;

            IPage page = null;

            IRawDependencyGroup dependencies;

            binder.Bind<DataModelMock1>(
                a =>
                {
                    page = new Mock<IPage>().Object;
                    return page;
                },
                (Type t, IAttributeSet a, IGroup p, IMetadata[] md, out int r) =>
                {
                    r = 1;
                    var ctrlMock = new Mock<IControl>();
                    ctrlMock.SetupGet(c => c.Id).Returns(() => a.Id);
                    return ctrlMock.Object;
                }, x => null, new Mock<IContextProvider>().Object, out bindings, out dependencies, out _);

            // 验证控件 ID 分别为 0, 1, 2
            Assert.AreEqual(0, bindings.ElementAt(0).Control.Id);
            Assert.AreEqual(1, bindings.ElementAt(1).Control.Id);
            Assert.AreEqual(2, bindings.ElementAt(2).Control.Id);
        }

        /// <summary>
        /// 测试用例目的：验证属性绑定的 Attribute 处理功能。
        /// </summary>
        [Test]
        public void TestBindAttributes()
        {
        }

        /// <summary>
        /// 测试用例目的：验证绑定依赖关系的管理功能。
        /// </summary>
        [Test]
        public void TestDependencies()
        {
        }
    }
}
