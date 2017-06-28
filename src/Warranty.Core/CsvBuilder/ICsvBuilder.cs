namespace Warranty.Core.CsvBuilder
{
    using System.Collections.Generic;

    public interface ICsvBuilder
    {
        byte[] GetCsvBytes<T>(IEnumerable<string> linesBeforeHeader, IEnumerable<T> csvRecords, char quoteChar = '"', bool quoteAllFields = false);
    }
}