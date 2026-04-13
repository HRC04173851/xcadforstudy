//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Documents;

namespace Xarial.XCad.Features.Delegates
{
    /// <summary>
    /// Delegate of <see cref="IXFeatureRepository.FeatureCreated"/> notification
    /// <see cref="IXFeatureRepository.FeatureCreated"/> 通知委托
    /// </summary>
    /// <param name="doc">Document where new feature is added（新增特征所在文档）</param>
    /// <param name="feature">Feature which is added to the document（新创建特征）</param>
    public delegate void FeatureCreatedDelegate(IXDocument doc, IXFeature feature);
}
