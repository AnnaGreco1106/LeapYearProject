using System;
using System.IO;
using CsvHelper;
using System.Linq;
using System.Collections.Generic;
using CsvHelper.Configuration.Attributes;

namespace LeapYearProject
{
    class Program
    {
        public static string InputPath { get; set; } = Environment.CurrentDirectory + "\\input.csv";
        public static string OutputPath { get; set; } = Environment.CurrentDirectory + "\\output.csv";

        // Checks if given integer is a leap year and returns true/false
        private static bool CheckDate(int year)
        {
            var result = false;
            if ((year % 4 == 0) && (year % 100 != 0)) { result = true; }
            if ((year % 4 == 0) && (year % 100 == 0) && (year % 400 == 0)) { result = true; }

            return result;
        }

        // used for CSV import
        public class Input
        {
            public DateTimeOffset Date { get; set; }
            public int Uploads { get; set; }
        }
        // used for CSV export
        public class Output
        {
            public int Year { get; set; }
            [Name("Average Uploads")]
            public double Average_Uploads { get; set; }
        }
        static void Main(string[] args)
        {
            using (var reader = new StreamReader(InputPath))
            using (var csv = new CsvReader(reader))
            {
                csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();
                var records = csv.GetRecords<Input>().ToList();
                var results = new List<Output>();
                // returns records grouped and sorted by leap year
                var LeapYearData = records.Where(l => CheckDate(l.Date.Year) == true).OrderBy(y => y.Date.Year).GroupBy(y => y.Date.Year);
                // for every grouped leapyear in leapyeardata it prepares output for CSV writer
                foreach (var leapyear in LeapYearData)
                {
                    var output = new Output();
                    output.Year = leapyear.Key;
                    output.Average_Uploads = leapyear.Select(u => u.Uploads).Average();
                    results.Add(output);
                }
                // writes prepared data
                using (var writer = new StreamWriter(OutputPath))
                using (var csvwriter = new CsvWriter(writer))
                {
                    csvwriter.WriteRecords(results);
                }
            }
        }
    }
}
