// -*- coding: utf-8 -*-
// src/Inventor/Documents/AiProperty.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// Inventor属性接口和实现类，实现IAiProperty接口。
// 封装Inventor COM属性的访问，提供属性值的读取和写入功能。
//*********************************************************************

using Inventor;
using System;
using System.Diagnostics;
using System.Threading;
using Xarial.XCad.Data;
using Xarial.XCad.Data.Delegates;

namespace Xarial.XCad.Inventor.Documents
{
    public interface IAiProperty : IXProperty
    {
        Property Property { get; }
    }

    public interface IAiPartCellProperty : IAiProperty 
    {
        iPartTableCell Cell { get; }
    }

    public interface IAiAssemblyCellProperty : IAiProperty
    {
        iAssemblyTableCell Cell { get; }
    }

    [DebuggerDisplay("{" + nameof(Name) + "} = {" + nameof(Value) + "} ({" + nameof(Expression) + "})")]
    internal class AiProperty : IAiProperty
    {
        public event PropertyValueChangedDelegate ValueChanged;

        internal AiProperty (Property prp)
        {
            Property = prp;
        }

        public Property Property { get; }

        public string Name 
        {
            get => Property.Name; 
            set => throw new NotSupportedException(); 
        }

        public virtual object Value 
        {
            get => Property.Value; 
            set => Property.Value = value; 
        }

        public string Expression 
        {
            get => Property.Expression; 
            set => Property.Expression = value; 
        }

        public bool IsCommitted => true;

        public void Commit(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    internal class AiPartCellProperty : AiProperty, IAiPartCellProperty
    {
        public iPartTableCell Cell { get; }

        internal AiPartCellProperty(Property prp, iPartTableCell cell) : base(prp)
        {
            Cell = cell;
        }

        public override object Value 
        {
            get => Cell.Value; 
            set => Cell.Value = value?.ToString(); 
        }
    }

    internal class AiAssemblyCellProperty : AiProperty, IAiAssemblyCellProperty
    {
        public iAssemblyTableCell Cell { get; }

        internal AiAssemblyCellProperty(Property prp, iAssemblyTableCell cell) : base(prp)
        {
            Cell = cell;
        }

        public override object Value
        {
            get => Cell.Value;
            set => Cell.Value = value?.ToString();
        }
    }
}
