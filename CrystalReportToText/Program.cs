using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalReportToText
{
    class Program
    {

        static bool Simplified = false;


        static string RemoveNumbersIfSimplified(string name)
        {
            if (Simplified)
            {
                return name.Replace("0", "").Replace("1", "").Replace("2", "").Replace("3", "").Replace("4", "").Replace("5", "").Replace("6", "").Replace("7", "").Replace("8", "").Replace("9", "");
            }
            return name;
        }

        static string MakeSingleLine(string text)
        {
                return text.Replace("\n", ";").Replace("\r", " ");
        }


        static void Main(string[] args)
        {

            if (args.Count() >= 1 && args[0] == "-s")
            {
                Simplified = true;
                args = args.Skip(1).ToArray();
            }

            if (args.Count() < 1 || args[0] == "-h")
            {
                Console.WriteLine("CrystalReportReaderFramework [-s]  report_file_name.rpt");
                Console.WriteLine("");
                Console.WriteLine("    -s  : Simplify output by removing numbers from sections and text fields and by removing location information.");
                Console.WriteLine("          Simplified output makes it easier to compare reports where layout is not important.");
                Console.WriteLine("");
                return;
            }

            string fileName = args[0];

            ReportDocument reportDocument = new ReportDocument();
            
            reportDocument.Load(fileName);

            Database db = reportDocument.Database;
            foreach (var t in db.Tables)
            {
                Table tbl = t as Table;
                string prefix = "Database:Table:Name:" + tbl.Name + ":";
                Console.WriteLine(prefix);
                Console.WriteLine(prefix + "Location:" + tbl.Location);
                foreach (var f in tbl.Fields)
                {
                    FieldDefinition fd = f as FieldDefinition;
                    string fieldPrefix = prefix + "Field:Name:" + fd.Name + ":";
                    Console.WriteLine(fieldPrefix);
                    Console.WriteLine(fieldPrefix + "Kind:" + fd.Kind);
                    Console.WriteLine(fieldPrefix + "ValueType:" + fd.ValueType);
                }
            }

            foreach (var l in db.Links)
            {
                TableLink lnk = l as TableLink;
                string linkPRefix = "Database:TableLink:SourceTable:" +  lnk.SourceTable.Name + ":DestinationTable:" + lnk.DestinationTable.Name + ":";
                Console.WriteLine(linkPRefix);               
                Console.WriteLine(linkPRefix + "JoinType:" + lnk.JoinType);
                for (int i = 0; i < lnk.SourceFields.Count;i++) {
                    DatabaseFieldDefinition sfd = lnk.SourceFields[i] as DatabaseFieldDefinition;
                    DatabaseFieldDefinition dfd = lnk.DestinationFields[i] as DatabaseFieldDefinition;
                    Console.WriteLine(linkPRefix + "SourceField:Name:" + sfd.Name + ":DestinationField:Name:" + dfd.Name);                    
                }

            }


            var ffs = reportDocument.DataDefinition.FormulaFields;
            foreach ( var ff in ffs)
            {
                FormulaFieldDefinition ffd = ff as FormulaFieldDefinition;
                Console.WriteLine("Formula:FormulaName:" + ffd.FormulaName + ":");
                if (ffd.Text != null)
                {
                    Console.WriteLine("Formula:FormulaName:" + ffd.FormulaName + ":Text:'" + MakeSingleLine(ffd.Text));
                }
            }          

            var rtfs = reportDocument.DataDefinition.RunningTotalFields;
            foreach (var rtf in rtfs)
            {
                RunningTotalFieldDefinition rtfd = rtf as RunningTotalFieldDefinition;
                Group rc = rtfd.ResetCondition as Group;
                string prefix = "RunningTotal:FormulaName:" + rtfd.FormulaName + ":";
                Console.WriteLine(prefix + "Operation:" + rtfd.Operation);
                Console.WriteLine(prefix + "SummarizedField:" + rtfd.SummarizedField.Name + "," + rtfd.SummarizedField.FormulaName);
                Console.WriteLine(prefix + "EvaluationCondition:" + rtfd.EvaluationCondition);
                Console.WriteLine(prefix + "ResetCondition:" + rc.ConditionField.FormulaName);
            }
          

            var groups = reportDocument.DataDefinition.Groups;
            foreach (var gp in groups)
            {
                Group gpd = gp as Group;
                string prefix = "Group:GroupName:" + RemoveNumbersIfSimplified(gpd.ConditionField.FormulaName) + ":";
                Console.WriteLine(prefix);
                Console.WriteLine(prefix + "GroupOptions.Condition:" + gpd.GroupOptions.Condition);                        
            }
          


            var parameterFields = reportDocument.DataDefinition.ParameterFields;
            foreach (var pf in parameterFields)
            {
                ParameterFieldDefinition pfd = pf as ParameterFieldDefinition;
                var x = "ParameterFields:FormulaName:" + pfd.FormulaName + ":" +
                    "ParameterFieldName:" + pfd.ParameterFieldName ;
                Console.WriteLine(x);
            }
            

            var sections = reportDocument.ReportDefinition.Sections;
            foreach (var section in sections)
            {
                Section sd = section as Section;
                string prefix = "Section:Name:" + RemoveNumbersIfSimplified(sd.Name) + ":";
                Console.WriteLine(prefix);
                Console.WriteLine(prefix + "Kind:" + sd.Kind);              
                Console.WriteLine(prefix + "EnableKeepTogether:" + sd.SectionFormat.EnableKeepTogether);
                Console.WriteLine(prefix + "EnableSuppress:" + sd.SectionFormat.EnableSuppress);
                if (!Simplified)
                {
                    Console.WriteLine(prefix + "Height:" + sd.Height);
                    Console.WriteLine(prefix + "Height:" + sd.Height);
                }
              

                AddReportObjects(prefix, sd.ReportObjects);           
            }

            // Console.WriteLine(reportDocument);
            //Console.ReadLine();
        }

        private static void AddReportObjects(string prefix , ReportObjects reportObjects)
        {
            foreach (var ro in reportObjects)
            {
                ReportObject rod = ro as ReportObject;
                string roprefix = prefix + "ReportObject:Name:" + RemoveNumbersIfSimplified(rod.Name) + ":";
                Console.WriteLine(roprefix);
                Console.WriteLine(roprefix +  "Kind:" + rod.Kind);

                if ( rod.Kind == CrystalDecisions.Shared.ReportObjectKind.FieldObject)
                {
                    FieldObject fo = rod as FieldObject;
                    Console.WriteLine(roprefix + "DataSource.FormulaName:" + fo.DataSource.FormulaName);
                }

                if (rod.Kind == CrystalDecisions.Shared.ReportObjectKind.TextObject)
                {
                    TextObject txto = rod as TextObject;
                    Console.WriteLine(roprefix + "Text:" + MakeSingleLine(txto.Text));
                }

                if (rod.Kind == CrystalDecisions.Shared.ReportObjectKind.FieldHeadingObject)
                {
                    FieldHeadingObject fho = rod as FieldHeadingObject;
                    Console.WriteLine(roprefix + "FieldObjectName:" + RemoveNumbersIfSimplified(fho.FieldObjectName));
                    Console.WriteLine(roprefix + "Text:" + MakeSingleLine(fho.Text));
                }

                if (rod.Kind == CrystalDecisions.Shared.ReportObjectKind.SubreportObject)
                {
                    SubreportObject o = rod as SubreportObject;
                    Console.WriteLine(roprefix + "SubreportName:" + RemoveNumbersIfSimplified(o.SubreportName));
                }


                if (rod.Kind == CrystalDecisions.Shared.ReportObjectKind.BoxObject)
                {
                    BoxObject o = rod as BoxObject;
                    Console.WriteLine(roprefix + "EnableExtendToBottomOfSection:" + o.EnableExtendToBottomOfSection);
                    Console.WriteLine(roprefix + "EndSectionName:" + o.EndSectionName);
                    if (!Simplified)
                    {                      
                        Console.WriteLine(roprefix + "LineThickness:" + o.LineThickness);
                        Console.WriteLine(roprefix + "LineStyle:" + o.LineStyle);
                        
                        Console.WriteLine(roprefix + "LineColor:IsNamedColor:" + o.LineColor.IsNamedColor);
                        if ( o.LineColor.IsNamedColor)
                        {
                            Console.WriteLine(roprefix + "LineColor:Name:" + o.LineColor.Name);
                        } 
                        else
                        {
                            Console.WriteLine(roprefix + "LineColor:RGB:" + o.LineColor.R.ToString() + o.LineColor.G.ToString()  + o.LineColor.B.ToString());
                        }

                        Console.WriteLine(roprefix + "FillColor:IsNamedColor:" + o.FillColor.IsNamedColor);
                        if (o.LineColor.IsNamedColor)
                        {
                            Console.WriteLine(roprefix + "FillColor:Name:" + o.FillColor.Name);
                        }
                        else
                        {
                            Console.WriteLine(roprefix + "FillColor:RGB:" + o.FillColor.R.ToString() + o.FillColor.G.ToString() + o.FillColor.B.ToString());
                        }
                    }
                }

                if (! Simplified && rod.Kind == CrystalDecisions.Shared.ReportObjectKind.LineObject)
                {
                    LineObject o = rod as LineObject;
                    Console.WriteLine(roprefix + "LineThickness:" + o.LineThickness);
                    Console.WriteLine(roprefix + "LineStyle:" + o.LineStyle);
                    Console.WriteLine(roprefix + "LineColor:IsNamedColor:" + o.LineColor.IsNamedColor);
                    if (o.LineColor.IsNamedColor)
                    {
                        Console.WriteLine(roprefix + "LineColor:Name:" + o.LineColor.Name);
                    }
                    else
                    {
                        Console.WriteLine(roprefix + "LineColor:RGB:" + o.LineColor.R.ToString() + o.LineColor.G.ToString() + o.LineColor.B.ToString());
                    }
                  
                }


                if (!Simplified)
                {
                    Console.WriteLine(roprefix + "Left:" + rod.Left);
                    Console.WriteLine(roprefix + "Top:" + rod.Top);
                    Console.WriteLine(roprefix + "Width:" + rod.Width);
                    Console.WriteLine(roprefix + "Height:" + rod.Height);
                }
            }
        }
    }
}
