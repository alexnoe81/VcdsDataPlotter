using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using VcdsDataPlotter.Lib.CalculatedColumns.ConfigFiles;

namespace VcdsDataPlotter.Lib.Tests
{
    [TestClass]
    public class Test_CreateDefaultConfigurationFiles
    {
        /// <summary>
        /// This is not an actual test, but creates a configuration file SemanticColumns.xml, which describes
        /// how to obtain certain columns with specific meanings, like
        /// - low pressure EGR mass flow is IDE07086 or IDE09886
        /// </summary>
        [TestMethod]
        public void Test_CreateSemanticColumns()
        {
            ColumnBuilderConfigurationDefinition speedDefinition = new ColumnBuilderConfigurationDefinition();
            speedDefinition.Title = "Vehicle speed";
            speedDefinition.ChannelId = "VIRT_VEHICLE_SPEED";
            speedDefinition.Steps = [ new Select() { ChannelId = "IDE00075" } ];

            ColumnBuilderConfigurationDefinition totalDefDefinition = new ColumnBuilderConfigurationDefinition();
            totalDefDefinition.Title = "Total DEF consumed";
            totalDefDefinition.ChannelId = "VIRT_TOTAL_DEF_CONSUMED";
            totalDefDefinition.Steps =
            [
                new SelectFirst()
                {
                    Choices =
                    [
                        new CalculationDefinition() { Steps = [ new Select() { ChannelId = "IDE16115" } ] },
                        new CalculationDefinition() { Steps = [ new Select() { ChannelId = "IDE03144" } ] }
                    ]
                }
            ];

            ColumnBuilderConfigurationDefinition hpEgrMassFlowActualDefinition = new ColumnBuilderConfigurationDefinition();
            hpEgrMassFlowActualDefinition.Title = "Mass flow of high pressure EGR (actual)";
            hpEgrMassFlowActualDefinition.ChannelId = "VIRT_HPEGR_MASS_FLOW_ACTUAL";
            hpEgrMassFlowActualDefinition.Steps = [new Select() { ChannelId = "IDE03383" } ];
            
            ColumnBuilderConfigurationDefinition lpEgrMassFlowActualDefinition = new ColumnBuilderConfigurationDefinition();
            lpEgrMassFlowActualDefinition.Title = "Mass flow of low pressure EGR (actual)";
            lpEgrMassFlowActualDefinition.ChannelId = "VIRT_LPEGR_MASS_FLOW_ACTUAL";
            lpEgrMassFlowActualDefinition.Steps = 
            [
                new SelectFirst()
                {
                    Choices =
                    [
                        new CalculationDefinition() { Steps = [ new Select() { ChannelId = "IDE07086" } ] },
                        new CalculationDefinition() { Steps = [ new Select() { ChannelId = "IDE09886" } ] }
                    ]
                }
            ];           

            ColumnBuilderConfigurationDefinition exhaustMassFlowActualDefinition = new ColumnBuilderConfigurationDefinition();
            exhaustMassFlowActualDefinition.Title = "Exhaust Mass flow";
            exhaustMassFlowActualDefinition.ChannelId = "VIRT_CYLINDER_EXHAUST_MASS_FLOW";
            exhaustMassFlowActualDefinition.Steps =
            [
                new SelectFirst()
                {
                    Choices =
                    [
                        new CalculationDefinition() { Steps = [ new Select() { ChannelId = "ENG247045" } ] },
                        new CalculationDefinition() { Steps = [ new Select() { ChannelId = "IDE07374" } ] }
                    ]
                }
            ];


            ColumnBuilderConfigurationDefinition noxBeforeSCRDefinition = new ColumnBuilderConfigurationDefinition();
            noxBeforeSCRDefinition.Title = "NOx before SCR";
            noxBeforeSCRDefinition.ChannelId = "VIRT_NOX_BEFORE_SCR";
            noxBeforeSCRDefinition.Steps =
            [
                new SelectFirst()
                {
                    Choices =
                    [
                        new CalculationDefinition() { Steps = [ new Select() { ChannelId = "IDE04098/1" } ] },
                        new CalculationDefinition()
                        {
                            Steps =
                            [
                                // Note: IDE03140 is 1650-1660 for a few seconds during warmup. We need to add an invalid value removal column
                                new Select() { ChannelId = "IDE03140" }
                            ]
                        }
                    ]
                }
            ];
          
            ColumnBuilderConfigurationDefinitionRoot root = new ColumnBuilderConfigurationDefinitionRoot();
            root.Columns = [
                speedDefinition,
                totalDefDefinition,
                hpEgrMassFlowActualDefinition,
                lpEgrMassFlowActualDefinition,
                exhaustMassFlowActualDefinition,
                noxBeforeSCRDefinition
            ];

            XmlSerializer serializer = new XmlSerializer(typeof(ColumnBuilderConfigurationDefinitionRoot));
            WriteToXmlFile(serializer, "SemanticColumns.xml", root);
            
            var noxBeforeSCRConfiguration = noxBeforeSCRDefinition.Build();
        }

        [TestMethod]
        public void Test_CreateCalculatedColumns()
        {
            ColumnBuilderConfigurationDefinition distanceDefinition = new ColumnBuilderConfigurationDefinition();
            distanceDefinition.Title = "Traveled distance";
            distanceDefinition.ChannelId = "VIRT_TRAVELED_DISTANCE";
            distanceDefinition.Steps =
            [
                new Select() { ChannelId = "VIRT_VEHICLE_SPEED" },
                new IntegrateByTime(),
                new ConvertUnit() { TargetUnit = "m" }
            ];


            ColumnBuilderConfigurationDefinition defDefinition = new ColumnBuilderConfigurationDefinition();
            defDefinition.Title = "DEF consumed since start of file";
            defDefinition.ChannelId = "VIRT_DEF_CONSUMED";
            defDefinition.Steps = 
            [ 
                new Select() { ChannelId = "VIRT_TOTAL_DEF_CONSUMED" }, 
                new DifferenceToFirstRow() 
            ];

            ColumnBuilderConfigurationDefinition totalHpEgrMassDefinition = new ColumnBuilderConfigurationDefinition();
            totalHpEgrMassDefinition.Title = "High pressure EGR total mass";
            totalHpEgrMassDefinition.ChannelId = "VIRT_HPEGR_TOTAL_MASS";
            totalHpEgrMassDefinition.Steps =
            [
                new Select() { ChannelId = "VIRT_HPEGR_MASS_FLOW_ACTUAL" },
                new IntegrateByTime()
            ];

            ColumnBuilderConfigurationDefinition totalLpEgrMassDefinition = new ColumnBuilderConfigurationDefinition();
            totalLpEgrMassDefinition.Title = "Low pressure EGR total mass";
            totalLpEgrMassDefinition.ChannelId = "VIRT_LPEGR_TOTAL_MASS";
            totalLpEgrMassDefinition.Steps =
            [
                new Select() { ChannelId = "VIRT_LPEGR_MASS_FLOW_ACTUAL" },
                new IntegrateByTime()
            ];

            ColumnBuilderConfigurationDefinition cylinderExhaustMassDefinition = new ColumnBuilderConfigurationDefinition();
            cylinderExhaustMassDefinition.Title = "Total exhaust mass after cylinder block";
            cylinderExhaustMassDefinition.ChannelId = "VIRT_CYLINDER_TOTAL_EXHAUST_MASS";
            cylinderExhaustMassDefinition.Steps =
            [
                new Select() { ChannelId = "VIRT_CYLINDER_EXHAUST_MASS_FLOW" },
                new IntegrateByTime()
            ];



            ColumnBuilderConfigurationDefinitionRoot root = new ColumnBuilderConfigurationDefinitionRoot(); 
            root = new ColumnBuilderConfigurationDefinitionRoot();
            root.Columns = 
            [
                distanceDefinition, 
                defDefinition,
                totalHpEgrMassDefinition,
                totalLpEgrMassDefinition,
                cylinderExhaustMassDefinition
            ];

            XmlSerializer serializer = new XmlSerializer(typeof(ColumnBuilderConfigurationDefinitionRoot));
            WriteToXmlFile(serializer, "CalculatedColumns.xml", root);
        }



        private void WriteToXmlFile(XmlSerializer serializer, string targetFile, object obj)
        {
            using var semanticColumnsFile = File.OpenWrite(targetFile);
            semanticColumnsFile.Position = 0;
            semanticColumnsFile.SetLength(0);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.NewLineOnAttributes = false;
            settings.Indent = true;
            settings.NewLineChars = ControlChars.NewLine;

            XmlWriter writer = XmlWriter.Create(semanticColumnsFile, settings);

            serializer.Serialize(writer, obj);
        }
    }
}
