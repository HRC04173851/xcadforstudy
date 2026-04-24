// -*- coding: utf-8 -*-
// tests/integration/SolidWorksDocMgr.Tests.Integration/CutListTest.cs
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Documents;
using Xarial.XCad.Enums;
using Xarial.XCad.SwDocumentManager.Documents;

namespace SolidWorksDocMgr.Tests.Integration
{
    /// <summary>
    /// 切割清单（Cut List）集成测试，验证 SOLIDWORKS Document Manager 对零件和装配体切割清单的访问能力。
    /// 切割清单主要用于焊接件和钣金件，包含材料数量、尺寸等信息。
    /// </summary>
    public class CutListTest : IntegrationTests
    {
        /// <summary>
        /// 测试钣金件的切割清单。
        /// 钣金零件通常包含多个切割体（Body），每个切割体对应一个切割清单项。
        /// </summary>
        [Test]
        public void SheetMetalCutListsTest()
        {
            Dictionary<string, int> cutListData;

            using (var doc = OpenDataDocument("SheetMetal1.SLDPRT"))
            {
                var part = (ISwDmPart)m_App.Documents.Active;
                var cutLists = part.Configurations.Active.CutLists;
                cutListData = cutLists.ToDictionary(c => c.Name, c => c.Bodies.Count());
            }

            Assert.AreEqual(2, cutListData.Count);
            Assert.That(cutListData.ContainsKey("Sheet<1>"));
            Assert.AreEqual(1, cutListData["Sheet<1>"]);
            Assert.That(cutListData.ContainsKey("Sheet<2>"));
            Assert.AreEqual(1, cutListData["Sheet<2>"]);
        }

        /// <summary>
        /// 测试不同类型的切割清单。
        /// 切割清单类型包括：Weldment（焊接件）、SheetMetal（钣金件）、SolidBody（实体）。
        /// </summary>
        [Test]
        public void CutListsTypes()
        {
            Dictionary<string, CutListType_e> cutListData;

            using (var doc = OpenDataDocument("CutListTypes_2021.SLDPRT"))
            {
                var part = (ISwDmPart)m_App.Documents.Active;
                var cutLists = part.Configurations.Active.CutLists;
                cutListData = cutLists.ToDictionary(c => c.Name, c => c.Type);
            }

            Assert.AreEqual(3, cutListData.Count);

            Assert.That(cutListData.ContainsKey("S 76.20 X 5.7<1>"));
            Assert.AreEqual(CutListType_e.Weldment, cutListData["S 76.20 X 5.7<1>"]);

            Assert.That(cutListData.ContainsKey("Sheet<1>"));
            Assert.AreEqual(CutListType_e.SheetMetal, cutListData["Sheet<1>"]);

            Assert.That(cutListData.ContainsKey("Cut-List-Item3"));
            Assert.AreEqual(CutListType_e.SolidBody, cutListData["Cut-List-Item3"]);
        }

        /// <summary>
        /// 测试焊接件的切割清单。
        /// 焊接件切割清单可能包含多个体（Body），表示不同的焊件轮廓。
        /// </summary>
        [Test]
        public void WeldmentCutListsTest()
        {
            Dictionary<string, int> cutListData;

            using (var doc = OpenDataDocument("Weldment1.SLDPRT"))
            {
                var part = (ISwDmPart)m_App.Documents.Active;
                var cutLists = part.Configurations.Active.CutLists;
                cutListData = cutLists.ToDictionary(c => c.Name, c => c.Bodies.Count());
            }

            Assert.AreEqual(1, cutListData.Count);
            Assert.That(cutListData.ContainsKey(" C CHANNEL, 76.20 X 5<1>"));
            Assert.AreEqual(3, cutListData[" C CHANNEL, 76.20 X 5<1>"]);
        }

        /// <summary>
        /// 测试过时（Outdated）切割清单的处理。
        /// 当零件几何或特征更改后，切割清单可能需要重新计算。
        /// </summary>
        [Test]
        public void OutdatedCutListsTest()
        {
            Dictionary<string, int> cutListData;

            using (var doc = OpenDataDocument("CutListsOutdated.SLDPRT"))
            {
                var part = (IXPart)m_App.Documents.Active;
                var cutLists = part.Configurations.Active.CutLists;
                cutListData = cutLists.ToDictionary(c => c.Name, c => c.Bodies.Count());
            }

            Assert.AreEqual(3, cutListData.Count);
            Assert.That(cutListData.ContainsKey(" C CHANNEL, 76.20 X 5<1>"));
            Assert.That(cutListData.ContainsKey(" C CHANNEL, 76.20 X 5<2>"));
            Assert.That(cutListData.ContainsKey(" C CHANNEL, 76.20 X 5<3>"));
            Assert.AreEqual(1, cutListData[" C CHANNEL, 76.20 X 5<1>"]);
            Assert.AreEqual(1, cutListData[" C CHANNEL, 76.20 X 5<2>"]);
            Assert.AreEqual(1, cutListData[" C CHANNEL, 76.20 X 5<3>"]);
        }

        /// <summary>
        /// 测试切割清单的 BOM 排除状态。
        /// 某些切割清单项可以被设置为不包含在物料清单中。
        /// </summary>
        [Test]
        public void ExcludeFromBomTest()
        {
            Dictionary<string, CutListStatus_e> cutListData;

            using (var doc = OpenDataDocument("CutListExcludeBom_2021.SLDPRT"))
            {
                var part = (IXPart)m_App.Documents.Active;
                var cutLists = part.Configurations.Active.CutLists;
                cutListData = cutLists.ToDictionary(c => c.Name, c => c.Status);
            }

            Assert.AreEqual((CutListStatus_e)0, cutListData["C CHANNEL 80.00 X 8<1>"]);
            Assert.AreEqual(CutListStatus_e.ExcludeFromBom, cutListData["PIPE, SCH 40, 25.40 DIA.<1>"]);
        }

        /// <summary>
        /// 测试装配体中组件的切割清单访问。
        /// 通过组件配置获取焊件零件的切割清单信息。
        /// </summary>
        [Test]
        public void ComponentsCutListTest()
        {
            Dictionary<string, int> cutListData1;
            Dictionary<string, int> cutListData2;

            using (var doc = OpenDataDocument(@"CutListsAssembly1\Assem1.SLDASM"))
            {
                var assm = (ISwDmAssembly)m_App.Documents.Active;
                cutListData1 = ((IXPartComponent)assm.Configurations.Active.Components["Part1-1"]).ReferencedConfiguration.CutLists.ToDictionary(c => c.Name, c => c.Bodies.Count());
                cutListData2 = ((IXPartComponent)assm.Configurations.Active.Components["Part1-2"]).ReferencedConfiguration.CutLists.ToDictionary(c => c.Name, c => c.Bodies.Count());
            }

            Assert.AreEqual(1, cutListData1.Count);
            Assert.That(cutListData1.ContainsKey("L 25.40 X 25.40 X 3.175<1>"));
            Assert.AreEqual(1, cutListData1["L 25.40 X 25.40 X 3.175<1>"]);

            Assert.AreEqual(1, cutListData2.Count);
            Assert.That(cutListData2.ContainsKey("PIPE 21.30 X 2.3<1>"));
            Assert.AreEqual(1, cutListData2["PIPE 21.30 X 2.3<1>"]);
        }

        /// <summary>
        /// 测试子焊接件（Sub-Weldment）的切割清单。
        /// 子焊接件是嵌套在主焊接件中的独立焊接零件。
        /// </summary>
        [Test]
        public void SubWeldmentsTest()
        {
            Dictionary<string, int> cutListData;

            using (var doc = OpenDataDocument(@"Weldment2.SLDPRT"))
            {
                var part = (IXPart)m_App.Documents.Active;
                var cutLists = part.Configurations.Active.CutLists;
                cutListData = cutLists.ToDictionary(c => c.Name, c => c.Bodies.Count());
            }

            Assert.AreEqual(4, cutListData.Count);

            Assert.That(cutListData.ContainsKey("TUBE, RECTANGULAR 60.00 X 40.00 X 3.20<2>"));
            Assert.AreEqual(2, cutListData["TUBE, RECTANGULAR 60.00 X 40.00 X 3.20<2>"]);
            Assert.That(cutListData.ContainsKey("TUBE, RECTANGULAR 60.00 X 40.00 X 3.20<3>"));
            Assert.AreEqual(1, cutListData["TUBE, RECTANGULAR 60.00 X 40.00 X 3.20<3>"]);
            Assert.That(cutListData.ContainsKey("Cut-List-Item5"));
            Assert.AreEqual(1, cutListData["Cut-List-Item5"]);
            Assert.That(cutListData.ContainsKey("Cut-List-Item6"));
            Assert.AreEqual(1, cutListData["Cut-List-Item6"]);
        }
    }
}
