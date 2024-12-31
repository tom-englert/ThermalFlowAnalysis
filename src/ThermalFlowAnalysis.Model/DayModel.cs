namespace ThermalFlowAnalysis.Model;

public record DayModel(DateTime Date, double MinTemp, double MaxTemp, double AverageTemp, DataEntry[] Items)
{
    public static IEnumerable<DayModel> Analyze(DataEntry[] table, AnalysisContext context)
    {
        var itemsByDay = table.GroupBy(item => item.Timestamp.Date);

        var (start, end) = context.ActivePeriod ?? new Period(TimeSpan.FromHours(7), TimeSpan.FromHours(20));

        return itemsByDay.Select(dayItems =>
        {
            var date = dayItems.Key;

            var records = dayItems
                .Where(r => r.Timestamp.TimeOfDay >= start && r.Timestamp.TimeOfDay <= end)
                .ToArray();

            var temperatures = records
                .Select(r => r.Temperature)
                .ToArray();

            var tRoom = context.RoomTemperature;
            var tRef = context.ReferenceTemperature;
            var coefficient = context.ThermalCoefficient;

            var deltaRef = (tRef - tRoom);

            double RelativePower(double tCurrent)
            {
                var deltaCurrent = Math.Max(0, tCurrent - tRoom);
                return Math.Pow(deltaCurrent / deltaRef, coefficient);
            }
            
            var pRel = temperatures.Select(RelativePower).ToArray();
            var pEff = pRel.Sum() / pRel.Length;

            var tAverage = Math.Pow(pEff, 1 / coefficient) * deltaRef + tRoom;

            var tMin = temperatures.Min();
            var tMax = temperatures.Max();

            return new DayModel(date, tMin, tMax, tAverage, dayItems.ToArray());
        });
    }
}