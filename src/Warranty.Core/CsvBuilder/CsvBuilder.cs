using System.Collections.Generic;
using System.Linq;

namespace Warranty.Core.CsvBuilder
{
    using System.IO;
    using CsvHelper;

    public class CsvBuilder : ICsvBuilder
    {
        public byte[] GetCsvBytes<T>(IEnumerable<string> linesBeforeHeader, IEnumerable<T> csvRecords, char quoteChar = '"', bool quoteAllFields = false)
        {
            var tempFileName = Path.GetTempFileName();
            using (var writer = new StreamWriter(tempFileName))
            {
                var csv = ConfigureCsvWriter<T>(quoteChar, quoteAllFields, writer);
                WriteLinesBeforeHeader<T>(linesBeforeHeader, writer);
                WriteHeader<T>(csvRecords, csv);
                WriteCsvRecords(csvRecords, csv);
            }
            var bytes = File.ReadAllBytes(tempFileName);
            if (File.Exists(tempFileName))
                File.Delete(tempFileName);

            return bytes;
        }

        private static CsvWriter ConfigureCsvWriter<T>(char quoteChar, bool quoteAllFields, StreamWriter writer)
        {
            var csv = new CsvWriter(writer);
            csv.Configuration.Quote = quoteChar;
            csv.Configuration.QuoteAllFields = quoteAllFields;
            return csv;
        }

        private static void WriteCsvRecords<T>(IEnumerable<T> csvRecords, CsvWriter csv)
        {
            foreach (var customer in csvRecords)
            {
                csv.WriteRecord(customer);
            }
        }

        private static void WriteHeader<T>(IEnumerable<T> csvRecords, CsvWriter csv)
        {
            var customer = csvRecords.FirstOrDefault();
            if (customer != null)
            {
                var columnNames = customer.GetType().GetProperties().Select(column => column.Name);                                          
                foreach (var column in columnNames)
                {
                    if (column == "EmptyField1")
                    {
                        csv.WriteField("");
                    }
                    else if (column == "EmptyField2")
                    {
                        csv.WriteField("");
                    }
                    else if (column == "HomeownerName")
                    {
                        csv.WriteField("Homeowner Name");
                    }
                    else if (column == "AddressLine")
                    {
                        csv.WriteField("Address");
                    }
                    else if (column == "City")
                    {
                        csv.WriteField("City");
                    }
                    else if (column == "StateCode")
                    {
                        csv.WriteField("State");
                    }
                    else if (column == "PostalCode")
                    {
                        csv.WriteField("Zip Code");
                    }
                    else if (column == "HomePhone")
                    {
                        csv.WriteField("Home Phone");
                    }
                    else if (column == "CommunityName")
                    {
                        csv.WriteField("Community");
                    }
                    else if (column == "CloseDate")
                    {
                        csv.WriteField("Close Date");
                    }
                }
            }
            csv.NextRecord();

        }

        private static void WriteLinesBeforeHeader<T>(IEnumerable<string> linesBeforeHeader, StreamWriter writer)
        {
            if (linesBeforeHeader.Any())
            {
                foreach (var lbh in linesBeforeHeader)
                {
                    writer.WriteLine(lbh);
                }
            }
        }
    }
}
