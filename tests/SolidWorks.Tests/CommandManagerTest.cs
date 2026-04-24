// -*- coding: utf-8 -*-
// tests/SolidWorks.Tests/CommandManagerTest.cs

//*********************************************************************
//xCAD
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Moq;
using NUnit.Framework;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Base.Attributes;
using Xarial.XCad.SolidWorks;
using Xarial.XCad.UI.Commands;
using Xarial.XCad.UI.Commands.Attributes;

namespace SolidWorks.Tests
{
    /// <summary>
    /// 测试 CommandManager 命令管理器的命令组和上下文菜单创建功能。
    /// 注：这些测试当前被注释掉（TODO: fix unit test），可能需要修复。
    /// </summary>
    public class CommandManagerTest
    {
        #region Mocks

        /// <summary>
        /// CommandsMock_1：无自定义属性的命令枚举，用于测试默认行为。
        /// </summary>
        public enum CommandsMock_1
        {
            Cmd1,
            Cmd2
        }

        /// <summary>
        /// CommandsMock_2：带自定义属性的命令枚举，包括 Title、Description、CommandItemInfo。
        /// </summary>
        public enum CommandsMock_2
        {
            [Title("Command1")]
            [System.ComponentModel.Description("Command1 Desc")]
            [CommandItemInfo(false, true, Xarial.XCad.UI.Commands.Enums.WorkspaceTypes_e.Assembly)]
            Cmd1,
        }

        #endregion

        /// <summary>
        /// 创建模拟命令组的辅助方法。
        /// </summary>
        /// <param name="rev">SOLIDWORKS 版本号</param>
        /// <param name="grps">输出参数，存储创建的命令组和命令项</param>
        /// <returns>模拟的 SwAddInEx 实例</returns>
        private SwAddInEx CreateMockCommandGroup(string rev, Dictionary<CommandGroup, List<object[]>> grps)
        {
            var type = "";

            var addInExMock = new Mock<SwAddInEx>();

            // 创建 CommandGroup 的 Mock 对象
            var createCommandGroupMockObjectFunc = new Func<CommandGroup>(() =>
            {
                var cmdGroupMock = new Mock<CommandGroup>().SetupAllProperties();
                var cmds = new List<object[]>();
                grps.Add(cmdGroupMock.Object, cmds);
                // 模拟 AddCommandItem2 方法调用
                cmdGroupMock.Setup(m => m.AddCommandItem2(
                    It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Callback<string, int, string, string, int, string, string, int, int>(
                    (name, pos, hint, tooltip, imgList, callback, enable, userId, menuTbOpts) =>
                    {
                        cmds.Add(new object[] { name, pos, hint, tooltip, imgList, callback, enable, userId, menuTbOpts });
                    }).Returns(cmds.Count);
                cmdGroupMock.Setup(m => m.ToString()).Returns(type);

                return cmdGroupMock.Object;
            });

            var cmdMgrMock = new Mock<CommandManager>();
            var cmdGrpRes = (int)swCreateCommandGroupErrors.swCreateCommandGroup_Success;
            // 模拟 CreateCommandGroup2 方法
            cmdMgrMock.Setup(m => m.CreateCommandGroup2(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), ref cmdGrpRes))
                .Returns(() =>
                {
                    type += "CmdGrp";
                    return createCommandGroupMockObjectFunc.Invoke();
                });

            // 模拟 AddContextMenu 方法
            cmdMgrMock.Setup(m => m.AddContextMenu(It.IsAny<int>(), It.IsAny<string>())).Returns(
                () =>
                {
                    type += "CtxMenu";
                    return createCommandGroupMockObjectFunc.Invoke();
                });

            var cmdMgr = cmdMgrMock.Object;

            var swMock = new Mock<SldWorks>();
            swMock.Setup(m => m.GetCommandManager(It.IsAny<int>()))
                .Returns(cmdMgr);
            swMock.Setup(m => m.RevisionNumber()).Returns(rev);

            var addIn = addInExMock.Object;
            addIn.ConnectToSW(swMock.Object, 0);

            return addIn;
        }

        /// <summary>
        /// 测试用例目的：验证 AddCommandGroup 方法的基本功能（TODO: 被注释）。
        /// 测试不同 SOLIDWORKS 版本（23, 24, 25）下的命令组创建行为差异。
        /// 版本 23/24 使用 LargeIcon，版本 25 使用 IconList。
        /// </summary>
        [Test]
        public void AddCommandGroupBaseTest()
        {
            //TODO: fix unit test

            //var cmds1 = new Dictionary<CommandGroup, List<object[]>>();
            //var addInMock1 = CreateMockCommandGroup("23.0.0", cmds1);
            //var grp1 = addInMock1.CommandManager.AddCommandGroup<CommandsMock_1>();

            //var cmds2 = new Dictionary<CommandGroup, List<object[]>>();
            //var addInMock2 = CreateMockCommandGroup("24.0.0", cmds2);
            //var grp2 = addInMock2.CommandManager.AddCommandGroup<CommandsMock_1>();

            //var cmds3 = new Dictionary<CommandGroup, List<object[]>>();
            //var addInMock3 = CreateMockCommandGroup("25.0.0", cmds3);
            //var grp3 = addInMock3.CommandManager.AddCommandGroup<CommandsMock_2>();

            //Assert.AreEqual("CmdGrp", grp1.ToString());
            //Assert.IsFalse(string.IsNullOrEmpty(grp1.LargeMainIcon));
            //Assert.IsFalse(string.IsNullOrEmpty(grp1.SmallMainIcon));
            //Assert.IsFalse(string.IsNullOrEmpty(grp1.LargeIconList));
            //Assert.IsFalse(string.IsNullOrEmpty(grp1.SmallIconList));
            //Assert.IsNull(grp1.MainIconList);
            //Assert.IsNull(grp1.IconList);

            //Assert.AreEqual("CmdGrp", grp2.ToString());
            //Assert.IsTrue(string.IsNullOrEmpty(grp2.LargeMainIcon));
            //Assert.IsTrue(string.IsNullOrEmpty(grp2.SmallMainIcon));
            //Assert.IsTrue(string.IsNullOrEmpty(grp2.LargeIconList));
            //Assert.IsTrue(string.IsNullOrEmpty(grp2.SmallIconList));
            //Assert.AreEqual(6, (grp2.MainIconList as string[]).Length);
            //Assert.AreEqual(6, (grp2.IconList as string[]).Length);

            //Assert.AreEqual(2, cmds2[grp2].Count);

            //Assert.AreEqual(2, cmds1[grp1].Count);
            //Assert.AreEqual("Cmd1", cmds1[grp1][0][0]);
            //Assert.AreEqual("Cmd1", cmds1[grp1][0][2]);
            //Assert.AreEqual("Cmd2", cmds1[grp1][1][0]);
            //Assert.AreEqual("Cmd2", cmds1[grp1][1][2]);

            //Assert.AreEqual("CmdGrp", grp3.ToString());
            //Assert.AreEqual(1, cmds3[grp3].Count);
            //Assert.AreEqual("Command1", cmds3[grp3][0][0]);
            //Assert.AreEqual("Command1 Desc", cmds3[grp3][0][2]);
            //Assert.AreEqual(2, cmds3[grp3][0][8]);
        }

        /// <summary>
        /// 测试用例目的：验证 AddContextMenu 方法的基本功能（TODO: 被注释）。
        /// </summary>
        [Test]
        public void AddContextMenuBaseTest()
        {
            //var cmds1 = new Dictionary<CommandGroup, List<object[]>>();
            //var addInMock1 = CreateMockCommandGroup("23.0.0", cmds1);
            //var grp1 = addInMock1.AddContextMenu<CommandsMock_1>(c => { });

            //var cmds2 = new Dictionary<CommandGroup, List<object[]>>();
            //var addInMock2 = CreateMockCommandGroup("24.0.0", cmds2);
            //var grp2 = addInMock2.AddContextMenu<CommandsMock_1>(c => { });

            //var cmds3 = new Dictionary<CommandGroup, List<object[]>>();
            //var addInMock3 = CreateMockCommandGroup("25.0.0", cmds3);
            //var grp3 = addInMock3.AddContextMenu<CommandsMock_2>(c => { });

            //Assert.AreEqual("CtxMenu", grp1.ToString());
            //Assert.IsFalse(string.IsNullOrEmpty(grp1.LargeMainIcon));
            //Assert.IsFalse(string.IsNullOrEmpty(grp1.SmallMainIcon));
            //Assert.IsFalse(string.IsNullOrEmpty(grp1.LargeIconList));
            //Assert.IsFalse(string.IsNullOrEmpty(grp1.SmallIconList));
            //Assert.IsNull(grp1.MainIconList);
            //Assert.IsNull(grp1.IconList);

            //Assert.AreEqual("CtxMenu", grp2.ToString());
            //Assert.IsTrue(string.IsNullOrEmpty(grp2.LargeMainIcon));
            //Assert.IsTrue(string.IsNullOrEmpty(grp2.SmallMainIcon));
            //Assert.IsTrue(string.IsNullOrEmpty(grp2.LargeIconList));
            //Assert.IsTrue(string.IsNullOrEmpty(grp2.SmallIconList));
            //Assert.AreEqual(6, (grp2.MainIconList as string[]).Length);
            //Assert.AreEqual(6, (grp2.IconList as string[]).Length);

            //Assert.AreEqual(2, cmds2[grp2].Count);

            //Assert.AreEqual(2, cmds1[grp1].Count);
            //Assert.AreEqual("Cmd1", cmds1[grp1][0][0]);
            //Assert.AreEqual("Cmd1", cmds1[grp1][0][2]);
            //Assert.AreEqual("Cmd2", cmds1[grp1][1][0]);
            //Assert.AreEqual("Cmd2", cmds1[grp1][1][2]);

            //Assert.AreEqual("CtxMenu", grp3.ToString());
            //Assert.AreEqual(1, cmds3[grp3].Count);
            //Assert.AreEqual("Command1", cmds3[grp3][0][0]);
            //Assert.AreEqual("Command1 Desc", cmds3[grp3][0][2]);
            //Assert.AreEqual(2, cmds3[grp3][0][8]);
        }
    }
}
