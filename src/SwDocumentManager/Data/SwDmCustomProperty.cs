//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.swdocumentmgr;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xarial.XCad.Data;
using Xarial.XCad.Data.Delegates;
using Xarial.XCad.Services;

namespace Xarial.XCad.SwDocumentManager.Data
{
    /// <summary>
    /// Represents a custom property exposed through Document Manager.
    /// 表示通过 Document Manager 访问的自定义属性。
    /// </summary>
    public interface ISwDmCustomProperty : IXProperty
    {
    }

    /// <summary>
    /// Base implementation that normalizes property read, write, and type conversion logic.
    /// 统一处理属性读写、提交状态与类型转换逻辑的基础实现。
    /// </summary>
    internal abstract class SwDmCustomProperty : ISwDmCustomProperty
    {
        public event PropertyValueChangedDelegate ValueChanged;

        private string m_Name;
        private object m_TempValue;

        public string Name 
        {
            get => m_Name;
            set => RenameProperty(m_Name, value);
        }

        /// <summary>
        /// Stores either the resolved value or the raw expression depending on the Document Manager state.
        /// 根据 Document Manager 当前状态返回解析后的属性值，或在未提交时返回暂存值。
        /// </summary>
        public object Value 
        {
            get 
            {
                if (IsCommitted)
                {
                    return ReadValue(out _);
                }
                else 
                {
                    return m_TempValue;
                }
            }
            set 
            {
                if (IsCommitted)
                {
                    SetValue(value);
                    ValueChanged?.Invoke(this, value);
                }
                else 
                {
                    m_TempValue = value;
                }
            }
        }

        /// <summary>
        /// Accesses the expression text behind the property value, such as equation-driven references.
        /// 访问属性值背后的表达式文本，例如由方程或链接驱动的属性表达式。
        /// </summary>
        public string Expression
        {
            get
            {
                if (IsCommitted)
                {
                    ReadValue(out string exp);
                    return exp;
                }
                else
                {
                    return m_TempValue as string;
                }
            }
            set
            {
                if (IsCommitted)
                {
                    SetValue(value);
                    ValueChanged?.Invoke(this, value);
                }
                else
                {
                    m_TempValue = value;
                }
            }
        }

        public bool IsCommitted { get; set; }

        internal SwDmCustomProperty(string name, bool isCreated) 
        {
            m_Name = name;
            IsCommitted = isCreated;
        }

        /// <summary>
        /// Commits a pre-created property into the underlying document/configuration storage.
        /// 将预创建的属性真正写入底层文档或配置的属性存储中。
        /// </summary>
        public void Commit(CancellationToken cancellationToken)
        {
            AddValue(Value);
            IsCommitted = true;
        }

        private void RenameProperty(string from, string to) 
        {
            m_Name = to;

            if (IsCommitted) 
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Infers the SOLIDWORKS custom property type from a .NET runtime value.
        /// 根据 .NET 运行时值推断 SOLIDWORKS 自定义属性的数据类型。
        /// </summary>
        protected SwDmCustomInfoType GetPropertyType(object value)
        {
            SwDmCustomInfoType type = SwDmCustomInfoType.swDmCustomInfoUnknown;

            if (value is string)
            {
                type = SwDmCustomInfoType.swDmCustomInfoText;
            }
            else if (value is bool)
            {
                type = SwDmCustomInfoType.swDmCustomInfoYesOrNo;
            }
            else if (value is int || value is double)
            {
                type = SwDmCustomInfoType.swDmCustomInfoNumber;
            }
            else if (value is DateTime)
            {
                type = SwDmCustomInfoType.swDmCustomInfoDate;
            }

            return type;
        }

        /// <summary>
        /// Converts the raw string-based Document Manager value into a .NET object while preserving the expression.
        /// 将 Document Manager 返回的字符串值转换为 .NET 对象，并同时保留原始表达式文本。
        /// </summary>
        protected object ReadValue(out string expression)
        {
            var val = ReadRawValue(out SwDmCustomInfoType type, out expression);

            object resVal;

            switch (type)
            {
                case SwDmCustomInfoType.swDmCustomInfoText:
                    resVal = val;
                    break;

                case SwDmCustomInfoType.swDmCustomInfoYesOrNo:
                    switch (val.ToLower())
                    {
                        case "yes":
                            resVal = true;
                            break;

                        case "no":
                            resVal = false;
                            break;

                        default:
                            if (bool.TryParse(val, out var boolVal))
                            {
                                resVal = boolVal;
                            }
                            else
                            {
                                resVal = val;
                            }
                            break;
                    }
                    break;

                case SwDmCustomInfoType.swDmCustomInfoNumber:
                    resVal = double.Parse(val);
                    break;

                case SwDmCustomInfoType.swDmCustomInfoDate:
                    resVal = DateTime.Parse(val);
                    break;

                default:
                    resVal = val;
                    break;
            }

            if (string.IsNullOrEmpty(expression))
            {
                expression = val;
            }

            return resVal;
        }

        internal abstract void Delete();
        protected abstract string ReadRawValue(out SwDmCustomInfoType type, out string linkedTo);
        protected abstract void AddValue(object value);
        protected abstract void SetValue(object value);
    }
}
