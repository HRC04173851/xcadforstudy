// -*- coding: utf-8 -*-
// src/SolidWorks/Utils/SwPointerComparer.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 通过 ISldWorks.IsSame 方法比较 COM 对象指针相等性的通用比较器，处理断开连接的指针。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.SolidWorks.Utils
{
    internal class SwPointerEqualityComparer<T> : IEqualityComparer<T>
        where T : class
    {
        private readonly ISldWorks m_App;

        internal SwPointerEqualityComparer(ISldWorks app) 
        {
            m_App = app;
        }

        public bool Equals(T x, T y)
        {
            if (object.ReferenceEquals(x, y)) 
            {
                return true;
            }

            if (x == null || y == null) 
            {
                return false;
            }

            try
            {
                //Note: ISldWorks::IsSame can crash if pointer is disconnected

                if (IsAlive(x) && IsAlive(y))
                {
                    return m_App.IsSame(x, y) == (int)swObjectEquality.swObjectSame;
                }
                else 
                {
                    return false;
                }
            }
            catch 
            {
                return false;
            }
        }

        protected virtual bool IsAlive(T obj) => true;

        public int GetHashCode(T obj)
        {
            return 0;
        }
    }
}
