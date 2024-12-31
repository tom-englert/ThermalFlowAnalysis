using System.Globalization;
using System.Text;
using ExcelDataReader;

namespace ThermalFlowAnalysis.Model;

public record DataEntry(DateTime Timestamp, double Temperature)
{
    public static IEnumerable<DataEntry> ReadFile(string filePath)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        using var spreadsheetStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        using var reader = ExcelReaderFactory.CreateReader(spreadsheetStream);

        do
        {
            // skip header row
            reader.Read();

            while (reader.Read())
            {
                var timeStamp = reader.GetString(1);
                var temperature = reader.GetString(2);

                yield return new(DateTime.Parse(timeStamp), double.Parse(temperature.TrimEnd('℃'), NumberStyles.Float, CultureInfo.InvariantCulture));
            }
        } while (reader.NextResult());
    }
}