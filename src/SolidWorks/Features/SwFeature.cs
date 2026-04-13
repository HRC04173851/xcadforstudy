//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using Xarial.XCad.Annotations;
using Xarial.XCad.Documents;
using Xarial.XCad.Features;
using Xarial.XCad.Geometry;
using Xarial.XCad.Services;
using Xarial.XCad.SolidWorks.Annotations;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Geometry;
using Xarial.XCad.SolidWorks.Utils;
using System.Linq;

namespace Xarial.XCad.SolidWorks.Features
{
    /// <summary>
    /// SolidWorks 特征接口，继承自可选对象、xCAD 特征、实体，以及支持弹性恢复的接口。
    /// 特征（Feature）是 SolidWorks 模型树中的基本构建单元，包括拉伸、旋转、草图、基准面等。
    /// </summary>
    public interface ISwFeature : ISwSelObject, IXFeature, ISwEntity, ISupportsResilience<ISwFeature>
    {
        /// <summary>获取底层 SolidWorks 特征 COM 对象（IFeature）。</summary>
        IFeature Feature { get; }
        /// <summary>获取该特征的尺寸标注集合。</summary>
        new ISwDimensionsCollection Dimensions { get; }
        /// <summary>获取该特征所属的零部件（装配体上下文中使用）。</summary>
        new ISwComponent Component { get; }
    }

    /// <summary>
    /// 特征编辑器抽象基类，负责打开和关闭特征定义编辑会话。
    /// 实现 IEditor&lt;SwFeature&gt; 接口，使用 using 语句可自动完成/取消编辑。
    /// </summary>
    internal abstract class SwFeatureEditor<TFeatData> : IEditor<SwFeature>
    {
        /// <summary>正在编辑的目标特征。</summary>
        public SwFeature Target { get; }

        /// <summary>若设为 true，则在 Dispose 时取消编辑（不应用修改）。</summary>
        public bool Cancel 
        {
            get;
            set;
        }

        private readonly TFeatData m_FeatData;
        private readonly ISwDocument m_Doc;
        private readonly ISwComponent m_Comp;

        internal SwFeatureEditor(SwFeature feat, TFeatData featData)
        {
            Target = feat;
            m_FeatData = featData;

            m_Doc = Target.OwnerDocument;
            m_Comp = Target.Component;

            // 调用 StartEdit 进入特征编辑模式
            if (!StartEdit(m_FeatData, m_Doc, m_Comp)) 
            {
                throw new Exception("Failed to start editing of the feature");
            }
        }

        /// <summary>开始编辑特征定义（进入编辑模式）。</summary>
        protected abstract bool StartEdit(TFeatData featData, ISwDocument doc, ISwComponent comp);
        /// <summary>取消编辑时的回滚操作。</summary>
        protected abstract void CancelEdit(TFeatData featData);

        private void EndEdit(bool cancel)
        {
            if (!cancel)
            {
                // 提交修改：调用 SolidWorks ModifyDefinition API 应用特征定义变更
                if (!Target.Feature.ModifyDefinition(m_FeatData, m_Doc.Model, m_Comp?.Component))
                {
                    throw new Exception("Failed to modify defintion of the feature");
                }
            }
            else 
            {
                CancelEdit(m_FeatData);
            }
        }

        public void Dispose()
        {
            EndEdit(Cancel);
        }
    }

    /// <summary>
    /// 特征的几何实体存储库，提供对特征所拥有的面、边、顶点等几何实体的访问。
    /// </summary>
    internal class SwFeatureEntityRepository : SwEntityRepository
    {
        private readonly SwFeature m_Feat;

        internal SwFeatureEntityRepository(SwFeature feat) 
        {
            m_Feat = feat;
        }

        protected override IEnumerable<ISwEntity> IterateEntities(bool faces, bool edges, bool vertices, bool silhouetteEdges)
        {
            if (faces)
            {
                // 获取特征的所有面（IFace2 列表）
                var featFaces = (object[])m_Feat.Feature.GetFaces();

                if (featFaces != null)
                {
                    foreach (var face in featFaces)
                    {
                        yield return m_Feat.OwnerDocument.CreateObjectFromDispatch<ISwFace>(face);
                    }
                }
            }
        }
    }

    /// <summary>
    /// SolidWorks 特征实现类，调试器中显示特征名称。
    /// 封装了 SolidWorks IFeature COM 对象，提供特征名称、状态、尺寸、几何实体等访问能力。
    /// </summary>
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    internal class SwFeature : SwSelObject, ISwFeature
    {
        /// <summary>
        /// SolidWorks 中被"焊死"（不可删除/不可重命名）的系统固有特征类型名称列表。
        /// 这些特征是 SolidWorks 文档的固定组成部分，如注释文件夹、材料节点、原点等。
        /// </summary>
        private static string[] m_SolderedFeatureTypes = new string[]
        {
            "CommentsFolder",       // 注释文件夹
            "FavoriteFolder",       // 收藏文件夹
            "HistoryFolder",        // 历史记录文件夹
            "SelectionSetFolder",   // 选择集文件夹
            "SensorFolder",         // 传感器文件夹
            "DocsFolder",           // 文档文件夹
            "DetailCabinet",        // 详图文件夹
            "NotesAreaFtrFolder",   // 注释区域文件夹
            "SurfaceBodyFolder",    // 曲面体文件夹
            "SolidBodyFolder",      // 实体体文件夹
            "EnvFolder",            // 环境文件夹
            "AmbientLight",         // 环境光
            "DirectionLight",       // 方向光
            "InkMarkupFolder",      // 墨迹标记文件夹
            "EqnFolder",            // 方程式文件夹
            "MaterialFolder",       // 材料文件夹
            SwOrigin.TypeName,      // 原点
            "LiveSectionFolder",    // 实时剖切文件夹
            "MateGroup",            // 配合组
            "BlockFolder",          // 块文件夹
            "MarkupCommentFolder",  // 标记注释文件夹
            "DrSheet",              // 工程图图纸
            "DetailFolder",         // 详图文件夹
            "DrTemplate",           // 工程图模板
            "GeneralTableAnchor",   // 通用表格锚点
            "BomTemplate",          // BOM（材料清单）模板
            "HoleTableAnchor",      // 孔表锚点
            "WeldmentTableAnchor",  // 焊接件表格锚点
            "RevisionTableAnchor",  // 修订表格锚点
            "WeldTableAnchor",      // 焊接表格锚点
            "BendTableAnchor",      // 折弯表格锚点（钣金）
            "PunchTableAnchor",     // 冲孔表格锚点
            "EditBorderFeature"     // 编辑边框特征
        };

        IXBody IXEntity.Body => Body;
        IXEntityRepository IXEntity.AdjacentEntities => AdjacentEntities;
        ISwEntity ISupportsResilience<ISwEntity>.CreateResilient() => CreateResilient();
        IXComponent IXEntity.Component => Component;
        IXDimensionRepository IDimensionable.Dimensions => Dimensions;
        IXObject ISupportsResilience.CreateResilient() => CreateResilient();

        protected readonly IElementCreator<IFeature> m_Creator;

        public virtual IFeature Feature 
        {
            get
            {
                var feat = m_Creator.Element;

                if (IsResilient)
                {
                    try
                    {
                        var testPtrAlive = feat.Name;
                    }
                    catch
                    {
                        var restoredFeat = (IFeature)OwnerDocument.Model.Extension.GetObjectByPersistReference3(m_PersistId, out _);

                        if (restoredFeat != null)
                        {
                            feat = restoredFeat;
                            m_Creator.Set(feat);
                        }
                        else
                        {
                            throw new NullReferenceException("Pointer to the feature cannot be restored");
                        }
                    }
                }

                return feat;
            }
        }

        public override object Dispatch => Feature;

        public override bool IsAlive => this.CheckIsAlive(() => Feature.GetID());

        public bool IsResilient { get; private set; }

        private byte[] m_PersistId;

        private readonly Lazy<SwFeatureDimensionsCollection> m_DimensionsLazy;
        private Context m_Context;

        internal SwFeature(IFeature feat, SwDocument doc, SwApplication app, bool created) : base(feat, doc, app)
        {
            if (doc == null) 
            {
                throw new ArgumentNullException(nameof(doc));
            }

            m_DimensionsLazy = new Lazy<SwFeatureDimensionsCollection>(() => 
            {
                if (m_Context == null)
                {
                    var comp = ((IEntity)Feature).GetComponent();

                    if (comp != null)
                    {
                        m_Context = new Context(OwnerDocument.CreateObjectFromDispatch<ISwComponent>(comp));
                    }
                    else
                    {
                        m_Context = new Context(OwnerDocument);
                    }
                }

                return new SwFeatureDimensionsCollection(this, OwnerDocument, m_Context);
            });

            m_Creator = new ElementCreator<IFeature>(CreateFeature, CommitCache, feat, created);

            AdjacentEntities = new SwFeatureEntityRepository(this);
        }

        internal void SetContext(Context context) 
        {
            m_Context = context;
        }

        public override void Commit(CancellationToken cancellationToken) => m_Creator.Create(cancellationToken);

        public virtual ISwFeature CreateResilient()
        {
            if (OwnerDocument == null)
            {
                throw new NullReferenceException("Owner document is not set");
            }

            var id = (byte[])OwnerDocument.Model.Extension.GetPersistReference3(Feature);

            if (id != null)
            {
                var feat = OwnerDocument.CreateObjectFromDispatch<SwFeature>(Feature);
                feat.MakeResilient(id);
                return feat;
            }
            else
            {
                throw new Exception("Failed to create resilient feature");
            }
        }

        private void MakeResilient(byte[] persistId)
        {
            IsResilient = true;
            m_PersistId = persistId;
        }

        public ISwComponent Component
        {
            get
            {
                var comp = (IComponent2)((IEntity)Feature).GetComponent();

                if (comp != null)
                {
                    return OwnerDocument.CreateObjectFromDispatch<ISwComponent>(comp);
                }
                else
                {
                    return null;
                }
            }
        }

        private IFeature CreateFeature(CancellationToken cancellationToken)
        {
            using (var viewFreeze = new UiFreeze(OwnerDocument))
            {
                var feat = InsertFeature(cancellationToken);

                var userName = Name;

                if (!string.IsNullOrEmpty(userName))
                {
                    feat.Name = userName;
                }

                var userColor = Color;

                if (userColor.HasValue)
                {
                    SetColor(feat, userColor);
                }

                return feat;
            }
        }

        protected virtual IFeature InsertFeature(CancellationToken cancellationToken)
            => throw new NotSupportedException("Creation of this feature is not supported");

        protected virtual void CommitCache(IFeature feat, CancellationToken cancellationToken)
        {
        }

        public ISwDimensionsCollection Dimensions => m_DimensionsLazy.Value;

        public string Name 
        {
            get
            {
                if (IsCommitted)
                {
                    return Feature.Name;
                }
                else 
                {
                    return m_Creator.CachedProperties.Get<string>();
                }
            }
            set 
            {
                if (IsCommitted)
                {
                    Feature.Name = value;
                }
                else 
                {
                    m_Creator.CachedProperties.Set(value);
                }
            }
        }
        
        public Color? Color
        {
            get
            {
                if (IsCommitted)
                {
                    return GetColor(Feature);
                }
                else
                {
                    return m_Creator.CachedProperties.Get<Color?>();
                }
            }
            set
            {
                if (IsCommitted)
                {
                    SetColor(Feature, value);
                }
                else
                {
                    m_Creator.CachedProperties.Set(value);
                }
            }
        }

        private Color? GetColor(IFeature feat) => SwColorHelper.GetColor((IComponent2)((IEntity)feat).GetComponent(),
            (o, c) => feat.GetMaterialPropertyValues2((int)o, c) as double[]);

        private void SetColor(IFeature feat, Color? color)=> SwColorHelper.SetColor(color, (IComponent2)((IEntity)feat).GetComponent(),
                (m, o, c) => feat.SetMaterialPropertyValues2(m, (int)o, c),
                (o, c) => feat.RemoveMaterialProperty2((int)o, c));

        public override bool IsCommitted => m_Creator.IsCreated;

        public FeatureState_e State 
        {
            get 
            {
                var state = FeatureState_e.Default;

                var suppStates = (bool[])Feature.IsSuppressed2((int)swInConfigurationOpts_e.swThisConfiguration, null);

                if (suppStates[0]) 
                {
                    state |= FeatureState_e.Suppressed;
                }

                return state;
            }
            set 
            {
                swFeatureSuppressionAction_e action;

                if (value.HasFlag(FeatureState_e.Suppressed))
                {
                    action = swFeatureSuppressionAction_e.swSuppressFeature;
                }
                else 
                {
                    action = swFeatureSuppressionAction_e.swUnSuppressFeature;
                }

                if (!Feature.SetSuppression2((int)action, (int)swInConfigurationOpts_e.swThisConfiguration, null)) 
                {
                    throw new Exception("Failed to change the suppresion of the feature");
                }
            }
        }

        public virtual bool IsUserFeature => Array.IndexOf(m_SolderedFeatureTypes, Feature.GetTypeName2()) == -1;

        public IEntity Entity => (IEntity)Feature;

        public ISwEntityRepository AdjacentEntities { get; }

        public virtual ISwBody Body 
        {
            get 
            {
                var bodies = AdjacentEntities.Cast<ISwEntity>().Select(e => e.Body).Distinct(new XObjectEqualityComparer<ISwBody>()).ToArray();

                if (bodies.Length == 1)
                {
                    return bodies.First();
                }
                else if (bodies.Length == 0)
                {
                    throw new Exception("This feature does not have bodies");
                }
                else
                {
                    throw new Exception("This feature has multiple bodies");
                }
            }
        }

        internal override void Select(bool append, ISelectData selData)
        {
            if (!Feature.Select2(append, selData?.Mark ?? 0))
            {
                throw new Exception("Faile to select feature");
            }
        }

        public virtual IEditor<IXFeature> Edit() => throw new NotSupportedException();

        public XCad.Geometry.Structures.Point FindClosestPoint(XCad.Geometry.Structures.Point point)
            => throw new NotSupportedException();

        protected override bool IsSameDispatch(object disp)
        {
            if (OwnerApplication.Sw.IsSame(disp, Dispatch) == (int)swObjectEquality.swObjectSame)
            {
                return true;
            }
            else 
            {
                //NOTE: some of the features override the dispatch to be a specific feature (e.g. RefPlane)
                //this results in different pointers when comparing and some of the methods (like IsSelected) may incorrectly
                //compare the pointers and return unexpected result depending on the method feature was selected (e.g. Feature::Select selects IFeature, while SelectByID2 selected IRefPlane)
                if (Dispatch != Feature)
                {
                    return OwnerApplication.Sw.IsSame(disp, Feature) == (int)swObjectEquality.swObjectSame;
                }
                else 
                {
                    return false;
                }
            }
        }
    }
}