// -*- coding: utf-8 -*-
// ISwOptions.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// SolidWorks应用程序选项接口定义，提供应用程序级别的选项配置
//*********************************************************************

namespace Xarial.XCad.SolidWorks
{
    public interface ISwOptions : IXOptions 
    {
    }

    internal class SwOptions : ISwOptions 
    {
    }
}