using System.Collections.Generic;
using System.Linq;

namespace Warranty.Core.CsvBuilder
{
    using System.IO;
    using CsvHelper;

    public class CsvBuilder : ICsvBuilder
    {
        public static readonly Dictionary<string, string> _ColumnDescription = new Dictionary<string, string>()
        {
            {"EmptyField1", ""},
            {"EmptyField2", ""},
            {"HomeownerName", "Homeowner Name"},
            {"AddressLine", "Address"},
            {"City", "City"},
            {"StateCode", "State"},
            {"PostalCode", "Zip Code"},
            {"HomePhone", "Home Phone"},
            {"CommunityName", "Community"},
            {"CloseDate", "Close Date"},
        };
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
                    string header = string.Empty;
                    _ColumnDescription.TryGetValue(column, out header);
                    csv.WriteField(header);
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
