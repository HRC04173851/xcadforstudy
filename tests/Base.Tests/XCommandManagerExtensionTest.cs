// -*- coding: utf-8 -*-
// tests/Base.Tests/XCommandManagerExtensionTest.cs

using Moq;
using NUnit.Framework;
using Xarial.XCad.Base.Attributes;
using Xarial.XCad.UI.Commands;
using Xarial.XCad.UI.Commands.Attributes;
using Xarial.XCad.UI.Commands.Structures;
using System.Linq;
using Xarial.XCad.UI.Exceptions;
using System.Collections.Generic;

namespace Base.Tests
{
    /// <summary>
    /// 测试 IXCommandManager 的扩展方法，特别是命令组（CommandGroup）的添加和管理功能。
    /// 包括自动 ID 分配、自定义属性解析、父组关联等功能。
    /// </summary>
    public class XCommandManagerExtensionTest
    {
        /// <summary>
        /// ResourcesMock：模拟命令图标资源的字节数组。
        /// 用于测试命令图标加载功能。
        /// </summary>
        public static class ResourcesMock
        {
            public static byte[] Res1 { get; set; } = new byte[] { 0, 1 };
            public static byte[] Res2 { get; set; } = new byte[] { 2, 3 };
            public static byte[] Res3 { get; set; } = new byte[] { 4, 5 };
        }

        /// <summary>
        /// Commands1_e：无自定义属性的命令枚举。
        /// 用于测试默认命令组创建行为。
        /// </summary>
        public enum Commands1_e
        {
            Cmd1,
            Cmd2
        }

        /// <summary>
        /// Commands2_e：带完整自定义属性的命令枚举。
        /// 包括 Title、Summary、Icon、CommandGroupInfo、CommandItemInfo 等属性。
        /// 用于测试属性解析的正确性。
        /// </summary>
        [Title("CG1")]
        [Summary("D0")]
        [Icon(typeof(ResourcesMock), nameof(ResourcesMock.Res1))]
        [CommandGroupInfo(150)]
        public enum Commands2_e
        {
            [Title("C1")]
            [Summary("D1")]
            [Icon(typeof(ResourcesMock), nameof(ResourcesMock.Res2))]
            // Cmd1 仅在装配工作区显示，带分隔条
            [CommandItemInfo(false, true, Xarial.XCad.UI.Commands.Enums.WorkspaceTypes_e.Assembly, true, Xarial.XCad.UI.Commands.Enums.RibbonTabTextDisplay_e.NoText)]
            [CommandSpacer]
            Cmd1,

            [Title("C2")]
            [Summary("D2")]
            [Icon(typeof(ResourcesMock), nameof(ResourcesMock.Res3))]
            Cmd2
        }

        /// <summary>
        /// Commands3_e：指定父组 ID=125 的命令枚举。
        /// 用于测试命令组父子关系。
        /// </summary>
        [CommandGroupParent(125)]
        public enum Commands3_e
        {
            Cmd1,
            Cmd2
        }

        /// <summary>
        /// Commands4_e：指定不存在的父组 ID=10 的命令枚举。
        /// 用于测试父组未找到时的异常抛出。
        /// </summary>
        [CommandGroupParent(10)]
        public enum Commands4_e
        {
            Cmd1,
            Cmd2
        }

        /// <summary>
        /// Commands5_e：用于测试循环依赖检测的枚举。
        /// </summary>
        [CommandGroupInfo(20)]
        public enum Commands5_e
        {
            Cmd1,
            Cmd2
        }

        // 无属性的命令枚举，用于测试自动 ID 分配
        public enum Commands6_1
        {
            Cmd1,
            Cmd2
        }

        // CommandGroupInfo(3) 手动指定 ID
        [CommandGroupInfo(3)]
        public enum Commands7_1
        {
            Cmd1,
            Cmd2
        }

        // CommandGroupInfo(4) 手动指定 ID
        [CommandGroupInfo(4)]
        public enum Commands8_1
        {
            Cmd1,
            Cmd2
        }

        // 无属性
        public enum Commands9_1
        {
            Cmd1,
            Cmd2
        }

        // 无属性
        public enum Commands10_1
        {
            Cmd1,
            Cmd2
        }

        /// <summary>
        /// 测试用例目的：验证 AddCommandGroup 方法创建基本命令组的正确性。
        /// 包括 ID 自动分配（默认从1开始）和命令标题解析。
        /// </summary>
        [Test]
        public void TestAddCommandGroup()
        {
            CommandGroupSpec res = null;

            var mock = new Mock<IXCommandManager>();

            mock.Setup(m => m.AddCommandGroup(It.IsAny<CommandGroupSpec>()))
                .Callback((CommandGroupSpec s) =>
                {
                    res = s;
                }).Returns(new Mock<IXCommandGroup>().Object);

            mock.Object.AddCommandGroup<Commands1_e>();

            // 验证命令组 ID 为 1（自动分配）
            Assert.AreEqual(1, res.Id);
            // 验证命令数量为 2
            Assert.AreEqual(2, res.Commands.Length);
            // 验证命令标题（枚举成员名称）
            Assert.AreEqual("Cmd1", res.Commands[0].Title);
            Assert.AreEqual("Cmd2", res.Commands[1].Title);
        }

        /// <summary>
        /// 测试用例目的：验证 AddCommandGroup 方法的自动 ID 分配逻辑。
        /// 规则：
        /// - 手动指定 ID 的使用指定值
        /// - 无属性的枚举自动分配最小可用 ID
        /// 预期结果：ID 分配为 g1=1, g2=3, g3=4, g4=2, g5=5
        /// </summary>
        [Test]
        public void TestAddCommandGroupAutoUserIds()
        {
            var cmdMgrMock = new Mock<IXCommandManager>();

            var cmgGrps = new List<IXCommandGroup>();

            // 模拟 CommandGroups 属性返回当前已添加的命令组
            cmdMgrMock.Setup(m => m.CommandGroups).Returns(() => cmgGrps.ToArray());

            cmdMgrMock.Setup(m => m.AddCommandGroup(It.IsAny<CommandGroupSpec>()))
                .Returns((CommandGroupSpec s) =>
                {
                    var cmdGrpMock = new Mock<IXCommandGroup>();
                    cmdGrpMock.Setup(x => x.Spec).Returns(s);

                    var cmdGrp = cmdGrpMock.Object;

                    cmgGrps.Add(cmdGrp);

                    return cmdGrp;
                });

            var cmdMgr = cmdMgrMock.Object;

            var g1 = cmdMgr.AddCommandGroup<Commands6_1>(); // 无属性，自动分配最小可用 ID=1
            var g2 = cmdMgr.AddCommandGroup<Commands7_1>(); // [CommandGroupInfo(3)] 指定 ID=3
            var g3 = cmdMgr.AddCommandGroup<Commands8_1>(); // [CommandGroupInfo(4)] 指定 ID=4
            var g4 = cmdMgr.AddCommandGroup<Commands9_1>(); // 无属性，自动分配（1,3,4 被占用，分配 2）
            var g5 = cmdMgr.AddCommandGroup<Commands10_1>(); // 无属性，自动分配（1,2,3,4 被占用，分配 5）

            // 验证 ID 分配结果
            Assert.AreEqual(1, g1.Spec.Id);
            Assert.AreEqual(3, g2.Spec.Id);
            Assert.AreEqual(4, g3.Spec.Id);
            Assert.AreEqual(2, g4.Spec.Id);
            Assert.AreEqual(5, g5.Spec.Id);
        }

        /// <summary>
        /// 测试用例目的：验证 AddCommandGroup 方法正确解析命令组和命令的自定义属性。
        /// 包括：组级别属性（Title、Tooltip、Icon）和命令级别属性。
        /// </summary>
        [Test]
        public void TestAddCommandGroupCustomAttributes()
        {
            CommandGroupSpec res = null;

            var mock = new Mock<IXCommandManager>();

            mock.Setup(m => m.AddCommandGroup(It.IsAny<CommandGroupSpec>()))
                .Callback((CommandGroupSpec s) =>
                {
                    res = s;
                }).Returns(new Mock<IXCommandGroup>().Object);

            mock.Object.AddCommandGroup<Commands2_e>();

            // 验证组级别属性
            Assert.AreEqual(150, res.Id);
            Assert.AreEqual("CG1", res.Title);
            Assert.AreEqual("D0", res.Tooltip);
            Assert.That(res.Icon.Buffer.SequenceEqual(new byte[] { 0, 1 }));

            Assert.AreEqual(2, res.Commands.Length);

            // 验证命令1的属性（Cmd1）
            Assert.AreEqual("C1", res.Commands[0].Title);
            Assert.AreEqual("D1", res.Commands[0].Tooltip);
            Assert.IsTrue(res.Commands[0].HasSpacer);      // 有分隔条
            Assert.IsFalse(res.Commands[0].HasMenu);        // 无菜单
            Assert.IsTrue(res.Commands[0].HasToolbar);     // 有工具栏
            Assert.IsTrue(res.Commands[0].HasRibbon);      // 有 Ribbon
            Assert.AreEqual(Xarial.XCad.UI.Commands.Enums.RibbonTabTextDisplay_e.NoText, res.Commands[0].RibbonTextStyle);
            Assert.AreEqual(Xarial.XCad.UI.Commands.Enums.WorkspaceTypes_e.Assembly, res.Commands[0].SupportedWorkspace);
            Assert.That(res.Commands[0].Icon.Buffer.SequenceEqual(new byte[] { 2, 3 }));

            // 验证命令2的属性（Cmd2）
            Assert.AreEqual("C2", res.Commands[1].Title);
            Assert.AreEqual("D2", res.Commands[1].Tooltip);
            Assert.AreEqual(Xarial.XCad.UI.Commands.Enums.WorkspaceTypes_e.All, res.Commands[1].SupportedWorkspace);
            Assert.That(res.Commands[1].Icon.Buffer.SequenceEqual(new byte[] { 4, 5 }));
        }

        /// <summary>
        /// 测试用例目的：验证命令组的父组关联功能和异常处理。
        /// 包括：正常父组关联、父组未找到异常、循环依赖异常。
        /// </summary>
        [Test]
        public void TestAddCommandGroupParent()
        {
            CommandGroupSpec res = null;

            // 创建 ID=125 的父命令组
            var parentSpec = new CommandGroupSpec(125);

            var cmdParentMock = new Mock<IXCommandGroup>();
            cmdParentMock.Setup(m => m.Spec).Returns(parentSpec);

            var mock = new Mock<IXCommandManager>();

            mock.Setup(m => m.AddCommandGroup(It.IsAny<CommandGroupSpec>()))
                .Callback((CommandGroupSpec s) =>
                {
                    res = s;
                }).Returns(new Mock<IXCommandGroup>().Object);

            mock.Setup(m => m.CommandGroups).Returns(new IXCommandGroup[] { cmdParentMock.Object });

            // Commands3_e 指定了 [CommandGroupParent(125)]，父组存在
            mock.Object.AddCommandGroup<Commands3_e>();

            // 验证父组关联正确
            Assert.AreEqual(parentSpec, res.Parent);

            // 验证父组未找到时抛出 ParentGroupNotFoundException
            Assert.Throws<ParentGroupNotFoundException>(() => mock.Object.AddCommandGroup<Commands4_e>());
            // 验证循环依赖检测（Commands5_e 的父组 ID=20，但自身也是 20）
            Assert.Throws<ParentGroupCircularDependencyException>(() => mock.Object.CreateSpecFromEnum<Commands5_e>(new CommandGroupSpec(20), null));
        }
    }
}