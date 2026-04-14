// -*- coding: utf-8 -*-
// src/Inventor/Documents/AiDocument3D.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// Inventor 3D文档基类，实现IAiDocument3D接口。
// 提供3D文档的通用功能，如组件枚举、几何数据访问和文档类型识别。
//*********************************************************************

using Inventor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xarial.XCad.Base;
using Xarial.XCad.Documents;
using Xarial.XCad.Documents.Delegates;
using Xarial.XCad.Geometry;

namespace Xarial.XCad.Inventor.Documents
{
    public interface IAiDocument3D : IAiDocument, IXDocument3D
    {
    }

    internal abstract class AiDocument3D : AiDocument, IAiDocument3D
    {
        internal AiDocument3D(Document doc3D, AiApplication ownerApp) : base(doc3D, ownerApp)
        {
        }

        public IXDocumentEvaluation Evaluation => throw new NotImplementedException();

        public IXDocumentGraphics Graphics => throw new NotImplementedException();

        public abstract IXConfigurationRepository Configurations { get; }

        IXModelView3DRepository IXDocument3D.ModelViews => throw new NotImplementedException();

        IXDocument3DSaveOperation IXDocument3D.PreCreateSaveAsOperation(string filePath)
        {
            var translator = TryGetTranslator(filePath);

            if (translator != null)
            {
                switch (translator.ClientId)
                {
                    case "{90AF7F40-0C01-11D5-8E83-0010B541CD80}":
                        return new AiStepSaveOperation(this, translator, filePath);

                    default:
                        return new AiDocument3DTranslatorSaveOperation(this, translator, filePath);
                }
            }
            else
            {
                return new AiDocument3DSaveOperation(this, filePath);
            }
        }

        TSelObject IXObjectContainer.ConvertObject<TSelObject>(TSelObject obj)
        {
            throw new NotImplementedException();
        }

        public override IXSaveOperation PreCreateSaveAsOperation(string filePath) => ((IXDocument3D)this).PreCreateSaveAsOperation(filePath);
    }
}
