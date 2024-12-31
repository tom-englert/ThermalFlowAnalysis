namespace ThermalFlowAnalysis.Model;

public record Period(TimeSpan Start, TimeSpan End);

public record AnalysisContext(
    double RoomTemperature = 21, 
    double ReferenceTemperature = 50, 
    double ThermalCoefficient = 1.3,
    Period? ActivePeriod = null
);