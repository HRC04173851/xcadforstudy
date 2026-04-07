//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.IO;
using Xarial.XCad.Data;
using Xarial.XCad.Documents;

namespace Xarial.XCad
{
    /// <summary>
    /// Wrapper interface over the specific object
    /// <para>中文：对特定对象的包装接口</para>
    /// </summary>
    public interface IXObject : IEquatable<IXObject>
    {
        /// <summary>
        /// Application which owns this object
        /// <para>中文：拥有此对象的应用程序（插件宿主）</para>
        /// </summary>
        IXApplication OwnerApplication { get; }

        /// <summary>
        /// Document which owns this object
        /// <para>中文：拥有此对象的文档（零件、装配体或工程图）</para>
        /// </summary>
        /// <remarks>This can be null for the application level objects</remarks>
        IXDocument OwnerDocument { get; }

        /// <summary>
        /// Identifies if current object is valid
        /// <para>中文：标识当前对象是否有效（存活）</para>
        /// </summary>
        bool IsAlive { get; }

        /// <summary>
        /// Provides an ability to store temp tags in this session
        /// <para>中文：提供在当前会话中存储临时标签的功能</para>
        /// </summary>
        ITagsManager Tags { get; }

        /// <summary>
        /// Saves this object into a stream
        /// <para>中文：将此对象序列化保存到流中</para>
        /// </summary>
        /// <param name="stream">Target stream</param>
        void Serialize(Stream stream);
    }
}
