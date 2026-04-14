// -*- coding: utf-8 -*-
// SwObject.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 所有SolidWorks对象的基类，提供COM调度访问、所属文档追踪和对象生命周期管理
//*********************************************************************

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.IO;
using Xarial.XCad.Base;
using Xarial.XCad.Data;
using Xarial.XCad.Documents;
using Xarial.XCad.Exceptions;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.Toolkit.Data;

namespace Xarial.XCad.SolidWorks
{
    /// <summary>
    /// Represents base interface for all SOLIDWORKS objects
    /// <para>中文：所有 SolidWorks 对象的基接口，提供对底层 COM 调度对象的访问</para>
    /// </summary>
    public interface ISwObject : IXObject
    {
        /// <summary>
        /// SOLIDWORKS specific dispatch
        /// <para>中文：SolidWorks 专用 COM 调度对象（底层 SolidWorks API 对象指针）</para>
        /// </summary>
        object Dispatch { get; }
    }

    /// <inheritdoc/>
    /// <summary>
    /// Internal base implementation for all SolidWorks objects.
    /// Holds dispatch reference, owner document, and application context.
    /// <para>中文：所有 SolidWorks 对象的内部基础实现，持有 COM 调度引用、所属文档及应用程序上下文。</para>
    /// </summary>
    internal class SwObject : ISwObject
    {
        IXApplication IXObject.OwnerApplication => OwnerApplication;
        IXDocument IXObject.OwnerDocument => OwnerDocument;

        /// <summary>
        /// Gets the underlying IModelDoc2 of the owner document
        /// <para>中文：获取所属文档的底层 IModelDoc2 模型对象</para>
        /// </summary>
        protected IModelDoc2 OwnerModelDoc => OwnerDocument.Model;

        /// <summary>Gets the owner SolidWorks application
        /// <para>中文：获取所属的 SolidWorks 应用程序实例</para></summary>
        internal SwApplication OwnerApplication { get; }

        /// <summary>Gets the owner SolidWorks document
        /// <para>中文：获取此对象所属的 SolidWorks 文档</para></summary>
        internal virtual SwDocument OwnerDocument { get; }

        /// <summary>
        /// The underlying SolidWorks COM dispatch object
        /// <para>中文：底层 SolidWorks COM 调度对象</para>
        /// </summary>
        public virtual object Dispatch { get; }

        /// <summary>
        /// Gets whether this object is still alive (valid) within SolidWorks
        /// <para>中文：检查此对象是否在 SolidWorks 中仍然有效（未被删除或失效）</para>
        /// </summary>
        public virtual bool IsAlive 
        {
            get 
            {
                try
                {
                    if (Dispatch != null)
                    {
                        if (OwnerDocument != null)
                        {
                            // 尝试获取持久化引用来验证对象是否仍然有效
                            if (OwnerModelDoc.Extension.GetPersistReference3(Dispatch) != null)
                            {
                                return true;
                            }
                        }
                        else 
                        {
                            //this is an assumption as memory object can still be destroyed
                            //TODO: find how to capture the object has been disconnected from its client exception
                            // 中文：此处为假设，内存中的对象可能已被销毁；待完善异常捕获逻辑
                            return true;
                        }
                    }
                }
                catch 
                {
                }

                return false;
            }
        }

        /// <summary>
        /// Tags manager for attaching custom metadata to this object
        /// <para>中文：用于为此对象附加自定义元数据的标签管理器</para>
        /// </summary>
        public ITagsManager Tags => m_TagsLazy.Value;

        private readonly Lazy<ITagsManager> m_TagsLazy;

        /// <summary>
        /// Constructs a new SwObject with the given dispatch, owner document, and application.
        /// <para>中文：使用给定的 COM 调度对象、所属文档和应用程序构造新的 SwObject 实例。</para>
        /// </summary>
        internal SwObject(object disp, SwDocument doc, SwApplication app) 
        {
            Dispatch = disp;
            // 懒加载标签管理器，避免不必要的初始化开销
            m_TagsLazy = new Lazy<ITagsManager>(() => new GlobalTagsManager(this, app.TagsRegistry));
            OwnerDocument = doc;
            OwnerApplication = app;
        }

        /// <summary>
        /// Compares this object to another for equality using SolidWorks' IsSame API.
        /// <para>中文：使用 SolidWorks IsSame API 比较两个对象是否相等。</para>
        /// </summary>
        public virtual bool Equals(IXObject other)
        {
            if (object.ReferenceEquals(this, other)) 
            {
                return true;
            }

            if (other is ISwObject)
            {
                // 未提交的事务对象不参与相等比较
                if (this is IXTransaction && !((IXTransaction)this).IsCommitted) 
                {
                    return false;
                }

                if (other is IXTransaction && !((IXTransaction)other).IsCommitted)
                {
                    return false;
                }

                if (Dispatch == (other as ISwObject).Dispatch)
                {
                    return true;
                }
                else
                {
                    // 使用 SolidWorks API 判断两个调度对象是否指向同一实体
                    return OwnerApplication.Sw.IsSame(Dispatch, (other as ISwObject).Dispatch) == (int)swObjectEquality.swObjectSame;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Serializes this object's persist reference to the given stream.
        /// <para>中文：将此对象的持久化引用序列化到指定的流中，用于跨会话存储和还原 SolidWorks 对象。</para>
        /// </summary>
        public virtual void Serialize(Stream stream)
        {
            if (OwnerModelDoc != null)
            {
                var disp = GetSerializationDispatch();

                if (disp != null)
                {
                    // 获取 SolidWorks 持久化引用（字节数组）并写入流
                    var persRef = OwnerModelDoc.Extension.GetPersistReference3(disp) as byte[];

                    if (persRef == null)
                    {
                        throw new ObjectSerializationException("Failed to serialize the object", -1);
                    }

                    stream.Write(persRef, 0, persRef.Length);
                    return;
                }
                else 
                {
                    throw new ObjectSerializationException("Dispatch is null", -1);
                }
            }
            else 
            {
                throw new ObjectSerializationException("Model is not set for this object", -1);
            }
        }

        /// <summary>
        /// In some instances it is required to serialize different dispatch (e.g. specific or base feature)
        /// <para>中文：某些情况下需要序列化不同的调度对象（例如特定特征或基础特征），可重写此方法以自定义序列化目标。</para>
        /// </summary>
        /// <returns></returns>
        protected virtual object GetSerializationDispatch() => Dispatch;
    }

    /// <summary>
    /// Extension methods for <see cref="SwObject"/>
    /// <para>中文：SwObject 的扩展方法，提供存活状态检查的辅助实现</para>
    /// </summary>
    internal static class SwObjectExtension
    {
        /// <summary>
        /// Checks whether the object is alive by invoking a checker action; returns false if an exception is thrown.
        /// <para>中文：通过执行检查操作来判断对象是否存活，若抛出异常则返回 false。</para>
        /// </summary>
        internal static bool CheckIsAlive(this SwObject obj, Action checker)
        {
            try
            {
                checker.Invoke();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}