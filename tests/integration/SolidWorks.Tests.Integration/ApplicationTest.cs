// -*- coding: utf-8 -*-
// tests/integration/SolidWorks.Tests.Integration/ApplicationTest.cs

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.SolidWorks;

namespace SolidWorks.Tests.Integration
{
    /// <summary>
    /// ApplicationTest 测试 SOLIDWORKS 应用程序级别的功能。
    /// 包括材料数据库的访问和材料属性的读取等。
    /// </summary>
    public class ApplicationTest : IntegrationTests
    {
        /// <summary>
        /// 测试访问 SOLIDWORKS 材料数据库，验证材料属性的完整性。
        /// 包括弹性模量、泊松比、剪切模量、热膨胀系数、密度、强度等物理属性。
        /// </summary>
        [Test]
        public void MaterialDatabaseTest() 
        {
            string dbFileName;
            string category;
            double elasticModulus;
            double poissonRatio;
            double shearModulus;
            double thermalExpansionCoefficient;
            double massDensity;
            double thermalConductivity;
            double specificHeat;
            double tensileStrength;
            double yieldStrength;
            double hardeningFactor;

            var db = (ISwMaterialsDatabase)m_App.MaterialDatabases[""];
            dbFileName = Path.GetFileName(db.FilePath);

            var absPcMat = db["ABS PC"];

            category = absPcMat.Category;
            elasticModulus = absPcMat.ElasticModulus;
            poissonRatio = absPcMat.PoissonRatio;
            shearModulus = absPcMat.ShearModulus;
            thermalExpansionCoefficient = absPcMat.ThermalExpansionCoefficient;
            massDensity = absPcMat.MassDensity;
            thermalConductivity = absPcMat.ThermalConductivity;
            specificHeat = absPcMat.SpecificHeat;
            tensileStrength = absPcMat.TensileStrength;
            yieldStrength = absPcMat.YieldStrength;
            hardeningFactor = absPcMat.HardeningFactor;

            Assert.That(string.Equals("solidworks materials.sldmat", dbFileName, StringComparison.CurrentCultureIgnoreCase));
            Assert.AreEqual("Plastics", category);
            
            AssertCompareDoubles(elasticModulus, 2410000000);
            AssertCompareDoubles(poissonRatio, 0.3897);
            AssertCompareDoubles(shearModulus, 862200000);
            AssertCompareDoubles(thermalExpansionCoefficient, double.NaN);
            AssertCompareDoubles(massDensity, 1070);
            AssertCompareDoubles(thermalConductivity, 0.2618);
            AssertCompareDoubles(specificHeat, 1900);
            AssertCompareDoubles(tensileStrength, 40000000);
            AssertCompareDoubles(yieldStrength, double.NaN);
            AssertCompareDoubles(hardeningFactor, double.NaN);
        }
    }
}
