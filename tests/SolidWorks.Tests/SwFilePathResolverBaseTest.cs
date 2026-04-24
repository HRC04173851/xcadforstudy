// -*- coding: utf-8 -*-
// tests/SolidWorks.Tests/SwFilePathResolverBaseTest.cs

using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Documents.Exceptions;
using Xarial.XCad.SolidWorks.Exceptions;
using Xarial.XCad.SolidWorks.Services;

namespace SolidWorks.Tests
{
    /// <summary>
    /// MockSwFilePathResolverBase：SwFilePathResolverBase 的测试用实现。
    /// 允许自定义搜索文件夹、引用存在判断、已加载文档路径查找。
    /// </summary>
    public class MockSwFilePathResolverBase : SwFilePathResolverBase
    {
        private readonly string[] m_SearchFolders;
        private readonly Predicate<string> m_RefExistsPred;
        private readonly Func<string> m_MatchingLoadedDocPath;

        public MockSwFilePathResolverBase(string[] searchFolders, Predicate<string> refExistsPred, Func<string> matchingLoadedDocPath)
        {
            m_SearchFolders = searchFolders;
            m_RefExistsPred = refExistsPred;
            m_MatchingLoadedDocPath = matchingLoadedDocPath;
        }

        protected override string[] GetSearchFolders() => m_SearchFolders;

        protected override bool IsReferenceExists(string path) => m_RefExistsPred.Invoke(path);

        protected override bool TryGetLoadedDocumentPath(string path, out string loadedPath)
        {
            loadedPath = m_MatchingLoadedDocPath.Invoke();
            return !string.IsNullOrEmpty(loadedPath);
        }
    }

    /// <summary>
    /// 测试 SwFilePathResolverBase 引用路径解析功能。
    /// SwFilePathResolverBase 负责在指定搜索文件夹中查找引用文档的路径。
    /// </summary>
    public class SwFilePathResolverBaseTest
    {
        /// <summary>
        /// 测试用例目的：验证路径解析算法遍历所有可能的搜索路径组合。
        /// 测试场景：两个搜索文件夹，解析失败，验证所有候选路径都被尝试。
        /// 预期结果：验证预期的所有路径组合都被尝试过。
        /// </summary>
        [Test]
        public void ResolvePath_AllRoutes()
        {
            var paths = new List<string>();

            var res = new MockSwFilePathResolverBase(new string[]
            {
                @"D:\aa\bb\",
                @"E:\cc\dd\"
            }, p =>
            {
                paths.Add(p);
                return false; // 所有路径都不存在
            }, () => "");

            // 预期路径：两个搜索文件夹的 7 层目录组合 + 原始文档所在目录的向上搜索
            var expPaths = new string[]
            {
                @"D:\aa\bb\p2.sldprt",
                @"D:\aa\bb\xx\p2.sldprt",
                @"D:\aa\bb\yy\xx\p2.sldprt",
                @"D:\aa\bb\zz\yy\xx\p2.sldprt",
                @"D:\aa\xx\p2.sldprt",
                @"D:\aa\yy\xx\p2.sldprt",
                @"D:\aa\zz\yy\xx\p2.sldprt",
                @"D:\xx\p2.sldprt",
                @"D:\yy\xx\p2.sldprt",
                @"D:\zz\yy\xx\p2.sldprt",
                @"E:\cc\dd\p2.sldprt",
                @"E:\cc\dd\xx\p2.sldprt",
                @"E:\cc\dd\yy\xx\p2.sldprt",
                @"E:\cc\dd\zz\yy\xx\p2.sldprt",
                @"E:\cc\xx\p2.sldprt",
                @"E:\cc\yy\xx\p2.sldprt",
                @"E:\cc\zz\yy\xx\p2.sldprt",
                @"E:\xx\p2.sldprt",
                @"E:\yy\xx\p2.sldprt",
                @"E:\zz\yy\xx\p2.sldprt",
                @"D:\ss\tt\p2.sldprt",
                @"D:\ss\tt\xx\p2.sldprt",
                @"D:\ss\tt\yy\xx\p2.sldprt",
                @"D:\ss\tt\zz\yy\xx\p2.sldprt",
                @"D:\ss\xx\p2.sldprt",
                @"D:\ss\yy\xx\p2.sldprt",
                @"D:\ss\zz\yy\xx\p2.sldprt",
                @"D:\xx\p2.sldprt",
                @"D:\yy\xx\p2.sldprt",
                @"D:\zz\yy\xx\p2.sldprt",
                @"C:\zz\yy\xx\p2.sldprt"
            };

            // 验证所有路径都尝试过但都失败，最终抛出异常
            Assert.Throws<FilePathResolveFailedException>(() => res.ResolvePath(@"D:\ss\tt\a1.sldasm", @"C:\zz\yy\xx\p2.sldprt"));
            CollectionAssert.AreEqual(expPaths, paths);
        }

        /// <summary>
        /// 测试用例目的：验证在搜索文件夹中找到引用文档时的路径解析。
        /// 测试场景：在 E:\yy\xx\ 找到目标文件。
        /// </summary>
        [Test]
        public void ResolvePath_Test()
        {
            var res = new MockSwFilePathResolverBase(new string[]
            {
                @"D:\aa\bb\",
                @"E:\cc\dd\"
            }, p => p == @"E:\yy\xx\p2.sldprt", () => "");

            var p1 = res.ResolvePath(@"D:\ss\tt\a1.sldasm", @"C:\zz\yy\xx\p2.sldprt");

            Assert.AreEqual(@"E:\yy\xx\p2.sldprt", p1);
        }

        /// <summary>
        /// 测试用例目的：验证优先使用已加载文档路径的功能。
        /// 测试场景：存在已加载文档 X:\p2.sldprt，优先使用而非搜索。
        /// </summary>
        [Test]
        public void ResolvePath_Active()
        {
            var res = new MockSwFilePathResolverBase(new string[]
            {
                @"D:\aa\bb\",
                @"E:\cc\dd\"
            }, p => false, () => "X:\\p2.sldprt"); // 存在已加载文档

            var p1 = res.ResolvePath(@"D:\ss\tt\a1.sldasm", @"C:\zz\yy\xx\p2.sldprt");

            // 返回已加载文档路径，不进行文件系统搜索
            Assert.AreEqual("X:\\p2.sldprt", p1);
        }

        /// <summary>
        /// 测试用例目的：验证直接使用原始路径（无搜索文件夹）的情况。
        /// 测试场景：无搜索文件夹，直接使用目标引用路径。
        /// </summary>
        [Test]
        public void ResolvePath_Initial()
        {
            var res = new MockSwFilePathResolverBase(new string[0], p => p == @"C:\zz\yy\xx\p2.sldprt", () => "");

            var p1 = res.ResolvePath(@"D:\ss\tt\a1.sldasm", @"C:\zz\yy\xx\p2.sldprt");

            Assert.AreEqual(@"C:\zz\yy\xx\p2.sldprt", p1);
        }
    }
}
