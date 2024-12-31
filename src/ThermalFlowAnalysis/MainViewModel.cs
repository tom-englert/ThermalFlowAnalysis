using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using PropertyChanged;
using ThermalFlowAnalysis.Model;
using TomsToolbox.Wpf;

namespace ThermalFlowAnalysis;

internal partial class MainViewModel : INotifyPropertyChanged
{
    private static readonly Period ActivePeriod = new(TimeSpan.FromHours(8), TimeSpan.FromHours(20));
    private static readonly AnalysisContext AnalysisContext = new() { ActivePeriod = ActivePeriod };
    private static readonly DateTime StartDate = DateTime.MinValue;

    public DayModel[]? Days { get; private set; }

    public ICommand LoadFileCommand => new DelegateCommand(() => Days = LoadFile());

    public ICollection<DataLine>? SelectedDayLines { get; private set; }

    [OnChangedMethod(nameof(SelectedDayChanged))]
    public DayModel? SelectedDay { get; set; }

    private void SelectedDayChanged()
    {
        if (SelectedDay is null)
        {
            SelectedDayLines = null;
            return;
        }

        var entries = SelectedDay.Items;

        var points = entries.Select((item, index) => new Point(index, item.Temperature)).ToArray();

        var temperatureLine = new DataLine(points, Brushes.SteelBlue);
        var averageTemp = SelectedDay.AverageTemp;

        var averageLine = new DataLine([new(0, averageTemp), new(points.Length - 1, averageTemp)], Brushes.Teal);

        var startIndex = entries.TakeWhile(item => item.Timestamp.TimeOfDay < ActivePeriod.Start).Count();
        var endIndex = entries.TakeWhile(item => item.Timestamp.TimeOfDay < ActivePeriod.End).Count();

        var startLine = new DataLine([new(startIndex, 50), new(startIndex, -15)], Brushes.DarkGray);
        var endLine = new DataLine([new(endIndex, 50), new(endIndex, -15)], Brushes.DarkGray);

        SelectedDayLines =
        [
            temperatureLine,
            averageLine,
            startLine,
            endLine
        ];
    }

    private static DayModel[] LoadFile()
    {
        var dialog = new OpenFileDialog
        {
            AddExtension = true,
            CheckFileExists = true,
            DefaultExt = ".xls",
            Filter = "ExcelFiles (*.xls,*.xlsx)|*.xlsx;*.xls"
        };

        if (dialog.ShowDialog() != true)
            return [];

        var filePath = dialog.FileName;

        try
        {
            var dataEntries = DataEntry
                .ReadFile(filePath);

            var table = dataEntries
                .Where(item => item.Timestamp >= StartDate)
                .OrderBy(item => item.Timestamp)
                .ToArray();

            return DayModel
                .Analyze(table, AnalysisContext)
                .ToArray();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return [];
        }
    }
}