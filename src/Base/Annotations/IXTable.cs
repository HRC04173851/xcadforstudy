// -*- coding: utf-8 -*-
// src/Base/Annotations/IXTable.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义表格标注（Table）的跨CAD平台接口。
// Table 是工程图中用于显示结构化数据的标注类型。
//
// Table 类型：
// - BOM（Bill of Materials）：物料清单表
// - Hole Chart：孔图表
// - Weldment Cut List：焊接切割清单
// - General Table：通用表格
//
// Table 功能：
// - CreateReader：创建数据读取器
// - Read：扩展方法，直接读取为 DataTable
// - 可以导出为 Excel 等格式
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.XCad.Annotations
{
    /// <summary>
    /// Represents the table annotation
    /// 表格标注接口
    /// </summary>
    public interface IXTable : IXAnnotation
    {
        /// <summary>
        /// Returns the table data reader
        /// 返回表格数据读取器
        /// </summary>
        IDataReader CreateReader();
    }

    /// <summary>
    /// Adds additional methods for the table
    /// 表格的扩展方法
    /// </summary>
    public static class TableExtension
    {
        /// <summary>
        /// Reads the content of the table
        /// 读取表格内容
        /// </summary>
        /// <param name="table">Table to read from</param>
        /// <returns>Data table</returns>
        public static DataTable Read(this IXTable table)
        {
            var dataTable = new DataTable();
            dataTable.Load(table.CreateReader());
            return dataTable;
        }
    }
}
