//*********************************************************************
//xCAD
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Xarial.XCad;
using Xarial.XCad.Data.Enums;
using Xarial.XCad.Documents;
using Xarial.XCad.Documents.Attributes;
using Xarial.XCad.Documents.Enums;
using Xarial.XCad.Documents.Services;
using Xarial.XCad.Extensions;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.UI;

namespace SwAddInExample
{
    // Document handler class: handles lifecycle events (open, save, close) for each SolidWorks document
    // 中文：文档处理器类，负责处理每个 SolidWorks 文档的生命周期事件（打开、保存、关闭）
    // Optionally filter to specific document types with [DocumentHandlerFilter]
    // 中文：可通过 [DocumentHandlerFilter] 特性限制仅处理特定类型的文档（如三维文档）
    //[DocumentHandlerFilter(typeof(ISwDocument3D))]
    public class SwDocHandler : IDocumentHandler
    {
        // Stores revision tracking data serialized into the document stream
        // 中文：存储序列化到文档数据流中的版本追踪数据
        public class RevData
        {
            // Incremental revision counter, increases by 1 on each save
            // 中文：递增修订计数器，每次保存时加 1
            public int Revision { get; set; }
            // Unique stamp (GUID) generated on each save to identify this specific revision
            // 中文：每次保存时生成的唯一标记（GUID），用于标识本次修订
            public Guid RevisionStamp { get; set; }
        }

        // Name of the document stream used to store revision data (XML-serialized RevData)
        // 中文：用于存储版本数据（XML 序列化的 RevData）的文档数据流名称
        private const string STREAM_NAME = "_xCadStream_";
        // Path to the sub-storage within the document storage (backslash-separated hierarchy)
        // 中文：文档存储中子存储的路径（以反斜杠分隔的层级结构）
        private const string SUB_STORAGE_PATH = "_xCadStorage1_\\SubStorage2";
        // Stream name for storing a save timestamp inside the sub-storage
        // 中文：在子存储中保存时间戳的数据流名称
        private const string TIME_STAMP_STREAM_NAME = "TimeStampStream";
        // Stream name for storing the current OS user's name inside the sub-storage
        // 中文：在子存储中保存当前操作系统用户名称的数据流名称
        private const string USER_NAME_STREAM_NAME = "UserName";

        // Cached revision data loaded from or to be saved into the document stream
        // 中文：从文档数据流加载或即将保存到文档数据流中的版本数据缓存
        private RevData m_RevData;

        // Reference to the SolidWorks application instance
        // 中文：SolidWorks 应用程序实例的引用
        private IXApplication m_App;
        // Reference to the document this handler is attached to
        // 中文：此处理器所关联文档的引用
        private IXDocument m_Model;

        // Reference to the add-in extension (used to create UI panels such as FeatureManager tabs)
        // 中文：插件扩展的引用（用于创建特征管理器选项卡等 UI 面板）
        private readonly IXExtension m_Ext;

        // Custom panel embedded in the FeatureManager tree for the WPF user control
        // 中文：嵌入特征管理器树中显示 WPF 用户控件的自定义面板
        private IXCustomPanel<WpfUserControl> m_FeatMgrTab;

        // Constructor: receives the add-in extension reference for creating UI elements later
        // 中文：构造函数：接收插件扩展引用，供后续创建 UI 元素使用
        public SwDocHandler(IXExtension ext) 
        {
            m_Ext = ext;
        }

        // Init: called by the framework when a document is opened; wire up stream/storage events
        // and create the FeatureManager tab panel for this document
        // 中文：Init 由框架在文档打开时调用；注册数据流/存储读写事件
        // 中文：并为当前文档创建特征管理器选项卡面板
        public void Init(IXApplication app, IXDocument model)
        {
            m_App = app;
            m_Model = model;

            // Subscribe to document stream and storage read/write events
            // 中文：订阅文档数据流与数据存储的读写事件
            m_Model.StreamReadAvailable += LoadFromStream;
            m_Model.StreamWriteAvailable += SaveToStream;
            m_Model.StorageReadAvailable += LoadFromStorage;
            m_Model.StorageWriteAvailable += SaveToStorage;

            // Create a WPF user control tab in the FeatureManager for this specific document
            // 中文：为当前文档在特征管理器中创建 WPF 用户控件选项卡
            m_FeatMgrTab = m_Ext.CreateFeatureManagerTab<WpfUserControl>(model);

            //m_App.ShowMessageBox($"Opened {model.Title}");
        }

        // Called when the document is saved; increments the revision counter and serializes it to the named stream
        // 中文：文档保存时调用；递增版本计数器并将其序列化到指定的数据流中
        private void SaveToStream(IXDocument doc)
        {
            // Open the stream for writing; 'using' ensures the stream is closed after writing
            // 中文：以写入模式打开数据流；using 语句确保写入后自动关闭流
            using (var stream = doc.OpenStream(STREAM_NAME, AccessType_e.Write))
            {
                var xmlSer = new XmlSerializer(typeof(RevData));

                // Initialize revision data on the very first save of this document
                // 中文：在文档首次保存时初始化版本数据
                if (m_RevData == null)
                {
                    m_RevData = new RevData();
                }

                // Increment revision number and assign a new unique GUID stamp for this save
                // 中文：递增版本号并为本次保存分配新的唯一 GUID 标记
                m_RevData.Revision = m_RevData.Revision + 1;
                m_RevData.RevisionStamp = Guid.NewGuid();

                // Serialize the revision data object to the stream as XML
                // 中文：将版本数据对象序列化为 XML 并写入数据流
                xmlSer.Serialize(stream, m_RevData);
            }
        }

        // Called when the document is opened; reads revision data from the named stream if it exists
        // 中文：文档打开时调用；若指定数据流存在，则从中读取版本数据
        private void LoadFromStream(IXDocument doc)
        {
            // TryOpenStream returns null if the stream does not exist (e.g., first-time open without prior save)
            // 中文：TryOpenStream 在数据流不存在时返回 null（例如，从未保存过版本数据的首次打开）
            using (var stream = doc.TryOpenStream(STREAM_NAME, AccessType_e.Read))
            {
                if (stream != null)
                {
                    // Deserialize the XML data back into a RevData object
                    // 中文：将 XML 数据反序列化还原为 RevData 对象
                    var xmlSer = new XmlSerializer(typeof(RevData));
                    m_RevData = xmlSer.Deserialize(stream) as RevData;
                    //m_App.ShowMessageBox($"Revision data of {doc.Title}: {m_RevData.Revision} - {m_RevData.RevisionStamp}");
                }
                else
                {
                    //m_App.ShowMessageBox($"No revision data stored in {doc.Title}");
                }
            }
        }

        // Called when the document is opened; reads nested sub-storage metadata (timestamps, user names)
        // 中文：文档打开时调用；从嵌套子存储中读取元数据（时间戳和用户名）
        private void LoadFromStorage(IXDocument doc)
        {
            // Split the storage path into root storage name and sub-storage name
            // 中文：将存储路径分割为根存储名称和子存储名称
            var path = SUB_STORAGE_PATH.Split('\\');

            // TryOpenStorage returns null if the root storage does not yet exist in this document
            // 中文：若文档中根存储尚不存在，TryOpenStorage 返回 null
            using (var storage = doc.TryOpenStorage(path[0], AccessType_e.Read))
            {
                if (storage != null)
                {
                    // Open the second-level sub-storage for reading (false = do not create if missing)
                    // 中文：以只读方式打开第二级子存储（false 表示不存在时不创建）
                    using (var subStorage = storage.TryOpenStorage(path[1], false))
                    {
                        if (subStorage != null)
                        {
                            // Iterate all stream names stored within the sub-storage
                            // 中文：遍历子存储中所有数据流的名称
                            foreach (var subStreamName in subStorage.GetSubStreamNames())
                            {
                                using (var str = subStorage.TryOpenStream(subStreamName, false))
                                {
                                    if (str != null)
                                    {
                                        // Read stream bytes and decode as UTF-8 text (timestamp or user name)
                                        // 中文：读取数据流字节并以 UTF-8 解码为文本（时间戳或用户名）
                                        var buffer = new byte[str.Length];

                                        str.Read(buffer, 0, buffer.Length);

                                        var timeStamp = Encoding.UTF8.GetString(buffer);

                                        //m_App.ShowMessageBox($"Metadata stamp in {subStreamName} of {doc.Title}: {timeStamp}");
                                    }
                                    else
                                    {
                                        //m_App.ShowMessageBox($"No metadata stamp stream in {doc.Title}");
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    //m_App.ShowMessageBox($"No metadata storage in {doc.Title}");
                }
            }
        }

        // Called when the document is saved; writes the current timestamp and OS user name into nested sub-storages
        // 中文：文档保存时调用；将当前时间戳和操作系统用户名写入嵌套子存储中
        private void SaveToStorage(IXDocument doc)
        {
            // Split the storage path into root storage and sub-storage names
            // 中文：将存储路径分割为根存储名称和子存储名称
            var path = SUB_STORAGE_PATH.Split('\\');

            // Open the root storage for writing; creates it if it does not yet exist
            // 中文：以写入模式打开根存储；若不存在则自动创建
            using (var storage = doc.OpenStorage(path[0], AccessType_e.Write))
            {
                // Open (or create) the sub-storage within the root storage
                // 中文：在根存储中打开（或创建）子存储
                using (var subStorage = storage.TryOpenStorage(path[1], true))
                {
                    // Write the current date/time as a UTF-8 string to the timestamp stream
                    // 中文：将当前日期/时间以 UTF-8 字符串写入时间戳数据流
                    using (var str = subStorage.TryOpenStream(TIME_STAMP_STREAM_NAME, true))
                    {
                        var buffer = Encoding.UTF8.GetBytes(DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss"));
                        str.Write(buffer, 0, buffer.Length);
                    }

                    // Write the current Windows user name as a UTF-8 string to the user name stream
                    // 中文：将当前 Windows 用户名以 UTF-8 字符串写入用户名数据流
                    using (var str = subStorage.TryOpenStream(USER_NAME_STREAM_NAME, true))
                    {
                        var buffer = Encoding.UTF8.GetBytes(System.Environment.UserName);
                        str.Write(buffer, 0, buffer.Length);
                    }
                }
            }
        }

        // Dispose: called when the document is closed; unsubscribe from events and close the FeatureManager tab
        // 中文：Dispose 在文档关闭时由框架调用；取消事件订阅并关闭特征管理器选项卡
        public void Dispose()
        {
            // Unsubscribe from all stream/storage events to prevent memory leaks
            // 中文：取消所有数据流/存储事件订阅，防止内存泄漏
            m_Model.StreamReadAvailable -= LoadFromStream;
            m_Model.StreamWriteAvailable -= SaveToStream;
            m_Model.StorageReadAvailable -= LoadFromStorage;
            m_Model.StorageWriteAvailable -= SaveToStorage;

            // Print a debug message to the Visual Studio Output window
            // 中文：将调试信息输出到 Visual Studio 的输出窗口
            System.Diagnostics.Debug.Print($"Closed {m_Model.Title}");

            // Close the FeatureManager tab panel associated with this document
            // 中文：关闭与此文档关联的特征管理器选项卡面板
            m_FeatMgrTab.Close();

            //m_App.ShowMessageBox($"Closed {m_Model.Title}");
        }
    }
}
