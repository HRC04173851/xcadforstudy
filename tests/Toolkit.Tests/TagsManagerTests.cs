// -*- coding: utf-8 -*-
// tests/Toolkit.Tests/TagsManagerTests.cs

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Toolkit.Data;

namespace Toolkit.Tests
{
    /// <summary>
    /// 测试 LocalTagsManager 的标签存储和检索功能。
    /// LocalTagsManager 提供键值对存储，支持 Put、Get、Pop、Contains 等操作。
    /// </summary>
    public class TagsManagerTests
    {
        /// <summary>
        /// 测试用例目的：验证 Put 和 Get 方法的基本功能。
        /// 包括：存储多个标签、覆盖已有标签、获取标签值。
        /// </summary>
        [Test]
        public void LocalTagsManagerTestPut()
        {
            var tagsMgr = new LocalTagsManager();

            tagsMgr.Put<string>("ABC", "Test");
            tagsMgr.Put<int>("XYZ", 10);

            var r1 = tagsMgr.Get<string>("ABC");

            // 覆盖已有标签
            tagsMgr.Put<string>("ABC", "Test2");

            var r2 = tagsMgr.Get<string>("ABC");

            var r3 = tagsMgr.Get<int>("XYZ");
            var r4 = tagsMgr.Contains("XYZ");

            Assert.AreEqual("Test", r1);
            Assert.AreEqual("Test2", r2);
            Assert.AreEqual(10, r3);
            Assert.IsTrue(r4);
        }

        /// <summary>
        /// 测试用例目的：验证 Pop 方法获取并移除标签的功能。
        /// Pop 与 Get 的区别：Pop 会移除标签，后续 Get 会失败。
        /// 同一标签多次 Put 时，Pop 返回最后放入的值（LIFO）。
        /// </summary>
        [Test]
        public void LocalTagsManagerTestPop()
        {
            var tagsMgr = new LocalTagsManager();

            tagsMgr.Put<string>("ABC", "Test");
            // 第二次 Put 覆盖，同一标签有多个值，栈式管理
            tagsMgr.Put<string>("ABC", "Test2");
            tagsMgr.Put<int>("XYZ", 10);

            var r1 = tagsMgr.Pop<string>("ABC"); // 返回 "Test2"（最后放入的）
            var r2 = tagsMgr.Pop<int>("XYZ");     // 返回 10

            var r3 = tagsMgr.Contains("ABC"); // 已被 Pop 移除
            var r4 = tagsMgr.Contains("XYZ"); // 已被 Pop 移除

            Assert.AreEqual("Test2", r1);
            Assert.AreEqual(10, r2);
            Assert.IsFalse(r3);
            Assert.IsFalse(r4);
        }

        /// <summary>
        /// 测试用例目的：验证错误处理：
        /// - 获取不存在的标签抛出 KeyNotFoundException
        /// - 类型不匹配抛出 InvalidCastException
        /// </summary>
        [Test]
        public void LocalTagsManagerTestErrors()
        {
            var tagsMgr = new LocalTagsManager();

            tagsMgr.Put<string>("ABC", "Test");
            tagsMgr.Put<int>("XYZ", 10);

            // 验证获取不存在的标签抛出 KeyNotFoundException
            Assert.Throws<KeyNotFoundException>(() => tagsMgr.Get<int>("KLM"));
            // 验证类型不匹配抛出 InvalidCastException（"ABC" 是 string 类型）
            Assert.Throws<InvalidCastException>(() => tagsMgr.Get<int>("ABC"));
        }
    }
}
