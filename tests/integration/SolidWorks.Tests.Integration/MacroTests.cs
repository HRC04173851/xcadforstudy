// -*- coding: utf-8 -*-
// tests/integration/SolidWorks.Tests.Integration/MacroTests.cs

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xarial.XCad;
using Xarial.XCad.SolidWorks;

namespace SolidWorks.Tests.Integration
{
    /// <summary>
    /// MacroTests 测试 SOLIDWORKS VBA 宏和 VSTA 宏的运行功能。
    /// 包括宏的运行入口点、自定义入口点选择、以及不同版本 SOLIDWORKS 的兼容性测试。
    /// </summary>
    public class MacroTests : IntegrationTests
    {
        /// <summary>
        /// 测试运行 VBA 宏文件，并验证宏执行后自定义属性是否被正确修改。
        /// </summary>
        [Test]
        public void RunVbaMacro() 
        {
            string val;

            using (var doc = NewDocument(Interop.swconst.swDocumentTypes_e.swDocPART))
            {
                var macro = m_App.OpenMacro(GetFilePath("VbaMacro.swp"));
                macro.Run();
                m_App.Sw.IActiveDoc2.Extension.CustomPropertyManager[""].Get5("Field1", false, out val, out _, out _);
            }

            Assert.AreEqual("main", val);
        }

        [Test]
        public void RunVbaMacroCustomEntryPoint()
        {
            string val;

            using (var doc = NewDocument(Interop.swconst.swDocumentTypes_e.swDocPART))
            {
                var macro = (ISwVbaMacro)m_App.OpenMacro(GetFilePath("VbaMacro.swp"));
                var proc = macro.EntryPoints.First(e => e.ProcedureName == "Func1");
                macro.Run(proc);
                m_App.Sw.IActiveDoc2.Extension.CustomPropertyManager[""].Get5("Field1", false, out val, out _, out _);
            }

            Assert.AreEqual("Func1", val);
        }

        [Test]
        public void VbaMacroEntryPoints()
        {
            var macro = (ISwVbaMacro)m_App.OpenMacro(GetFilePath("VbaMacro.swp"));
            var entryPoints = macro.EntryPoints.Select(e => $"{e.ModuleName}.{e.ProcedureName}");

            Assert.That(entryPoints.SequenceEqual(new string[] { "VbaMacro1.main", "VbaMacro1.Func1", "VbaMacro1.Func3", "Module1.Func4" }));
        }

        [Test]
        public void RunVsta1Macro() 
        {
            using (var doc = NewDocument(Interop.swconst.swDocumentTypes_e.swDocPART))
            {
                var macro = (ISwVstaMacro)m_App.OpenMacro(GetFilePath(@"VstaMacro\Vsta1Macro\SwMacro\bin\Debug\Vsta1Macro.dll"));
                macro.Version = VstaMacroVersion_e.Vsta1;

                var proc = macro.EntryPoints.First();

                if (m_App.Version.Major < Xarial.XCad.SolidWorks.Enums.SwVersion_e.Sw2021)
                {
                    macro.Run(proc, Xarial.XCad.Enums.MacroRunOptions_e.UnloadAfterRun);
                    m_App.Sw.IActiveDoc2.Extension.CustomPropertyManager[""].Get5("Field1", false, out string val, out _, out _);
                    Assert.AreEqual("VstaMacroText", val);
                }
                else 
                {
                    Assert.Throws<NotSupportedException>(() => macro.Run(proc, Xarial.XCad.Enums.MacroRunOptions_e.UnloadAfterRun));
                }
            }
        }
    }
}
