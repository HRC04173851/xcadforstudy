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
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using Xarial.XCad.Annotations;
using Xarial.XCad.Data;
using Xarial.XCad.Documents;
using Xarial.XCad.Documents.Enums;
using Xarial.XCad.Features;
using Xarial.XCad.Reflection;
using Xarial.XCad.Services;
using Xarial.XCad.SolidWorks.Annotations;
using Xarial.XCad.SolidWorks.Data;
using Xarial.XCad.SolidWorks.Documents.Exceptions;
using Xarial.XCad.SolidWorks.Enums;
using Xarial.XCad.SolidWorks.Features;
using Xarial.XCad.SolidWorks.Utils;
using Xarial.XCad.UI;

namespace Xarial.XCad.SolidWorks.Documents
{
    /// <summary>
    /// SolidWorks-specific configuration interface extending the base XCad configuration.
    /// <para>中文：SolidWorks 专用配置接口，扩展 xCAD 基础配置接口，提供对 SolidWorks 配置对象和自定义属性集合的访问。</para>
    /// </summary>
    public interface ISwConfiguration : ISwSelObject, IXConfiguration, IDisposable
    {
        /// <summary>
        /// Gets the underlying SolidWorks native <see cref="IConfiguration"/> object.
        /// <para>中文：获取底层 SolidWorks 原生配置对象（IConfiguration）。</para>
        /// </summary>
        IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the custom properties collection associated with this configuration.
        /// <para>中文：获取与此配置关联的自定义属性集合。</para>
        /// </summary>
        new ISwCustomPropertiesCollection Properties { get; }
    }

    /// <summary>
    /// SolidWorks configuration implementation representing a named variant of a document.
    /// <para>中文：SolidWorks 配置实现类，表示文档的一个命名变体（配置），提供名称、自定义属性、尺寸、预览图、零件号、数量、物料清单子项解析等功能。</para>
    /// </summary>
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    internal class SwConfiguration : SwSelObject, ISwConfiguration
    {
        /// <summary>
        /// The custom property name used to determine the quantity (数量) of the component in the BOM.
        /// <para>中文：用于从自定义属性中解析组件数量的属性名称常量（物料清单数量属性键）。</para>
        /// </summary>
        internal const string QTY_PROPERTY = "UNIT_OF_MEASURE";

        /// <summary>
        /// Gets the underlying native SolidWorks IConfiguration object.
        /// <para>中文：获取底层 SolidWorks 原生 IConfiguration 配置对象。</para>
        /// </summary>
        public IConfiguration Configuration => m_Creator.Element;

        /// <summary>
        /// 中文：所属的三维文档（零件或装配体文档）。
        /// </summary>
        private readonly SwDocument3D m_Doc;

        /// <summary>
        /// Gets or sets the name of this configuration.
        /// <para>中文：获取或设置此配置的名称。若配置尚未创建，则读写缓存属性。</para>
        /// </summary>
        public virtual string Name
        {
            get
            {
                if (m_Creator.IsCreated)
                {
                    return Configuration.Name;
                }
                else
                {
                    return m_Creator.CachedProperties.Get<string>();
                }
            }
            set
            {
                if (m_Creator.IsCreated)
                {
                    Configuration.Name = value;
                }
                else
                {
                    m_Creator.CachedProperties.Set(value);
                }
            }
        }

        IXPropertyRepository IPropertiesOwner.Properties => Properties;
        IXDimensionRepository IDimensionable.Dimensions => Dimensions;

        /// <summary>
        /// Gets the custom properties collection for this configuration.
        /// <para>中文：获取此配置的自定义属性集合（配置级自定义属性）。</para>
        /// </summary>
        public virtual ISwCustomPropertiesCollection Properties => m_PropertiesLazy.Value;

        /// <summary>
        /// Gets the dimensions collection for this configuration.
        /// <para>中文：获取此配置的尺寸集合（配置关联的所有尺寸）。</para>
        /// </summary>
        public ISwDimensionsCollection Dimensions => m_DimensionsLazy.Value;

        private readonly Lazy<ISwCustomPropertiesCollection> m_PropertiesLazy;
        private readonly Lazy<ISwDimensionsCollection> m_DimensionsLazy;

        public override bool IsCommitted => m_Creator.IsCreated;

        private readonly IElementCreator<IConfiguration> m_Creator;

        internal SwConfiguration(IConfiguration conf, SwDocument3D doc, SwApplication app, bool created) : base(conf, doc, app)
        {
            m_Doc = doc;

            m_Creator = new ElementCreator<IConfiguration>(Create, conf, created);

            m_PropertiesLazy = new Lazy<ISwCustomPropertiesCollection>(
                () => new SwConfigurationCustomPropertiesCollection(Name, m_Doc, OwnerApplication));

            m_DimensionsLazy = new Lazy<ISwDimensionsCollection>(CreateDimensions);
        }

        public override object Dispatch => Configuration;

        /// <summary>
        /// Gets the preview image of this configuration.
        /// <para>中文：获取此配置的预览图（在进程内时使用 SolidWorks API 获取高质量预览，否则使用文档缩略图）。</para>
        /// </summary>
        public IXImage Preview
        {
            get
            {
                if (OwnerApplication.IsInProcess())
                {
                    return PictureDispUtils.PictureDispToXImage(OwnerApplication.Sw.GetPreviewBitmap(m_Doc.Path, Name));
                }
                else
                {
                    return new XDrawingImage(m_Doc.GetThumbnailImage());
                }
            }
        }

        /// <summary>
        /// Gets the part number of this configuration based on its BOM part number source setting.
        /// <para>中文：根据物料清单零件号来源设置获取此配置的零件号。</para>
        /// </summary>
        public string PartNumber => GetPartNumber(Configuration);

        /// <summary>
        /// Gets the quantity of this component as defined by the UNIT_OF_MEASURE custom property.
        /// <para>中文：从自定义属性（UNIT_OF_MEASURE）中解析并获取组件的数量（物料清单数量），默认为 1。</para>
        /// </summary>
        public double Quantity
        {
            get
            {
                var qtyPrp = GetPropertyValue(Configuration.CustomPropertyManager, QTY_PROPERTY);

                if (string.IsNullOrEmpty(qtyPrp))
                {
                    qtyPrp = GetPropertyValue(m_Doc.Model.Extension.CustomPropertyManager[""], QTY_PROPERTY);
                }

                if (!string.IsNullOrEmpty(qtyPrp))
                {
                    var qtyStr = GetPropertyValue(Configuration.CustomPropertyManager, qtyPrp);

                    double qty;

                    if (!string.IsNullOrEmpty(qtyStr))
                    {
                        if (double.TryParse(qtyStr, out qty))
                        {
                            return qty;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                    else
                    {
                        qtyStr = GetPropertyValue(m_Doc.Model.Extension.CustomPropertyManager[""], qtyPrp);

                        if (double.TryParse(qtyStr, out qty))
                        {
                            return qty;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                }
                else
                {
                    return 1;
                }
            }
        }

        /// <summary>
        /// Gets how child components are shown in the BOM for assembly configurations.
        /// <para>中文：获取装配体配置中子组件在物料清单（BOM）中的显示方式（显示、隐藏或提升）。</para>
        /// </summary>
        public BomChildrenSolving_e BomChildrenSolving
        {
            get
            {
                if (m_Doc is ISwAssembly)
                {
                    var bomDispOpt = Configuration.ChildComponentDisplayInBOM;

                    switch ((swChildComponentInBOMOption_e)bomDispOpt)
                    {
                        case swChildComponentInBOMOption_e.swChildComponent_Show:
                            return BomChildrenSolving_e.Show;

                        case swChildComponentInBOMOption_e.swChildComponent_Hide:
                            return BomChildrenSolving_e.Hide;

                        case swChildComponentInBOMOption_e.swChildComponent_Promote:
                            return BomChildrenSolving_e.Promote;

                        default:
                            throw new NotSupportedException($"Not supported BOM display option: {bomDispOpt}");
                    }
                }
                else
                {
                    return BomChildrenSolving_e.Show;
                }
            }
        }

        /// <summary>
        /// Gets the parent configuration of this configuration, or null if it is a root configuration.
        /// <para>中文：获取此配置的父配置；若为根配置则返回 null。</para>
        /// </summary>
        public virtual IXConfiguration Parent 
        {
            get 
            {
                var conf = Configuration.GetParent();

                if (conf != null)
                {
                    return OwnerDocument.CreateObjectFromDispatch<ISwConfiguration>(conf);
                }
                else 
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the value of a custom property from the given property manager.
        /// <para>中文：从指定的自定义属性管理器中读取属性值，兼容 SW2014/SW2018 及更新版本的 API。</para>
        /// </summary>
        private string GetPropertyValue(ICustomPropertyManager prpMgr, string prpName) 
        {
            string resVal;

            if (OwnerApplication.IsVersionNewerOrEqual(SwVersion_e.Sw2018))
            {
                prpMgr.Get6(prpName, false, out _, out resVal, out _, out _);
            }
            else if (OwnerApplication.IsVersionNewerOrEqual(SwVersion_e.Sw2014))
            {
                prpMgr.Get5(prpName, false, out _, out resVal, out _);
            }
            else
            {
                prpMgr.Get4(prpName, false, out _, out resVal);
            }

            return resVal;
        }

        /// <summary>
        /// Resolves the part number for a configuration based on its BOM part number source.
        /// <para>中文：根据配置的物料清单零件号来源（配置名、文档名、父配置名或用户指定名），递归解析并返回零件号。</para>
        /// </summary>
        private string GetPartNumber(IConfiguration conf) 
        {
            switch ((swBOMPartNumberSource_e)conf.BOMPartNoSource)
            {
                case swBOMPartNumberSource_e.swBOMPartNumber_ConfigurationName:
                    return conf.Name;
                case swBOMPartNumberSource_e.swBOMPartNumber_DocumentName:
                    return Path.GetFileNameWithoutExtension(m_Doc.Title);
                case swBOMPartNumberSource_e.swBOMPartNumber_ParentName:
                    return GetPartNumber(conf.GetParent());
                case swBOMPartNumberSource_e.swBOMPartNumber_UserSpecified:
                    return conf.AlternateName;
                default:
                    throw new NotSupportedException();
            }
        }

        public override void Commit(CancellationToken cancellationToken) => m_Creator.Create(cancellationToken);

        /// <summary>
        /// Creates the dimensions collection for this configuration.
        /// <para>中文：创建此配置关联的尺寸集合，通过文档特征管理器构建。</para>
        /// </summary>
        protected virtual ISwDimensionsCollection CreateDimensions()
            => new SwFeatureManagerDimensionsCollection(new SwDocumentFeatureManager(m_Doc, m_Doc.OwnerApplication, new Context(this)), new Context(this));

        /// <summary>
        /// Creates the underlying SolidWorks IConfiguration object.
        /// <para>中文：调用 SolidWorks API 创建底层配置对象，兼容 SW2018 及更早版本，若创建失败则抛出异常。</para>
        /// </summary>
        private IConfiguration Create(CancellationToken cancellationToken) 
        {
            IConfiguration conf;

            if (OwnerApplication.IsVersionNewerOrEqual(SwVersion_e.Sw2018))
            {
                conf = m_Doc.Model.ConfigurationManager.AddConfiguration2(Name, "", "", (int)swConfigurationOptions2_e.swConfigOption_DontActivate, "", "", false);
            }
            else 
            {
                conf = m_Doc.Model.ConfigurationManager.AddConfiguration(Name, "", "", (int)swConfigurationOptions2_e.swConfigOption_DontActivate, "", "");
            }

            if (conf == null) 
            {
                throw new Exception("Failed to create configuration");
            }

            return conf;
        }

        /// <summary>
        /// Disposes this configuration and releases any lazy-initialized property resources.
        /// <para>中文：释放此配置占用的资源，若自定义属性集合已初始化则一并释放。</para>
        /// </summary>
        public void Dispose()
        {
            if (m_PropertiesLazy.IsValueCreated) 
            {
                m_PropertiesLazy.Value.Dispose();
            }
        }
    }

    /// <summary>
    /// Abstract base class for component-specific configurations in SolidWorks.
    /// <para>中文：组件配置的抽象基类，关联 SolidWorks 组件与其引用文档中的配置，并支持组件级尺寸集合。</para>
    /// </summary>
    internal abstract class SwComponentConfiguration : SwConfiguration
    {
        /// <summary>
        /// Gets the configuration object for the referenced component document.
        /// <para>中文：获取组件引用文档中的配置对象；若文档尚未提交则返回 null。</para>
        /// </summary>
        private static IConfiguration GetConfiguration(SwComponent comp, string compName)
        {
            var doc = comp.ReferencedDocument;

            if (doc.IsCommitted)
            {
                return (IConfiguration)doc.Model.GetConfigurationByName(compName);
            }
            else
            {
                return null;
            }
        }

        protected readonly SwComponent m_Comp;

        internal SwComponentConfiguration(SwComponent comp, SwApplication app, string confName)
            : this(GetConfiguration(comp, confName), (SwDocument3D)comp.ReferencedDocument, app, comp.Component.ReferencedConfiguration)
        {
            m_Comp = comp;
        }

        public override IXConfiguration Parent
        {
            get
            {
                var conf = Configuration.GetParent();

                if (conf != null)
                {
                    return m_Comp.GetReferencedConfiguration(conf.Name);
                }
                else
                {
                    return null;
                }
            }
        }

        private SwComponentConfiguration(IConfiguration conf, SwDocument3D doc, SwApplication app, string name)
            : base(conf, doc, app, conf != null)
        {
            if (conf == null)
            {
                Name = name;
            }
        }

        protected override ISwDimensionsCollection CreateDimensions()
            => new SwFeatureManagerDimensionsCollection(
                new SwComponentFeatureManager(m_Comp, m_Comp.RootAssembly, OwnerApplication, new Context(this)), new Context(this));
    }

    /// <summary>
    /// Configuration for a SolidWorks part component, providing material and cut list access.
    /// <para>中文：SolidWorks 零件组件配置类，提供该配置下的材料属性和剪切清单访问。</para>
    /// </summary>
    internal class SwPartComponentConfiguration : SwComponentConfiguration, ISwPartConfiguration
    {
        /// <summary>
        /// Initializes a new part component configuration.
        /// <para>中文：初始化零件组件配置，并创建对应的剪切清单集合。</para>
        /// </summary>
        public SwPartComponentConfiguration(SwPartComponent comp, SwApplication app, string confName) : base(comp, app, confName)
        {
            CutLists = new SwPartComponentCutListItemCollection(comp);
        }

        /// <summary>
        /// 中文：剪切清单集合（焊接件或钣金件的切割清单）。
        /// </summary>
        public IXCutListItemRepository CutLists { get; }

        /// <summary>
        /// Gets or sets the material applied to this part configuration.
        /// <para>中文：获取或设置此零件配置的材料属性；设置为 null 时清除材料。</para>
        /// </summary>
        public IXMaterial Material
        {
            get
            {
                var materialName = ((SwPart)m_Comp.ReferencedDocument).Part.GetMaterialPropertyName2(Name, out var database);

                if (!string.IsNullOrEmpty(materialName))
                {
                    return new SwMaterial(materialName, OwnerApplication.MaterialDatabases.GetOrTemp(database));
                }
                else
                {
                    return null;
                }
            }
            set 
            {
                if (value != null)
                {
                    ((SwPart)m_Comp.ReferencedDocument).Part.SetMaterialPropertyName2(Name, value.Database.Name, value.Name);
                }
                else 
                {
                    ((SwPart)m_Comp.ReferencedDocument).Part.SetMaterialPropertyName2(Name, "", "");
                }
            }
        }
    }

    /// <summary>
    /// Configuration for a SolidWorks assembly component, exposing child components.
    /// <para>中文：SolidWorks 装配体组件配置类，提供对子组件集合的访问（装配体文档组件配置）。</para>
    /// </summary>
    internal class SwAssemblyComponentConfiguration : SwComponentConfiguration, IXAssemblyConfiguration
    {
        /// <summary>
        /// 中文：初始化装配体组件配置。
        /// </summary>
        public SwAssemblyComponentConfiguration(SwComponent comp, SwApplication app, string confName) : base(comp, app, confName)
        {
        }

        /// <summary>
        /// Gets the child components repository for this assembly configuration.
        /// <para>中文：获取此装配体配置下的子组件集合。</para>
        /// </summary>
        public IXComponentRepository Components => m_Comp.Children;
    }

    /// <summary>
    /// Represents a view-only configuration that is not fully loaded (unloaded state).
    /// <para>中文：表示仅查看模式下未完全加载的配置（轻量化/SpeedPak简化配置的只读占位对象），不支持修改或提交操作。</para>
    /// </summary>
    internal class SwViewOnlyUnloadedConfiguration : SwConfiguration
    {
        public override string Name
        {
            get => m_ViewOnlyConfName;
            set => throw new NotSupportedException("Name of view-only configuration cannot be changed");
        }

        private string m_ViewOnlyConfName;

        internal SwViewOnlyUnloadedConfiguration(string confName, SwDocument3D doc, SwApplication app)
            : base(null, doc, app, false)
        {
            m_ViewOnlyConfName = confName;
        }

        public override void Commit(CancellationToken cancellationToken) => throw new InactiveLdrConfigurationNotSupportedException();
        public override object Dispatch => throw new InactiveLdrConfigurationNotSupportedException();
        public override ISwCustomPropertiesCollection Properties => throw new InactiveLdrConfigurationNotSupportedException();
    }

    /// <summary>
    /// Represents an inactive large design review (LDR) assembly configuration that is not loaded.
    /// <para>中文：表示未激活的大型设计评审（LDR）装配体配置（轻量化装配体中的非活动配置），不支持属性访问、提交或派发。</para>
    /// </summary>
    internal class SwLdrAssemblyUnloadedConfiguration : SwAssemblyConfiguration
    {
        public override string Name 
        {
            get => m_LdrConfName;
            set => throw new NotSupportedException("Name of inactive LDR configuration cannot be changed");
        }

        private string m_LdrConfName;

        internal SwLdrAssemblyUnloadedConfiguration(SwAssembly assm, SwApplication app, string confName) 
            : base(null, assm, app, false)
        {
            m_LdrConfName = confName;
        }

        public override void Commit(CancellationToken cancellationToken) => throw new InactiveLdrConfigurationNotSupportedException();
        public override object Dispatch => throw new InactiveLdrConfigurationNotSupportedException();
        public override ISwCustomPropertiesCollection Properties => throw new InactiveLdrConfigurationNotSupportedException();
    }

    /// <summary>
    /// Represents an inactive large design review (LDR) part configuration that is not loaded.
    /// <para>中文：表示未激活的大型设计评审（LDR）零件配置（轻量化零件中的非活动配置），不支持属性访问、提交或派发。</para>
    /// </summary>
    internal class SwLdrPartUnloadedConfiguration : SwPartConfiguration
    {
        public override string Name
        {
            get => m_LdrConfName;
            set => throw new NotSupportedException("Name of inactive LDR configuration cannot be changed");
        }

        private string m_LdrConfName;

        internal SwLdrPartUnloadedConfiguration(SwPart part, SwApplication app, string confName)
            : base(null, part, app, false)
        {
            m_LdrConfName = confName;
        }

        public override void Commit(CancellationToken cancellationToken) => throw new InactiveLdrConfigurationNotSupportedException();
        public override object Dispatch => throw new InactiveLdrConfigurationNotSupportedException();
        public override ISwCustomPropertiesCollection Properties => throw new InactiveLdrConfigurationNotSupportedException();
    }
}