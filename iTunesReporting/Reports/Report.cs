using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using iTunesLib;

namespace iTunesReporting.Reports
{
    public abstract class Report
    {
        private const string FILE_NAME = "{0} - {1}.txt";

        private const int COL_WIDTH = 40;
        private const string FILES_FOUND_SINGULAR = "\n1 file found";
        private const string FILES_FOUND_PLURAL = "\n{0} files found";

        protected string ReportName { get; set; }
        protected List<string> OutputHeaders { get; set; }
        protected List<List<string>> OutputRows { get; set; }

        public abstract void LoadParameters(Dictionary<string, string> parameters);
        public abstract void InitializeOutput();
        public abstract void ProcessTrack(IITFileOrCDTrack track);

        public virtual void FinalizeReport()
        {
            
        }

        public Report()
        {
            OutputHeaders = new List<string>();
            OutputRows = new List<List<string>>();

            InitializeOutput();
        }

        private string GetReportFileName()
        {
            string dateStamp = DateTime.Now.ToString("yyyy.MM.dd-HH.mm");
            string result = string.Format(FILE_NAME, dateStamp, ReportName);

            return result;        
        }

        public void Write()
        {
            string cellFormat = "{0,-" + COL_WIDTH + "} ";
            int separatorWidth = ((COL_WIDTH + 1) * OutputHeaders.Count);
            string separator = new String('-', separatorWidth);
            string fileName = GetReportFileName();

            StreamWriter writer = new StreamWriter(fileName);

            //Header
            writer.WriteLine(separator);
            for (int col = 0; col < OutputHeaders.Count; col++)
            {
                writer.Write(String.Format(cellFormat, OutputHeaders[col].ToUpper()));
            }
            writer.Write("\n" + separator);

            foreach (List<string> row in OutputRows)
            {

                //Data
                writer.Write("\n");
                for (int col = 0; col < OutputHeaders.Count; col++)
                {
                    writer.Write(String.Format(cellFormat, row[col]));
                }
            }

            //Write footer
            writer.WriteLine("\n" + separator);

            if (OutputRows.Count == 1)
                writer.WriteLine(FILES_FOUND_SINGULAR);
            else
                writer.WriteLine(string.Format(FILES_FOUND_PLURAL, OutputRows.Count));

            writer.Close();
            writer = null;

            Console.WriteLine("Report created: " + fileName);
        }

    }
}
