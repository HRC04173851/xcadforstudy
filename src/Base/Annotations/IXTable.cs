//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
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
