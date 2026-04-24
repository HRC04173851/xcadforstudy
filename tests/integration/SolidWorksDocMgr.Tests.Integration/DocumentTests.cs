// -*- coding: utf-8 -*-
// tests/integration/SolidWorksDocMgr.Tests.Integration/DocumentTests.cs
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Xarial.XCad.Base;
using Xarial.XCad.Data.Enums;
using Xarial.XCad.Documents;
using Xarial.XCad.Documents.Enums;
using Xarial.XCad.Documents.Exceptions;
using Xarial.XCad.Documents.Extensions;
using Xarial.XCad.SwDocumentManager;
using Xarial.XCad.SwDocumentManager.Documents;

namespace SolidWorksDocMgr.Tests.Integration
{
    /// <summary>
    /// 文档相关集成测试，验证 SOLIDWORKS Document Manager 对各类文档的操作能力。
    /// 测试内容包括：文档打开关闭、版本检测、依赖关系、第三方数据存储、模板文档等。
    /// </summary>
    public class DocumentTests : IntegrationTests
    {
        /// <summary>
        /// 测试文档版本检测功能。
        /// 验证 Document Manager 能正确识别不同版本的 SOLIDWORKS 文档格式。
        /// </summary>
        [Test]
        public void VersionTest()
        {
            ISwDmVersion v1;
            ISwDmVersion v2;

            using (var doc = OpenDataDocument("Part_2020.sldprt"))
            {
                var part1 = m_App.Documents.Active;
                v1 = part1.Version;
            }
            
            using (var doc = OpenDataDocument("Part_2019.sldprt"))
            {
                var part2 = m_App.Documents.Active;
                v2 = part2.Version;
            }

            Assert.AreEqual(SwDmVersion_e.Sw2020, v1.Major);
            Assert.AreEqual(SwDmVersion_e.Sw2019, v2.Major);
        }

        /// <summary>
        /// 测试文档集合的管理功能。
        /// 验证打开和关闭文档时，Documents 集合的计数和活动文档状态正确更新。
        /// </summary>
        [Test]
        public void DocumentsTest()
        {
            var c1 = m_App.Documents.Count;
            var activeIsNull = m_App.Documents.Active == null;

            var doc1 = m_App.Documents.Open(GetFilePath("Part_2020.sldprt"), DocumentState_e.ReadOnly);

            var c2 = m_App.Documents.Count;
            var activeIsDoc1 = m_App.Documents.Active.Equals(doc1);

            var doc2 = m_App.Documents.Open(GetFilePath("Part_2019.sldprt"), DocumentState_e.ReadOnly);

            var c3 = m_App.Documents.Count;
            var activeIsDoc2 = m_App.Documents.Active.Equals(doc2);

            doc1.Close();
            var c4 = m_App.Documents.Count;
            var activeIsDoc21 = m_App.Documents.Active.Equals(doc2);

            doc2.Close();
            var c5 = m_App.Documents.Count;
            var activeIsNull1 = m_App.Documents.Active == null;

            Assert.AreEqual(0, c1);
            Assert.IsTrue(activeIsNull);
            Assert.AreEqual(1, c2);
            Assert.IsTrue(activeIsDoc1);
            Assert.AreEqual(2, c3);
            Assert.IsTrue(activeIsDoc2);
            Assert.AreEqual(1, c4);
            Assert.IsTrue(activeIsDoc21);
            Assert.AreEqual(0, c5);
            Assert.IsTrue(activeIsNull1);
        }

        /// <summary>
        /// 测试文档存活状态检测。
        /// 文档关闭后 IsAlive 属性应返回 false，表示文档已不再可用。
        /// </summary>
        [Test]
        public void IsAliveTest() 
        {
            bool r1;
            bool r2;
            //bool r3;
            
            var doc1 = m_App.Documents.Open(GetFilePath("Part_2020.sldprt"), Xarial.XCad.Documents.Enums.DocumentState_e.ReadOnly);
            r1 = doc1.IsAlive;
            doc1.Close();
            r2 = doc1.IsAlive;

            //doc1 = m_App.Documents.Open(GetFilePath("Part_2020.sldprt"), Xarial.XCad.Documents.Enums.DocumentState_e.ReadOnly);
            //((ISwDmDocument)doc1).Document.CloseDoc();
            //r3 = doc1.IsAlive;

            Assert.IsTrue(r1);
            Assert.IsFalse(r2);
            //Assert.IsFalse(r3);
        }

        /// <summary>
        /// 测试文档直接依赖项的获取。
        /// Dependencies 返回当前文档直接引用的文档集合。
        /// </summary>
        [Test]
        public void DocumentDependenciesTest()
        {
            using (var doc = OpenDataDocument(@"Assembly2\TopAssem.SLDASM"))
            {
                var assm = m_App.Documents.Active;

                var deps = assm.Dependencies.ToArray();

                var dir = Path.GetDirectoryName(assm.Path);

                Assert.AreEqual(4, deps.Length);
                Assert.That(deps.Any(d => string.Equals(d.Path, Path.Combine(dir, "Part4-1 (XYZ).SLDPRT"))));
                Assert.That(deps.Any(d => string.Equals(d.Path, Path.Combine(dir, "Assem1.SLDASM"))));
                Assert.That(deps.Any(d => string.Equals(d.Path, Path.Combine(dir, "Assem2.SLDASM"))));
                Assert.That(deps.Any(d => string.Equals(d.Path, Path.Combine(dir, "Part1.SLDPRT"))));
            }
        }
        
        /// <summary>
        /// 测试递归获取所有依赖项（包括嵌套层级的依赖）。
        /// TryIterateAll 会遍历所有层级的子文档依赖，返回完整的依赖树。
        /// </summary>
        [Test]
        public void DocumentAllDependenciesTest()
        {
            using (var doc = OpenDataDocument(@"Assembly2\TopAssem.SLDASM"))
            {
                var assm = m_App.Documents.Active;

                var deps = assm.Dependencies.TryIterateAll().ToArray();

                var dir = Path.GetDirectoryName(assm.Path);

                Assert.AreEqual(6, deps.Length);
                Assert.That(deps.Any(d => string.Equals(d.Path, Path.Combine(dir, "Part2.SLDPRT"))));
                Assert.That(deps.Any(d => string.Equals(d.Path, Path.Combine(dir, "Part3.SLDPRT"))));
                Assert.That(deps.Any(d => string.Equals(d.Path, Path.Combine(dir, "Part4-1 (XYZ).SLDPRT"))));
                Assert.That(deps.Any(d => string.Equals(d.Path, Path.Combine(dir, "Assem1.SLDASM"))));
                Assert.That(deps.Any(d => string.Equals(d.Path, Path.Combine(dir, "Assem2.SLDASM"))));
                Assert.That(deps.Any(d => string.Equals(d.Path, Path.Combine(dir, "Part1.SLDPRT"))));
            }
        }

        /// <summary>
        /// 测试包含缺失和虚拟组件的文档依赖解析。
        /// 虚拟组件和缺失组件的引用需要特殊处理，验证 IsAlive 和 IsCommitted 状态。
        /// </summary>
        [Test]
        public void DocumentAllDependenciesMissingAndVirtualTest()
        {
            using (var doc = OpenDataDocument(@"Assembly6\Assem1.SLDASM"))
            {
                var assm = m_App.Documents.Active;

                var deps = assm.Dependencies.TryIterateAll().ToArray();

                var dir = Path.GetDirectoryName(assm.Path);

                var d1 = deps.FirstOrDefault(d => string.Equals(Path.GetFileName(d.Path), "_temp_Part1^Assem1.sldprt",
                    StringComparison.CurrentCultureIgnoreCase));
                var d2 = deps.FirstOrDefault(d => string.Equals(Path.GetFileName(d.Path), "Part2.sldprt",
                    StringComparison.CurrentCultureIgnoreCase));
                var d3 = deps.FirstOrDefault(d => string.Equals(Path.GetFileName(d.Path), "Assem2.sldasm",
                    StringComparison.CurrentCultureIgnoreCase));
                var d4 = deps.FirstOrDefault(d => string.Equals(Path.GetFileName(d.Path), "_temp_Assem3^Assem1.sldasm",
                    StringComparison.CurrentCultureIgnoreCase));
                var d5 = deps.FirstOrDefault(d => string.Equals(Path.GetFileName(d.Path), "Part4.sldprt",
                    StringComparison.CurrentCultureIgnoreCase));
                var d6 = deps.FirstOrDefault(d => string.Equals(Path.GetFileName(d.Path), "_temp_Part5^Assem3_Assem1.sldprt",
                    StringComparison.CurrentCultureIgnoreCase));
                var d7 = deps.FirstOrDefault(d => string.Equals(Path.GetFileName(d.Path), "Assem4.sldasm",
                    StringComparison.CurrentCultureIgnoreCase));
                var d8 = deps.FirstOrDefault(d => string.Equals(Path.GetFileName(d.Path), "_temp_Part1^Assem4.sldprt",
                    StringComparison.CurrentCultureIgnoreCase));

                Assert.AreEqual(8, deps.Length);
                Assert.IsTrue(d1.IsAlive);
                //Assert.IsTrue(d1.IsCommitted);
                Assert.Throws<OpenDocumentFailedException>(() => d2.Commit());
                Assert.Throws<OpenDocumentFailedException>(() => d3.Commit());
                Assert.IsTrue(d4.IsAlive);
                //Assert.IsTrue(d4.IsCommitted);
                //Assert.IsFalse(d5.IsCommitted);
                Assert.That(string.Equals(d5.Path, Path.Combine(dir, "Part4.SLDPRT"), StringComparison.CurrentCultureIgnoreCase));
                Assert.IsTrue(d6.IsAlive);
                //Assert.IsTrue(d6.IsCommitted);
                //Assert.IsFalse(d7.IsCommitted);
                Assert.That(string.Equals(d7.Path, Path.Combine(dir, "Assem4.sldasm"), StringComparison.CurrentCultureIgnoreCase));
                Assert.IsTrue(d8.IsAlive);
                //Assert.IsTrue(d8.IsCommitted);
            }
        }

        /// <summary>
        /// 测试依赖项标题的提取。
        /// 虚拟组件的标题格式为 "零件名^装配体名"，用于识别组件的唯一性。
        /// </summary>
        [Test]
        public void DocumentAllDependenciesTitleTest()
        {
            string[] titles;

            using (var doc = OpenDataDocument(@"Assembly7\Assem1.SLDASM"))
            {
                var assm = m_App.Documents.Active;

                titles = assm.Dependencies.TryIterateAll().Select(d => Path.GetFileNameWithoutExtension(d.Title)).ToArray();
            }

            Assert.AreEqual(7, titles.Length);

            CollectionAssert.AreEquivalent(new string[] 
            {
                "Part1^Assem1",
                "__Part2^Assem1",
                "_temp_ABC^Assem1",
                "Assem2^Assem1",
                "Part4^Assem2_Assem1",
                "Assem2",
                "Part5^Assem2"
            }, titles);
        }

        /// <summary>
        /// 测试 3D Interconnect 格式文档的依赖关系。
        /// 某些 CAD 格式（如 .prt.sldprt）通过 3D Interconnect 转换，需要特殊处理。
        /// </summary>
        [Test]
        public void DocumentDependencies3DInterconnect()
        {
            Dictionary<string, bool> r1;

            using (var assm = OpenDataDocument(@"Assembly9\Assem1.SLDASM"))
            {
                var deps = m_App.Documents.Active.Dependencies.TryIterateAll().ToArray();
                r1 = deps.ToDictionary(d => Path.GetFileName(d.Path), d => d.IsCommitted, StringComparer.CurrentCultureIgnoreCase);
            }

            Assert.AreEqual(2, r1.Count);
            Assert.That(r1.ContainsKey("Part2.sldprt"));
            Assert.That(r1.ContainsKey("Part1.prt.sldprt"));
        }

        /// <summary>
        /// 测试依赖项的只读状态。
        /// 只读模式打开文档时，其所有依赖项也应保持只读状态。
        /// </summary>
        [Test]
        public void DocumentAllDependenciesReadOnlyState()
        {
            var destPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            Directory.CreateDirectory(destPath);

            foreach (var srcFile in Directory.GetFiles(GetFilePath(@"Assembly6"), "*.*")) 
            {
                File.Copy(srcFile, Path.Combine(destPath, Path.GetFileName(srcFile)));
            }

            using (var doc = OpenDataDocument(Path.Combine(destPath, "Assem1.SLDASM"), false))
            {
                var assm = m_App.Documents.Active;

                var deps = assm.Dependencies.TryIterateAll().ToArray();

                Assert.AreEqual(8, deps.Length);
                Assert.That(deps.All(d => !d.State.HasFlag(DocumentState_e.ReadOnly)));

                foreach (var dep in deps) 
                {
                    if (dep.IsCommitted)
                    {
                        dep.Close();
                    }
                }
            }

            Directory.Delete(destPath, true);

            using (var doc = OpenDataDocument(@"Assembly6\Assem1.SLDASM", true))
            {
                var assm = m_App.Documents.Active;

                var deps = assm.Dependencies.TryIterateAll().ToArray();

                Assert.AreEqual(8, deps.Length);
                Assert.That(deps.All(d => d.State.HasFlag(DocumentState_e.ReadOnly)));

                foreach (var dep in deps)
                {
                    if (dep.IsCommitted)
                    {
                        dep.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 测试移动路径后依赖项的解析。
        /// 当装配体被移动到新位置时，需要验证依赖路径能否正确重定位。
        /// </summary>
        [Test]
        public void DocumentAllDependenciesMovedPath()
        {
            var destPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            Directory.CreateDirectory(destPath);

            var srcPath = GetFilePath(@"Assembly6");

            foreach (var srcFile in Directory.GetFiles(srcPath, "*.*"))
            {
                File.Copy(srcFile, Path.Combine(destPath, Path.GetFileName(srcFile)));
            }

            using (var doc = OpenDataDocument(Path.Combine(destPath, "Assem1.SLDASM")))
            {
                var assm = m_App.Documents.Active;

                var deps = assm.Dependencies.TryIterateAll().ToArray();

                var d1 = deps.FirstOrDefault(d => string.Equals(Path.GetFileName(d.Path), "_temp_Part1^Assem1.sldprt",
                    StringComparison.CurrentCultureIgnoreCase));
                var d2 = deps.FirstOrDefault(d => string.Equals(Path.GetFileName(d.Path), "Part2.sldprt",
                    StringComparison.CurrentCultureIgnoreCase));
                var d3 = deps.FirstOrDefault(d => string.Equals(Path.GetFileName(d.Path), "Assem2.sldasm",
                    StringComparison.CurrentCultureIgnoreCase));
                var d4 = deps.FirstOrDefault(d => string.Equals(Path.GetFileName(d.Path), "_temp_Assem3^Assem1.sldasm",
                    StringComparison.CurrentCultureIgnoreCase));
                var d5 = deps.FirstOrDefault(d => string.Equals(Path.GetFileName(d.Path), "Part4.sldprt",
                    StringComparison.CurrentCultureIgnoreCase));
                var d6 = deps.FirstOrDefault(d => string.Equals(Path.GetFileName(d.Path), "_temp_Part5^Assem3_Assem1.sldprt",
                    StringComparison.CurrentCultureIgnoreCase));
                var d7 = deps.FirstOrDefault(d => string.Equals(Path.GetFileName(d.Path), "Assem4.sldasm",
                    StringComparison.CurrentCultureIgnoreCase));
                var d8 = deps.FirstOrDefault(d => string.Equals(Path.GetFileName(d.Path), "_temp_Part1^Assem4.sldprt",
                    StringComparison.CurrentCultureIgnoreCase));

                Assert.AreEqual(8, deps.Length);

                Assert.IsNotNull(d1);
                Assert.IsNotNull(d2);
                Assert.IsNotNull(d3);
                Assert.IsNotNull(d4);
                Assert.IsNotNull(d5);
                Assert.IsNotNull(d6);
                Assert.IsNotNull(d7);
                Assert.IsNotNull(d8);

                Assert.IsTrue(d1.IsCommitted);
                Assert.IsFalse(d2.IsCommitted);
                Assert.IsFalse(d3.IsCommitted);
                Assert.IsTrue(d4.IsCommitted);
                Assert.IsTrue(d5.IsCommitted);
                Assert.IsTrue(d6.IsCommitted);
                Assert.IsTrue(d7.IsCommitted);
                Assert.IsTrue(d8.IsCommitted);

                Assert.That(string.Equals(Path.GetFileName(d2.Path), "Part2.SLDPRT", StringComparison.CurrentCultureIgnoreCase));
                Assert.That(string.Equals(Path.GetFileName(d3.Path), "Assem2.sldasm", StringComparison.CurrentCultureIgnoreCase));
                Assert.Throws<OpenDocumentFailedException>(() => d2.Commit());
                Assert.Throws<OpenDocumentFailedException>(() => d3.Commit());
                //Assert.That(string.Equals(d5.Path, Path.Combine(destPath, "Part4.SLDPRT"), StringComparison.CurrentCultureIgnoreCase));//NOTE: SOLIDWORKS does not follow the path resolution for the components of virtual component
                Assert.That(string.Equals(d7.Path, Path.Combine(destPath, "Assem4.sldasm"), StringComparison.CurrentCultureIgnoreCase));

                d1.Close();
                d4.Close();
                d5.Close();
                d6.Close();
                d7.Close();
                d8.Close();
            }

            Directory.Delete(destPath, true);
        }

        /// <summary>
        /// 测试缓存引用（未打开的依赖）的解析。
        /// 对于移动位置的装配体，Document Manager 可以从缓存数据解析引用路径。
        /// </summary>
        [Test]
        public void DocumentDependenciesCachedTest()
        {
            using (var doc = OpenDataDocument(@"MovedNonOpenedAssembly1\TopAssembly.SLDASM"))
            {
                var assm = m_App.Documents.Active;

                var deps = assm.Dependencies.ToArray();

                var dir = Path.GetDirectoryName(assm.Path);

                Assert.AreEqual(1, deps.Length);
                Assert.That(deps.All(d => !d.IsCommitted));
                Assert.That(deps.Any(d => string.Equals(d.Path, Path.Combine(dir, "Assemblies\\Assem1.SLDASM"), StringComparison.CurrentCultureIgnoreCase)));
            }
        }

        /// <summary>
        /// 测试外部文件夹中缓存引用的解析。
        /// 组件可能引用装配体目录之外的零件文件。
        /// </summary>
        [Test]
        public void DocumentDependenciesCachedExtFolderTest()
        {
            using (var doc = OpenDataDocument(@"Assembly3\Assemblies\Assem1.SLDASM"))
            {
                var assm = m_App.Documents.Active;

                var deps = assm.Dependencies.ToArray();

                var dir = GetFilePath(@"Assembly3");

                Assert.AreEqual(1, deps.Length);
                Assert.That(deps.All(d => !d.IsCommitted));
                Assert.That(deps.Any(d => string.Equals(d.Path, Path.Combine(dir, "Parts\\Part1.SLDPRT"), StringComparison.CurrentCultureIgnoreCase)));
            }
        }

        /// <summary>
        /// 测试复制文件后的依赖关系解析。
        /// 将装配体复制到新位置后，需要验证依赖路径能否正确更新。
        /// </summary>
        [Test]
        public void DocumentDependenciesCopiedFilesTest()
        {
            var tempPath = Path.Combine(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));

            Dictionary<string, bool> refs;

            var destPath = Path.Combine(tempPath, "_Assembly11");
            var tempSrcAssmPath = Path.Combine(tempPath, "Assembly11");

            try
            {
                var srcPath = GetFilePath("Assembly11");

                CopyDirectory(srcPath, tempSrcAssmPath);
                UpdateSwReferences(tempSrcAssmPath, "TopLevel\\Assem1.sldasm", "SubAssemblies\\Assem3.SLDASM", "SubAssemblies\\A\\Assem2.SLDASM");

                CopyDirectory(tempSrcAssmPath, destPath);

                File.Delete(Path.Combine(destPath, "Parts\\Part4.sldprt"));
                File.Delete(Path.Combine(destPath, "SubAssemblies\\Part2.sldprt"));
                File.Delete(Path.Combine(tempSrcAssmPath, "Parts\\Part4.sldprt"));

                using (var doc = OpenDataDocument(Path.Combine(destPath, "TopLevel\\Assem1.sldasm")))
                {
                    var assm = (ISwDmAssembly)doc.Document;

                    var deps = assm.Dependencies.TryIterateAll().ToArray();

                    refs = deps.ToDictionary(x => x.Path, x => x.IsCommitted, StringComparer.CurrentCultureIgnoreCase);

                    foreach (var refDoc in assm.Dependencies.TryIterateAll().ToArray())
                    {
                        if (refDoc.IsCommitted && refDoc.IsAlive)
                        {
                            refDoc.Close();
                        }
                    }
                }
            }
            finally
            {
                try
                {
                    Directory.Delete(tempPath, true);
                }
                catch //folder can be locked by SW while files can be deleted
                {
                    foreach (var file in Directory.GetFiles(tempPath, "*.*", SearchOption.AllDirectories))
                    {
                        File.Delete(file);
                    }
                }
            }

            Assert.AreEqual(8, refs.Count);

            var virtComp = refs.FirstOrDefault(x => x.Key.EndsWith("Part6^Assem1.sldprt", StringComparison.CurrentCultureIgnoreCase));

            Assert.AreEqual(refs[Path.Combine(destPath, @"SubAssemblies\A\Assem2.SLDASM")], true);
            Assert.AreEqual(refs[Path.Combine(destPath, @"Parts\Part1.SLDPRT")], true);
            Assert.AreEqual(refs[Path.Combine(destPath, @"SubAssemblies\Assem3.SLDASM")], true);
            Assert.That(!string.IsNullOrEmpty(virtComp.Key));
            Assert.AreEqual(virtComp.Value, true);
            Assert.AreEqual(refs[Path.Combine(destPath, @"SubAssemblies\A\Part3.SLDPRT")], true);
            Assert.AreEqual(refs[Path.Combine(tempSrcAssmPath, @"Parts\Part4.SLDPRT")], false);
            Assert.AreEqual(refs[Path.Combine(tempSrcAssmPath, @"SubAssemblies\Part2.SLDPRT")], true);
            Assert.AreEqual(refs[Path.Combine(destPath, @"SubAssemblies\Part5.SLDPRT")], true);
        }

        public class TestData
        {
            public string Text { get; set; }
            public int Number { get; set; }
        }

        /// <summary>
        /// 测试第三方数据流的读写功能。
        /// Document Manager 支持在文档中存储自定义数据流，用于保存应用程序特定数据。
        /// 使用 XML 序列化来存储结构化数据。
        /// </summary>
        [Test]
        public void ThirdPartyStreamTest()
        {
            const string STREAM_NAME = "_xCadIntegrationTestStream_";

            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".sldprt");

            File.Copy(GetFilePath("EmptyPart1.sldprt"), tempFile);

            using (var doc = OpenDataDocument(tempFile, false))
            {
                var part = m_App.Documents.Active;

                part.StreamWriteAvailable += (d) =>
                {
                    using (var stream = d.OpenStream(STREAM_NAME, AccessType_e.Write))
                    {
                        var xmlSer = new XmlSerializer(typeof(TestData));

                        var data = new TestData()
                        {
                            Text = "Test1",
                            Number = 15
                        };

                        xmlSer.Serialize(stream, data);
                    }
                };

                part.Save();
            }

            TestData result = null;

            using (var doc = OpenDataDocument(tempFile))
            {
                var part = m_App.Documents.Active;

                using (var stream = part.OpenStream(STREAM_NAME, AccessType_e.Read))
                {
                    var xmlSer = new XmlSerializer(typeof(TestData));
                    result = xmlSer.Deserialize(stream) as TestData;
                }
            }

            File.Delete(tempFile);

            Assert.IsNotNull(result);
            Assert.AreEqual("Test1", result.Text);
            Assert.AreEqual(15, result.Number);
        }

        /// <summary>
        /// 测试第三方存储（Storage）的读写功能。
        /// Storage 是 COM IStorage 接口的包装，支持嵌套存储和多个数据流。
        /// 用于存储更复杂的数据结构，如二进制配置信息。
        /// </summary>
        [Test]
        public void ThirdPartyStorageTest()
        {
            const string SUB_STORAGE_PATH = "_xCadIntegrationTestStorage1_\\SubStorage2";
            const string STREAM1_NAME = "_xCadIntegrationStream1_";
            const string STREAM2_NAME = "_xCadIntegrationStream2_";

            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".sldprt");

            File.Copy(GetFilePath("EmptyPart1.sldprt"), tempFile);

            using (var doc = OpenDataDocument(tempFile, false))
            {
                var part = m_App.Documents.Active;

                part.StorageWriteAvailable += (d) =>
                {
                    var path = SUB_STORAGE_PATH.Split('\\');

                    using (var storage = part.OpenStorage(path[0], AccessType_e.Write))
                    {
                        using (var subStorage = storage.TryOpenStorage(path[1], true))
                        {
                            using (var str = subStorage.TryOpenStream(STREAM1_NAME, true))
                            {
                                var buffer = Encoding.UTF8.GetBytes("Test2");
                                str.Write(buffer, 0, buffer.Length);
                            }

                            using (var str = subStorage.TryOpenStream(STREAM2_NAME, true))
                            {
                                using (var binWriter = new BinaryWriter(str))
                                {
                                    binWriter.Write(25);
                                }
                            }
                        }
                    }
                };

                part.Save();
            }

            var subStreamsCount = 0;
            var txt = "";
            var number = 0;

            using (var doc = OpenDataDocument(tempFile))
            {
                var part = m_App.Documents.Active;

                var path = SUB_STORAGE_PATH.Split('\\');

                using (var storage = part.TryOpenStorage(path[0], AccessType_e.Read))
                {
                    using (var subStorage = storage.TryOpenStorage(path[1], false))
                    {
                        subStreamsCount = subStorage.GetSubStreamNames().Length;

                        using (var str = subStorage.TryOpenStream(STREAM1_NAME, false))
                        {
                            var buffer = new byte[str.Length];

                            str.Read(buffer, 0, buffer.Length);

                            txt = Encoding.UTF8.GetString(buffer);
                        }

                        using (var str = subStorage.TryOpenStream(STREAM2_NAME, false))
                        {
                            using (var binReader = new BinaryReader(str))
                            {
                                number = binReader.ReadInt32();
                            }
                        }
                    }
                }
            }

            File.Delete(tempFile);

            Assert.AreEqual(2, subStreamsCount);
            Assert.AreEqual("Test2", txt);
            Assert.AreEqual(25, number);
        }

        /// <summary>
        /// 测试配置的零件数量。
        /// 零件数量影响 BOM 中的用量计算。
        /// </summary>
        [Test]
        public void QuantityTest()
        {
            double q1;
            double q2;
            double q3;
            double q4;
            double q5;

            using (var doc = OpenDataDocument("PartQty.SLDPRT"))
            {
                var part = (IXDocument3D)m_App.Documents.Active;
                
                q1 = part.Configurations["Conf1"].Quantity;
                q2 = part.Configurations["Conf2"].Quantity;
                q3 = part.Configurations["Conf3"].Quantity;
                q4 = part.Configurations["Conf4"].Quantity;
                q5 = part.Configurations["Conf5"].Quantity;
            }

            Assert.AreEqual(2, q1);
            Assert.AreEqual(1, q2);
            Assert.AreEqual(3, q3);
            Assert.AreEqual(2, q4);
            Assert.AreEqual(1, q5);
        }

        /// <summary>
        /// 测试打开各类原生 SOLIDWORKS 文档。
        /// 包括：装配体(.SLDASM)、零件(.SLDPRT)、工程图(.SLDDRW)、块(.SLDBLK)、库特征(.SLDLFP)、
        /// 装配体模板(.ASMDOT)、工程图模板(.DRWDOT)、零件模板(.PRTDOT)。
        /// </summary>
        [Test]
        public void OpenNativeTest() 
        {
            bool r1;
            bool r2;
            bool r3;
            bool r4;
            bool r5;
            bool r6;
            bool r7;
            bool r8;

            var a1 = m_App.Documents.PreCreateFromPath(GetFilePath(@"Native\Assembly.SLDASM"));
            a1.State = DocumentState_e.ReadOnly;
            a1.Commit();
            r1 = a1.IsAlive;
            a1.Close();

            var b1 = m_App.Documents.PreCreateFromPath(GetFilePath(@"Native\Block.SLDBLK"));
            b1.State = DocumentState_e.ReadOnly;
            b1.Commit();
            r2 = b1.IsAlive;
            b1.Close();

            var d1 = m_App.Documents.PreCreateFromPath(GetFilePath(@"Native\Drawing.SLDDRW"));
            d1.State = DocumentState_e.ReadOnly;
            d1.Commit();
            r3 = d1.IsAlive;
            d1.Close();

            var l1 = m_App.Documents.PreCreateFromPath(GetFilePath(@"Native\LibFeatPart.SLDLFP"));
            l1.State = DocumentState_e.ReadOnly;
            l1.Commit();
            r4 = l1.IsAlive;
            l1.Close();

            var p1 = m_App.Documents.PreCreateFromPath(GetFilePath(@"Native\Part.SLDPRT"));
            p1.State = DocumentState_e.ReadOnly;
            p1.Commit();
            r5 = p1.IsAlive;
            p1.Close();

            var at1 = m_App.Documents.PreCreateFromPath(GetFilePath(@"Native\TemplateAssembly.ASMDOT"));
            at1.State = DocumentState_e.ReadOnly;
            at1.Commit();
            r6 = at1.IsAlive;
            at1.Close();

            var dt1 = m_App.Documents.PreCreateFromPath(GetFilePath(@"Native\TemplateDrawing.DRWDOT"));
            dt1.State = DocumentState_e.ReadOnly;
            dt1.Commit();
            r7 = dt1.IsAlive;
            dt1.Close();

            var pt1 = m_App.Documents.PreCreateFromPath(GetFilePath(@"Native\TemplatePart.PRTDOT"));
            pt1.State = DocumentState_e.ReadOnly;
            pt1.Commit();
            r8 = pt1.IsAlive;
            pt1.Close();

            Assert.IsInstanceOf<IXAssembly>(a1);
            Assert.IsInstanceOf<IXPart>(b1);
            Assert.IsInstanceOf<IXDrawing>(d1);
            Assert.IsInstanceOf<IXPart>(l1);
            Assert.IsInstanceOf<IXPart>(p1);
            Assert.IsInstanceOf<IXAssembly>(at1);
            Assert.IsInstanceOf<IXDrawing>(dt1);
            Assert.IsInstanceOf<IXPart>(pt1);

            Assert.IsTrue(r1);
            Assert.IsTrue(r2);
            Assert.IsTrue(r3);
            Assert.IsTrue(r4);
            Assert.IsTrue(r5);
            Assert.IsTrue(r6);
            Assert.IsTrue(r7);
            Assert.IsTrue(r8);
        }

        /// <summary>
        /// 测试通过通用 IXDocument 接口打开未知类型文档。
        /// 使用 GetSpecific 方法将通用文档转换为具体类型（IXAssembly、IXPart、IXDrawing）。
        /// </summary>
        [Test]
        public void OpenNativeUnknownTest()
        {
            bool r1;
            bool r2;
            bool r3;
            bool r4;
            bool r5;
            bool r6;
            bool r7;
            bool r8;

            var a1 = m_App.Documents.PreCreate<IXDocument>();
            a1.Path = GetFilePath(@"Native\Assembly.SLDASM");
            a1.State = DocumentState_e.ReadOnly;
            a1.Commit();
            var a1_1 = ((IXUnknownDocument)a1).GetSpecific();
            r1 = a1_1.IsAlive;
            a1.Close();

            var b1 = m_App.Documents.PreCreate<IXDocument>();
            b1.Path = GetFilePath(@"Native\Block.SLDBLK");
            b1.State = DocumentState_e.ReadOnly;
            b1.Commit();
            var b1_1 = ((IXUnknownDocument)b1).GetSpecific();
            r2 = b1_1.IsAlive;
            b1.Close();

            var d1 = m_App.Documents.PreCreate<IXDocument>();
            d1.Path = GetFilePath(@"Native\Drawing.SLDDRW");
            d1.State = DocumentState_e.ReadOnly;
            d1.Commit();
            var d1_1 = ((IXUnknownDocument)d1).GetSpecific();
            r3 = d1_1.IsAlive;
            d1.Close();

            var l1 = m_App.Documents.PreCreate<IXDocument>();
            l1.Path = GetFilePath(@"Native\LibFeatPart.SLDLFP");
            l1.State = DocumentState_e.ReadOnly;
            l1.Commit();
            var l1_1 = ((IXUnknownDocument)l1).GetSpecific();
            r4 = l1_1.IsAlive;
            l1.Close();

            var p1 = m_App.Documents.PreCreate<IXDocument>();
            p1.Path = GetFilePath(@"Native\Part.SLDPRT");
            p1.State = DocumentState_e.ReadOnly;
            p1.Commit();
            var p1_1 = ((IXUnknownDocument)p1).GetSpecific();
            r5 = p1_1.IsAlive;
            p1.Close();

            var at1 = m_App.Documents.PreCreate<IXDocument>();
            at1.Path = GetFilePath(@"Native\TemplateAssembly.ASMDOT");
            at1.State = DocumentState_e.ReadOnly;
            at1.Commit();
            var at1_1 = ((IXUnknownDocument)at1).GetSpecific();
            r6 = at1_1.IsAlive;
            at1.Close();

            var dt1 = m_App.Documents.PreCreate<IXDocument>();
            dt1.Path = GetFilePath(@"Native\TemplateDrawing.DRWDOT");
            dt1.State = DocumentState_e.ReadOnly;
            dt1.Commit();
            var dt1_1 = ((IXUnknownDocument)dt1).GetSpecific();
            r7 = dt1_1.IsAlive;
            dt1.Close();

            var pt1 = m_App.Documents.PreCreate<IXDocument>();
            pt1.Path = GetFilePath(@"Native\TemplatePart.PRTDOT");
            pt1.State = DocumentState_e.ReadOnly;
            pt1.Commit();
            var pt1_1 = ((IXUnknownDocument)pt1).GetSpecific();
            r8 = pt1_1.IsAlive;
            pt1.Close();

            Assert.IsInstanceOf<IXAssembly>(a1_1);
            Assert.IsInstanceOf<IXPart>(b1_1);
            Assert.IsInstanceOf<IXDrawing>(d1_1);
            Assert.IsInstanceOf<IXPart>(l1_1);
            Assert.IsInstanceOf<IXPart>(p1_1);
            Assert.IsInstanceOf<IXAssembly>(at1_1);
            Assert.IsInstanceOf<IXDrawing>(dt1_1);
            Assert.IsInstanceOf<IXPart>(pt1_1);

            Assert.IsTrue(r1);
            Assert.IsTrue(r2);
            Assert.IsTrue(r3);
            Assert.IsTrue(r4);
            Assert.IsTrue(r5);
            Assert.IsTrue(r6);
            Assert.IsTrue(r7);
            Assert.IsTrue(r8);
        }
    }
}
