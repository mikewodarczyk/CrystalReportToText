# CrystalReportToText
Translate a Crystal Report definition (.rpt) file into a human readable text format suitable for diff style comparisons.


Each property of each reporting element is output in a single line with colon separated context information preceeding it.
For example a section name will preceed any properties for the section such that the line will uniquely identify the property 
and will contain its value.

Multi-line text fields and formulas are merged into a single line with semicolons replacing Carriage-returns.  This way the context
information stays with the entire text block.  

An optional parameter "-s" can be provided to print a simplified output.  The only differences with simplified output
is that numbers for sections and text elements will be removed, and location information (left, top, width, height) are
removed.  This makes it easier to "diff" two reports where it doesn't matter exactly what sequence the text elements were
put in or exactly where on the page the element goes.

Here is some example normal output:

		Formula:FormulaName:{@ExtWeight}:
		Formula:FormulaName:{@ExtWeight}:Text:'Round (if({SO_PackingListWrk.ItemWeight})> 0 then({SO_PackingListWrk.ItemWeight}*{SO_PackingListWrk.UDF_SALESUMCNVFCTR}*100)else 0)*{SO_PackingListWrk.QuantityShipped}/100
		RunningTotal:FormulaName:{#TotalWeight}:Operation:Sum
		RunningTotal:FormulaName:{#TotalWeight}:SummarizedField:ExtWeight,{@ExtWeight}
		RunningTotal:FormulaName:{#TotalWeight}:EvaluationCondition:{SO_PackingListWrk.LotSerialNo}=""
		RunningTotal:FormulaName:{#TotalWeight}:ResetCondition:{SO_PackingListWrk.HeaderSeqNo}
		Section:Name:PageFooterSection1:ReportObject:Name:Text11:
		Section:Name:PageFooterSection1:ReportObject:Name:Text11:Kind:TextObject
		Section:Name:PageFooterSection1:ReportObject:Name:Text11:Text:PRO#:
		Section:Name:PageFooterSection1:ReportObject:Name:Text11:Left:128
		Section:Name:PageFooterSection1:ReportObject:Name:Text11:Top:1724
		Section:Name:PageFooterSection1:ReportObject:Name:Text11:Width:944
		Section:Name:PageFooterSection1:ReportObject:Name:Text11:Height:360
		Section:Name:PageFooterSection1:ReportObject:Name:TotalWeight1:
		Section:Name:PageFooterSection1:ReportObject:Name:TotalWeight1:Kind:FieldObject
		Section:Name:PageFooterSection1:ReportObject:Name:TotalWeight1:DataSource.FormulaName:{#TotalWeight}
		Section:Name:PageFooterSection1:ReportObject:Name:TotalWeight1:Left:9783
		Section:Name:PageFooterSection1:ReportObject:Name:TotalWeight1:Top:1769
		Section:Name:PageFooterSection1:ReportObject:Name:TotalWeight1:Width:1227
		Section:Name:PageFooterSection1:ReportObject:Name:TotalWeight1:Height:228


The simplified version would look like this:

Here is some example normal output:

		Formula:FormulaName:{@ExtWeight}:
		Formula:FormulaName:{@ExtWeight}:Text:'Round (if({SO_PackingListWrk.ItemWeight})> 0 then({SO_PackingListWrk.ItemWeight}*{SO_PackingListWrk.UDF_SALESUMCNVFCTR}*100)else 0)*{SO_PackingListWrk.QuantityShipped}/100
		RunningTotal:FormulaName:{#TotalWeight}:Operation:Sum
		RunningTotal:FormulaName:{#TotalWeight}:SummarizedField:ExtWeight,{@ExtWeight}
		RunningTotal:FormulaName:{#TotalWeight}:EvaluationCondition:{SO_PackingListWrk.LotSerialNo}=""
		RunningTotal:FormulaName:{#TotalWeight}:ResetCondition:{SO_PackingListWrk.HeaderSeqNo}
		Section:Name:PageFooterSection:ReportObject:Name:Text:
		Section:Name:PageFooterSection:ReportObject:Name:Text:Kind:TextObject
		Section:Name:PageFooterSection:ReportObject:Name:Text:Text:PRO#:
		Section:Name:PageFooterSection:ReportObject:Name:TotalWeight1:
		Section:Name:PageFooterSection:ReportObject:Name:TotalWeight1:Kind:FieldObject
		Section:Name:PageFooterSection:ReportObject:Name:TotalWeight1:DataSource.FormulaName:{#TotalWeight}


Though the application only runs on one report at a time, by using Windows sybsystem for linux, one can
run across a folder of reports with a bash command like this:

    find . -type f -name *.rpt -exec bash -xc "./CrystalReportToText/bin/Debug/CrystalReportToText.exe -s '{}' | sort | uniq > '{}.txt'" \;

The "sort" and "uniq" commands were added so that a diff of two resulting text files will only find differences in content,
not differences in layout or ordering.



