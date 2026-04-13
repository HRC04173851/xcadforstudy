//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using Xarial.XCad.SolidWorks.UI.Commands.Toolkit.Enums;
using Xarial.XCad.UI.Commands;
using Xarial.XCad.UI.Commands.Delegates;
using Xarial.XCad.UI.Commands.Enums;
using Xarial.XCad.UI.Commands.Structures;

namespace Xarial.XCad.SolidWorks.UI.Commands
{
    /// <summary>
    /// SolidWorks 命令组接口，封装工具栏/菜单命令组的特有属性。
    /// </summary>
    public interface ISwCommandGroup : IXCommandGroup 
    {
        /// <summary>
        /// Indicates if this group is context menu
        /// <para>中文：标识该命令组是否为右键上下文菜单</para>
        /// </summary>
        bool IsContextMenu { get; }

        /// <summary>
        /// SOLIDWORKS specific command group
        /// <para>中文：底层 SolidWorks 命令组对象</para>
        /// </summary>
        CommandGroup CommandGroup { get; }
    }

    /// <summary>
    /// SolidWorks 命令组实现，封装命令点击事件和命令状态解析逻辑。
    /// </summary>
    internal class SwCommandGroup : ISwCommandGroup
    {
        /// <inheritdoc/>
        public event CommandClickDelegate CommandClick;

        /// <inheritdoc/>
        public event CommandStateDelegate CommandStateResolve;

        /// <inheritdoc/>
        public CommandGroup CommandGroup { get; }

        /// <inheritdoc/>
        public CommandGroupSpec Spec { get; }

        public bool IsContextMenu { get; }

        private readonly ISwApplication m_App;

        internal SwCommandGroup(ISwApplication app, CommandGroupSpec spec, CommandGroup cmdGroup, bool isContextMenu)
        {
            m_App = app;
            Spec = spec;
            CommandGroup = cmdGroup;
            IsContextMenu = isContextMenu;
        }

        internal void RaiseCommandClick(CommandSpec spec)
            => CommandClick?.Invoke(spec);

        internal CommandItemEnableState_e RaiseCommandEnable(CommandSpec spec)
        {
            var state = new CommandState();
            ResolveState(state, spec.SupportedWorkspace);

            CommandStateResolve?.Invoke(spec, state);

            if (state.Enabled)
            {
                if (state.Checked)
                {
                    return CommandItemEnableState_e.SelectEnable;
                }
                else
                {
                    return CommandItemEnableState_e.DeselectEnable;
                }
            }
            else
            {
                if (state.Checked)
                {
                    return CommandItemEnableState_e.SelectDisable;
                }
                else
                {
                    return CommandItemEnableState_e.DeselectDisable;
                }
            }
        }

        /// <remarks>Using this method instead of the generic extension trying to improve the performance
        /// as this method is called frequently and finding the document from documents collection invokes several APIs as per the equality comparer</remarks>
        /// <para>中文：该方法高频调用，直接读取活动文档类型以提高命令启用状态判断性能。</para>
        private void ResolveState(CommandState state, WorkspaceTypes_e ws)
        {
            bool enabled;

            if (ws == WorkspaceTypes_e.All)
            {
                enabled = true;
            }
            else
            {
                enabled = false;

                var activeDoc = m_App.Sw.IActiveDoc2;

                if (activeDoc == null)
                {
                    enabled = ws.HasFlag(WorkspaceTypes_e.NoDocuments);
                }
                else
                {
                    switch (activeDoc)
                    {
                        case IPartDoc _:
                            enabled = ws.HasFlag(WorkspaceTypes_e.Part);
                            break;

                        case IAssemblyDoc assm:
                            enabled = ws.HasFlag(WorkspaceTypes_e.Assembly);
                            if (!enabled)
                            {
                                if (ws.HasFlag(WorkspaceTypes_e.InContextPart))
                                {
                                    var editComp = assm.GetEditTargetComponent();

                                    if (editComp != null && editComp.IsRoot())
                                    {
                                        editComp = null;
                                    }

                                    enabled = editComp != null && editComp.GetModelDoc2() is IPartDoc;
                                }
                            }
                            break;

                        case IDrawingDoc _:
                            enabled = ws.HasFlag(WorkspaceTypes_e.Drawing);
                            break;
                    }
                }
            }

            state.Enabled = enabled;
        }
    }
}