// -*- coding: utf-8 -*-
// src/Toolkit/Data/ComStream.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 COM 流的 .NET Stream 封装类 ComStream。
// 将 COM IStream 接口适配为标准 .NET Stream 接口，支持读写、定位和长度设置。
// 用于在 COM 组件和 .NET 代码之间传递二进制数据流。
//*********************************************************************

using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;

namespace Xarial.XCad.Toolkit.Data
{
    /// <summary>
    /// Represents the COM stream wrapper
    /// <para>表示 COM 流（IStream）的 .NET Stream 包装器。</para>
    /// </summary>
    public class ComStream : Stream
    {
        private readonly bool m_Commit;

        private bool m_IsWritable;

        /// <summary>
        /// Underlying COM stream instance.
        /// <para>底层 COM 流实例。</para>
        /// </summary>
        public IStream Stream { get; private set; }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => m_IsWritable;

        public override long Length
        {
            get
            {
                const int STATSFLAG_NONAME = 1;

                STATSTG statstg;

                Stream.Stat(out statstg, STATSFLAG_NONAME);

                return statstg.cbSize;
            }
        }

        public override long Position
        {
            get
            {
                return Seek(0, SeekOrigin.Current);
            }
            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Initializes COM stream wrapper and loads stream instance.
        /// <para>初始化 COM 流包装器并加载流实例。</para>
        /// </summary>
        public ComStream(IStream comStream, bool writable, bool commit = true)
            : this(writable, commit)
        {
            Load(comStream);
        }

        protected ComStream(bool writable, bool commit = true) 
        {
            m_Commit = commit;
            m_IsWritable = writable;
        }

        protected void Load(IStream comStream) 
        {
            if (comStream == null)
            {
                throw new ArgumentNullException(nameof(comStream));
            }

            Stream = comStream;
        }

        /// <summary>
        /// Flushes stream to storage by committing COM stream.
        /// <para>通过提交 COM 流将缓存写回存储。</para>
        /// </summary>
        public override void Flush()
        {
            if (m_Commit)
            {
                const int STGC_DEFAULT = 0;

                Stream.Commit(STGC_DEFAULT);
            }
        }

        /// <summary>
        /// Reads bytes from COM stream.
        /// <para>从 COM 流中读取字节数据。</para>
        /// </summary>
        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = 0;
            object boxBytesRead = bytesRead; //must be boxed otherwise - will fail
            var hObject = default(System.Runtime.InteropServices.GCHandle);

            try
            {
                hObject = System.Runtime.InteropServices.GCHandle.Alloc(boxBytesRead,
                    System.Runtime.InteropServices.GCHandleType.Pinned);

                var pBytesRead = hObject.AddrOfPinnedObject();

                if (offset != 0)
                {
                    var tmpBuffer = new byte[count];
                    Stream.Read(tmpBuffer, count, pBytesRead);
                    bytesRead = Convert.ToInt32(boxBytesRead);
                    Array.Copy(tmpBuffer, 0, buffer, offset, bytesRead);
                }
                else
                {
                    Stream.Read(buffer, count, pBytesRead);
                    bytesRead = Convert.ToInt32(boxBytesRead);
                }
            }
            finally
            {
                if (hObject.IsAllocated)
                {
                    hObject.Free();
                }
            }

            return bytesRead;
        }

        /// <summary>
        /// Seeks to position in COM stream.
        /// <para>在 COM 流中移动读写位置。</para>
        /// </summary>
        public override long Seek(long offset, SeekOrigin origin)
        {
            long curPosition = 0;
            var boxCurPosition = curPosition; //must be boxed otherwise - will fail
            var hObject = default(System.Runtime.InteropServices.GCHandle);

            try
            {
                hObject = System.Runtime.InteropServices.GCHandle.Alloc(
                    boxCurPosition, System.Runtime.InteropServices.GCHandleType.Pinned);

                var pCurPosition = hObject.AddrOfPinnedObject();

                Stream.Seek(offset, (int)origin, pCurPosition);
                curPosition = Convert.ToInt64(boxCurPosition);
            }
            finally
            {
                if (hObject.IsAllocated)
                {
                    hObject.Free();
                }
            }

            return curPosition;
        }

        public override void SetLength(long value)
        {
            Stream.SetSize(value);
        }

        /// <summary>
        /// Writes bytes to COM stream.
        /// <para>向 COM 流写入字节数据。</para>
        /// </summary>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (offset != 0)
            {
                var bufferSize = buffer.Length - offset;
                var tmpBuffer = new byte[bufferSize];
                Array.Copy(buffer, offset, tmpBuffer, 0, bufferSize);
                Stream.Write(tmpBuffer, bufferSize, IntPtr.Zero);
            }
            else
            {
                Stream.Write(buffer, count, IntPtr.Zero);
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    m_IsWritable = false;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        ~ComStream()
        {
            Dispose(false);
        }
    }
}
