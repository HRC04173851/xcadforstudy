// -*- coding: utf-8 -*-
// src/Base/UI/Commands/Structures/EnumCommandGroup.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 基于枚举的命令组接口和实现类，将枚举类型作为命令组，每个枚举字段对应一个命令按钮，支持枚举强类型的点击和状态解析事件。
//*********************************************************************

using System;
using System.ComponentModel;
using Xarial.XCad.UI.Commands.Delegates;
using static Xarial.XCad.UI.Commands.XCommandManagerExtension;

namespace Xarial.XCad.UI.Commands.Structures
{
    /// <summary>
    /// Command group based on the enumeration
    /// 基于枚举的命令组
    /// </summary>
    /// <typeparam name="TCmdEnum">Enumeration with commands</typeparam>
    public interface IEnumCommandGroup<TCmdEnum> : IXCommandGroup
        where TCmdEnum : Enum
    {
        /// <inheritdoc/>
        new event CommandEnumClickDelegate<TCmdEnum> CommandClick;

        /// <inheritdoc/>
        new event CommandEnumStateDelegate<TCmdEnum> CommandStateResolve;
    }

    internal class EnumCommandGroup<TCmdEnum> : IEnumCommandGroup<TCmdEnum>, IDisposable
                where TCmdEnum : Enum
    {
        event CommandClickDelegate IXCommandGroup.CommandClick
        {
            add => m_CmdBar.CommandClick += value;
            remove => m_CmdBar.CommandClick -= value;
        }

        event CommandEnumClickDelegate<TCmdEnum> IEnumCommandGroup<TCmdEnum>.CommandClick
        {
            add => m_CommandClick += value;
            remove => m_CommandClick -= value;
        }

        event CommandStateDelegate IXCommandGroup.CommandStateResolve
        {
            add => m_CmdBar.CommandStateResolve += value;
            remove => m_CmdBar.CommandStateResolve -= value;
        }

        event CommandEnumStateDelegate<TCmdEnum> IEnumCommandGroup<TCmdEnum>.CommandStateResolve
        {
            add => m_CommandState += value;
            remove => m_CommandState -= value;
        }

        private readonly IXCommandGroup m_CmdBar;
        private CommandEnumClickDelegate<TCmdEnum> m_CommandClick;
        private CommandEnumStateDelegate<TCmdEnum> m_CommandState;

        public CommandGroupSpec Spec => m_CmdBar.Spec;

        internal EnumCommandGroup(IXCommandGroup cmdBar)
        {
            m_CmdBar = cmdBar;

            m_CmdBar.CommandClick += OnCommandClick;
            m_CmdBar.CommandStateResolve += OnCommandStateResolve;
        }

        private void OnCommandClick(CommandSpec spec)
        {
            if (spec is EnumCommandSpec<TCmdEnum>)
            {
                m_CommandClick?.Invoke((spec as EnumCommandSpec<TCmdEnum>).Value);
            }
        }

        private void OnCommandStateResolve(CommandSpec spec, CommandState state)
        {
            if (spec is EnumCommandSpec<TCmdEnum>)
            {
                m_CommandState?.Invoke((spec as EnumCommandSpec<TCmdEnum>).Value, state);
            }
        }

        public void Dispose()
        {
            m_CmdBar.CommandClick -= OnCommandClick;
            m_CmdBar.CommandStateResolve -= OnCommandStateResolve;

            if (m_CmdBar is IDisposable) 
            {
                (m_CmdBar as IDisposable).Dispose();
            }
        }
    }
}