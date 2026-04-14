// -*- coding: utf-8 -*-
// src/Inventor/Utils/AiDocumentPointerEqualityComparer.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// Inventor文档指针相等比较器，实现IEqualityComparer接口。
// 用于比较两个Inventor COM文档对象是否引用同一个底层文档。
//*********************************************************************

using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.XCad.Inventor.Utils
{
    internal class AiDocumentPointerEqualityComparer: IEqualityComparer<Document>
    {
        public bool Equals(Document x, Document y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }

            string id1;
            string id2;

            try
            {
                id1 = x.InternalName;
            }
            catch
            {
                return false;
            }

            try
            {
                id2 = y.InternalName;
            }
            catch
            {
                return false;
            }

            return string.Equals(id1, id2, StringComparison.CurrentCultureIgnoreCase);
        }

        public int GetHashCode(Document obj) => 0;
    }
}
