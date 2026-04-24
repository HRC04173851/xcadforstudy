// -*- coding: utf-8 -*-
// samples/StandAlone.Inventor/Program.cs

using Inventor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Xarial.XCad.Base;
using Xarial.XCad.Documents;
using Xarial.XCad.Documents.Extensions;
using Xarial.XCad.Enums;
using Xarial.XCad.Inventor;
using Xarial.XCad.Inventor.Enums;
using Xarial.XCad.Toolkit.Windows;
using Xarial.XCad.Utils.Diagnostics;

namespace StandAlone.Ai
{
    /// <summary>
    /// Standalone console application demonstrating how to connect to Autodesk Inventor via the xCAD API
    /// and iterate through document properties and configurations.
    /// 中文：独立控制台应用程序，演示如何通过 xCAD API 连接到 Autodesk Inventor，
    /// 中文：并遍历文档属性和配置信息。
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Entry point for the standalone Inventor application.
        /// Connects to Inventor 2023 instance and reads document metadata.
        /// 中文：独立 Inventor 应用程序的入口点。连接到 Inventor 2023 实例并读取文档元数据。
        /// </summary>
        static void Main(string[] args)
        {
            // Create a new Inventor 2023 application instance via xCAD API
            // 中文：通过 xCAD API 创建新的 Inventor 2023 应用程序实例
            // Alternative connection methods:
            // 中文：备选的连接方式：
            // var app = AiApplicationFactory.FromProcess(Process.GetProcessesByName("Inventor").FirstOrDefault());
            //   — 连接到已在运行的 Inventor 进程（按进程名称查找）
            var app = AiApplicationFactory.Create(AiVersion_e.Inventor2023);

            // Open a specific part file (.ipt) from the desktop
            // 中文：打开桌面上的特定零件文件（.ipt）
            using (var doc = app.Documents.Open(@"C:\Users\artem\Desktop\Inventor\C001.ipt"))
            {
                // Check if the document is a 3D document (part, assembly, or presentation)
                // 中文：检查文档是否为三维文档（零件、装配体或演示文件）
                if (doc is IXDocument3D)
                {
                    var doc3D = (IXDocument3D)doc;

                    // Iterate through all configurations in the document
                    // 中文：遍历文档中的所有配置
                    foreach (var conf in doc3D.Configurations)
                    {
                        // Iterate through all custom properties in each configuration
                        // 中文：遍历每个配置中的所有自定义属性
                        foreach (var prp in conf.Properties)
                        {
                            // Property access point — can read/write prp.Name, prp.Value, etc.
                            // 中文：属性访问点 — 可以读取/写入 prp.Name、prp.Value 等
                        }
                    }

                    // Alternative: access the active configuration
                    // var active = doc3D.Configurations.Active;
                    // 中文：备选方式：访问活动配置

                    // Alternative: set the active configuration to the last one
                    // doc3D.Configurations.Active = doc3D.Configurations.Last();
                    // 中文：备选方式：将活动配置设置为最后一个
                }

                // Iterate through document-level properties (not configuration-specific)
                // 中文：遍历文档级别的属性（不是特定于配置的）
                foreach (var prp in doc.Properties)
                {
                    // Property access point — can read/write prp.Name, prp.Value, etc.
                    // 中文：属性访问点 — 可以读取/写入 prp.Name、prp.Value 等
                }
            }

            // Wait for user to press Enter before closing the console window
            // 中文：等待用户按 Enter 键后再关闭控制台窗口
            Console.ReadLine();
        }
    }
}
