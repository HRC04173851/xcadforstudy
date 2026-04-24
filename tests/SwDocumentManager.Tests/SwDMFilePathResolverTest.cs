// -*- coding: utf-8 -*-
// tests/SwDocumentManager.Tests/SwDMFilePathResolverTest.cs

//*********************************************************************
//xCAD
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Documents.Exceptions;
using Xarial.XCad.SwDocumentManager.Services;

namespace SolidWorks.Tests
{
    /// <summary>
    /// MockSwDmFilePathResolver：SwDmFilePathResolver 的测试用实现。
    /// 允许自定义引用存在判断逻辑。
    /// </summary>
    public class MockSwDmFilePathResolver : SwDmFilePathResolver
    {
        private readonly Predicate<string> m_RefExistsPred;

        public MockSwDmFilePathResolver(Predicate<string> refExistsPred)
        {
            m_RefExistsPred = refExistsPred;
        }

        protected override bool IsReferenceExists(string path) => m_RefExistsPred.Invoke(path);
    }

    /// <summary>
    /// 测试 SwDmFilePathResolver SOLIDWORKS Document Manager 文件路径解析功能。
    /// 与 SwFilePathResolverBase 类似，但用于 Document Manager 环境（只读访问）。
    /// </summary>
    public class SwDmFilePathResolverTest
    {
        /// <summary>
        /// 测试用例目的：验证路径解析算法遍历所有可能的搜索路径组合。
        /// 测试场景：所有路径都不存在，验证所有候选路径都被尝试。
        /// </summary>
        [Test]
        public void ResolvePath_AllRoutes()
        {
            var paths = new List<string>();

            var res = new MockSwDmFilePathResolver(p =>
            {
                paths.Add(p);
                return false; // 所有路径都不存在
            });

            // 预期路径：从参考文档所在目录向上搜索 4 层目录的所有组合
            var expPaths = new string[]
            {
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
            Assert.Throws<FilePathResolveFailedException>(() => res.ResolvePath(@"D:\ss\tt", @"C:\zz\yy\xx\p2.sldprt"));
            CollectionAssert.AreEqual(expPaths, paths);
        }

        /// <summary>
        /// 测试用例目的：验证在搜索路径中找到引用文档时的路径解析。
        /// 测试场景：在 D:\ss\tt\yy\xx\ 找到目标文件。
        /// </summary>
        [Test]
        public void ResolvePath_Test()
        {
            var res = new MockSwDmFilePathResolver(p => p == @"D:\ss\tt\yy\xx\p2.sldprt");

            var p1 = res.ResolvePath(@"D:\ss\tt\a1.sldasm", @"C:\zz\yy\xx\p2.sldprt");

            Assert.AreEqual(@"D:\ss\tt\yy\xx\p2.sldprt", p1);
        }

        /// <summary>
        /// 测试用例目的：验证直接使用原始路径的情况。
        /// 测试场景：引用文档路径已存在，直接使用。
        /// </summary>
        [Test]
        public void ResolvePath_Initial()
        {
            var res = new MockSwDmFilePathResolver(p => p == @"C:\zz\yy\xx\p2.sldprt");

            var p1 = res.ResolvePath(@"D:\ss\tt\a1.sldasm", @"C:\zz\yy\xx\p2.sldprt");

            Assert.AreEqual(@"C:\zz\yy\xx\p2.sldprt", p1);
        }
    }
}
