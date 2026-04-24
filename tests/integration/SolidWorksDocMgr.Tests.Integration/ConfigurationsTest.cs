// -*- coding: utf-8 -*-
// tests/integration/SolidWorksDocMgr.Tests.Integration/ConfigurationsTest.cs
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Base;
using Xarial.XCad.Documents;
using Xarial.XCad.Documents.Enums;
using Xarial.XCad.Exceptions;
using Xarial.XCad.SwDocumentManager.Documents;

namespace SolidWorksDocMgr.Tests.Integration
{
    /// <summary>
    /// 配置相关集成测试，验证 SOLIDWORKS Document Manager 对文档配置的访问能力。
    /// 测试内容包括：活动配置、配置遍历、按名称查找、删除配置、零件号、BOM 子件显示等。
    /// </summary>
    public class ConfigurationsTest : IntegrationTests
    {
        /// <summary>
        /// 测试获取活动配置的名称。
        /// 验证当前激活的配置是否为预期的配置。
        /// </summary>
        [Test]
        public void ActiveConfTest()
        {
            string name;

            using (var doc = OpenDataDocument("Configs1.SLDPRT"))
            {
                name = (m_App.Documents.Active as ISwDmDocument3D).Configurations.Active.Name;
            }

            Assert.AreEqual("Conf3", name);
        }

        /// <summary>
        /// 测试遍历所有配置（包括嵌套子配置）。
        /// 验证配置层级结构和名称顺序是否正确。
        /// </summary>
        [Test]
        public void IterateConfsTest()
        {
            string[] confNames;

            using (var doc = OpenDataDocument("Configs1.SLDPRT"))
            {
                confNames = (m_App.Documents.Active as ISwDmDocument3D).Configurations.Select(x => x.Name).ToArray();
            }

            Assert.That(confNames.SequenceEqual(new string[]
            {
                "Conf1", "Conf2", "SubConf1", "SubSubConf1", "SubConf2", "Conf3"
            }));
        }

        /// <summary>
        /// 测试通过不同方式获取配置：直接索引、TryGet 方法。
        /// 验证索引器在配置不存在时抛出异常，而 TryGet 返回 false。
        /// </summary>
        [Test]
        public void GetConfigByNameTest()
        {
            IXConfiguration conf1;
            IXConfiguration conf2;
            IXConfiguration conf3;
            bool r1;
            bool r2;
            Exception e1 = null;

            using (var doc = OpenDataDocument("Configs1.SLDPRT"))
            {
                var confs = (m_App.Documents.Active as ISwDmDocument3D).Configurations;

                conf1 = confs["Conf1"];
                r1 = confs.TryGet("Conf2", out conf2);
                r2 = confs.TryGet("Conf4", out conf3);

                try
                {
                    var conf4 = confs["Conf5"];
                }
                catch (EntityNotFoundException ex)
                {
                    e1 = ex;
                }
            }

            Assert.IsNotNull(conf1);
            Assert.IsNotNull(conf2);
            Assert.IsNull(conf3);
            Assert.IsTrue(r1);
            Assert.IsFalse(r2);
            Assert.IsNotNull(e1);
        }

        /// <summary>
        /// 测试批量删除配置。
        /// 验证删除后剩余配置的数量和活动配置名称是否正确。
        /// </summary>
        [Test]
        public void DeleteConfsTest()
        {
            int count;
            string name;

            using (var doc = OpenDataDocument("Configs1.SLDPRT"))
            {
                var confsToDelete
                    = (m_App.Documents.Active as ISwDmDocument3D).Configurations
                    .Where(c => c.Name != "Conf3").ToArray();

                (m_App.Documents.Active as ISwDmDocument3D).Configurations.RemoveRange(confsToDelete);

                count = m_App.Documents.Active.Document.ConfigurationManager.GetConfigurationCount();
                name = m_App.Documents.Active.Document.ConfigurationManager.GetActiveConfigurationName();
            }

            Assert.AreEqual(1, count);
            Assert.AreEqual("Conf3", name);
        }

        /// <summary>
        /// 测试零件号（Part Number）的读取。
        /// 配置可以指定不同的零件号，Default 配置使用文档名称作为零件号。
        /// </summary>
        [Test]
        public void PartNumberTest()
        {
            string p1;
            string p2;
            string p3;
            string p4;

            using (var doc = OpenDataDocument("PartNumber1.SLDPRT"))
            {
                var confs = (m_App.Documents.Active as ISwDmDocument3D).Configurations;
                p1 = confs["Default"].PartNumber;
                p2 = confs["Conf1"].PartNumber;
                p3 = confs["Conf4"].PartNumber;
                p4 = confs["Conf5"].PartNumber;
            }

            Assert.AreEqual("PartNumber1", p1);
            Assert.AreEqual("Conf1", p2);
            Assert.AreEqual("Conf3", p3);
            Assert.AreEqual("ABC", p4);
        }

        /// <summary>
        /// 测试 BOM 子件显示方式的读取。
        /// Show/Hide/Promote 三种方式影响 BOM 中子件的显示方式。
        /// </summary>
        [Test]
        public void BomChildrenDisplayTest()
        {
            BomChildrenSolving_e s1;
            BomChildrenSolving_e s2;
            BomChildrenSolving_e s3;

            using (var doc = OpenDataDocument("BomChildrenDisplay.SLDASM"))
            {
                var confs = (m_App.Documents.Active as ISwDmDocument3D).Configurations;
                s1 = confs["Conf1"].BomChildrenSolving;
                s2 = confs["Conf2"].BomChildrenSolving;
                s3 = confs["Conf3"].BomChildrenSolving;
            }

            Assert.AreEqual(BomChildrenSolving_e.Show, s1);
            Assert.AreEqual(BomChildrenSolving_e.Hide, s2);
            Assert.AreEqual(BomChildrenSolving_e.Promote, s3);
        }

        /// <summary>
        /// 测试配置层级关系（父配置）。
        /// 验证子配置能正确找到其父配置，根配置返回 null。
        /// </summary>
        [Test]
        public void ParentConfTest()
        {
            string c1, c2, c3, c4, c5, c6;

            using (var doc = OpenDataDocument(@"Assembly16\Part1.SLDPRT"))
            {
                var part = m_App.Documents.Active as ISwDmDocument3D;

                c1 = part.Configurations["SubConfA"].Parent?.Name;
                c2 = part.Configurations["SubConfA"].Parent.Parent?.Name;

                c3 = part.Configurations["ConfB"].Parent?.Name;

                c4 = part.Configurations["SubSubConf1"].Parent?.Name;
                c5 = part.Configurations["SubConf1"].Parent?.Name;
                c6 = part.Configurations["SubConf1"].Parent.Parent?.Name;
            }

            Assert.AreEqual("ConfA", c1);
            Assert.AreEqual(null, c2);
            Assert.AreEqual(null, c3);
            Assert.AreEqual("SubConf1", c4);
            Assert.AreEqual("Default", c5);
            Assert.AreEqual(null, c6);
        }

        /// <summary>
        /// 测试组件引用的配置及其父配置关系。
        /// 组件引用的配置可能是装配体配置或零件配置，验证类型转换正确。
        /// </summary>
        [Test]
        public void ParentConfComponentTest()
        {
            IXConfiguration conf1, conf2, conf3, conf4, conf5;

            string c1, c2, c3, c4, c5, c6, c7;

            using (var doc = OpenDataDocument(@"Assembly16\Assem1.SLDASM"))
            {
                var assm = m_App.Documents.Active as ISwDmAssembly;
                var comp1 = assm.Configurations.Active.Components["Part1-1"];
                var comp2 = assm.Configurations.Active.Components["Part1-2"];
                var comp3 = assm.Configurations.Active.Components["SubAssem1-1"];
                var comp4 = assm.Configurations.Active.Components["SubAssem1-2"];

                conf1 = comp1.ReferencedConfiguration.Parent;
                c1 = conf1?.Name;

                conf2 = conf1.Parent;
                c2 = conf2?.Name;

                conf3 = conf2.Parent;
                c3 = conf3?.Name;

                c4 = comp2.ReferencedConfiguration.Parent?.Name;

                c5 = comp3.ReferencedConfiguration.Parent?.Name;

                conf4 = comp4.ReferencedConfiguration.Parent;
                c6 = conf4?.Name;

                conf5 = conf4.Parent;
                c7 = conf5?.Name;
            }

            Assert.AreEqual("SubConf1", c1);
            Assert.AreEqual("Default", c2);
            Assert.AreEqual(null, c3);
            Assert.IsInstanceOf<IXPartConfiguration>(conf1);
            Assert.IsInstanceOf<IXPartConfiguration>(conf2);
            Assert.IsNull(conf3);
            Assert.AreEqual(null, c4);
            Assert.AreEqual(null, c5);
            Assert.AreEqual("Default", c6);
            Assert.IsInstanceOf<IXAssemblyConfiguration>(conf4);
            Assert.IsNull(conf5);
            Assert.AreEqual(null, c7);
        }
    }
}
