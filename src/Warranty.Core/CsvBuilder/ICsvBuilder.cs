namespace Warranty.Core.CsvBuilder
{
    using System.Collections.Generic;

    public interface ICsvBuilder
    {
        byte[] GetCsvBytes<T>(IEnumerable<string> linesBeforeHeader, IEnumerable<T> csvRecords, bool includeHeaderRow = true, char quoteChar = '"', bool quoteAllFields = false);
    }
}